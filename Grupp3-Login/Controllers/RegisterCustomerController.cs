using Grupp3_Login.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Grupp3_Login.Controllers
{
    public class RegisterCustomerController : Controller
    {
        private readonly AccountService _accountService;

        public RegisterCustomerController(AccountService accountService)
        {
            _accountService = accountService;
        }

        // (Visar registreringsformuläret)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(CreateAccountDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _accountService.CreateCustomerAsync(model);

            if (!success)
            {
                ModelState.AddModelError("", "Misslyckades att skapa kontot.");
                return View(model);
            }

            return RedirectToAction("Login", "Home");
        }
    }
}
