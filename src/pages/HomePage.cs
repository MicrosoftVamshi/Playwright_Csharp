using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Demoblaze.Tests.src.pages
{
    public class HomePage
    {
        private readonly IPage _page;

        public HomePage(IPage page) => _page = page;

        private async Task WaitForProducts()
        {
            await _page.WaitForSelectorAsync("#tbodyid .card-title a", new() { Timeout = 20000 });
        }

        public async Task Goto(string url)
        {
            await _page.GotoAsync(url, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
            await WaitForProducts();
        }

        public async Task<bool> OpenProduct(string productName)
        {
            var nextBtn = _page.Locator("#next2");

            // Match JS openProductByName approach: loop pages, click Next only if visible 【1-952152】
            for (int attempt = 0; attempt < 2; attempt++)
            {
                await _page.GotoAsync("/", new() { WaitUntil = WaitUntilState.DOMContentLoaded });
                await WaitForProducts();

                for (int i = 0; i < 10; i++)
                {
                    var product = _page.Locator("#tbodyid .card-title a", new() { HasText = productName });
                    if (await product.CountAsync() > 0)
                    {
                        await product.First.ClickAsync();

                        // Demoblaze sometimes does not full-navigate; wait for PDP signal
                        await _page.WaitForSelectorAsync("a:has-text('Add to cart')",
                            new() { State = WaitForSelectorState.Attached, Timeout = 20000 });

                        return true;
                    }

                    // ✅ IMPORTANT: if Next is not visible, do NOT try scrollIntoView (it will time out)
                    if (!await nextBtn.IsVisibleAsync())
                        break;

                    // Helpful: jump near bottom; does not require element visibility
                    await _page.EvaluateAsync("() => window.scrollTo(0, document.body.scrollHeight)");

                    try
                    {
                        await nextBtn.ClickAsync(new LocatorClickOptions { Timeout = 8000 });
                    }
                    catch
                    {
                        // JS click fallback (similar spirit to your JS fallback patterns) 【1-952152】
                        await _page.EvaluateAsync("() => { const el = document.querySelector('#next2'); if (el) el.click(); }");
                    }

                    await _page.WaitForTimeoutAsync(900);
                }

                await _page.ReloadAsync(new() { WaitUntil = WaitUntilState.DOMContentLoaded });
            }

            return false;
        }

        public async Task<string?> AddToCart(int timeoutMs = 20000)
        {
            await _page.WaitForSelectorAsync("a:has-text('Add to cart')",
                new() { State = WaitForSelectorState.Attached, Timeout = timeoutMs });

            var addBtn = _page.Locator("a:has-text('Add to cart')");
            await addBtn.ScrollIntoViewIfNeededAsync();

            var tcs = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);

            async void Handler(object? sender, IDialog dialog)
            {
                try
                {
                    var msg = dialog.Message;
                    await dialog.AcceptAsync();
                    tcs.TrySetResult(msg);
                }
                catch
                {
                    tcs.TrySetResult(null);
                }
                finally
                {
                    _page.Dialog -= Handler;
                }
            }

            _page.Dialog += Handler;

            await addBtn.ClickAsync(new LocatorClickOptions { Force = true, Timeout = timeoutMs });

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
            if (completed != tcs.Task)
            {
                _page.Dialog -= Handler;
                return null;
            }

            return await tcs.Task;
        }

        public async Task OpenCart()
        {
            await _page.Locator("#cartur").ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }
}
