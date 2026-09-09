namespace Template.BlazorWasm.Frontend.App.Infrastructure.Authentication;

public sealed class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly TokenStore tokenStore;

    private readonly TokenRefreshService tokenRefreshService;

    private readonly TimeProvider timeProvider;

    public JwtAuthenticationStateProvider(
        TokenStore tokenStore,
        TokenRefreshService tokenRefreshService,
        TimeProvider timeProvider)
    {
        this.tokenStore = tokenStore;
        this.tokenRefreshService = tokenRefreshService;
        this.timeProvider = timeProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await tokenStore.GetTokenAsync();
        var principal = String.IsNullOrEmpty(token) ? null : JwtParser.Parse(token, timeProvider.GetUtcNow());
        if (principal is not null)
        {
            return new AuthenticationState(principal);
        }

        // 失効・不正トークンはリフレッシュを試み、更新できなければ未認証へ
        var refreshed = await tokenRefreshService.RefreshAsync(token);
        if (String.IsNullOrEmpty(refreshed))
        {
            await tokenStore.ClearAsync();
            return Anonymous;
        }

        principal = JwtParser.Parse(refreshed, timeProvider.GetUtcNow());
        if (principal is null)
        {
            await tokenStore.ClearAsync();
            return Anonymous;
        }

        return new AuthenticationState(principal);
    }

    public void NotifyStateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
