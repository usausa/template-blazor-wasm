namespace Template.BlazorWasm.Frontend.App.Components.Layout;

public partial class MainLayout
{
    [Inject]
    public required IHttpClientFactory HttpClientFactory { get; set; }

    [Inject]
    public required TokenStore TokenStore { get; set; }

    [Inject]
    public required JwtAuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public required NavigationManager Navigation { get; set; }

    private async Task OnLogoutClickAsync()
    {
        // サーバー側のリフレッシュトークンも失効させる(失敗してもログアウトは継続する)
        var refreshToken = await TokenStore.GetRefreshTokenAsync();
        if (!String.IsNullOrEmpty(refreshToken))
        {
            try
            {
                using var client = HttpClientFactory.CreateClient(ApiClientNames.Refresh);
                using var response = await client.PostAsJsonAsync(new Uri(ApiPaths.Logout, UriKind.Relative), new RefreshRequest(refreshToken));
                _ = response;
            }
            catch (HttpRequestException)
            {
                // Ignore
            }
        }

        await TokenStore.ClearAsync();
        AuthenticationStateProvider.NotifyStateChanged();

        Navigation.NavigateTo("login");
    }
}
