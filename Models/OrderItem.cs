using System;
using System.Collections.Generic;

namespace dotnet_products_rest_api.Models;

public partial class OrderItem
{
    public uint OrderId { get; set; }

    public uint ProductId { get; set; }

    public uint? Quantity { get; set; }

    public sbyte State { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
