namespace Template.BlazorWasm.Frontend.App.Infrastructure.Authentication;

using Microsoft.JSInterop;

public sealed class TokenStore
{
    private const string TokenKey = "authToken";

    private const string RefreshTokenKey = "authRefreshToken";

    private readonly IJSRuntime jsRuntime;

    private string? token;

    private string? refreshToken;

    public TokenStore(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async ValueTask<string?> GetTokenAsync()
    {
        token ??= await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", TokenKey);
        return token;
    }

    public async ValueTask<string?> GetRefreshTokenAsync()
    {
        refreshToken ??= await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", RefreshTokenKey);
        return refreshToken;
    }

    public async ValueTask SetTokenAsync(string value, string refreshValue)
    {
        token = value;
        refreshToken = refreshValue;
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", TokenKey, value);
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", RefreshTokenKey, refreshValue);
    }

    public async ValueTask ClearAsync()
    {
        token = null;
        refreshToken = null;
        await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", TokenKey);
        await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", RefreshTokenKey);
    }
}
