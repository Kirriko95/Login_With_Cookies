using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Grupp3_Login.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Grupp3_Login.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() { 
            return View();
        }

        // Login GET-action (visar inloggningssidan)
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl; // S�tt returnUrl i ViewBag f�r att kunna anv�nda den i vyn
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Account account, string returnUrl)
        {
            // Kolla anv�ndarnamn och l�senord
            bool accountValid = account.userName == "superadmin" && account.password == "Robert54321";

            // Fel anv�ndarnamn eller l�senord
            if (accountValid == false)
            {
                ViewBag.ErrorMessage = "Login failed: Wrong username or password";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            // Korrekt anv�ndarnamn och l�senord, logga in anv�ndaren
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, account.userName));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            // Ifall vi inte har en returnUrl, g� till Home
            if (String.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Admin", "Home");
            }

            // G� tillbaka via returnUrl
            return Redirect(returnUrl);
        }

        [Authorize]
        public IActionResult Admin()
        {
            return View();
        }

    }
}
