using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Grupp3_Login.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Grupp3_MVC.Controllers
{
    [Authorize] // Kräver att användaren är inloggad
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7200/api/Account";  // URL för ditt API

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Hjälpmetod för att lägga till JWT-token i headers
        private void AddAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // ✅ Visa alla konton i Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AddAuthorizationHeader();

            var response = await _httpClient.GetAsync(_apiUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Home"); // Om token är ogiltig, skicka användaren till login
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Kunde inte hämta konton.");
                return View(new List<Account>());
            }

            try
            {
                var accounts = await response.Content.ReadFromJsonAsync<List<Account>>();
                return View(accounts);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Fel vid deserialisering: {ex.Message}");
                return View(new List<Account>());
            }
        }

        // ✅ Registrera kund
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(Account model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/register", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Kunde inte skapa konto.");
            return View(model);
        }

        // ✅ Skapa Employee (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Account model)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/create-employee", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Kunde inte skapa employee-konto.");
            return View(model);
        }

        // ✅ Uppdatera konto (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Home");
            }

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var account = await response.Content.ReadFromJsonAsync<Account>();
            return View(account);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Account model)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"{_apiUrl}/{id}", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Kunde inte uppdatera konto.");
            return View(model);
        }

        // ✅ Ta bort konto (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Kunde inte ta bort konto.");
            return RedirectToAction("Index");
        }
    }
}
