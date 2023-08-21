using System;
using System.Collections.Generic;

namespace WebStore.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? Customer { get; set; }

    public string? StreetAddress { get; set; }

    public int? PostCode { get; set; }

    public string? Suburb { get; set; }

    public string? State { get; set; }

    public virtual To? CustomerNavigation { get; set; }
}
