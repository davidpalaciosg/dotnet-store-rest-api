using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dotnet_products_rest_api.Models;

public partial class OrderItem
{
    [Display(Name = "# Orden")]
    public uint OrderId { get; set; }

	[Display(Name = "Producto")]
    public uint ProductId { get; set; }

    [Display(Name = "Cantidad")]
    public uint? Quantity { get; set; }

    public sbyte State { get; set; }

	[Display(Name = "# Orden")]
    [System.Text.Json.Serialization.JsonIgnore]
	public virtual Order Order { get; set; } = null!;
    
    [Display(Name = "Producto")]
    [System.Text.Json.Serialization.JsonIgnore]
	public virtual Product Product { get; set; } = null!;
}
