using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SalesService.Data;
using SalesService.Services;
using Shared.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/sales-service.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Sales Service API (Interno)", Version = "v1" });
    c.SwaggerDoc("internal", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Endpoints Internos - Não usar diretamente", Version = "v1" });
});

// Health Checks
builder.Services.AddHealthChecks();

// Entity Framework
builder.Services.AddDbContext<SalesDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")!)));

// Services
builder.Services.AddScoped<IOrderService, OrderService>();

// HTTP Client para Stock Service
builder.Services.AddHttpClient<IStockServiceClient, StockServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:StockService"] ?? "https://localhost:7001");
    
    // Configurar JWT token se necessário
    var token = builder.Configuration["Services:StockServiceToken"];
    if (!string.IsNullOrEmpty(token))
    {
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
});

// RabbitMQ
builder.Services.AddSingleton<IRabbitMQService>(provider =>
    new RabbitMQService(builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672/"));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (!string.IsNullOrEmpty(jwtKey))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Aplicar migrações automaticamente
// Comentado para usar banco correto
/*
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
    context.Database.Migrate();
}
*/

app.Run();
