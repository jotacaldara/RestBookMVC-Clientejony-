using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestBookMVC.Services;
using RestBookMVC__Cliente_.DTOs;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly ApiService _api;
    public AccountController(ApiService api) => _api = api;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var response = await _api.PostWithResultAsync<LoginResponseDTO>("api/auth/login", dto);

        if (response != null)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, response.Name.ToString()),
            new Claim(ClaimTypes.NameIdentifier, response.UserId.ToString()),
            new Claim(ClaimTypes.Role, response.Role.ToString())
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true 
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "E-mail ou senha inválidos.");
        return View(dto);
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> MyReservations()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var reservations = await _api.GetAsync<List<ReservationListDTO>>($"api/reservations/user/{userId}");

        return View(reservations ?? new List<ReservationListDTO>());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Reserve(int restaurantId, DateTime reservationDate, int numberOfPeople)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
            return Challenge();

        var dto = new CreateReservationDTO
        {
            RestaurantId = restaurantId,
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
            ReservationDate = reservationDate,
            NumberOfPeople = numberOfPeople
        };

        // Envia o DTO para a API
        var success = await _api.PostAsync("api/reservations", dto);

        if (success)
        {
            return RedirectToAction("MyReservations", "Account");
        }

        TempData["Error"] = "Não foi possível realizar a reserva.";
        return RedirectToAction("Details", new { id = restaurantId });
    }


    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        if (!ModelState.IsValid) return View(dto);

        // Chamamos a API para criar o usuário
        var result = await _api.PostAsync("api/auth/register", dto);

        if (result)
        {
            TempData["SuccessMessage"] = "Conta criada com sucesso! Agora você já pode fazer login.";
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", "Não foi possível realizar o cadastro. Verifique se o e-mail já existe.");
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // Boa prática de segurança
    public async Task<IActionResult> CancelReservation(int id)
    {
        var success = await _api.DeleteAsync($"api/reservations/{id}");

        if (success)
        {
            TempData["Success"] = "Reserva cancelada com sucesso!";
        }
        else
        {
            TempData["Error"] = "Não foi possível cancelar. Tente novamente.";
        }

        return RedirectToAction(nameof(MyReservations));
    }

}