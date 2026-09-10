namespace Template.BlazorWasm.Contracts.Auth;

public sealed record RefreshRequest(
    [property: Required] string RefreshToken);
