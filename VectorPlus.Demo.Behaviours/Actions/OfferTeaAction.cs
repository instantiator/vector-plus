using System;
using System.Threading.Tasks;
using Anki.Vector;
using VectorPlus.Lib;

namespace VectorPlus.Demo.Behaviour.Actions
{
    public class OfferTeaAction : AbstractVectorActionPlus
    {
        private string name;

        public OfferTeaAction(IVectorBehaviourPlus behaviour, TimeSpan? timeout, string name = null) : base(behaviour, timeout, true)
        {
            this.name = name;
        }

        protected override async Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            if (ReadyToInteract(controller.Robot))
            {
                await controller.Robot.Behavior.SetLiftHeight(1.0f);
            }

            bool spoke = false;
            if (await WaitUntilReadyToSpeakAsync(controller.Robot, TimeSpan.FromSeconds(5)))
            {
                if (name != null)
                {
                    await controller.Robot.Behavior.SayText("Would you like a cup of tea, " + name + "?", true);
                }
                else
                {
                    await controller.Robot.Behavior.SayText("Would you like a cup of tea?", true);
                }
                spoke = true;
            }

            if (ReadyToInteract(controller.Robot))
            {
                await controller.Robot.Behavior.SetLiftHeight(0.0f);
            }

            return spoke;
        }
    }
}
