using Microsoft.EntityFrameworkCore;
using WebApp.Components;
using WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Přidání DbContext pro SQLite
builder.Services.AddDbContext<AccountDetailsContext>(options =>
    options.UseSqlite("Data Source=WebAppDb.db"));

// Add services for Razor Pages and Blazor Components
builder.Services.AddRazorComponents();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5237/"); // Set to your API base address
});


var app = builder.Build();

// Konfigurace HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Přidejte autentifikaci/autorizační middleware, pokud používáte
app.UseAuthentication();
app.UseAuthorization();

// Správné umístění antiforgery middleware mezi routingem a endpointy
app.UseAntiforgery(); 

// Mapování API controllerů
app.MapControllers();

// Mapování Razor Pages nebo Razor Components, pokud je používáte
app.MapRazorPages();
app.MapRazorComponents<App>();

app.Run();
