using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Anki.Vector;
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

        Robot Robot { get; }
        ConnectedState Connection { get; }

        Task ConnectAsync();
        Task DisconnectAsync();

        Task AddBehaviourAsync(IVectorBehaviourPlus behaviour);
        Task RemoveBehaviourAsync(IVectorBehaviourPlus behaviour);

        void EnqueueAction(IVectorActionPlus action);

        Task ReportAsync(VectorBehaviourPlusReport report);
        IEnumerable<VectorBehaviourPlusReport> BehaviourReports { get; }
    }
}
