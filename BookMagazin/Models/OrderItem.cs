using System;
using System.Collections.Generic;

namespace BookMagazin.Models;

public partial class OrderItem
{
    public int IdOrderItem { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public int? OrderId { get; set; }

    public int? BookId { get; set; }

    public virtual Book? Book { get; set; }

    public virtual Order? Order { get; set; }
}
