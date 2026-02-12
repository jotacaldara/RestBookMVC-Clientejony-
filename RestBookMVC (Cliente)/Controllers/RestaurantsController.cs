using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestBookMVC.Services;
using System.Security.Claims;
using RestBookMVC__Cliente_.DTOs;

public class RestaurantsController : Controller
{
    private readonly ApiService _api;
    public RestaurantsController(ApiService api) => _api = api;

    public async Task<IActionResult> Details(int id)
    {
        var restaurant =
            await _api.GetAsync<RestaurantDetailDTO>($"api/restaurants/{id}");

        if (restaurant == null)
            return NotFound();

        var reviews =
            await _api.GetAsync<List<ReviewDTO>>($"api/Reviews/restaurant/{id}");

        restaurant.Reviews = reviews ?? new List<ReviewDTO>();

        return View(restaurant);
    }


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Reserve(CreateReservationDTO reservation)
    {
        reservation.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var success = await _api.PostAsync("api/reservations", reservation);

        if (success)
        {
            TempData["Success"] = "Reserva realizada com sucesso!";
            return RedirectToAction("MyReservations", "Account");
        }

        ViewBag.Error = "Não foi possível realizar a reserva.";
        return RedirectToAction("Details", new { id = reservation.RestaurantId });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SubmitReview(CreateReviewDTO dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Dados da avaliação inválidos.";
            return RedirectToAction("MyReservations", "Account");
        }

        var success = await _api.PostAsync("api/Reviews", dto);

        if (success)
        {
            TempData["Success"] = "Obrigado! Sua avaliação foi enviada com sucesso.";
        }
        else
        {
            TempData["Error"] = "Não foi possível enviar a avaliação. Verifique se você já avaliou esta reserva.";
        }

        return RedirectToAction("MyReservations", "Account");
    }


}