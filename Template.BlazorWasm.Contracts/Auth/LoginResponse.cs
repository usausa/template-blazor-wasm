namespace Template.BlazorWasm.Contracts.Auth;

public sealed record LoginResponse(string Token, DateTimeOffset ExpireAt);
