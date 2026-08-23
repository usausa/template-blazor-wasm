using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

using Template.BlazorWasm.Frontend.App.Infrastructure.Authentication;
using Template.BlazorWasm.Frontend.App.Services;

//--------------------------------------------------------------------------------
// Configure builder
//--------------------------------------------------------------------------------
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<Template.BlazorWasm.Frontend.App.Components.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// System
builder.Services.AddSingleton(TimeProvider.System);

// Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<TokenStore>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(static p => p.GetRequiredService<JwtAuthenticationStateProvider>());

// API client (トークンはベースアドレス配下のリクエストにのみ付与)
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
builder.Services
    .AddHttpClient(ApiClientNames.Default, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();
builder.Services.AddScoped(static p => new ApiClient(p.GetRequiredService<IHttpClientFactory>().CreateClient(ApiClientNames.Default)));

// UI
builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
