using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Demoblaze.Tests.src.common
{
    public class CommonMethods
    {
        private readonly IPage _page;
        public CommonMethods(IPage page) => _page = page;

        public async Task Click(ILocator locator)
        {
            await locator.WaitForAsync();
            await locator.ClickAsync();
        }

        public async Task Type(ILocator locator, string text)
        {
            await locator.FillAsync(text);
        }

        public async Task<string> GetText(ILocator locator)
        {
            return await locator.InnerTextAsync();
        }
    }
}
