using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using EnterpriseApp.Shared.Contracts;

namespace EnterpriseApp.Web.Services;

/// <summary>
/// HTTP client service with authentication token handling.
/// </summary>
public class HttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpClientService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Sends a GET request and returns the response.
    /// </summary>
    public async Task<ApiResponse<T>> GetAsync<T>(string url)
    {
        await AddAuthorizationHeader();
        var response = await _httpClient.GetAsync(url);
        return await ProcessResponse<T>(response);
    }

    /// <summary>
    /// Sends a POST request with a body and returns the response.
    /// </summary>
    public async Task<ApiResponse<T>> PostAsync<T>(string url, object data)
    {
        await AddAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync(url, data, _jsonOptions);
        return await ProcessResponse<T>(response);
    }

    /// <summary>
    /// Sends a PUT request with a body and returns the response.
    /// </summary>
    public async Task<ApiResponse<T>> PutAsync<T>(string url, object data)
    {
        await AddAuthorizationHeader();
        var response = await _httpClient.PutAsJsonAsync(url, data, _jsonOptions);
        return await ProcessResponse<T>(response);
    }

    /// <summary>
    /// Sends a DELETE request and returns the response.
    /// </summary>
    public async Task<ApiResponse<T>> DeleteAsync<T>(string url)
    {
        await AddAuthorizationHeader();
        var response = await _httpClient.DeleteAsync(url);
        return await ProcessResponse<T>(response);
    }

    /// <summary>
    /// Sends a POST request without expecting a specific response type.
    /// </summary>
    public async Task<ApiResponse<object>> PostAsync(string url, object data)
    {
        return await PostAsync<object>(url, data);
    }

    private async Task AddAuthorizationHeader()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task<ApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                return ApiResponse<T>.SuccessResponse(data!);
            }
            catch (JsonException)
            {
                return ApiResponse<T>.SuccessResponse(default!);
            }
        }

        try
        {
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _jsonOptions);
            return ApiResponse<T>.ErrorResponse(
                errorResponse?.Message ?? "An error occurred",
                (int)response.StatusCode);
        }
        catch (JsonException)
        {
            return ApiResponse<T>.ErrorResponse(
                $"Request failed with status {response.StatusCode}",
                (int)response.StatusCode);
        }
    }
}
