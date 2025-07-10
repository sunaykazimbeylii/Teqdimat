using Microsoft.AspNetCore.Mvc;

namespace TravelFinalProject.Controllers
{
    public class CurrencyController : Controller
    {

        [HttpPost]
        public IActionResult ChangeCurrency(string currencyCode)
        {
            if (!string.IsNullOrEmpty(currencyCode))
            {
                Response.Cookies.Append("SelectedCurrency", currencyCode, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });
            }


            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

    }
}
