namespace Template.BlazorWasm.Infrastructure.Authentication;

using System.Text;
using System.Text.Json;

using Template.BlazorWasm.Frontend.App.Infrastructure.Authentication;

public sealed class JwtParserTest
{
    private static readonly DateTimeOffset Now = new(2026, 8, 21, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void ParseValidTokenReturnsPrincipal()
    {
        // Arrange
        var token = CreateToken(new { sub = "admin", role = "Administrator", exp = Now.AddHours(1).ToUnixTimeSeconds() });

        // Act
        var principal = JwtParser.Parse(token, Now);

        // Assert
        Assert.NotNull(principal);
        Assert.Equal("admin", principal.Identity!.Name);
        Assert.True(principal.Identity.IsAuthenticated);
        Assert.True(principal.IsInRole("Administrator"));
    }

    [Fact]
    public void ParseExpiredTokenReturnsNull()
    {
        // Arrange
        var token = CreateToken(new { sub = "admin", exp = Now.AddHours(-1).ToUnixTimeSeconds() });

        // Act
        var principal = JwtParser.Parse(token, Now);

        // Assert
        Assert.Null(principal);
    }

    [Fact]
    public void ParseTokenWithinExpireMarginReturnsNull()
    {
        // Arrange
        var token = CreateToken(new { sub = "admin", exp = Now.AddSeconds(10).ToUnixTimeSeconds() });

        // Act
        var principal = JwtParser.Parse(token, Now);

        // Assert
        Assert.Null(principal);
    }

    [Fact]
    public void ParseBrokenTokenReturnsNull()
    {
        // Arrange & Act
        var principal = JwtParser.Parse("broken-token", Now);

        // Assert
        Assert.Null(principal);
    }

    private static string CreateToken(object payload)
    {
        static string Encode(string json) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(json)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        return $"{Encode("{\"alg\":\"HS256\"}")}.{Encode(JsonSerializer.Serialize(payload))}.signature";
    }
}
