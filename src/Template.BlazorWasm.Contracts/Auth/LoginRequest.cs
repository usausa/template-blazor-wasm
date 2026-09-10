namespace Template.BlazorWasm.Contracts.Auth;

public sealed record LoginRequest(
    [property: Required][property: MaxLength(Length.Name)] string Name,
    [property: Required] string Password);
