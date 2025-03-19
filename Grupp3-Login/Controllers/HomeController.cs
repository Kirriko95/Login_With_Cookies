using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Grupp3_Login.Models;
using Grupp3_Login.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

public class HomeController : Controller
{
    private readonly ApiService _apiService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApiService apiService, ILogger<HomeController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // 🏠 Visa loginformuläret
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Admin()
    {
        // Kontrollera om användaren är inloggad via cookies
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login"); // Om ej inloggad, omdirigera till login
        }

        return View();
    }

    // ✅ Hantera inloggning via API:et och skapa cookie
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model); // Om formuläret är felaktigt, visa det igen
        }

        // 🔹 Skicka loginförfrågan till API:et
        var response = await _apiService.LoginAsync(model);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Felaktigt användarnamn eller lösenord.";
            return View("Index");
        }

        // 🔹 Läs API-svaret som en `LoginResponse`
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (result == null || string.IsNullOrEmpty(result.Role))
        {
            _logger.LogError("API-svaret kunde inte deserialiseras korrekt!");
            ViewBag.Error = "Något gick fel. Försök igen";
            return View("Index"); // Visa login igen om något gick fel
        }

        // Skapa cookie med användartoken
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.UserName),
            new Claim(ClaimTypes.Role, result.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Logga in användaren med cookie
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        _logger.LogInformation($"Användaren {model.UserName} loggade in.");

        // Omdirigera beroende på roll
        return result.Role == "Admin" ? RedirectToAction("Admin") : RedirectToAction("Dashboard");
    }

    // 🔒 Skyddad vy (Dashboard)
    [Authorize] // Kräver autentisering
    public IActionResult Dashboard()
    {
        // Kontrollera om användaren är inloggad via cookies
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login"); // Skicka tillbaka till login om ej inloggad
        }

        return View();
    }

    // 🚪 Logga ut
    public async Task<IActionResult> Logout()
    {
        // Logga ut användaren och rensa cookies
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login"); // Omdirigera till login
    }
}
