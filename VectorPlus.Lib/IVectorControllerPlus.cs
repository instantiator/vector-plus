using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Anki.Vector;
using VectorPlus.Lib.Vision;

namespace VectorPlus.Lib
{
    public interface IVectorControllerPlus : IAsyncDisposable
    {
        public enum ConnectedState { Disconnected, Connecting, Connected, Disconnecting }
        public enum ActionEvent {  Add, Start, Finish }
        public enum BehaviourEvent {  Add, Remove }

        event Func<ConnectedState, Task> OnConnectionChanged;
        event Func<List<IVectorBehaviourPlus>, BehaviourEvent, Task> OnBehaviourEvent;
        event Func<VectorBehaviourPlusReport, Task> OnBehaviourReport;
        event Func<IVectorActionPlus, ActionEvent, Task> OnActionEvent;
        event Func<ObjectSeenState, Task> OnObjectAppeared;
        event Func<ObjectSeenState, Task> OnObjectDisappeared;
        event Func<CameraFrameProcessingResult, Task> OnCameraFrameProcessingResult;

        Robot Robot { get; }
        ConnectedState Connection { get; }
        bool IsConnected { get; }

        List<IVectorBehaviourPlus> Behaviours { get; }
        Queue<IVectorActionPlus> Actions { get; }
        List<VectorBehaviourPlusReport> Reports { get; }
        List<ICameraFrameProcessor> FrameProcessors { get; }

        Task<bool> ConnectAsync(VectorControllerPlusConfig controllerConfig, RobotConfiguration robotConfig = null);
        Task DisconnectAsync();

        string LastConnectionError { get; }
        Exception LastConnectionException { get; }

        Task AddBehaviourAsync(IVectorBehaviourPlus behaviour);
        Task RemoveBehaviourAsync(string reference);

        void EnqueueAction(IVectorActionPlus action);

        Task ReportAsync(VectorBehaviourPlusReport report);
        IEnumerable<VectorBehaviourPlusReport> BehaviourReports { get; }

        Task StartMainLoopAsync(CancellationToken cancellationToken, char? haltOn = ' ');
        bool MainLoopRunning { get; }
    }
}
