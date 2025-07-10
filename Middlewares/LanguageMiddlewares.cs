using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace TravelFinalProject.Middlewares
{
    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;

        public LanguageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var langCode = context.Request.Query["langCode"].ToString();
            if (!string.IsNullOrWhiteSpace(langCode))
            {
                var culture = new CultureInfo(langCode);
                var requestCulture = new RequestCulture(culture);

                context.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(requestCulture),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddYears(1),
                        IsEssential = true,
                        HttpOnly = false,
                        Secure = false
                    }
                );
            }

            await _next(context);
        }

    }
}
