namespace Template.BlazorWasm;

using System.Text.RegularExpressions;

using Microsoft.Playwright;
using Microsoft.Playwright.Xunit.v3;

public sealed class LoginTest : PageTest
{
    [Fact]
    public async Task LoginShowsHomePage()
    {
        // Arrange
        await using var factory = new E2EApplicationFactory();
        factory.UseKestrel(0);
        factory.StartServer();

        // Act (WASMの初回起動が遅いためタイムアウトを長めにとる)
        await Page.GotoAsync(factory.ServerAddress + "/");
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login.*"), new PageAssertionsToHaveURLOptions { Timeout = 60_000 });

        await Page.Locator("fluent-text-field input").Nth(0).FillAsync("admin");
        await Page.Locator("fluent-text-field input").Nth(1).FillAsync("admin");
        await Page.Locator("fluent-button", new PageLocatorOptions { HasTextString = "ログイン" }).ClickAsync();

        // Assert
        await Expect(Page).ToHaveTitleAsync(new Regex("ホーム.*"), new PageAssertionsToHaveTitleOptions { Timeout = 30_000 });
    }

    [Fact]
    public async Task LoginWithWrongPasswordShowsError()
    {
        // Arrange
        await using var factory = new E2EApplicationFactory();
        factory.UseKestrel(0);
        factory.StartServer();

        // Act
        await Page.GotoAsync(factory.ServerAddress + "/login");
        await Expect(Page.Locator("fluent-text-field input").Nth(0)).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 60_000 });
        await Page.Locator("fluent-text-field input").Nth(0).FillAsync("admin");
        await Page.Locator("fluent-text-field input").Nth(1).FillAsync("wrong");
        await Page.Locator("fluent-button", new PageLocatorOptions { HasTextString = "ログイン" }).ClickAsync();

        // Assert
        await Expect(Page.GetByText("ログインに失敗しました")).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 30_000 });
    }
}
