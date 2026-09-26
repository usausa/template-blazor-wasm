namespace Template.BlazorWasm;

using System.Text.RegularExpressions;

using Microsoft.Playwright;

public sealed class LoginTests : E2ETestBase
{
    [Fact]
    public async Task LoginShowsHomePage()
    {
        // Given
        await using var factory = new E2EApplicationFactory();
        factory.UseKestrel(0);
        factory.StartServer();

        // When
        await Page.GotoAsync(factory.ServerAddress + "/");
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login.*"));

        await Page.Locator("fluent-text-field input").Nth(0).FillAsync("admin");
        await Page.Locator("fluent-text-field input").Nth(1).FillAsync("admin");
        await Page.Locator("fluent-button", new PageLocatorOptions { HasTextString = "ログイン" }).ClickAsync();

        // Then
        await Expect(Page).ToHaveTitleAsync(new Regex("ホーム.*"));
    }

    [Fact]
    public async Task LoginWithWrongPasswordShowsError()
    {
        // Given
        await using var factory = new E2EApplicationFactory();
        factory.UseKestrel(0);
        factory.StartServer();

        // When
        await Page.GotoAsync(factory.ServerAddress + "/login");
        await Expect(Page.Locator("fluent-text-field input").Nth(0)).ToBeVisibleAsync();
        await Page.Locator("fluent-text-field input").Nth(0).FillAsync("admin");
        await Page.Locator("fluent-text-field input").Nth(1).FillAsync("wrong");
        await Page.Locator("fluent-button", new PageLocatorOptions { HasTextString = "ログイン" }).ClickAsync();

        // Then
        await Expect(Page.GetByText("ログインに失敗しました")).ToBeVisibleAsync();
    }
}
