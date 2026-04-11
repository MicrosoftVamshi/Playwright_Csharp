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
    public class ItemsSearchTests : BaseTest
    {
        [Test]
        [CancelAfter(600000)]
        public async Task RotateItemSearch_UsingExcelCsvJson()
        {
            var home = new HomePage(Page);

            var items = new List<ItemData>
            {
                new ItemData { Item = "Samsung galaxy s6", ExpectedAvailable = true },
                new ItemData { Item = "Nokia lumia 1520", ExpectedAvailable = true },
                new ItemData { Item = "Imaginary Phone XYZ", ExpectedAvailable = false }
            };

            var dir = PathUtil.DataFilesDir();
            var excelPath = System.IO.Path.Combine(dir, "items.xlsx");
            var csvPath   = System.IO.Path.Combine(dir, "items.csv");
            var jsonPath  = System.IO.Path.Combine(dir, "items.json");

            ExcelUtil.Write(excelPath, "items", items);
            CsvUtil.Write(csvPath, items);
            JsonUtil.Write(jsonPath, items);

            var sources = new List<(string Name, List<ItemData> Data)>
            {
                ("EXCEL", ExcelUtil.Read<ItemData>(excelPath, "items")),
                ("CSV",   CsvUtil.Read<ItemData>(csvPath)),
                ("JSON",  JsonUtil.Read<List<ItemData>>(jsonPath))
            };

            await home.Goto(BaseUrl);

            foreach (var src in sources)
            {
                foreach (var row in src.Data)
                {
                    var found = await home.OpenProduct(row.Item);
                    Assert.That(found, Is.EqualTo(row.ExpectedAvailable), $"{row.Item} failed from {src.Name}");
                }
            }
        }
    }
}

