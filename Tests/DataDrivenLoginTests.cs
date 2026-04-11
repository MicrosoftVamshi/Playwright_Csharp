using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demoblaze.Tests.src.common;
using Demoblaze.Tests.src.models;
using Demoblaze.Tests.src.pages;
using Demoblaze.Tests.src.utils;

namespace Demoblaze.Tests.Tests
{
    [TestFixture]
    public class DataDrivenLoginTests : BaseTest
    {
        [Test]
        [CancelAfter(900000)]
        public async Task CreateUsers_StoreInExcelCsvJson_RotateRead_AndLoginAll()
        {
            var home = new HomePage(Page);
            var signup = new SignupPage(Page);
            var login = new LoginPage(Page);

            await home.Goto(BaseUrl);

            // 1) Create users
            var users = new List<UserData>
            {
                new UserData { Username = DataFactory.UniqueUser("u1").Username, Password = DataFactory.UniqueUser("u1").Password },
                new UserData { Username = DataFactory.UniqueUser("u2").Username, Password = DataFactory.UniqueUser("u2").Password },
                new UserData { Username = DataFactory.UniqueUser("u3").Username, Password = DataFactory.UniqueUser("u3").Password },
            };

            // 2) Signup users (with retry if "exists" or flaky)
            for (int i = 0; i < users.Count; i++)
            {
                int attempts = 0;
                bool done = false;

                while (!done && attempts < 3)
                {
                    await Page.Locator("#signin2").ClickAsync();
                    var msg = await signup.Signup(users[i].Username, users[i].Password);

                    if (SignupPage.IsSuccessOrExists(msg))
                    {
                        done = true; // ✅ accept both success or already exists 【1-2ba38c】
                    }
                    else
                    {
                        // regenerate and retry
                        var fresh = DataFactory.UniqueUser($"u{i}_{attempts}");
                        users[i] = new UserData { Username = fresh.Username, Password = fresh.Password };
                        attempts++;
                        await Page.WaitForTimeoutAsync(400);
                    }
                }

                Assert.That(done, Is.True, $"Signup failed after retries for user index {i}");
                await Page.WaitForTimeoutAsync(500);
            }

            // 3) Write to Excel/CSV/JSON
            var dir = PathUtil.DataFilesDir();
            var excelPath = System.IO.Path.Combine(dir, "users.xlsx");
            var csvPath   = System.IO.Path.Combine(dir, "users.csv");
            var jsonPath  = System.IO.Path.Combine(dir, "users.json");

            ExcelUtil.Write(excelPath, "users", users);
            CsvUtil.Write(csvPath, users);
            JsonUtil.Write(jsonPath, users);

            // 4) Rotate reads and login all
            var sources = new List<(string Name, List<UserData> Data)>
            {
                ("EXCEL", ExcelUtil.Read<UserData>(excelPath, "users")),
                ("CSV",   CsvUtil.Read<UserData>(csvPath)),
                ("JSON",  JsonUtil.Read<List<UserData>>(jsonPath))
            };

            foreach (var src in sources)
            {
                foreach (var u in src.Data)
                {
                    await home.Goto(BaseUrl);

                    await Page.Locator("#login2").ClickAsync();
                    var err = await login.Login(u.Username, u.Password);
                    Assert.That(err, Is.Null, $"Login failed for {u.Username} from {src.Name}");

                    await Page.Locator("#logout2").ClickAsync();
                    await Page.WaitForTimeoutAsync(500);
                    Assert.That(await Page.Locator("#login2").IsVisibleAsync(), Is.True);
                }
            }
        }
    }
}
