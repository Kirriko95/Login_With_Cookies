using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Grupp3_Login.Models;

namespace Grupp3_Login.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Skicka loginförfrågan till API:et och få tillbaka resultatet
        public async Task<HttpResponseMessage> LoginAsync(LoginRequest model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7200/api/Authentication/login", model);
                return response;
            }
            catch (Exception ex)
            {
                // Hantera eventuella fel vid HTTP-förfrågan
                throw new Exception($"Ett fel inträffade vid API-anropet: {ex.Message}", ex);
            }
        }
    }
}
