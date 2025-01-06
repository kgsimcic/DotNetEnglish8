namespace BookingMicroservice.Services
{
    public interface INotificationService
    {
        Task NotifyBookingStatus(string connectionId, string status);
    }
}
