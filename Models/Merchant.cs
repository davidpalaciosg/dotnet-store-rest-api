using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace dotnet_products_rest_api.Models;

public partial class Merchant
{
	public uint Id { get; set; }

	[Display(Name = "Nombre")]
	public string MerchantName { get; set; } = null!;

	[Display(Name = "Email Admin")]
	public uint? AdminId { get; set; }

	[Display(Name = "País")]
	public uint? CountryCode { get; set; }

	[Display(Name = "Fecha de creación")]
	public DateOnly? CreatedAt { get; set; }

	[Display(Name = "Estado")]
	public sbyte State { get; set; }

	public virtual User? Admin { get; set; }

	[Display(Name = "País")]
	[JsonPropertyName("country")]
	public virtual Country? CountryCodeNavigation { get; set; }

	public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
