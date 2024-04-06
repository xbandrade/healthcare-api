using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return _token;
        }
        catch (HttpRequestException)
        {
            return "Exception";
        }
    }

    public async Task<List<T>?> GetAllItems<T>(string route)
    {
        try
        {
            if (_token is not null)
            {
                var response = await _httpClient.GetAsync(route);
                if (response.IsSuccessStatusCode)
                {
                    var itemsJson = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<List<T>>(itemsJson);
                    return items;
                }
            }
            MessageBox.Show("Invalid authentication token");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Exception raised on request: " + ex.Message);
        }
        return null;
    }

    public async Task<T?> GetItemById<T>(string route, int id)
    {
        try
        {
            if (_token is null)
            {
                MessageBox.Show("Invalid authentication token");
                return default;
            }
            var response = await _httpClient.GetAsync($"{route}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var itemJson = await response.Content.ReadAsStringAsync();
                var item = JsonConvert.DeserializeObject<T>(itemJson);
                return item;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Exception raised on request: " + ex.Message);
        }
        return default;
    }

    public async Task<string> Post(string route, string json)
    {
        try
        {
            if (_token is null)
            {
                MessageBox.Show("Invalid authentication token");
                return "Invalid Token";
            }
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(route, content);
            if (response.IsSuccessStatusCode)
            {
                return "Ok";
            }
            string responseContent = await response.Content.ReadAsStringAsync();
            JObject jsonResponse = JObject.Parse(responseContent);
            if (jsonResponse.TryGetValue("errors", out JToken? errorsToken))
            {
                return string.Join("\n", errorsToken.Children().SelectMany(c => c.First().Values<string>()));
            }
            return "Invalid request, please verify if all fields are valid";
        }
        catch (Exception ex)
        {
            MessageBox.Show("Exception raised on request: " + ex.Message);
            return "Exception raised on request: " + ex.Message;
        }
    }

    public async Task<string> Patch(string route, string json, int id)
    {
        try
        {
            if (_token is null)
            {
                MessageBox.Show("Invalid authentication token");
                return "Invalid Token";
            }
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{route}/{id}");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return "Ok";
            }
            string responseContent = await response.Content.ReadAsStringAsync();
            JObject jsonResponse = JObject.Parse(responseContent);
            if (jsonResponse.TryGetValue("errors", out JToken? errorsToken))
            {
                return string.Join("\n", errorsToken.Children().SelectMany(c => c.First().Values<string>()));
            }
            return "Invalid request, please verify if all fields are valid";
        }
        catch (Exception ex)
        {
            MessageBox.Show("Exception raised on request: " + ex.Message);
            return "Exception raised on request: " + ex.Message;
        }
    }
}
