using System;
using System.Collections.Generic;

namespace WebStore.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Author { get; set; }

    public string? Description { get; set; }

    public int? Genre { get; set; }

    public int? SubGenre { get; set; }

    public DateTime? Published { get; set; }

    public string? LastUpdatedBy { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Genre? GenreNavigation { get; set; }

    public virtual User? LastUpdatedByNavigation { get; set; }

    public virtual ICollection<Stocktake> Stocktakes { get; set; } = new List<Stocktake>();
}
