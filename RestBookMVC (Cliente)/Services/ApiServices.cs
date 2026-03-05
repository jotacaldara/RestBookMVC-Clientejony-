using System.Net.Http.Json;

namespace RestBookMVC.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _clientName = "RestBookAPI";

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var client = _httpClientFactory.CreateClient("RestBookAPI");
            var response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();

            return default;
        }

        public async Task<bool> PostAsync<T>(string endpoint, T data)
        {
            var client = _httpClientFactory.CreateClient("RestBookAPI");
            var response = await client.PostAsJsonAsync(endpoint, data);
            return response.IsSuccessStatusCode;

        }

        // POST (Com Resultado): Essencial para LOGIN e REGISTRO
        // Ele envia o DTO e retorna o objeto que a API respondeu
        public async Task<T?> PostWithResultAsync<T>(string endpoint, object data)
        {
            var client = _httpClientFactory.CreateClient("RestBookAPI");
            var response = await client.PostAsJsonAsync(endpoint, data);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            return default;
        }

        public async Task<(bool Success, string? ErrorMessage)> PostWithDetailAsync<T>(string endpoint, T data)
        {
            var client = _httpClientFactory.CreateClient(_clientName);
            var response = await client.PostAsJsonAsync(endpoint, data);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await response.Content.ReadAsStringAsync();
            return (false, $"HTTP {(int)response.StatusCode}: {error}");
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var client = _httpClientFactory.CreateClient(_clientName);
            var response = await client.DeleteAsync(endpoint);

            return response.IsSuccessStatusCode;
        }


    }
}