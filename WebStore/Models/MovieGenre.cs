using System;
using System.Collections.Generic;

namespace WebStore.Models;

public partial class MovieGenre
{
    public int SubGenreId { get; set; }

    public string? Name { get; set; }
}
