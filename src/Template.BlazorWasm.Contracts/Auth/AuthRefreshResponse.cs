namespace Template.BlazorWasm.Contracts.Auth;

public sealed record AuthRefreshResponse(string Token, DateTimeOffset ExpireAt, string RefreshToken);
