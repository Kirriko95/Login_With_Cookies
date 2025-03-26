using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Grupp3_Login.Models;
using Microsoft.Extensions.Configuration;

public class AccountService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AccountService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

    }

    // ✅ Hämta alla konton
    public async Task<List<AccountView>> GetAccountsAsync()
    {
        var response = await _httpClient.GetAsync($"{_configuration["ApiBaseUrl"]}/api/Account");

        if (!response.IsSuccessStatusCode)
        {
            return new List<AccountView>();
        }

        return await response.Content.ReadFromJsonAsync<List<AccountView>>();
    }
    // ✅ Hämta ett specifikt konto
    public async Task<AccountView> GetAccountAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{_configuration["ApiBaseUrl"]}/api/Account/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return null; // Eller hantera bättre beroende på din applikation
        }

        return await response.Content.ReadFromJsonAsync<AccountView>();
    }

    // ✅ Uppdatera konto
    public async Task<bool> UpdateAccountAsync(int id, UpdateAccountDto updateAccountDto)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_configuration["ApiBaseUrl"]}/api/Account/update/{id}", updateAccountDto);

        return response.IsSuccessStatusCode;
    }

    // ✅ Ta bort konto
    public async Task<bool> DeleteAccountAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_configuration["ApiBaseUrl"]}/api/Account/{id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateAccountAsync(CreateAccountDto createAccountDto)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_configuration["ApiBaseUrl"]}/api/Account/create-employee", createAccountDto);
        return response.IsSuccessStatusCode;
    }
}
