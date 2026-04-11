using NUnit.Framework;
using System.Threading.Tasks;
using Demoblaze.Tests.src.common;
using Demoblaze.Tests.src.pages;

namespace Demoblaze.Tests.Tests
{
    public class CartAddTests : BaseTest
    {
        [Test]
        public async Task SearchSelectItems_AddToCart_ValidateAdded()
        {
            var home = new HomePage(Page);
            var cart = new CartPage(Page);

            await home.Goto(BaseUrl);

            string[] itemsToAdd = { "Samsung galaxy s6", "Nokia lumia 1520" };

            foreach (var item in itemsToAdd)
            {
                var found = await home.OpenProduct(item);
                Assert.That(found, Is.True);

                var msg = await home.AddToCart();
                Assert.That(msg, Does.Contain("Product added")); // same as JS 【3-43f1c5】

                await Page.WaitForTimeoutAsync(800);
            }

            await home.OpenCart();

            var prices = await cart.GetAllItemPrices(itemsToAdd.Length);
            Assert.That(prices.Count, Is.GreaterThanOrEqualTo(itemsToAdd.Length));
        }
    }
}
