using BookingMicroservice.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using BookingMicroservice.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingMicroservice.Services
{
    public class BookingConsumerService : BackgroundService
    {
        private readonly ILogger<BookingConsumerService> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IChannel _channel;

        private const string QueueName = "appointment_queue";
        private const string ExchangeName = "direct_exchange";
        private const string RoutingKey = "appointment_routing_key";

        public BookingConsumerService(ILogger<BookingConsumerService> logger, IConnectionFactory connectionFactory, IServiceProvider serviceProvider){
            _logger = logger;
            _connectionFactory = connectionFactory;
            _serviceProvider = serviceProvider;
        }

        private async Task InitializeRabbitMQAsync()
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Direct
            );

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: RoutingKey
            );

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 200, global: false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await InitializeRabbitMQAsync();

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += HandleMessageAsync;

                await Task.Run(() =>
                    _channel.BasicConsumeAsync(
                        queue: QueueName,
                        autoAck: false,
                        consumer: consumer
                    ),
                    stoppingToken
                );

                _logger.LogInformation(" [*] Waiting for messages. To exit press CTRL+C");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Booking consumer service is shutting down...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error occurred in the booking consumer service");
                throw;
            }
            finally
            {
                await CleanupAsync();
            }
        }

        private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var bookingModel = JsonConvert.DeserializeObject<BookingModel>(message);

                _logger.LogInformation($"Processing booking request for connection {bookingModel.Appointment.ConnectionId}");

                string status = null;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    status = await bookingService.ProcessBookingAsync(bookingModel);
                    await notificationService.NotifyBookingStatus(
                        bookingModel.Appointment.ConnectionId,
                        status);
                }

                if (_channel.IsOpen)
                {
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    _logger.LogDebug("Message successfully acknowledged");
                }
                else
                {
                    _logger.LogWarning("Channel is not open, message not acknowledged");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                if (_channel.IsOpen)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            }
        }

        private async Task CleanupAsync()
        {
            try
            {
                _channel?.Dispose();
                if (_connection?.IsOpen == true)
                {
                    await _connection.CloseAsync();
                }
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cleanup");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await CleanupAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
