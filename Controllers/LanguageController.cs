using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace TravelFinalProject.Controllers
{
    public class LanguageController : Controller
    {
        public IActionResult Change(string culture, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "/";

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), Path = "/" }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
