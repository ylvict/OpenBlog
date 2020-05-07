using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using OpenBlog.AdminWeb.Services;

namespace OpenBlog.AdminWeb
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            var services = builder.Services;

            services.AddOptions();
            services.AddBlazoredLocalStorage();
            services.AddAuthorizationCore();
            services.AddScoped<ApiAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<ApiAuthenticationStateProvider>());
            services.AddScoped<IAuthService, AuthService>();
            
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient
                {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});
            Console.WriteLine($"App:{Assembly.GetEntryAssembly()?.FullName} BaseAddress: {builder.HostEnvironment.BaseAddress}");
            await builder.Build().RunAsync();

          
        }

        //public interface IAuthService
        //{
        //    Task<LoginResult> Login(LoginModel loginModel);
        //    Task Logout();
        //    Task<RegisterResult> Register(RegisterModel registerModel);
        //}

        //public class AuthService : IAuthService
        //{
        //    private readonly HttpClient _httpClient;
        //    private readonly AuthenticationStateProvider _authenticationStateProvider;
        //    private readonly ILocalStorageService _localStorage;

        //    public AuthService(HttpClient httpClient,
        //                       AuthenticationStateProvider authenticationStateProvider,
        //                       ILocalStorageService localStorage)
        //    {
        //        _httpClient = httpClient;
        //        _authenticationStateProvider = authenticationStateProvider;
        //        _localStorage = localStorage;
        //    }

        //    public async Task<RegisterResult> Register(RegisterModel registerModel)
        //    {
        //        var result = await _httpClient.PostJsonAsync<RegisterResult>("api/accounts", registerModel);

        //        return result;
        //    }

        //    public async Task<LoginResult> Login(LoginModel loginModel)
        //    {
        //        var loginAsJson = JsonSerializer.Serialize(loginModel);
        //        var response = await _httpClient.PostAsync("api/Login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
        //        var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            return loginResult;
        //        }

        //        await _localStorage.SetItemAsync("authToken", loginResult.Token);
        //        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Email);
        //        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

        //        return loginResult;
        //    }

        //    public async Task Logout()
        //    {
        //        await _localStorage.RemoveItemAsync("authToken");
        //        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        //        _httpClient.DefaultRequestHeaders.Authorization = null;
        //    }
        //}
    }
}