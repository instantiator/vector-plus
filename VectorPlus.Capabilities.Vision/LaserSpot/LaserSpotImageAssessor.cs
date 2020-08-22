using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace VectorPlus.Capabilities.Vision.LaserSpot
{
    public class LaserSpotImageAssessor
    {
        public byte[] Data { get; private set; }
        public Bitmap Bmp { get; private set; }
        public BitmapData BmpData { get; private set; }

        public LaserSpotImageAssessor(byte[] data)
        {
            Data = data;
            Bmp = new Bitmap(new MemoryStream(data));
            BmpData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
        }

        public int GetAverageBrightnessOfArea()
        {
            throw new NotImplementedException();
        }

        public Tuple<Point,int> CalcDarkestPoint()
        {
            
            Point darkestPoint = new Point(0, 0);
            int darkestValue = 255 + 255 + 255;
            unsafe
            {
                byte* imgPtr = (byte*)(BmpData.Scan0);
                for (int i = 0; i < BmpData.Height; i++)
                {
                    for (int j = 0; j < BmpData.Width; j++)
                    {
                        if (darkestValue > (*imgPtr) + *(imgPtr + 1) + *(imgPtr + 2))
                        {
                            darkestPoint = new Point(j, i);
                        }
                        imgPtr += 3;
                    }
                    imgPtr += BmpData.Stride - BmpData.Width * 3;
                }
            }
            return new Tuple<Point,int>(darkestPoint, darkestValue);
        }

        public Tuple<Point, int> CalcLightestPoint()
        {
            Point lightestPoint = new Point(0, 0);
            int lightestValue = 0 + 0 + 0;
            unsafe
            {
                byte* imgPtr = (byte*)(BmpData.Scan0);
                for (int i = 0; i < BmpData.Height; i++)
                {
                    for (int j = 0; j < BmpData.Width; j++)
                    {
                        if (lightestValue > (*imgPtr) + *(imgPtr + 1) + *(imgPtr + 2))
                        {
                            lightestPoint = new Point(j, i);
                        }
                        imgPtr += 3;
                    }
                    imgPtr += BmpData.Stride - BmpData.Width * 3;
                }
            }

            return new Tuple<Point, int>(lightestPoint, lightestValue);

        }

    }
}
