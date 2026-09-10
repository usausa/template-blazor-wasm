namespace Template.BlazorWasm.Frontend.App.Infrastructure.Authentication;

// ベースアドレス(=同居Backend)配下のリクエストにのみBearerトークンを付与する。
// 401が返った場合はリフレッシュトークンで1回だけ更新して再送する
public sealed class JwtAuthorizationMessageHandler : DelegatingHandler
{
    private readonly TokenStore tokenStore;

    private readonly TokenRefreshService tokenRefreshService;

    private readonly Uri authorizedBase;

    public JwtAuthorizationMessageHandler(
        TokenStore tokenStore,
        TokenRefreshService tokenRefreshService,
        NavigationManager navigation)
    {
        this.tokenStore = tokenStore;
        this.tokenRefreshService = tokenRefreshService;
        authorizedBase = new Uri(navigation.BaseUri);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if ((request.RequestUri is null) || !authorizedBase.IsBaseOf(request.RequestUri))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var token = await tokenStore.GetTokenAsync();
        SetAuthorization(request, token);

        var response = await base.SendAsync(request, cancellationToken);
        if ((response.StatusCode != HttpStatusCode.Unauthorized) || String.IsNullOrEmpty(token))
        {
            return response;
        }

        var refreshed = await tokenRefreshService.RefreshAsync(token, cancellationToken);
        if (String.IsNullOrEmpty(refreshed))
        {
            return response;
        }

        // 再送は1回だけ。更新後も401ならそのまま呼び出し元へ返す
        response.Dispose();

        using var retry = await CloneAsync(request, cancellationToken);
        SetAuthorization(retry, refreshed);
        return await base.SendAsync(retry, cancellationToken);
    }

    private static void SetAuthorization(HttpRequestMessage request, string? token) =>
        request.Headers.Authorization = String.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);

    // HttpRequestMessageは1回しか送信できないため、再送用に複製する
    private static async ValueTask<HttpRequestMessage> CloneAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri) { Version = request.Version };

        if (request.Content is not null)
        {
            var buffer = await request.Content.ReadAsByteArrayAsync(cancellationToken);
            var content = new ByteArrayContent(buffer);
            foreach (var header in request.Content.Headers)
            {
                content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            clone.Content = content;
        }

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var property in request.Options)
        {
            clone.Options.TryAdd(property.Key, property.Value);
        }

        return clone;
    }
}
