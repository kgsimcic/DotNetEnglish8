using BookingMicroservice.Entities;
using BookingMicroservice.Models;
using BookingMicroservice.Services;
using System.Collections.Concurrent;

namespace SchedulingWorkerService
{
   
    public class BookingWorkerService : BackgroundService
    {
        private readonly ILogger<BookingWorkerService> _logger;
        private readonly ConcurrentQueue<BookingModel> _bookingQueue = new();
        private IBookingService _bookingService;

        public BookingWorkerService(ILogger<BookingWorkerService> logger, IBookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;

        }

        public void EnqueueBooking(BookingModel bookingModel)
        {
            _bookingQueue.Enqueue(bookingModel);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                while (_bookingQueue.TryDequeue(out var bookingModel))
                {
                    // good name?
                    await ProcessBookingAsync(bookingModel);
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<int> ProcessBookingAsync(BookingModel bookingModel)
        {
            return await _bookingService.CreateBooking(bookingModel);
        }
    }

}
