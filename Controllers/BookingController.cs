
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Globalization;
using TravelFinalProject.DAL;
using TravelFinalProject.Interfaces;
using TravelFinalProject.Models;
using TravelFinalProject.Utilities;
using TravelFinalProject.ViewModels;
[Authorize]
[Route("booking")]
public class BookingController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ICurrencyService _currencyService;
    private readonly ILogger<BookingController> _logger;
    private readonly IConfiguration _configuration;
    private readonly StripeSettings _stripeSettings;

    public BookingController(AppDbContext context,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        ICurrencyService currencyService,
        ILogger<BookingController> logger,
        IOptions<StripeSettings> stripeOptions,
        IConfiguration configuration
        )
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
        _currencyService = currencyService;
        _logger = logger;
        _configuration = configuration;
        _stripeSettings = stripeOptions.Value;
    }


    //[HttpGet("create/{tourId?}")]
    //public async Task<IActionResult> Create(int tourId, int adults = 1, int children = 0, string langCode = "en")
    //{
    //    if (adults < 1) adults = 1;
    //    if (children < 0) children = 0;

    //    var tour = await _context.Tours
    //        .Include(t => t.TourTranslations.Where(tt => tt.LangCode == langCode))
    //        .Include(t => t.TourImages)
    //        .Include(t => t.Destination)
    //            .ThenInclude(d => d.DestinationTranslations)
    //        .FirstOrDefaultAsync(t => t.Id == tourId);

    //    if (tour == null)
    //        return NotFound();

    //    decimal originalPriceAdult = tour.Price ?? 0m;
    //    decimal originalPriceChild = originalPriceAdult * 0.5m;
    //    string currencyCode = Request.Cookies["SelectedCurrency"] ?? "USD";

    //    decimal pricePerAdult = await _currencyService.ConvertAsync(originalPriceAdult, currencyCode);
    //    decimal pricePerChild = await _currencyService.ConvertAsync(originalPriceChild, currencyCode);

    //    int guestsCount = adults + children;
    //    decimal promoDiscountPercent = 0;

    //    decimal totalPrice = (adults * pricePerAdult + children * pricePerChild) * (1 - promoDiscountPercent / 100);


    //    var guests = new List<BookingTravellerVM>();
    //    for (int i = 0; i < guestsCount; i++)
    //    {
    //        guests.Add(new BookingTravellerVM());
    //    }

    //    var model = new BookingVM
    //    {
    //        TourId = tourId,
    //        AdultsCount = adults,
    //        ChildrenCount = children,
    //        GuestsCount = guestsCount,
    //        PricePerAdult = pricePerAdult,
    //        PricePerChild = pricePerChild,
    //        PromoDiscountPercent = promoDiscountPercent,
    //        Guests = guests,

    //        Booking = new Booking
    //        {
    //            TourId = tourId,
    //            GuestsCount = guestsCount,
    //            TotalPrice = totalPrice,
    //            Tour = tour,
    //            CurrencyCode = currencyCode,

    //        }
    //    };

    //    return View(model);
    //}
    [HttpGet("create/{tourId?}")]
    public async Task<IActionResult> Create(int tourId, int adults = 1, int children = 0)
    {
        var langCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        if (adults < 1) adults = 1;
        if (children < 0) children = 0;

        var tour = await _context.Tours
            .Include(t => t.TourTranslations.Where(tt => tt.LangCode == langCode))
            .Include(t => t.TourImages)
            .Include(t => t.Destination)
                .ThenInclude(d => d.DestinationTranslations)
            .FirstOrDefaultAsync(t => t.Id == tourId);

        if (tour == null)
            return NotFound();

        decimal originalPriceAdult = tour.Price ?? 0m;
        decimal originalPriceChild = originalPriceAdult * 0.5m;
        string currencyCode = Request.Cookies["SelectedCurrency"] ?? "USD";

        decimal pricePerAdult = await _currencyService.ConvertAsync(originalPriceAdult, currencyCode);
        decimal pricePerChild = await _currencyService.ConvertAsync(originalPriceChild, currencyCode);

        string currencySymbol = _currencyService.GetSymbol(currencyCode);

        int guestsCount = adults + children;
        decimal promoDiscountPercent = 0;

        decimal totalPrice = (adults * pricePerAdult + children * pricePerChild) * (1 - promoDiscountPercent / 100);

        var guests = new List<BookingTravellerVM>();
        for (int i = 0; i < guestsCount; i++)
        {
            guests.Add(new BookingTravellerVM());
        }

        var model = new BookingVM
        {
            TourId = tourId,
            AdultsCount = adults,
            ChildrenCount = children,
            GuestsCount = guestsCount,
            PricePerAdult = pricePerAdult,
            PricePerChild = pricePerChild,
            PromoDiscountPercent = promoDiscountPercent,
            Guests = guests,
            CurrencySymbol = currencySymbol,

            Booking = new Booking
            {
                TourId = tourId,
                GuestsCount = guestsCount,
                TotalPrice = totalPrice,
                Tour = tour,
                CurrencyCode = currencyCode,
            }
        };

        return View(model);
    }


    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingVM bookingVM)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            _logger.LogWarning("Unauthorized access attempt to booking creation.");
            return Unauthorized();
        }

        bookingVM.GuestsCount = bookingVM.Guests?.Count ?? 0;
        if (bookingVM.Guests == null || bookingVM.Guests.Count != bookingVM.GuestsCount)
        {
            ModelState.AddModelError("", "Bütün qonaqlar üçün məlumatları daxil edin.");
        }
        else
        {
            if (bookingVM.Guests.Any(g => string.IsNullOrWhiteSpace(g.PassportNumber)))
                ModelState.AddModelError("", "Pasport nömrələri boş ola bilməz.");

            var duplicatePassport = bookingVM.Guests
                .Where(g => !string.IsNullOrWhiteSpace(g.PassportNumber))
                .GroupBy(g => g.PassportNumber.Trim().ToLower())
                .Any(grp => grp.Count() > 1);

            if (duplicatePassport)
                ModelState.AddModelError("", "Pasport nömrələri təkrarlana bilməz.");

            if (bookingVM.Guests.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(bookingVM.Guests[0].Email))
                    ModelState.AddModelError("", "Əsas qonaq üçün Email daxil edin.");

                if (string.IsNullOrWhiteSpace(bookingVM.Guests[0].PhoneNumber))
                    ModelState.AddModelError("", "Əsas qonaq üçün Telefon nömrəsi daxil edin.");
            }
            else
            {
                ModelState.AddModelError("", "Ən azı bir qonaq məlumatı daxil edilməlidir.");
            }
        }
        if (bookingVM.GuestsCount < 1 || bookingVM.GuestsCount > 100)
        {
            ModelState.AddModelError(nameof(bookingVM.GuestsCount), "Qonaq sayı 1 ilə 100 arasında olmalıdır.");
        }

        if (!ModelState.IsValid)
        {
            var tour = await _context.Tours
                .Include(t => t.TourTranslations)
                .Include(t => t.TourImages)
                .Include(t => t.Destination)
                    .ThenInclude(d => d.DestinationTranslations)
                .FirstOrDefaultAsync(t => t.Id == bookingVM.TourId);

            if (bookingVM.Booking == null)
                bookingVM.Booking = new Booking();

            bookingVM.Booking.Tour = tour;

            return View(bookingVM);
        }

        var tourForBooking = await _context.Tours.FindAsync(bookingVM.TourId);
        if (tourForBooking == null)
        {
            ModelState.AddModelError("", "Seçdiyiniz tur tapılmadı.");
            return View(bookingVM);
        }
        if (tourForBooking.Available_seats < bookingVM.GuestsCount)
        {
            ModelState.AddModelError("", "Tur üçün kifayət qədər boş yer yoxdur.");
            return View(bookingVM);
        }


        tourForBooking.Available_seats -= bookingVM.GuestsCount;
        _context.Tours.Update(tourForBooking);

        string currencyCode = Request.Cookies["SelectedCurrency"] ?? "usd";
        currencyCode = currencyCode.ToLower();

        decimal originalPriceAdult = tourForBooking.Price ?? 0m;
        decimal originalPriceChild = originalPriceAdult * 0.5m;

        decimal pricePerAdult = await _currencyService.ConvertAsync(originalPriceAdult, currencyCode.ToUpper());
        decimal pricePerChild = await _currencyService.ConvertAsync(originalPriceChild, currencyCode.ToUpper());

        int adults = bookingVM.AdultsCount;
        int children = bookingVM.ChildrenCount;
        int guestsCount = adults + children;

        decimal subtotal = adults * pricePerAdult + children * pricePerChild;
        decimal discountAmount = bookingVM.PromoDiscountPercent > 0 ? subtotal * bookingVM.PromoDiscountPercent / 100 : 0;
        decimal finalTotal = subtotal - discountAmount;

        var booking = new Booking
        {
            TourId = tourForBooking.Id,
            UserId = currentUser.Id,
            GuestsCount = guestsCount,
            AdultsCount = bookingVM.AdultsCount,
            ChildrenCount = bookingVM.ChildrenCount,
            TotalPrice = finalTotal,
            Status = BookingStatus.Pending,
            PricePerAdult = pricePerAdult,
            PricePerChild = pricePerChild,
            PromoDiscountPercent = bookingVM.PromoDiscountPercent,
            CurrencyCode = currencyCode.ToUpper(),
            BookingDate = DateTime.Now,

        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        foreach (var guest in bookingVM.Guests)
        {
            var traveller = new BookingTraveller
            {
                BookingId = booking.Id,
                DateOfBirth = guest.DateOfBirth ?? default,
                PassportNumber = guest.PassportNumber?.Trim(),
                Email = guest.Email,
                PhoneNumber = guest.PhoneNumber,
                BookingTravellerTranslations = new List<BookingTravellerTranslation>
            {
                new BookingTravellerTranslation
                {
                    FirstName = guest.FirstName,
                    LastName = guest.LastName,
                    Gender = guest.Gender,
                    Nationality = guest.Nationality,
                    LangCode = "en"
                }
            }
            };

            _context.BookingTravellers.Add(traveller);
        }

        await _context.SaveChangesAsync();

        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = currencyCode,
                        UnitAmountDecimal = finalTotal * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Booking for Tour #{tourForBooking.Id}",
                            Description = $"Booking ID: {booking.Id}"
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = Url.Action("BookingConfirmation", "Booking", new { id = booking.Id }, protocol: Request.Scheme),
            CancelUrl = Url.Action("PaymentCancel", "Booking", null, Request.Scheme),



            Metadata = new Dictionary<string, string>
        {
            { "BookingId", booking.Id.ToString() },
            { "UserId", currentUser.Id }
        }
        };

        var service = new SessionService();
        var session = service.Create(options);
        return Redirect(session.Url);
    }



    [HttpPost("create-checkout-session/{bookingId}")]
    public async Task<IActionResult> CreateCheckoutSession(int bookingId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Tour)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            return NotFound();

        var domain = $"{Request.Scheme}://{Request.Host}";

        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.TotalPrice * 100),
                    Currency = booking.CurrencyCode?.ToLower() ?? "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = booking.Tour.TourTranslations.FirstOrDefault().Title?? "Tour Booking",
                        Description = $"Tour reservation with {booking.GuestsCount} guests"
                    }
                },
                Quantity = 1
            }
        },
            Mode = "payment",
            SuccessUrl = domain + Url.Action("BookingConfirmation", "Booking", new { id = booking.Id }) + "?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = Url.Action("PaymentCancel", "Booking", new { bookingId = booking.Id }, protocol: Request.Scheme),
            Metadata = new Dictionary<string, string>
        {
            { "bookingId", booking.Id.ToString() },
            { "userId", booking.UserId }
        }
        };

        var service = new SessionService();
        var session = service.Create(options);

        return Json(new { id = session.Id });
    }

    [HttpGet("confirmation/{id}")]
    public async Task<IActionResult> BookingConfirmation(int id, string langCode = "en")
    {
        var booking = await _context.Bookings
            .Include(b => b.Tour).ThenInclude(b => b.TourTranslations.Where(tt => tt.LangCode == langCode))
            .Include(b => b.Travellers)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return NotFound();
        if (booking.Status == BookingStatus.Pending)
        {
            booking.Status = BookingStatus.Confirmed;
            await _context.SaveChangesAsync();
        }

        try
        {
            string subject = "Booking Confirmation";
            string body = $@"
<div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; background-color: #f9f9f9;'>
    <p style='font-size: 16px;'>Dear <strong>{booking.User.Name}</strong>,</p>
    
    <p style='font-size: 16px;'>
        We are pleased to inform you that your booking has been <strong style='color: #27ae60;'>confirmed successfully</strong>.
    </p>

    <h3 style='color: #2c3e50; border-bottom: 2px solid #27ae60; padding-bottom: 5px;'>Booking Details:</h3>
    
    <ul style='list-style-type: none; padding-left: 0; font-size: 15px; line-height: 1.6;'>
        <li><strong>Tour Name:</strong> {booking.Tour.TourTranslations.FirstOrDefault()?.Title ?? "N/A"}</li>
        <li><strong>Destination:</strong> {booking.Tour.Destination?.DestinationTranslations.FirstOrDefault()?.Name ?? "N/A"}</li>
        <li><strong>Start Date:</strong> {booking.Tour.Start_Date.ToString("dd MMMM yyyy")}</li>
        <li><strong>End Date:</strong> {booking.Tour.End_Date.ToString("dd MMMM yyyy")}</li>
        <li><strong>Number of Adults:</strong> {booking.AdultsCount}</li>
        <li><strong>Number of Children:</strong> {booking.ChildrenCount}</li>
        <li><strong>Total Price:</strong> <span style='color: #27ae60;'>{booking.TotalPrice.ToString("C")}</span></li>
    </ul>

    <p style='font-size: 16px; margin-top: 20px;'>
        If you have any questions or need further assistance, please do not hesitate to contact our support team.
    </p>

    <p style='font-size: 16px; margin-top: 30px;'>
        Thank you for choosing <strong>Yatriiworld</strong>. We look forward to serving you on your trip!
    </p>

    <p style='margin-top: 40px; font-weight: bold; color: #2c3e50; font-size: 16px;'>
        Best regards,<br/>
        Yatriiworld Team
    </p>
</div>
";



            await _emailService.SendMailAsync(booking.User.Email, subject, body, true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send booking confirmation email for booking ID {id}. Error: {ex.Message}");
        }

        var model = new BookingDetailVM
        {
            Booking = booking,
            Travellers = booking.Travellers.ToList()
        };

        return View(model);
    }
    [HttpPost("create-payment-intent/{bookingId}")]
    public async Task<IActionResult> CreatePaymentIntent(int bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null)
            return NotFound();

        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(booking.TotalPrice * 100),
            Currency = booking.CurrencyCode.ToLower(),
            PaymentMethodTypes = new List<string> { "card" }
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return Ok(new { clientSecret = paymentIntent.ClientSecret });
    }
    [HttpGet("paymentcancel/{bookingId}")]
    public async Task<IActionResult> PaymentCancel(int bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null) return NotFound();

        if (booking.Status == BookingStatus.Pending)
        {
            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();
        }

        return View();
    }


}




