using System;
using System.Collections.Generic;

namespace WebStore.Models;

public partial class Source
{
    public int Sourceid { get; set; }

    public string? SourceName { get; set; }

    public string? ExternalLink { get; set; }

    public int? Genre { get; set; }

    public virtual Genre? GenreNavigation { get; set; }

    public virtual ICollection<Stocktake> Stocktakes { get; set; } = new List<Stocktake>();
}
