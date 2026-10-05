namespace Template.BlazorWasm.Contracts.Auth;

public sealed record AuthLogoutRequest(
    [property: Required] string RefreshToken);
