using System;
using System.Threading.Tasks;

namespace VectorPlusLib
{
    public interface IVectorActionPlus
    {
        enum ActionState { Pending, Timeout, Running, Completed, Failed, Dead }

        ActionState State { get; set; }

        IVectorBehaviourPlus Behaviour { get; }

        DateTime Created { get; }
        TimeSpan? StartTimeout { get; }

        DateTime? Started { get; }
        TimeSpan? Duration { get; }

        bool NeedsControl { get; }

        bool IsTimedOut { get; }

        bool Attempted { get; }

        bool Ran { get; }

        bool Killed { get; }

        void Kill();

        Task<ActionState> ExecuteAsync(IVectorControllerPlus controller);

        event Action<ActionState> OnStateChange;

        Exception Exception { get; }
    }
}
