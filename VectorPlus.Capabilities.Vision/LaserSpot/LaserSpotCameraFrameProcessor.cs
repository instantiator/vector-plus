using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using VectorPlus.Lib.Vision;

namespace VectorPlus.Capabilities.Vision.LaserSpot
{
    public class LaserSpotCameraFrameProcessor : ICameraFrameProcessor
    {
        public LaserSpotCameraFrameProcessor()
        {
        }

        public bool Ready => true;

        public string ModelPath => null;

        public int FramesProcessed { get; private set; } = 0;

        public void Dispose()
        {
        }

        public CameraFrameProcessingResult Process(byte[] image)
        {
            // image is a jpeg
            MemoryStream stream = new MemoryStream(image);
            Bitmap bmp = new Bitmap(stream);



            var boxes = new List<IdentifiedObjectBoundingBox>();

            // TODO: determine likelihood of there being a laser spot
            // TODO: generate a box around it

            return new CameraFrameProcessingResult()
            {
                Processor = this,
                Image = image,
                Boxes = boxes,
            };
        }


    }
}
