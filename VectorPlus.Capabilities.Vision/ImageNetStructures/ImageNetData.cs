using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML.Data;

namespace VectorPlus.Capabilities.Vision.ImageNetStructures
{
    public class ImageNetData
    {
        [LoadColumn(0)]
        public string ImagePath; // path to file containing image

        [LoadColumn(1)]
        public string Label; // used to identify images

        /// <summary>
        /// This is absolutely horrible, but the ONNX pipelines seem to rely on
        /// file paths. We write the images to temporary files. It is important
        /// to erase them after use.
        /// </summary>
        public static IEnumerable<ImageNetData> FromImages(IEnumerable<byte[]> images)
        {
            int img = 0;
            return images.Select(image => {
                var path = Path.GetTempFileName();
                File.WriteAllBytes(path, image);
                return new ImageNetData() { ImagePath = path, Label = "Image_" + img++ };
            });
        }

        /// <summary>
        /// Erases the temporary files for each ImageNetData file provided.
        /// </summary>
        public static void EraseFiles(IEnumerable<ImageNetData> data)
        {
            foreach(var path in data.Select(i => i.ImagePath))
            {
                try { File.Delete(path); } catch { /* NOP */ }
            }
        }

    }
}
