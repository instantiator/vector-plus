using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Anki.Vector;
using Anki.Vector.Objects;
using static VectorPlusLib.VectorControllerPlus;

namespace VectorPlusLib
{
    public interface IVectorControllerPlus : IAsyncDisposable
    {
        public enum ConnectedState { Disconnected, Connecting, Connected, Disconnecting }
        public enum ActionEvent {  Add, Start, Finish }
        public enum BehaviourEvent {  Add, Remove }

        event Func<ConnectedState, Task> OnConnectionChanged;
        event Func<VectorBehaviourPlusReport, Task> OnBehaviourReport;
        event Func<IVectorActionPlus, ActionEvent, Task> OnActionEvent;

        event Func<ObjectSeenState, Task> OnObjectAppeared;
        event Func<ObjectSeenState, Task> OnObjectDisappeared;

        Robot Robot { get; }
        ConnectedState Connection { get; }

        Task ConnectAsync();
        Task DisconnectAsync();

        Task AddBehaviourAsync(IVectorBehaviourPlus behaviour);
        Task RemoveBehaviourAsync(IVectorBehaviourPlus behaviour);

        void EnqueueAction(IVectorActionPlus action);

        Task ReportAsync(VectorBehaviourPlusReport report);
        IEnumerable<VectorBehaviourPlusReport> BehaviourReports { get; }

        Task StartMainLoopAsync(bool stopOnKeypress = true);
    }
}
