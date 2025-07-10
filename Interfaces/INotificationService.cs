namespace TravelFinalProject.Interfaces
{
    public interface INotificationService
    {
        Task<bool> HasNotificationBeenSentAsync(string userId, int tourId);
        Task SendNotificationAsync(string userId, int tourId);
        Task MarkReviewGivenAsync(string userId, int tourId);
        Task<bool> HasUserReviewedAsync(string userId, int tourId);
    }
}
