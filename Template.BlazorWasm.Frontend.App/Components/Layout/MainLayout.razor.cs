namespace Template.BlazorWasm.Frontend.App.Components.Layout;

public partial class MainLayout
{
    [Inject]
    public required TokenStore TokenStore { get; set; }

    [Inject]
    public required JwtAuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public required NavigationManager Navigation { get; set; }

    private async Task OnLogoutClickAsync()
    {
        await TokenStore.ClearAsync();
        AuthenticationStateProvider.NotifyStateChanged();

        Navigation.NavigateTo("login");
    }
}
