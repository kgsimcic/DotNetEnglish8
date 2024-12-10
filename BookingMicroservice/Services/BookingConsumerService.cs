using BookingMicroservice.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using BookingMicroservice.Models;

namespace BookingMicroservice.Services
{
    public class BookingConsumerService : BackgroundService
    {
        private readonly ILogger<BookingConsumerService> _logger;
        private readonly IConnectionFactory _connectionFactory; 
        private readonly IServiceProvider _serviceProvider;
        private const string queueName = "appointment_queue";
        private const string exchangeName = "direct_exchange";
        private const string routingKey = "appointment_routing_key";

        public BookingConsumerService(ILogger<BookingConsumerService> logger, IConnectionFactory connectionFactory, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) { 
            return Task.Run(() => ConsumeAsync(stoppingToken)); 
        }

        public async Task ConsumeAsync(CancellationToken stoppingToken) { 

            using (var connection = await _connectionFactory.CreateConnectionAsync()) 
            using (var channel = await connection.CreateChannelAsync()) {

                await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);

                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );

                await channel.QueueBindAsync(queueName, exchangeName, routingKey, null);
                
                var consumer = new AsyncEventingBasicConsumer(channel); 
                consumer.ReceivedAsync += async (model, ea) => {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var bookingModel = JsonConvert.DeserializeObject<BookingModel>(message);

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                            await bookingService.ProcessBookingAsync(bookingModel);
                            _logger.LogDebug("What is going on?");
                        }

                        // temp error handling -- not sure if necessary. 
                        if (channel.IsOpen)
                        {
                            await channel.BasicAckAsync(ea.DeliveryTag, false);
                            _logger.LogDebug("Ack should have happened!");
                        }
                        else
                        {
                            _logger.LogWarning("Channel is not open, message not acknowledged.");
                        }
                    } catch (Exception ex) { 
                        _logger.LogError($"Error processing message: {ex.Message}"); 
                        { await channel.BasicNackAsync(ea.DeliveryTag, false, true); } 
                    }
                }; 
                
                await channel.BasicConsumeAsync(
                    queue: queueName, 
                    autoAck: false, 
                    consumer: consumer);
                
                Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

                stoppingToken.WaitHandle.WaitOne();
            } 
        }
    }
}
