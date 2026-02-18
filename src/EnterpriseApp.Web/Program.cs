using Blazored.LocalStorage;
using Blazored.Toast;
using EnterpriseApp.Web;
using EnterpriseApp.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HTTP Client for API communication
builder.Services.AddScoped(sp =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5001";
    return new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
});

// HTTP Client Service with authentication
builder.Services.AddScoped<HttpClientService>();

// Authentication
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

// Local Storage
builder.Services.AddBlazoredLocalStorage();

// Toast Notifications
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
