using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace dotnet_products_rest_api.Models;

public partial class User
{
    public uint Id { get; set; }

    [Display(Name = "Nombre completo")]
    public string FullName { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Display(Name = "Género")]
    public string Gender { get; set; } = null!;

    [JsonProperty(PropertyName = "DateOfBirth")]
    [Newtonsoft.Json.JsonConverter(typeof(DateOnlyConverter), "yyyy-MM-dd")]
    [Display(Name = "Fecha de nacimiento")]
    public DateOnly? DateOfBirth { get; set; }

    [Display(Name = "País")]
    public uint CountryCode { get; set; }

    [Display(Name = "Fecha de creación")]
    public DateOnly? CreatedAt { get; set; }

    [Display(Name = "Estado")]
    public sbyte State { get; set; }

    [Display(Name = "País")]
    [JsonPropertyName("country")]
    public virtual Country? CountryCodeNavigation { get; set; } = null;

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<Merchant> Merchants { get; set; } = new List<Merchant>();

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
