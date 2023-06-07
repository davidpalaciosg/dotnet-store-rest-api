using System;
using System.Collections.Generic;

namespace dotnet_products_rest_api.Models;

public partial class Order
{
    public uint Id { get; set; }

    public uint? UserId { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly? CreatedAt { get; set; }

    public sbyte State { get; set; }

    public virtual User? User { get; set; }
}
