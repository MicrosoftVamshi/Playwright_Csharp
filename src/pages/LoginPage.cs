using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Demoblaze.Tests.src.pages
{
    public class LoginPage
    {
        private readonly IPage _page;

        public LoginPage(IPage page) => _page = page;

        private ILocator Username => _page.Locator("#loginusername");
        private ILocator Password => _page.Locator("#loginpassword");
        private ILocator LoginBtn => _page.Locator("#logInModal button:has-text('Log in')");
        private ILocator Logout => _page.Locator("#logout2");
        private ILocator Welcome => _page.Locator("#nameofuser");

        // returns alert message if invalid; returns null if success
        public async Task<string?> Login(string user, string pass, int timeoutMs = 20000)
        {
            await Username.WaitForAsync(new() { Timeout = timeoutMs });
            await Username.FillAsync(user);
            await Password.FillAsync(pass);

            var tcs = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);

            async void Handler(object? s, IDialog d)
            {
                try
                {
                    var msg = d.Message;
                    await d.AcceptAsync();
                    tcs.TrySetResult(msg);
                }
                catch
                {
                    tcs.TrySetResult("UNKNOWN");
                }
                finally
                {
                    _page.Dialog -= Handler;
                }
            }

            _page.Dialog += Handler;
            await LoginBtn.ClickAsync(new() { Force = true });

            // If dialog appears quickly => invalid login
            var done = await Task.WhenAny(tcs.Task, Task.Delay(8000));
            if (done == tcs.Task) return await tcs.Task;

            _page.Dialog -= Handler;

            // success path: wait for logout or welcome
            try
            {
                await Task.WhenAny(
                    Logout.WaitForAsync(new() { Timeout = timeoutMs }),
                    Welcome.WaitForAsync(new() { Timeout = timeoutMs })
                );
            }
            catch { }

            return null;
        }
    }
}
