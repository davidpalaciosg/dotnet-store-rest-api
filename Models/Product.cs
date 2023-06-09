using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dotnet_products_rest_api.Models;

public partial class Product
{
    public uint Id { get; set; }

	[Display(Name = "Mercader")]
	public uint? MerchantId { get; set; }
	
    [Display(Name = "Nombre")]
	public string Name { get; set; } = null!;

	[Display(Name = "Precio")]
	public double Price { get; set; }

	[Display(Name = "Estado del producto")]
	public string Status { get; set; } = null!;
	
	[Display(Name = "Fecha de creación")]
	public DateOnly? CreatedAt { get; set; }

	[Display(Name = "Estado")]
	public sbyte State { get; set; }

	[Display(Name = "Mercader")]
	[System.Text.Json.Serialization.JsonIgnore]
	public virtual Merchant? Merchant { get; set; }
}
