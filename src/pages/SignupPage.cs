using Microsoft.Playwright;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Demoblaze.Tests.src.pages
{
    public class SignupPage
    {
        private readonly IPage _page;

        public SignupPage(IPage page) => _page = page;

        private ILocator Username => _page.Locator("#sign-username");
        private ILocator Password => _page.Locator("#sign-password");
        private ILocator SignupBtn => _page.Locator("#signInModal button:has-text('Sign up')");

        public async Task<string?> Signup(string user, string pass, int timeoutMs = 20000)
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
                    tcs.TrySetResult(null);
                }
                finally
                {
                    _page.Dialog -= Handler;
                }
            }

            _page.Dialog += Handler;
            await SignupBtn.ClickAsync(new() { Force = true });

            var done = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
            if (done != tcs.Task)
            {
                _page.Dialog -= Handler;
                return null;
            }

            return await tcs.Task;
        }

        public static bool IsSuccessOrExists(string? msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) return false;
            return Regex.IsMatch(msg, "Sign up successful|This user already exist", RegexOptions.IgnoreCase);
        }
    }
}
