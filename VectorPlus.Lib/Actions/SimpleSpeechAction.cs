using System;
using System.Threading.Tasks;

namespace VectorPlus.Lib.Actions
{
    public class SimpleSpeechAction : AbstractVectorActionPlus
    {
        private string speech;

        public SimpleSpeechAction(IVectorBehaviourPlus behaviour, string speech) : base(behaviour, TimeSpan.FromSeconds(15), true)
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
