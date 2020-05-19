using System;
using System.Threading.Tasks;

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
        bool NeedsObjectDetection { get; }

        Task ReceiveKeypressAsync(char c);

        Task SetControllerAsync(IVectorControllerPlus controller);

        void StartMainLoop();
        void StopMainLoop();
    }
}
