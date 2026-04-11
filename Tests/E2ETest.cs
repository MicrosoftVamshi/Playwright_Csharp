using NUnit.Framework;
using System.Threading.Tasks;
using Demoblaze.Tests.src.common;
using Demoblaze.Tests.src.pages;

namespace Demoblaze.Tests.Tests
{
    public class E2ETest : BaseTest
    {
        [Test]
        public async Task FullFlow()
        {
            var home = new HomePage(Page);
            var cart = new CartPage(Page);

            await home.Goto(BaseUrl);

            var found = await home.OpenProduct("Samsung galaxy s6");
            Assert.That(found, Is.True);

            var msg = await home.AddToCart();
            Assert.That(msg, Does.Contain("Product added"));

            // small settle after alert accept (Demoblaze is flaky)
            await Page.WaitForTimeoutAsync(800);

            await home.OpenCart();

            // ✅ wait until cart has at least 1 row (same as JS idea)
            var prices = await cart.GetPrices(1);
            Assert.That(prices.Count, Is.GreaterThanOrEqualTo(1));
        }
    }
}
