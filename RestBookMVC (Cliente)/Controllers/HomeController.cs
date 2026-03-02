using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RestBookMVC.Services;
using RestBookMVC__Cliente_;
using RestBookMVC__Cliente_.DTOs;

public class HomeController : Controller
{
    private readonly ApiService _api;

    private readonly IStringLocalizer<RestBookMVC__Cliente_.SharedResource> _localizer;

    public HomeController(IStringLocalizer<SharedResource> localizer, ApiService api)
    {
        _api = api;
        _localizer = localizer;
    }

    public IActionResult TestLocalization()
    {
        // 1. Listar TODOS os recursos embebidos na DLL
        var assembly = typeof(SharedResource).Assembly;
        var recursos = assembly.GetManifestResourceNames();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== RECURSOS EMBEBIDOS NA DLL ===");
        foreach (var r in recursos)
            sb.AppendLine("  → " + r);

        sb.AppendLine();

        // 2. Testar o localizer
        var valor = _localizer["Home_HeroTitle"];
        sb.AppendLine("=== TESTE DO LOCALIZER ===");
        sb.AppendLine($"Chave:             Home_HeroTitle");
        sb.AppendLine($"Valor:             {valor.Value}");
        sb.AppendLine($"NãoEncontrado:     {valor.ResourceNotFound}");
        sb.AppendLine($"ProcurouEm:        {valor.SearchedLocation}");

        return Content(sb.ToString(), "text/plain");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
    public async Task<IActionResult> Index(string city)
    {
        var url = string.IsNullOrEmpty(city) ? "api/restaurants" : $"api/restaurants?city={city}";
        var restaurantes = await _api.GetAsync<List<RestaurantListDTO>>(url);
        return View(restaurantes);
    }
}