namespace Template.BlazorWasm;

using System.Text.RegularExpressions;

using Microsoft.Playwright;

public sealed class DataCrudTests : E2ETestBase
{
    [Fact]
    public async Task CreateDataShowsInGrid()
    {
        // Given
        await using var factory = new E2EApplicationFactory();
        factory.UseKestrel(0);
        factory.StartServer();

        await Page.GotoAsync(factory.ServerAddress + "/login");
        await Expect(Page.Locator("fluent-text-field input").Nth(0)).ToBeVisibleAsync();
        await Page.Locator("fluent-text-field input").Nth(0).FillAsync("admin");
        await Page.Locator("fluent-text-field input").Nth(1).FillAsync("admin");
        await Page.Locator("fluent-button", new PageLocatorOptions { HasTextString = "ログイン" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync(new Regex("ホーム.*"));

        // When
        await Page.GotoAsync(factory.ServerAddress + "/data");
        await Page.Locator("fluent-button", new PageLocatorOptions { HasTextString = "新規作成" }).ClickAsync();
        await Expect(Page.Locator("fluent-dialog fluent-text-field input")).ToBeVisibleAsync();
        await Page.Locator("fluent-dialog fluent-text-field input").FillAsync("E2EItem");
        await Page.Locator("fluent-dialog fluent-number-field input").FillAsync("123");
        await Page.Locator("fluent-dialog fluent-button", new PageLocatorOptions { HasTextString = "保存" }).ClickAsync();

        // Then
        await Expect(Page.GetByText("データを作成しました")).ToBeVisibleAsync();
        await Expect(Page.Locator("table")).ToContainTextAsync("E2EItem");
    }
}
