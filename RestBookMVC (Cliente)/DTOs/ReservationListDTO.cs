using System.Text.Json.Serialization;

namespace RestBookMVC__Cliente_.DTOs

{
    public class ReservationListDTO
    {
        public int ReservationId { get; set; }

        public int RestaurantId { get; set; }

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
