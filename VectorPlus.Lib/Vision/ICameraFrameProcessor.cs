using System;
namespace VectorPlus.Lib.Vision
{
    public interface ICameraFrameProcessor
    {
        CameraFrameProcessingResult Process(byte[] image);
    }
}
