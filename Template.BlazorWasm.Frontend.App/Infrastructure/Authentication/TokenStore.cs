namespace Template.BlazorWasm.Frontend.App.Infrastructure.Authentication;

using Microsoft.JSInterop;

public sealed class TokenStore
{
    private const string StorageKey = "authToken";

    private readonly IJSRuntime jsRuntime;

    private string? token;

    public TokenStore(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async ValueTask<string?> GetTokenAsync()
    {
        token ??= await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", StorageKey);
        return token;
    }

    public async ValueTask SetTokenAsync(string value)
    {
        token = value;
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", StorageKey, value);
    }

    public async ValueTask ClearAsync()
    {
        token = null;
        await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", StorageKey);
    }
}
