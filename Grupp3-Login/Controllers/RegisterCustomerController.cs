using Grupp3_Login.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Grupp3_Login.Controllers
{
    public class RegisterCustomerController : Controller
    {
        private readonly HttpClient _httpClient;

        public RegisterCustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        // (Visar registreringsformuläret)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //(Hantera registrering)
        [HttpPost]
        public async Task<IActionResult> RegisterCustomer(Account model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", model); // Om valideringen misslyckas, visa formuläret igen
            }

            var response = await _httpClient.PostAsJsonAsync("Authentication/register", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home"); // Gå tillbaka till startsidan
            }

            ModelState.AddModelError("", "Kunde inte skapa konto.");
            return View("Register", model);
        }
    }
}
