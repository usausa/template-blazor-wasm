namespace Template.BlazorWasm.Contracts.Auth;

public sealed record AuthRefreshRequest(
    [property: Required] string RefreshToken);
