using Grupp3_Login.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Lägg till session och cookie-baserad autentisering
builder.Services.AddDistributedMemoryCache(); // Cache för session
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Grupp3_Login.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Timeout för sessionen
    options.Cookie.IsEssential = true; // Gör sessionen nödvändig
});

// Lägg till HttpClient via IHttpClientFactory
builder.Services.AddHttpClient();

// Lägg till ApiService för DI
builder.Services.AddScoped<ApiService>();

// Lägg till autentisering med cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Timeout för cookies
    });

// Lägg till MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Använd autentisering och session
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
