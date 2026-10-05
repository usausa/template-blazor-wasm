namespace Template.BlazorWasm.Contracts.Auth;

public sealed record AuthLoginResponse(string Token, DateTimeOffset ExpireAt, string RefreshToken);
