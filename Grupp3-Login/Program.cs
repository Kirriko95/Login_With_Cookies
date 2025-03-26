using Grupp3_Login.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Lägg till session och cookie-baserad autentisering
builder.Services.AddDistributedMemoryCache(); // Cache för session
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "sessionCookie";
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Timeout för sessionen
    options.Cookie.IsEssential = true; // Gör sessionen nödvändig
    options.Cookie.SameSite = SameSiteMode.None;
});

// Lägg till HttpClient via IHttpClientFactory
builder.Services.AddHttpClient<AccountService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7200");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        AllowAutoRedirect = true,
        UseCookies = true,
        CookieContainer = new System.Net.CookieContainer()
    };
    return handler;
});

// Lägg till ApiService för DI
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<AccountService>();

// Lägg till autentisering med cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MVC_Cookie";
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Timeout för cookies
        options.SlidingExpiration = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
    });

// Lägg till MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();

// Använd autentisering och session
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
