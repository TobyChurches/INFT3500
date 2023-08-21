using System;
using System.Collections.Generic;

namespace WebStore.Models;

public partial class BookGenre
{
    public int SubGenreId { get; set; }

    public string? Name { get; set; }
}
