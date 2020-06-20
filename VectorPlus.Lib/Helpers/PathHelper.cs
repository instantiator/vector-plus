using System.IO;
using System.Reflection;

namespace VectorPlus.Lib.Helpers
{
    public class PathHelper
    {
        public static string CopyResourceToFile(Assembly assembly, string resource)
        {
            var readStream = assembly.GetManifestResourceStream(resource);
            var path = Path.GetTempFileName();
            var writeStream = File.Create(path);
            readStream.Seek(0, SeekOrigin.Begin);
            readStream.CopyTo(writeStream);
            writeStream.Close();
            return path;
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo root = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string assemblyFolderPath = root.Directory.FullName;
            string fullPath = Path.Combine(assemblyFolderPath, relativePath);
            return fullPath;
        }
    }
}
