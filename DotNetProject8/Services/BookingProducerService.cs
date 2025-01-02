using RabbitMQ.Client;
using System;
using DotNetProject8.Models;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Connections.Features;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DotNetProject8.Services
{
   
    public class BookingProducerService 
    {
        private readonly ILogger<BookingProducerService> _logger;
        private readonly SignalRService _signalRService;
        private const string queueName = "appointment_queue";
        private const string exchangeName = "direct_exchange";
        private const string routingKey = "appointment_routing_key";

        public BookingProducerService(ILogger<BookingProducerService> logger, SignalRService signalRService)
        {
            _logger = logger;
            _signalRService = signalRService;
        }

        public BookingRequestModel CreateBookingModel(DateTime date, ConsultantModel consultant, List<DateTime> takenAppointmentTimes)
        {

            List<DateTime> appointmentTimes = new();

            DateTime startTime = date.AddHours(8);

            for (int i = 0; i < 10; i++)
            {
                if (!takenAppointmentTimes.Contains(startTime.AddHours(i)))
                {
                    DateTime test = startTime.AddHours(i);
                    appointmentTimes.Add(test);
                }
            }

            AppointmentRequestDetails appointmentRequestDetails = new()
            {
                AppointmentDate = date,
                AppointmentTimes = new SelectList(appointmentTimes),
                SelectedAppointmentTime = appointmentTimes.Min()
            };

            ConsultantRequestDetails consultantRequestDetails = new()
            {
                ConsultantId = consultant.Id,
                ConsultantName = consultant.Fname + ' ' + consultant.Lname,
                ConsultantSpeciality = consultant.Speciality
            };

            BookingRequestModel bookingRequestModel = new()
            {
                Consultant = consultantRequestDetails,
                Appointment = appointmentRequestDetails,
                Patient = new()
            };

            return bookingRequestModel;
        }

        public async Task EnqueueBookingAsync(BookingRequestModel bookingRequestModel)
        {

            string connectionId = await _signalRService.StartConnectionAsync();
            _logger.LogInformation($"ConnectionId started: {connectionId}");


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
                ConnectionId = connectionId,
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

                    _logger.LogInformation($" [x] Sent {appointment.ConnectionId}");
                }
            }
        }
    }

}
