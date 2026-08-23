namespace Template.BlazorWasm.Frontend.App.Infrastructure.Authentication;

public sealed class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly TokenStore tokenStore;

    private readonly TimeProvider timeProvider;

    public JwtAuthenticationStateProvider(TokenStore tokenStore, TimeProvider timeProvider)
    {
        this.tokenStore = tokenStore;
        this.timeProvider = timeProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await tokenStore.GetTokenAsync();
        if (String.IsNullOrEmpty(token))
        {
            return Anonymous;
        }

        var principal = JwtParser.Parse(token, timeProvider.GetUtcNow());
        if (principal is null)
        {
            // 失効・不正トークンは破棄して未認証へ
            await tokenStore.ClearAsync();
            return Anonymous;
        }

        return new AuthenticationState(principal);
    }

    public void NotifyStateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
