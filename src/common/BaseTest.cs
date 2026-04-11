using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using DotNetEnv;

namespace Demoblaze.Tests.src.common
{
    public class BaseTest
    {
        protected IPlaywright Playwright;
        protected IBrowser Browser;
        protected IBrowserContext Context;
        protected IPage Page;
        protected string BaseUrl;

        [SetUp]
        public async Task Setup()
        {
            Env.Load();

            BaseUrl = System.Environment.GetEnvironmentVariable("BASE_URL") ?? "https://www.demoblaze.com/";
            if (!BaseUrl.EndsWith("/")) BaseUrl += "/";

            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            // BaseURL enables relative navigations like Page.GotoAsync("/")
            Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                BaseURL = BaseUrl,
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
            });

            Page = await Context.NewPageAsync();
Page.SetDefaultTimeout(60000);
Page.SetDefaultNavigationTimeout(60000);
        }

        [TearDown]
        public async Task TearDown()
        {
            await Page.CloseAsync();
await Context.CloseAsync();
await Browser.CloseAsync();
Playwright.Dispose();
        }
    }
}


