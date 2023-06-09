using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dotnet_products_rest_api.Models;

public partial class Order
{
    public uint Id { get; set; }

	[Display(Name = "Usuario")]
    public uint? UserId { get; set; }

	[Display(Name = "Estado de la orden")]
    public string Status { get; set; } = null!;

	[Display(Name = "Fecha de creación")]
    public DateOnly? CreatedAt { get; set; }

	[Display(Name = "Estado")]
    public sbyte State { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User? User { get; set; }
}
