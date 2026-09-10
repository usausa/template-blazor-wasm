namespace Template.BlazorWasm.Frontend.App.Infrastructure.Authentication;

// アクセストークンの期限切れを検出したときに、リフレッシュトークンで再取得する。
// 認証ハンドラーを通さない専用HttpClientを使い、再帰的な更新を避ける
public sealed class TokenRefreshService : IDisposable
{
    private readonly IHttpClientFactory httpClientFactory;

    private readonly TokenStore tokenStore;

    private readonly SemaphoreSlim semaphore = new(1, 1);

    public TokenRefreshService(
        IHttpClientFactory httpClientFactory,
        TokenStore tokenStore)
    {
        this.httpClientFactory = httpClientFactory;
        this.tokenStore = tokenStore;
    }

    public async ValueTask<string?> RefreshAsync(string? staleToken, CancellationToken cancellationToken = default)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            // 待機中に他の要求が更新済みなら、その結果をそのまま使う
            var current = await tokenStore.GetTokenAsync();
            if (!String.IsNullOrEmpty(current) && (current != staleToken))
            {
                return current;
            }

            var refreshToken = await tokenStore.GetRefreshTokenAsync();
            if (String.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            using var client = httpClientFactory.CreateClient(ApiClientNames.Refresh);
            using var response = await client.PostAsJsonAsync(
                new Uri(ApiPaths.Refresh, UriKind.Relative),
                new RefreshRequest(refreshToken),
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // リフレッシュトークンも失効している場合は未認証へ落とす
                await tokenStore.ClearAsync();
                return null;
            }

            var body = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);
            if (body is null)
            {
                await tokenStore.ClearAsync();
                return null;
            }

            await tokenStore.SetTokenAsync(body.Token, body.RefreshToken);
            return body.Token;
        }
        finally
        {
            semaphore.Release();
        }
    }

    public void Dispose() => semaphore.Dispose();
}
