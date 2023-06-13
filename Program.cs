using dotnet_products_rest_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add MariaDB database
builder.Services.AddDbContext<StoreDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MariaDbConnectionString");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Store APP - ASP.NET MVC",
        Version = "v1",
        Description = "Store App is an application that allows you to manage the products, merchants, orders, users, and countries of a store. It is a project developed in ASP.NET MVC with C# and MariaDB. It uses Entity Framework Core as ORM and Bootstrap for the frontend.",
        Contact = new OpenApiContact
        {
            Name = "David Enrique Palacios García",
            Email = "paladavid@hotmail.com",
            Url = new Uri("https://github.com/davidpalaciosg")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store APP - ASP.NET MVC");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
