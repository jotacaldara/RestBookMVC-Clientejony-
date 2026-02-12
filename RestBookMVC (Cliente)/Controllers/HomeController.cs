using Microsoft.AspNetCore.Mvc;
using RestBookMVC.Services;
using RestBookMVC__Cliente_.DTOs;

public class HomeController : Controller
{
    private readonly ApiService _api;
    public HomeController(ApiService api) => _api = api;

    public async Task<IActionResult> Index(string city)
    {
        var url = string.IsNullOrEmpty(city) ? "api/restaurants" : $"api/restaurants?city={city}";
        var restaurantes = await _api.GetAsync<List<RestaurantListDTO>>(url);
        return View(restaurantes);
    }
}