namespace Template.BlazorWasm.Backend.Host.Endpoints;

using Template.BlazorWasm.Backend.Host.Application.Authentication;
using Template.BlazorWasm.Contracts.Auth;

public static class AuthEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapApiGroup(ApiRoutes.Auth);

        group.MapPost("/login", HandleLoginAsync)
            .WithName("AuthLogin")
            .AllowAnonymous()
            .Produces<AuthLoginResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapPost("/refresh", HandleRefreshAsync)
            .WithName("AuthRefresh")
            .AllowAnonymous()
            .Produces<AuthRefreshResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
        group.MapPost("/logout", HandleLogoutAsync)
            .WithName("AuthLogout")
            .AllowAnonymous()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();
    }

    //--------------------------------------------------------------------------------
    // Handler
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleLoginAsync(
        AccountService accountService,
        RefreshTokenService refreshTokenService,
        JwtTokenProvider tokenProvider,
        AuthSetting setting,
        AuthLoginRequest request)
    {
        var account = await accountService.AuthenticateAsync(request.Name, request.Password);
        if (account is null)
        {
            return TypedResults.Unauthorized();
        }

        var (token, expireAt) = tokenProvider.CreateToken(account.Name, account.Role);
        var refreshToken = await refreshTokenService.IssueAsync(account.Name, setting.RefreshExpireDays);
        return TypedResults.Ok(new AuthLoginResponse(token, expireAt, refreshToken));
    }

    // リフレッシュトークンは1回限りで、成功時に新しいトークンへ差し替える(ローテーション)
    private static async ValueTask<IResult> HandleRefreshAsync(
        AccountService accountService,
        RefreshTokenService refreshTokenService,
        JwtTokenProvider tokenProvider,
        AuthSetting setting,
        AuthRefreshRequest request)
    {
        var name = await refreshTokenService.ConsumeAsync(request.RefreshToken);
        if (name is null)
        {
            return TypedResults.Unauthorized();
        }

        // アカウントの削除・ロール変更を反映するため、保存値ではなく現在の状態を読み直す
        var account = await accountService.QueryAsync(name);
        if (account is null)
        {
            return TypedResults.Unauthorized();
        }

        var (token, expireAt) = tokenProvider.CreateToken(account.Name, account.Role);
        var refreshToken = await refreshTokenService.IssueAsync(account.Name, setting.RefreshExpireDays);
        return TypedResults.Ok(new AuthRefreshResponse(token, expireAt, refreshToken));
    }

    private static async ValueTask<IResult> HandleLogoutAsync(
        RefreshTokenService refreshTokenService,
        AuthLogoutRequest request)
    {
        await refreshTokenService.RevokeAsync(request.RefreshToken);
        return TypedResults.NoContent();
    }
}
