using System.IO;
using System.Text.Json;

namespace Demoblaze.Tests.src.utils
{
    public static class JsonUtil
    {
        private static readonly JsonSerializerOptions Opt = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static void Write<T>(string filePath, T data)
            => File.WriteAllText(filePath, JsonSerializer.Serialize(data, Opt));

        public static T Read<T>(string filePath)
            => JsonSerializer.Deserialize<T>(File.ReadAllText(filePath), Opt)!;
    }
}
