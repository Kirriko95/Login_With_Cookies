using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grupp3_Login.Models;

namespace Grupp3_MVC.Controllers
{
    [Authorize] // Kräver att användaren är inloggad
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        // ✅ Visa alla konton i Index
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var accounts = await _accountService.GetAccountsAsync();
            return View(accounts);
        }

        // ✅ Uppdatera konto (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var account = await _accountService.GetAccountAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            var model = new UpdateAccountDto
            {
                UserName = account.UserName,
                Password = "" // Lämna lösenord tomt så att användaren kan ange ett nytt om de vill
            };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateAccountDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _accountService.UpdateAccountAsync(id, model);

            if (!success)
            {
                ModelState.AddModelError("", "Misslyckades att uppdatera kontot.");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        // ✅ Ta bort konto (Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _accountService.DeleteAccountAsync(id);

            if (!success)
            {
                TempData["Error"] = "Misslyckades att radera kontot.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Kontot raderades framgångsrikt.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _accountService.CreateAccountAsync(model);

            if (!success)
            {
                ModelState.AddModelError("", "Misslyckades att skapa kontot.");
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
