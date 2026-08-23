namespace Template.BlazorWasm.Frontend.App.Infrastructure.Authentication;

// ベースアドレス(=同居Backend)配下のリクエストにのみBearerトークンを付与する
public sealed class JwtAuthorizationMessageHandler : DelegatingHandler
{
    private readonly TokenStore tokenStore;

    private readonly Uri authorizedBase;

    public JwtAuthorizationMessageHandler(TokenStore tokenStore, NavigationManager navigation)
    {
        this.tokenStore = tokenStore;
        authorizedBase = new Uri(navigation.BaseUri);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if ((request.RequestUri is not null) && authorizedBase.IsBaseOf(request.RequestUri))
        {
            var token = await tokenStore.GetTokenAsync();
            if (!String.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
