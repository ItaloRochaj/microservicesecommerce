using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Shared.Services;

public interface IRabbitMQService
{
    void PublishMessage<T>(string queueName, T message);
    void Subscribe<T>(string queueName, Action<T> onMessage);
}

public class RabbitMQService : IRabbitMQService, IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;
    private readonly string _connectionString;
    private readonly ILogger<RabbitMQService>? _logger;

    public RabbitMQService(string connectionString = "amqp://guest:guest@localhost:5672/", ILogger<RabbitMQService>? logger = null)
    {
        _connectionString = connectionString;
        _logger = logger;
        
        try
        {
            InitializeConnection();
        }
        catch (Exception ex)
        {
            _logger?.LogWarning("RabbitMQ connection failed during initialization: {Message}. Service will continue without messaging.", ex.Message);
        }
    }

    private void InitializeConnection()
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri(_connectionString)
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    private bool EnsureConnection()
    {
        if (_connection?.IsOpen == true && _channel?.IsOpen == true)
            return true;

        try
        {
            _connection?.Close();
            _channel?.Close();
            InitializeConnection();
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning("Failed to establish RabbitMQ connection: {Message}", ex.Message);
            return false;
        }
    }

    public void PublishMessage<T>(string queueName, T message)
    {
        if (!EnsureConnection())
        {
            _logger?.LogWarning("Cannot publish message to queue {QueueName}: RabbitMQ connection unavailable", queueName);
            return;
        }

        try
        {
            _channel!.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
            _logger?.LogDebug("Message published to queue {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to publish message to queue {QueueName}", queueName);
        }
    }

    public void Subscribe<T>(string queueName, Action<T> onMessage)
    {
        if (!EnsureConnection())
        {
            _logger?.LogWarning("Cannot subscribe to queue {QueueName}: RabbitMQ connection unavailable", queueName);
            return;
        }

        try
        {
            _channel!.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonConvert.DeserializeObject<T>(json);
                    
                    if (message != null)
                    {
                        onMessage(message);
                    }
                    
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing message from queue {QueueName}", queueName);
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            _logger?.LogDebug("Subscribed to queue {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to subscribe to queue {QueueName}", queueName);
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
