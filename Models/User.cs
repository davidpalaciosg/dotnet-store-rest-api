using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace dotnet_products_rest_api.Models;

public partial class User
{
    public uint Id { get; set; }

    public string FullName { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;

    public string Gender { get; set; } = null!;

    [JsonProperty(PropertyName = "dateOfBirth")]
    [JsonConverter(typeof(DateOnlyConverter), "yyyy-MM-dd")]
    public DateOnly? DateOfBirth { get; set; }

    public uint CountryCode { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public sbyte State { get; set; }

    public virtual Country CountryCodeNavigation { get; set; } = null!;

    public virtual ICollection<Merchant> Merchants { get; set; } = new List<Merchant>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
