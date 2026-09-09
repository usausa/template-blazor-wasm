namespace Template.BlazorWasm;

using Template.BlazorWasm.Contracts.Auth;
using Template.BlazorWasm.Contracts.Data;

public sealed class AuthTests : IClassFixture<TestApplicationFactory>
{
    private readonly TestApplicationFactory factory;

    public AuthTests(TestApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task LoginReturnsToken()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(new Uri("/api/auth/login", UriKind.Relative), new LoginRequest("admin", "admin"), TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(body);
        Assert.False(String.IsNullOrEmpty(body.Token));
    }

    [Fact]
    public async Task LoginWithWrongPasswordReturnsUnauthorized()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(new Uri("/api/auth/login", UriKind.Relative), new LoginRequest("admin", "wrong"), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshReturnsNewTokenAndRotates()
    {
        // Arrange
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/auth/login", UriKind.Relative), new LoginRequest("admin", "admin"), TestContext.Current.CancellationToken);
        var issued = (await login.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken))!;

        // Act
        var response = await client.PostAsJsonAsync(new Uri("/api/auth/refresh", UriKind.Relative), new RefreshRequest(issued.RefreshToken), TestContext.Current.CancellationToken);
        var refreshed = await response.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);
        var reuse = await client.PostAsJsonAsync(new Uri("/api/auth/refresh", UriKind.Relative), new RefreshRequest(issued.RefreshToken), TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(refreshed);
        Assert.False(String.IsNullOrEmpty(refreshed.Token));
        Assert.NotEqual(issued.RefreshToken, refreshed.RefreshToken);
        Assert.Equal(HttpStatusCode.Unauthorized, reuse.StatusCode);
    }

    [Fact]
    public async Task RefreshWithInvalidTokenReturnsUnauthorized()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(new Uri("/api/auth/refresh", UriKind.Relative), new RefreshRequest("invalid-token"), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task LogoutRevokesRefreshToken()
    {
        // Arrange
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/auth/login", UriKind.Relative), new LoginRequest("admin", "admin"), TestContext.Current.CancellationToken);
        var issued = (await login.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken))!;

        // Act
        var logout = await client.PostAsJsonAsync(new Uri("/api/auth/logout", UriKind.Relative), new RefreshRequest(issued.RefreshToken), TestContext.Current.CancellationToken);
        var refresh = await client.PostAsJsonAsync(new Uri("/api/auth/refresh", UriKind.Relative), new RefreshRequest(issued.RefreshToken), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }

    [Fact]
    public async Task DataApiWorksWithToken()
    {
        // Arrange
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/auth/login", UriKind.Relative), new LoginRequest("admin", "admin"), TestContext.Current.CancellationToken);
        var token = (await login.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken))!.Token;
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var create = await client.PostAsJsonAsync(new Uri("/api/data", UriKind.Relative), new DataCreateRequest("IntegrationItem", 100), TestContext.Current.CancellationToken);
        var list = await client.GetFromJsonAsync<DataListResponse>(new Uri("/api/data?name=IntegrationItem", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        Assert.NotNull(list);
        Assert.Single(list.Items);
        Assert.Equal("IntegrationItem", list.Items[0].Name);
    }
}
