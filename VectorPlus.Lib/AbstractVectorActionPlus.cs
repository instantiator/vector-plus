using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Anki.Vector;
using static VectorPlus.Lib.IVectorActionPlus;

namespace VectorPlus.Lib
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
                var diff = DateTime.Now - Created;
                return diff.Ticks > StartTimeout.Value.Ticks;
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
                    // TODO: log the exception?
                    State = ActionState.Failed;
                }
            }
            return State;
        }

        protected abstract Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller);

        public Exception Exception { get; protected set; }

        public async Task<bool> WaitUntilReadyToSpeakAsync(Robot robot, TimeSpan timeout)
        {
            return await WaitUntil(robot, timeout, () => ReadyToSpeak(robot));
        }

        public async Task<bool> WaitUntilReadyToInteractAsync(Robot robot, TimeSpan timeout)
        {
            return await WaitUntil(robot, timeout, () => ReadyToInteract(robot));
        }

        protected async Task<bool> WaitUntil(Robot robot, TimeSpan timeout, Func<bool> method)
        {
            var finish = DateTime.Now.Add(timeout);
            do
            {
                if (method()) { return true; }
                await Task.Delay(100);
            } while (DateTime.Now < finish);
            return false;
        }

        public bool ReadyToSpeak(Robot robot)
        {
            return
                !robot.Status.IsInCalmPowerMode;
        }

        public bool ReadyToInteract(Robot robot)
        {
            return
                !robot.Status.IsOnCharger &&
                !robot.Status.IsBeingHeld &&
                !robot.Status.IsCarryingBlock &&
                !robot.Status.IsInCalmPowerMode;
        }

        protected async Task<PlaybackResult> PlayWavFileAsync(IVectorControllerPlus controller, string wavPath, uint volume = 50)
        {
            return await PlayWavByteArrayAsync(controller, File.ReadAllBytes(wavPath), volume);
        }

        protected async Task<PlaybackResult> PlayWavByteArrayAsync(IVectorControllerPlus controller, byte[] wav, uint volume = 50)
        {
            // Audio starts at offset 44...
            var header = new byte[44];
            var audio = new byte[wav.Length - 44];
            wav.CopyTo(header, 0);
            wav.CopyTo(audio, 44);
            var stream = new MemoryStream(audio);
            return await PlaySoundAsync(controller, header, stream, volume);
        }

        protected async Task<PlaybackResult> PlayEmbeddedSoundAsync(IVectorControllerPlus controller, string resourceId, uint volume = 50)
        {
            //var thisAssembly = Assembly.GetExecutingAssembly();
            var thisAssembly = GetType().GetTypeInfo().Assembly;
            var stream = thisAssembly.GetManifestResourceStream(resourceId);
            return await PlayWavStreamAsync(controller, stream, volume);
        }
        
        protected async Task<PlaybackResult> PlayWavStreamAsync(IVectorControllerPlus controller, Stream stream, uint volume = 50)
        {
            var reader = new BinaryReader(stream);
            var header = reader.ReadBytes(44);
            return await PlaySoundAsync(controller, header, stream, volume);
        }

        private async Task<PlaybackResult> PlaySoundAsync(IVectorControllerPlus controller, byte[] header, Stream stream, uint volume)
        {
            // WAV format guide: http://www.topherlee.com/software/pcm-tut-wavformat.html

            var channels = BitConverter.ToUInt16(header, 22);
            if (channels != 1) { throw new ArgumentException("Vector requires a 1 channel WAV."); }

            var bitdepth = BitConverter.ToUInt16(header, 34);
            if (bitdepth != 16) { throw new ArgumentException("Vector requires a WAV with 16-bit samples."); }

            var sampleRate = BitConverter.ToUInt32(header, 24);
            uint frameRate = sampleRate; // not right I think
            if (frameRate < 8000 || frameRate > 16025) { throw new ArgumentException("Frame rate must bet between 8000 and 16025 hz."); }

            return await controller.Robot.Audio.PlayStream(stream, frameRate, volume);
        }

        protected void StopPlayback(IVectorControllerPlus controller)
        {
            controller.Robot.Audio.CancelPlayback();
        }

    }
}
