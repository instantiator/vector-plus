using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VectorPlus.Lib.Vision;

namespace VectorPlus.Lib
{
    public interface IVectorBehaviourPlus
    {
        Guid Id { get; }
        string UniqueReference { get; }

        string Name { get; }
        string Description { get; }

        bool NeedsPermanentRobotControl { get; }
        bool NeedsPermanentObjectAppearanceMonitoring { get; }

        ICollection<Type> RequestedFrameProcessors { get; }
        bool NeedsFrameProcessing { get; }

        Task ReceiveKeypressAsync(char c);

        Task SetControllerAsync(IVectorControllerPlus controller);

        void StartMainLoop();
        void StopMainLoop();
    }
}
