using System;
using System.Threading.Tasks;
using static VectorPlusLib.IVectorActionPlus;

namespace VectorPlusLib
{
    public abstract class AbstractVectorActionPlus : IVectorActionPlus
    {
        protected AbstractVectorActionPlus(IVectorBehaviourPlus behaviour, TimeSpan? timeout, bool needsControl)
        {
            Behaviour = behaviour;
            Created = DateTime.Now;
            State = ActionState.Pending;
            StartTimeout = timeout;
            NeedsControl = needsControl;
        }

        private ActionState state;
        public ActionState State
        {
            get { return state; }
            set { if (state != value) { state = value; OnStateChange?.Invoke(value); } }
        }

        public IVectorBehaviourPlus Behaviour { get; private set; }

        public DateTime Created { get; private set; }

        public TimeSpan? StartTimeout { get; private set; }

        public DateTime? Started { get; private set; }

        public TimeSpan? Duration { get; private set; }

        public bool Attempted { get; private set; }

        public bool Ran { get { return State == ActionState.Completed || State == ActionState.Failed; } }

        public bool Killed { get; private set; }

        public void Kill() { Killed = true; }

        public bool NeedsControl { get; private set; }

        public event Action<ActionState> OnStateChange;

        public bool IsTimedOut
        {
            get
            {
                if (!StartTimeout.HasValue) { return false; }
                // TODO: something not right here - actions are timing out too soon
                return Created + StartTimeout.Value > DateTime.Now;
            }
        }

        public async Task<ActionState> ExecuteAsync(IVectorControllerPlus controller)
        {
            Started = DateTime.Now;
            if (IsTimedOut)
            {
                State = ActionState.Timeout;
            }
            else if (Killed)
            {
                State = ActionState.Dead;
            }
            else
            {
                try
                {
                    Attempted = true;
                    State = ActionState.Running;
                    bool ok = await ExecuteImplementationAsync(controller);
                    Duration = DateTime.Now - Started;
                    State = ok ? ActionState.Completed : ActionState.Failed;
                }
                catch (Exception e)
                {
                    Exception = e;
                    Console.WriteLine("! " + e.Message);
                    State = ActionState.Failed;
                }
            }
            return State;
        }

        protected abstract Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller);

        public Exception Exception { get; protected set; }
    }
}
