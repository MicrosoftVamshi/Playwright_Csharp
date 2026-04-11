using System.IO;

namespace Demoblaze.Tests.src.utils
{
    public static class PathUtil
    {
        public static string FindProjectRoot()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                if (dir.GetFiles("*.csproj").Length > 0)
                    return dir.FullName;

                dir = dir.Parent;
            }
            return Directory.GetCurrentDirectory();
        }

        public static string DataFilesDir()
        {
            var root = FindProjectRoot();
            var path = Path.Combine(root, "DataFiles");
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
