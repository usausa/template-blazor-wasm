namespace Template.BlazorWasm.Frontend.App.Services;

public static class ApiClientNames
{
    public const string Default = "api";

    // トークン更新用(認証ハンドラーを通さない)
    public const string Refresh = "api-refresh";
}
