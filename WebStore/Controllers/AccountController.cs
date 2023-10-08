using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Models.ViewModels;

namespace WebStore.Controllers;

[Authorize(Roles = "Customer, Admin, Employee")]
public class AccountController : Controller
{
    private readonly StoreDbContext _context;
    private readonly UserManager<User> _userManager;

    public AccountController(StoreDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.FindByNameAsync(User.Identity?.Name);

        return View("Account", new EditAccountViewModel()
        {
            Username = user.UserName,
            Email = user.Email ?? "",
            Name = user.Name ?? ""
        });
    }

    [HttpPost]
    public async Task<IActionResult> Update(EditAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "The supplied data is invalid");
            return View("Account", model);
        }

        var user = await _userManager.FindByNameAsync(User.Identity?.Name);

        user.Email = model.Email;
        user.Email = _userManager.NormalizeEmail(model.Email);
        user.Name = model.Name;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Failed to update account.");
            return View("Account", model);
        }

        if (!string.IsNullOrEmpty(model.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var success = await _userManager.ResetPasswordAsync(user, token, AuthenticationController.GenerateHash(model.Password, user.Salt));

            if (!success.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to update password.");
                return View("Account", model);
            }
        }

        TempData.Add("AccountMessage", "Your account has been updated.");

        return RedirectToAction("Index");
    }
}
