using Microsoft.EntityFrameworkCore;
using TravelFinalProject.DAL;
using TravelFinalProject.Interfaces;
using TravelFinalProject.Models;
using TravelFinalProject.Utilities;

namespace TravelFinalProject.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;

        }

        public async Task<bool> HasNotificationBeenSentAsync(string userId, int tourId)
        {
            return await _context.NotificationSents.AnyAsync(n => n.UserId == userId && n.TourId == tourId);
        }
        public async Task SendNotificationAsync(string userId, int tourId)
        {
            var alreadySent = await HasNotificationBeenSentAsync(userId, tourId);
            if (alreadySent)
                return;

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var tour = await _context.Tours
                .Include(t => t.TourTranslations)
                .FirstOrDefaultAsync(t => t.Id == tourId);

            if (tour == null)
                throw new Exception("Tour not found");

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.UserId == userId && b.TourId == tourId &&
                                          (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed));
            if (booking == null)
                return;

            if (tour.End_Date > DateOnly.FromDateTime(DateTime.UtcNow))
                return;

            var baseUrl = "https://localhost:44364";
            var reviewUrl = $"{baseUrl}/Review/ReviewAdd?tourId={tour.Id}";

            string emailBody = $@"
<div style='font-family:Arial,sans-serif;padding:20px;background:#f9f9f9;border:1px solid #ddd;border-radius:10px;'>
    <h2 style='color:#2c3e50;'>Salam, {user.Name}!</h2>
    <p style='font-size:16px;color:#333;'>Sizin <strong>{tour.TourTranslations.FirstOrDefault()?.Title}</strong> turunuz uğurla başa çatdı.</p>
    <p style='font-size:16px;color:#333;'>Təcrübəniz bizim üçün önəmlidir. Zəhmət olmasa, 1 dəqiqə vaxt ayırıb rəy bildirin:</p>
    <a href='{reviewUrl}' 
       style='display:inline-block;margin-top:15px;padding:12px 20px;background:#3498db;color:#fff;text-decoration:none;border-radius:5px;'>
       Rəy Bildir
    </a>
    <p style='font-size:14px;color:#999;margin-top:20px;'>Təşəkkür edirik və sizi yenidən görməkdən məmnun olarıq!</p>
</div>";

            await _emailService.SendMailAsync(user.Email, "Turunuz haqqında rəy bildirin", emailBody, true);

            var notification = new NotificationSent
            {
                UserId = userId,
                TourId = tourId,
                SentDate = DateTime.UtcNow,
                IsReviewGiven = false
            };
            _context.NotificationSents.Add(notification);
            await _context.SaveChangesAsync();
        }

        //        public async Task SendNotificationAsync(string userId, int tourId)
        //        {


        //            var alreadySent = await HasNotificationBeenSentAsync(userId, tourId);
        //            if (alreadySent)
        //                return;

        //            var user = await _context.Users.FindAsync(userId);
        //            if (user == null)
        //                throw new Exception("User not found");

        //            var tour = await _context.Tours
        //.Include(t => t.TourTranslations)
        //.FirstOrDefaultAsync(t => t.Id == tourId);
        //            var baseUrl = "https://localhost:44364";
        //            var reviewUrl = $"{baseUrl}/Review/ReviewAdd?tourId={tour.Id}";
        //            if (tour == null)
        //                throw new Exception("Tour not found");
        //            string emailBody = $@"
        //<div style='font-family:Arial,sans-serif;padding:20px;background:#f9f9f9;border:1px solid #ddd;border-radius:10px;'>
        //    <h2 style='color:#2c3e50;'>Salam, {user.Name}!</h2>
        //    <p style='font-size:16px;color:#333;'>Sizin <strong>{tour.TourTranslations.FirstOrDefault().Title}</strong> turunuz uğurla başa çatdı.</p>
        //    <p style='font-size:16px;color:#333;'>Təcrübəniz bizim üçün önəmlidir. Zəhmət olmasa, 1 dəqiqə vaxt ayırıb rəy bildirin:</p>
        //    <a href='{reviewUrl}' 
        //       style='display:inline-block;margin-top:15px;padding:12px 20px;background:#3498db;color:#fff;text-decoration:none;border-radius:5px;'>
        //       Rəy Bildir
        //    </a>
        //    <p style='font-size:14px;color:#999;margin-top:20px;'>Təşəkkür edirik və sizi yenidən görməkdən məmnun olarıq!</p>
        //</div>";

        //            await _emailService.SendMailAsync(user.Email, "Turunuz haqqında rəy bildirin", emailBody, true);


        //            var notification = new NotificationSent
        //            {
        //                UserId = userId,
        //                TourId = tourId,
        //                SentDate = DateTime.UtcNow,
        //                IsReviewGiven = false
        //            };
        //            _context.NotificationSents.Add(notification);
        //            await _context.SaveChangesAsync();


        //        }

        public async Task MarkReviewGivenAsync(string userId, int tourId)
        {
            var notification = await _context.NotificationSents.FirstOrDefaultAsync(n => n.UserId == userId && n.TourId == tourId);
            if (notification != null)
            {
                notification.IsReviewGiven = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasUserReviewedAsync(string userId, int tourId)
        {
            return await _context.Reviews.AnyAsync(r => r.UserId == userId && r.TourId == tourId);
        }
    }

}
