using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using RestBookMVC.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("RestBookAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7149/");
});

builder.Services.AddScoped<ApiService>();

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Localization";
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("pt"),
        new CultureInfo("en")
    };

    options.DefaultRequestCulture = new RequestCulture("pt");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // Cookie definido pelo LanguageController tem prioridade
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options => {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(RestBookMVC__Cliente_.SharedResource));
    });

var assembly = typeof(RestBookMVC__Cliente_.SharedResource).Assembly;
var recursos = assembly.GetManifestResourceNames();
foreach (var r in recursos)
{
    Console.WriteLine("RECURSO ENCONTRADO: " + r);
}

// Autenticação
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "RestBookSession";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

var app = builder.Build();

app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();

app.UseRequestLocalization();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
