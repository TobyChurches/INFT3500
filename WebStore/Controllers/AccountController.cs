using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using WebStore.Models;
using WebStore.Models.ViewModels;

namespace WebStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly StoreDbContext _context;

        public AccountController(StoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Get account information from the session
            var id = 11;

            var user = _context.Users
                .Where(x => x.UserId == id)
                .Select(x => new EditAccountViewModel()
                {
                    Username = x.UserName,
                    Email = x.Email,
                    Name = x.Name
                })
                .FirstOrDefault();

            return View("Account", user);
        }

        [HttpPost]
        public IActionResult Update(EditAccountViewModel model)
        {
            // Get account information from the session
            var id = "TobyChurches";

            var user = _context.Users.Find(id);

            user.Email = model.Email;
            user.Name = model.Name;

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.HashPw = AuthenticationController.GenerateHash(model.Password, user.Salt);
            }

            _context.SaveChanges();

            TempData.Add("AccountMessage", "Your account has been updated.");

            return RedirectToAction("Index");
        }

    }
}
