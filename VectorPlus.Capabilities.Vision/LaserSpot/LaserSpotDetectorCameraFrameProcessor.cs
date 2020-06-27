using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using VectorPlus.Lib.Vision;

namespace VectorPlus.Capabilities.Vision.LaserSpot
{
    public class LaserSpotDetectorCameraFrameProcessor : ICameraFrameProcessor
    {
        public LaserSpotDetectorCameraFrameProcessor()
        {
        }

        public bool Ready => true;

        public string ModelPath => null;

        public int FramesProcessed { get; private set; } = 0;

        public void Dispose() { }

        public CameraFrameProcessingResult Process(byte[] jpeg)
        {
            // var boxes = DownsizeMethod(jpeg);
            var boxes = ColourFilteringBlobsMethod(jpeg);

            FramesProcessed++;
            return new CameraFrameProcessingResult()
            {
                Boxes = boxes,
                Image = jpeg,
                Processor = this
            };
        }

        private void ColourFilteringBlobsMethod(byte[] jpeg)
        {
            var srcBmp = Bitmap.FromStream(new MemoryStream(jpeg));

        }

        private IEnumerable<IdentifiedObjectBoundingBox> DownsizeMethod(byte[] jpeg)
        {
            // input dimensions: 640 x 300, output dimensions: 1 x 1

            var image = Image.FromStream(new MemoryStream(jpeg));
            Image.GetThumbnailImageAbort dummy = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            var thumbnail = image.GetThumbnailImage(20, 20, dummy, IntPtr.Zero);
            var bmp = new Bitmap(thumbnail);

            int foundX = -1, foundY = -1;
            double foundBrightness = 0;

            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    var brightness = (pixel.R * 0.30) + (pixel.G * 0.59) + (pixel.B * 0.11);
                    if (brightness > foundBrightness)
                    {
                        foundBrightness = brightness;
                        foundX = x;
                        foundY = y;
                    }
                }
            }


            // TODO: confidence is a measure of how much brighter this pixel is than its little friends

            var box = new IdentifiedObjectBoundingBox()
            {
                Dimensions = new BoundingBoxDimensions()
                {
                    X = cx,
                    Y = cy,
                    Width = unitX,
                    Height = unitY
                },
                Confidence = confidence,
                Label = "pointer",
                

            };

            return new[] { box };
        }

        private static Bitmap ApplyGrayscale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

                //create some image attributes
                using (ImageAttributes attributes = new ImageAttributes())
                {

                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }

        private bool ThumbnailCallback() => false;
    }
}
