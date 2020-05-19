using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace VectorPlus.Lib.ML.ImageNetStructures
{
    public class ImageNetData
    {
        [LoadColumn(0)]
        public string ImagePath; // path to file containing image

        [LoadColumn(1)]
        public string Label; // used to identify images

        public static IEnumerable<ImageNetData> FromImages(IEnumerable<byte[]> images)
        {
            int img = 0;
            return images.Select(image => {
                var path = Path.GetTempFileName();
                File.WriteAllBytes(path, image);
                return new ImageNetData() { ImagePath = path, Label = "Image_" + img++ };
            });
        }

        public static void EraseFiles(IEnumerable<ImageNetData> data)
        {
            foreach(var path in data.Select(i => i.ImagePath))
            {
                try { File.Delete(path); } catch { /* NOP */ }
            }
        }

    }
}
