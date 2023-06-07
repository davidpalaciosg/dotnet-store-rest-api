using System;
using System.Collections.Generic;

namespace dotnet_products_rest_api.Models;

public partial class Product
{
    public uint Id { get; set; }

    public uint? MerchantId { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly? CreatedAt { get; set; }

    public sbyte State { get; set; }

    public virtual Merchant? Merchant { get; set; }
}
