using System.Text.Json.Serialization;

namespace RestBookMVC__Cliente_.DTOs

{
    public class ReservationListDTO
    {
        [JsonPropertyName("reservationId")]
        public int Id { get; set; }

        [JsonPropertyName("restaurantName")]
        public string RestaurantName { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public DateTime ReservationDate { get; set; }

        [JsonPropertyName("people")]
        public int NumberOfPeople { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Pending";

        [JsonPropertyName("isReviewed")]
        public bool IsReviewed { get; set; }

    }
}
