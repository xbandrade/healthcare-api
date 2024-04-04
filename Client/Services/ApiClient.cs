using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Client.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private string? _token;

    public ApiClient()
    {
        _token = null;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://127.0.0.1:5138/")
        };
    }

    public async Task<string?> Login(string username, string password)
    {
        _token = null;
        var request = new
        {
            username,
            password
        };
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        try
        {
            var response = await _httpClient.PostAsync("auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                _token = await response.Content.ReadAsStringAsync();
            }
            return _token;
        }
        catch (HttpRequestException)
        {
            return "Exception";
        }
    }
}