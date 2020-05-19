using System;
using System.IO;
using VectorPlus.Lib.ML;

namespace VectorPlus.Lib.Helpers
{
    public class PathHelper
    {
        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(CameraFrameProcessor).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;
            string fullPath = Path.Combine(assemblyFolderPath, relativePath);
            return fullPath;
        }
    }
}
