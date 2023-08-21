using System;
using System.Collections.Generic;

namespace WebStore.Models;

public partial class Genre
{
    public int GenreId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Source> Sources { get; set; } = new List<Source>();
}
