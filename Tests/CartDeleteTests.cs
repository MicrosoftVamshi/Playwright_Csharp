using NUnit.Framework;
using System.Threading.Tasks;
using Demoblaze.Tests.src.common;
using Demoblaze.Tests.src.pages;

namespace Demoblaze.Tests.Tests
{
    [TestFixture]
    public class CartDeleteTests : BaseTest
    {
        [Test]
        public async Task AddItems_DeleteOne_ValidateDeletion()
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
                Assert.That(msg, Does.Contain("Product added"));

                await Page.WaitForTimeoutAsync(800);
            }

            await home.OpenCart();

            var beforePrices = await cart.GetAllItemPrices(itemsToAdd.Length);
            Assert.That(beforePrices.Count, Is.GreaterThanOrEqualTo(itemsToAdd.Length));

            var deleted = await cart.DeleteItemByIndex(0, itemsToAdd.Length);
            Assert.That(deleted, Is.True);

            await Page.WaitForTimeoutAsync(1500);

            var afterPrices = await cart.GetAllItemPrices(1);
            Assert.That(afterPrices.Count, Is.LessThan(beforePrices.Count));
        }
    }
}
