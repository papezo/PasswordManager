using Microsoft.EntityFrameworkCore;
using WebApp.Components;
using WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Přidání DbContext pro SQLite
builder.Services.AddDbContext<AccountDetailsContext>(options =>
    options.UseSqlite("Data Source=WebAppDb.db"));

// Přidání Razor Components a povolení interaktivního serverového renderování
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // Toto je klíčové pro správnou konfiguraci

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5237/"); // Změňte podle potřeby
});

// Přidejte antiforgery služby
builder.Services.AddAntiforgery(options =>
{
    // Můžete nastavit volitelné možnosti zde
    options.HeaderName = "X-CSRF-TOKEN";
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

// Přidejte autentifikaci/autorizační middleware
app.UseAuthentication();
app.UseAuthorization();

// **Umístěte app.UseAntiforgery() zde, mezi UseRouting a UseEndpoints**
app.UseAntiforgery();

app.MapControllers();
app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
