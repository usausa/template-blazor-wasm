namespace Template.BlazorWasm.Backend.Host.Endpoints;

using Template.BlazorWasm.Backend.Host.Infrastructure.Authentication;
using Template.BlazorWasm.Contracts.Auth;

public static class AuthEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.Auth);

        group.MapPost("/login", HandleLoginAsync)
            .WithName("Login")
            .AllowAnonymous()
            .Produces<LoginResponse>()
            .Produces(StatusCodes.Status401Unauthorized);
    }

    //--------------------------------------------------------------------------------
    // Handler
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleLoginAsync(
        AccountService accountService,
        TokenService tokenService,
        LoginRequest request)
    {
        var account = await accountService.AuthenticateAsync(request.Name, request.Password);
        if (account is null)
        {
            return TypedResults.Unauthorized();
        }

        var (token, expireAt) = tokenService.CreateToken(account.Name, account.Role);
        return TypedResults.Ok(new LoginResponse(token, expireAt));
    }
}
