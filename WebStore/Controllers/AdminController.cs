using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebStore.Models;
using WebStore.Models.ViewModels;

namespace WebStore.Controllers;

public class AdminController : Controller
{
    private readonly StoreDbContext _context;
    public AdminController(StoreDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult EditItem(int id)
    {
        var vm = _context.Products
            .Where(x => x.Id == id)
            .Select(p => new AdminProductViewModel()
            {
                Product = p,
                Stocktake = p.Stocktakes
                    .Select(s => new Stocktake
                    {
                        ItemId = s.ItemId,
                        SourceId = s.SourceId,
                        ProductId = s.ProductId,
                        Quantity = s.Quantity,
                        Price = s.Price,
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefault();

        if (vm.Product == null)
        {
            return RedirectToAction("AddItem");
        }

        vm.Genres = _context.Genres
            .Select(g => new GenreViewModel
            {
                Genre = new Genre
                {
                    GenreId = g.GenreId,
                    Name = g.Name,
                },
            })
            .ToList();

        vm.Sources = _context.Sources
            .ToList();

        PopulateSubGenres(vm.Genres);

        return View(vm);
    }

    private void PopulateSubGenres(List<GenreViewModel> genres)
    {
        var genreToSubGenreMap = new Dictionary<string, Func<List<Dictionary<int, string>>>>
    {
        { "Book", () => GetSubGenres(_context.BookGenres) },
        { "Movie", () => GetSubGenres(_context.MovieGenres) },
        { "Game", () => GetSubGenres(_context.GameGenres) }
    };

        foreach (var genre in genres)
        {
            foreach (var key in genreToSubGenreMap.Keys)
            {
                if (genre.Genre.Name.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    genre.SubGenre = genreToSubGenreMap[key]();
                    break;
                }
            }
        }
    }

    private List<Dictionary<int, string>> GetSubGenres(IEnumerable<dynamic> subGenreCollection)
    {
        return subGenreCollection
            .Select(bg => new Dictionary<int, string>
            {
                { bg.SubGenreId, bg.Name }
            })
            .ToList();
    }


    [HttpPost]
    public IActionResult EditItem(AdminProductViewModel updatedProduct)
    {
        if (ModelState.IsValid)
        {
            var item = _context.Products.Find(updatedProduct.Product.Id);

            item.Name = updatedProduct.Product.Name;
            item.Author = updatedProduct.Product.Author;
            item.Description = updatedProduct.Product.Description;
            item.Genre = updatedProduct.Product.Genre;
            item.SubGenre = updatedProduct.Product.SubGenre;
            item.Published = updatedProduct.Product.Published;

            var stocktake = _context.Stocktakes
                .Where(s => s.ProductId == item.Id)
                .FirstOrDefault();

            if (stocktake == null)
                stocktake = new Stocktake();

            stocktake.Price = updatedProduct.Stocktake.Price;
            stocktake.Quantity = updatedProduct.Stocktake.Quantity;
            stocktake.SourceId = updatedProduct.Source.Sourceid;

            _context.SaveChanges();
            TempData["Added"] = "Item updated successfully";
        }
        else
        {
            updatedProduct.Genres = _context.Genres
            .Select(g => new GenreViewModel
            {
                Genre = new Genre
                {
                    GenreId = g.GenreId,
                    Name = g.Name,
                },
            })
            .ToList();

            updatedProduct.Sources = _context.Sources
                .ToList();

            return View(updatedProduct);
        }

        return RedirectToAction("EditItem", new { id = updatedProduct.Product.Id });
    }

    [HttpGet]
    public IActionResult AddItem()
    {
        var vm = new AdminProductViewModel
        {
            Product = new Product(),
            Genres = _context.Genres
                .Select(g => new GenreViewModel
                {
                    Genre = new Genre
                    {
                        GenreId = g.GenreId,
                        Name = g.Name,
                    },
                })
                .ToList()
        };

        PopulateSubGenres(vm.Genres);

        return View("EditItem", vm);
    }


    [HttpPost]
    public IActionResult AddItem(Product newProduct)
    {
        if (ModelState.IsValid)
        {
            var item = new Product();
            item.Name = newProduct.Name;
            item.Author = newProduct.Author;
            item.Description = newProduct.Description;
            item.Genre = newProduct.Genre;
            item.SubGenre = newProduct.SubGenre;
            item.Published = newProduct.Published;

            _context.Products.Add(item);
            _context.SaveChanges();
            TempData["Added"] = "Item Added successfully";

            newProduct.Id = _context.Products
                .Where(p => p.Name == newProduct.Name
                            && p.Author == newProduct.Author)
                .Select(p => p.Id)
                .FirstOrDefault();
        }
        else
        {
            return View(newProduct);
        }

        return RedirectToAction("EditItem", new { id = newProduct.Id });
    }

    [HttpGet]
    [HttpPost]
    public ActionResult ItemList(string searchQuery)
    {
        searchQuery = searchQuery.IsNullOrEmpty() ? "" : searchQuery.ToLower().Trim();
        var results = _context.Products
            .Where(p => p.Id.ToString().Equals(searchQuery)
                        || p.Name.ToLower().Contains(searchQuery)
                        || p.Author.ToLower().Contains(searchQuery)
                        || p.Description.ToLower().Contains(searchQuery))
            .Select(p => new ProductViewModel()
            {
                Product = p,
                Quantity = p.Stocktakes
                    .Select(s => s.Quantity)
                    .FirstOrDefault()
            })
            .ToList();

        return View(results);
    }

    [HttpGet]
    [HttpPost]
    public IActionResult UserList(string searchQuery)
    {
        searchQuery = searchQuery.IsNullOrEmpty() ? "" : searchQuery.ToLower().Trim();
        var results = _context.Users
            .Where(p =>    p.UserId.ToString().Contains(searchQuery)
                        || p.Name.ToLower().Contains(searchQuery)
                        || p.UserName.ToLower().Contains(searchQuery)
                        || p.Email.ToLower().Contains(searchQuery))
            .ToList();

        return View(results);
    }

    [HttpGet]
    public IActionResult EditUser(string id)
    {
        var user = _context.Users.Find(id);

        if (user == null)
        {
            return RedirectToAction("AddUser");
        }

        return View(user);
    }

    [HttpPost]
    public IActionResult EditUser(User updatedUser)
    {
        var user = _context.Users.Find(updatedUser.UserName);

        if (user == null)
        {
              return RedirectToAction("AddUser");
        }

        if (ModelState.IsValid)
        {
            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.IsAdmin = updatedUser.IsAdmin;

            if(!string.IsNullOrEmpty(updatedUser.HashPw))
            {
                user.HashPw = AuthenticationController.GenerateHash(updatedUser.HashPw, user.Salt);
            }

            _context.SaveChanges();
            TempData["Added"] = "User updated successfully";
        }
        else
        {
            return View(updatedUser);
        }

        return RedirectToAction("EditUser", new { id = updatedUser.UserName });
    }

    [HttpGet]
    public IActionResult AddUser()
    {
        return View("EditUser", new User());
    }

    [HttpPost]
    public IActionResult AddUser(User newUser)
    {
        if(_context.Users.Any(x => x.UserName == newUser.UserName))
        {
            ModelState.AddModelError("", "Username already exists. Please try another.");
            return View("EditUser", newUser);
        }

        if (ModelState.IsValid)
        {
            var item = new User();
            item.Name = newUser.Name;
            item.Email = newUser.Email;
            item.UserName = newUser.UserName;
            item.IsAdmin = newUser.IsAdmin;
            item.Salt = AuthenticationController.GenerateSalt();
            item.HashPw = AuthenticationController.GenerateHash(newUser.HashPw, item.Salt);
            _context.Users.Add(item);

            _context.SaveChanges();
            TempData["Added"] = "Item Added successfully";
        }
        else
        {
            return View("EditUser",newUser);
        }

        return RedirectToAction("EditUser", new { id = newUser.UserName });
    }

    [HttpGet]
    public IActionResult DeleteUser(string id)
    {
        var user = _context.Users.Find(id);

        if (user == null)
        {
            TempData["Deleted"] = "The user you attempted to delete does not exist.";
            return RedirectToAction("UserList");
        }

        _context.Users.Remove(user);
        _context.SaveChanges();


        TempData["Deleted"] = "User deleted successfully";
        return RedirectToAction("UserList", "Admin", new { searchQuery = ""});
    }

    [HttpGet]
    public IActionResult DeleteItem(int id)
    {
        var product = _context.Products.Find(id);

        if (product == null)
        {
            TempData["Deleted"] = "The item you attempted to delete does not exist.";
            return RedirectToAction("ItemList");
        }

        _context.Products.Remove(product);
        _context.SaveChanges();


        TempData["Deleted"] = "Item deleted successfully";
        return RedirectToAction("ItemList", "Admin", new { searchQuery = "" });
    }
}
