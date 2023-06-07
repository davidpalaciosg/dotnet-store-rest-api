using System;
using System.Collections.Generic;

namespace dotnet_products_rest_api.Models;

public partial class Country
{
    public uint Code { get; set; }

    public string Name { get; set; } = null!;

    public string ContinentName { get; set; } = null!;

    public sbyte State { get; set; }

    public virtual ICollection<Merchant> Merchants { get; set; } = new List<Merchant>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
