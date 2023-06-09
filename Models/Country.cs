using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace dotnet_products_rest_api.Models;

public partial class Country
{
    [Display(Name = "Código")]
    public uint Code { get; set; }

    [Display(Name = "Nombre")]
    public string Name { get; set; } = null!;

    [Display(Name = "Nombre del continente")]
    public string ContinentName { get; set; } = null!;

    [Display(Name = "Estado")]
    public sbyte State { get; set; }

    [JsonIgnore]
    public virtual ICollection<Merchant> Merchants { get; set; } = new List<Merchant>();

    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
