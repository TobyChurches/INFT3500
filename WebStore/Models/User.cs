using Microsoft.AspNetCore.Identity;

namespace WebStore.Models;

public partial class User : IdentityUser
{
    public int UserId { get; set; }

    override public string UserName { get; set; } = null!;

    override public string? Email { get; set; }

    public string? Name { get; set; }

    public bool? IsAdmin { get; set; }

    public string? Salt { get; set; }

    public string? HashPw { get; set; }

    public bool? IsEmployee { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<To> Tos { get; set; } = new List<To>();
}
