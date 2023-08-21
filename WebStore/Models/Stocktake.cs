using System;
using System.Collections.Generic;

namespace WebStore.Models;

public partial class Stocktake
{
    public int ItemId { get; set; }

    public int? SourceId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public double? Price { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Source? Source { get; set; }
}
