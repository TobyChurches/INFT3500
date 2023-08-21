using System;
using System.Collections.Generic;

namespace WebStore.Models;

public partial class Patron
{
    public int UserId { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? Salt { get; set; }

    public string? HashPw { get; set; }

    public virtual ICollection<To> Tos { get; set; } = new List<To>();
}
