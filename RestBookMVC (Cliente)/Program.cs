using Microsoft.AspNetCore.Authentication.Cookies;
using RestBookMVC.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpClient("RestBookAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7149/"); // URL da sua API
});

builder.Services.AddScoped<ApiService>();
builder.Services.AddControllersWithViews();
// Adicionar suporte a Sessão (para guardar o UserId e Nome do cliente logado)

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Onde redirecionar se não estiver logado
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "RestBookSession"; // Nome do cookie
        options.Cookie.HttpOnly = true; // Segurança: impede acesso via JavaScript
        options.ExpireTimeSpan = TimeSpan.FromSeconds(300); // Duração da sessão
        options.SlidingExpiration = true; // Renova o tempo se o usuário estiver ativo
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true; // Ativa a necessidade de consentimento
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

var app = builder.Build();

app.UseStaticFiles();
app.UseCookiePolicy(); // Ativa a política de cookies
app.UseRouting();
app.UseAuthentication(); // Quem é você?
app.UseAuthorization();  // O que você pode fazer?

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();