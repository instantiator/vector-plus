using System;
using System.Threading.Tasks;
using VectorPlus.Lib;

namespace VectorPlus.Demo.Behaviour.Actions
{
    public class SimpleSpeechAction : AbstractVectorActionPlus
    {
        private string speech;

        public SimpleSpeechAction(IVectorBehaviourPlus behaviour, TimeSpan? timeout, string speech) : base(behaviour, timeout, true)
        {
            this.speech = speech;
        }

        protected async override Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            await controller.Robot.Behavior.SayText(speech, true);
            return true;
        }
    }
}
