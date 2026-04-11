using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace Demoblaze.Tests.src.utils
{
    public static class CsvUtil
    {
        public static void Write<T>(string filePath, IEnumerable<T> rows)
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(rows);
        }

        public static List<T> Read<T>(string filePath)
        {
            if (!File.Exists(filePath)) return new List<T>();
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<T>().ToList();
        }
    }
}
