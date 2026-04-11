using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demoblaze.Tests.src.pages
{
    public class CartPage
    {
        private readonly IPage _page;
        private readonly ILocator _rows;
        private readonly ILocator _total;

        public CartPage(IPage page)
        {
            _page = page;
            _rows = page.Locator("#tbodyid > tr");
            _total = page.Locator("#totalp");
        }

        public async Task WaitForItems(int minCount = 1, int timeoutMs = 30000)
        {
            await _page.WaitForFunctionAsync(
                @"(count) => document.querySelectorAll('#tbodyid > tr').length >= count",
                minCount,
                new() { Timeout = timeoutMs }
            );
        }

        // ✅ New name (matches your JS naming pattern) 【1-bae573】
        public async Task<List<int>> GetAllItemPrices(int minCount = 1)
        {
            await WaitForItems(minCount);
            int count = await _rows.CountAsync();
            var prices = new List<int>();

            for (int i = 0; i < count; i++)
            {
                var txt = (await _rows.Nth(i).Locator("td:nth-child(3)").TextContentAsync())?.Trim() ?? "0";
                if (!int.TryParse(txt, out var p)) p = 0;
                prices.Add(p);
            }
            return prices;
        }

        // ✅ Backward-compatible alias so E2ETest.cs compiles without edits
        public Task<List<int>> GetPrices(int minCount = 1) => GetAllItemPrices(minCount);

        public async Task<List<string>> GetAllItemNames(int minCount = 1)
        {
            await WaitForItems(minCount);
            int count = await _rows.CountAsync();
            var names = new List<string>();

            for (int i = 0; i < count; i++)
            {
                var txt = (await _rows.Nth(i).Locator("td:nth-child(2)").TextContentAsync())?.Trim() ?? "";
                names.Add(txt);
            }
            return names;
        }

        public async Task<bool> DeleteItemByIndex(int index = 0, int minCount = 1)
        {
            await WaitForItems(minCount);
            int count = await _rows.CountAsync();
            if (count == 0) return false;
            if (index < 0 || index >= count) return false;

            var del = _rows.Nth(index).Locator("a:has-text('Delete')");
            await del.ClickAsync();
            await _page.WaitForTimeoutAsync(2000);
            return true;
        }

        public async Task<int> GetTotal()
        {
            var txt = (await _total.TextContentAsync())?.Trim() ?? "0";
            if (!int.TryParse(txt, out var total)) total = 0;
            return total;
        }
    }
}
