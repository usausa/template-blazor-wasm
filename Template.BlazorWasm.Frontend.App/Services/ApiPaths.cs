namespace Template.BlazorWasm.Frontend.App.Services;

// 生成クライアント(ApiClient.g.cs)を経由せず直接呼び出すエンドポイント
public static class ApiPaths
{
    public const string Refresh = "api/auth/refresh";

    public const string Logout = "api/auth/logout";
}
