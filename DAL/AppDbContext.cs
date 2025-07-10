using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TravelFinalProject.Models;

namespace TravelFinalProject.DAL
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourTranslation> TourTranslations { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<DestinationTranslation> DestinationTranslations { get; set; }
        public DbSet<SlideTranslation> SlideTranslations { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<DestinationCategoryTranslation> DestinationCategoryTranslations { get; set; }
        public DbSet<DestinationCategory> DestinationCategories { get; set; }
        public DbSet<DestinationImage> DestinationImages { get; set; }
        public DbSet<TourImage> TourImages { get; set; }
        public DbSet<BookingTravellerTranslation> BookingTravellerTranslations { get; set; }
        public DbSet<BookingTraveller> BookingTravellers { get; set; }

        public DbSet<TravellerPassportNumber> PassportNumbers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewTranslation> ReviewTranslations { get; set; }
        public DbSet<NotificationSent> NotificationSents { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogReview> BlogReviews { get; set; }
        public DbSet<BlogReviewReply> BlogReviewReplys { get; set; }

        public DbSet<BlogTranslation> BlogTranslations { get; set; }
        public DbSet<Setting> Settings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<DestinationTranslation>()
                .HasIndex(x => new { x.DestinationId, x.LangCode })
                .IsUnique();


            modelBuilder.Entity<DestinationCategoryTranslation>()
                .HasIndex(x => new { x.DestinationCategoryId, x.LangCode })
                .IsUnique();

            modelBuilder.Entity<TourTranslation>()
                .HasIndex(x => new { x.TourId, x.LangCode })
                .IsUnique();
        }

    }
}
