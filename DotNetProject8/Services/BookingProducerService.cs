using RabbitMQ.Client;
using System;
using DotNetProject8.Models;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Connections.Features;
using Newtonsoft.Json;

namespace DotNetProject8.Services
{
   
    public class BookingProducerService 
    {
        private readonly ILogger<BookingProducerService> _logger;
        private const string queueName = "appointment_queue";
        private const string exchangeName = "direct_exchange";
        private const string routingKey = "appointment_routing_key";

        public BookingProducerService(ILogger<BookingProducerService> logger)
        {
            _logger = logger;
        }

        public async Task EnqueueBookingAsync(BookingRequestModel bookingRequestModel)
        {

            PatientDetails patient = new()
            {
                PatientFName = bookingRequestModel.Patient.PatientFName,
                PatientLName = bookingRequestModel.Patient.PatientLName,
                AddressLine1 = bookingRequestModel.Patient.AddressLine1,
                AddressLine2 = bookingRequestModel.Patient.AddressLine2,
                City = bookingRequestModel.Patient.City,
                Postcode = bookingRequestModel.Patient.Postcode,
                ContactNumber = bookingRequestModel.Patient.ContactNumber
            };

            AppointmentDetails appointment = new()
            {
                AppointmentDate = bookingRequestModel.Appointment.AppointmentDate,
                AppointmentTime = bookingRequestModel.Appointment.SelectedAppointmentTime,
                ConsultantId = bookingRequestModel.Consultant.ConsultantId
            };
            BookingModel bookingModel = new()
            {
                Patient = patient,
                Appointment = appointment
            };

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = await factory.CreateConnectionAsync())
            {
                using (var channel = await connection.CreateChannelAsync())
                {
                    await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);

                    await channel.QueueDeclareAsync(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    await channel.QueueBindAsync(queueName, exchangeName, routingKey, null);

                    var json = JsonConvert.SerializeObject(bookingModel); 
                    var body = Encoding.UTF8.GetBytes(json);
                    var properties = new BasicProperties { Persistent = true };


                    await channel.BasicPublishAsync(
                        exchange: exchangeName,
                        routingKey: routingKey,
                        mandatory: true,
                        basicProperties: properties,
                        body: body
                        );

                    // placeholder for sessionId
                    Console.WriteLine(" [x] Sent {0}", 0);
                }
            }
        }
    }

}
