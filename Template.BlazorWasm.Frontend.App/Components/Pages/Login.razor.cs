namespace Template.BlazorWasm.Frontend.App.Components.Pages;

public partial class Login
{
    private string name = string.Empty;

    private string password = string.Empty;

    private string? errorMessage;

    private bool processing;

    [Inject]
    public required ApiClient ApiClient { get; set; }

    [Inject]
    public required TokenStore TokenStore { get; set; }

    [Inject]
    public required JwtAuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public required NavigationManager Navigation { get; set; }

    [SupplyParameterFromQuery(Name = "returnUrl")]
    public string? ReturnUrl { get; set; }

    private async Task OnLoginClickAsync()
    {
        if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(password))
        {
            errorMessage = "IDとパスワードを入力してください";
            return;
        }

        errorMessage = null;
        processing = true;
        try
        {
            var response = await ApiClient.LoginAsync(new LoginRequest(name, password));
            await TokenStore.SetTokenAsync(response.Token);
            AuthenticationStateProvider.NotifyStateChanged();

            Navigation.NavigateTo(String.IsNullOrEmpty(ReturnUrl) ? string.Empty : ReturnUrl);
        }
        catch (ApiException ex) when (ex.StatusCode == 401)
        {
            errorMessage = "ログインに失敗しました";
        }
        finally
        {
            processing = false;
        }
    }
}
