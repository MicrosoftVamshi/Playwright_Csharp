using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

namespace Demoblaze.Tests.src.utils
{
    public static class ExcelUtil
    {
        public static void Write<T>(string filePath, string sheetName, IEnumerable<T> rows)
        {
            var list = rows.ToList();
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);

            if (list.Count > 0)
            {
                var props = typeof(T).GetProperties();

                for (int c = 0; c < props.Length; c++)
                    ws.Cell(1, c + 1).Value = props[c].Name;

                for (int r = 0; r < list.Count; r++)
                    for (int c = 0; c < props.Length; c++)
                        ws.Cell(r + 2, c + 1).Value = props[c].GetValue(list[r])?.ToString() ?? "";
            }

            wb.SaveAs(filePath);
        }

        public static List<T> Read<T>(string filePath, string sheetName) where T : new()
        {
            if (!File.Exists(filePath)) return new List<T>();

            using var wb = new XLWorkbook(filePath);
            var ws = wb.Worksheet(sheetName);
            if (ws == null) return new List<T>();

            var headers = ws.Row(1).CellsUsed().Select(c => c.GetString().Trim()).ToList();
            if (headers.Count == 0) return new List<T>();

            var props = typeof(T).GetProperties().ToDictionary(p => p.Name, p => p, System.StringComparer.OrdinalIgnoreCase);

            var result = new List<T>();
            int rowNum = 2;

            while (true)
            {
                var row = ws.Row(rowNum);
                if (row == null || row.CellsUsed().Count() == 0) break;

                var obj = new T();
                for (int i = 0; i < headers.Count; i++)
                {
                    var h = headers[i];
                    if (!props.TryGetValue(h, out var prop)) continue;

                    var cell = row.Cell(i + 1).GetString();

                    if (prop.PropertyType == typeof(bool))
                    {
                        prop.SetValue(obj, cell.Trim().Equals("true", System.StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        prop.SetValue(obj, cell);
                    }
                }

                result.Add(obj);
                rowNum++;
            }

            return result;
        }
    }
}
