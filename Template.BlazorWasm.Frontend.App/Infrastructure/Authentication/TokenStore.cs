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

    public ValueTask SetTokenAsync(string value)
    {
        token = value;
        return jsRuntime.InvokeVoidAsync("sessionStorage.setItem", StorageKey, value);
    }

    public ValueTask ClearAsync()
    {
        token = null;
        return jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", StorageKey);
    }
}
