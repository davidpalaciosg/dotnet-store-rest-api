using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace dotnet_products_rest_api.Models;

public partial class User
{
    public uint Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public uint CountryCode { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public sbyte State { get; set; }

	[JsonPropertyName("country")]
	public virtual Country CountryCodeNavigation { get; set; } = null!;

	[System.Text.Json.Serialization.JsonIgnore]
	public virtual ICollection<Merchant> Merchants { get; set; } = new List<Merchant>();

	[System.Text.Json.Serialization.JsonIgnore]
	public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
