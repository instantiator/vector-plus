using System.IO;
using System.Reflection;

namespace VectorPlus.Lib.Helpers
{
    public class PathHelper
    {
        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo root = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string assemblyFolderPath = root.Directory.FullName;
            string fullPath = Path.Combine(assemblyFolderPath, relativePath);
            return fullPath;
        }
    }
}
