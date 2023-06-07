using System;
using System.Collections.Generic;

namespace dotnet_products_rest_api.Models;

public partial class Merchant
{
    public uint Id { get; set; }

    public string MerchantName { get; set; } = null!;

    public uint? AdminId { get; set; }

    public uint? CountryCode { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public sbyte State { get; set; }

    public virtual User? Admin { get; set; }

    public virtual Country? CountryCodeNavigation { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
