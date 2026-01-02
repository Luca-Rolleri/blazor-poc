using Microsoft.Playwright;
using PlaywrightTests.Pages;

namespace PlaywrightTests;

[TestClass]
public class CounterTest : BlazorPageTest
{
    private string appUrl = $"{RootUri.AbsoluteUri}counter";

    [TestMethod("Counter Increment")]
    public async Task Counter_Should_Increase_When_Button_Pressed()
    {
        // we can step through our playright tests using playright inspector with the method PauseAsync() (with have to set Headless to false in the runsettings file)
        //await Page.PauseAsync();

        await Page.GotoAsync(appUrl);

        // Allow to select dom element (css selectors are not encouraged)
        await Page.GetByRole(AriaRole.Link, new() { Name = "Counter" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click me" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Status)).ToHaveTextAsync("Current count: 1");

    }
}
