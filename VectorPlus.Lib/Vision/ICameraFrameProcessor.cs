using System;
namespace VectorPlus.Lib.Vision
{
    public interface ICameraFrameProcessor : IDisposable
    {
        bool Ready { get; }

        string ModelPath { get; }

        CameraFrameProcessingResult Process(byte[] image);

        int FramesProcessed { get; }
    }
}
