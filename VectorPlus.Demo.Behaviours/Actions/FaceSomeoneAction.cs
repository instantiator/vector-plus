using System;
using System.Threading.Tasks;
using VectorPlus.Lib;

namespace VectorPlus.Demo.Behaviour.Actions
{
    public class FaceSomeoneAction : AbstractVectorActionPlus
    {
        public FaceSomeoneAction(IVectorBehaviourPlus behaviour, TimeSpan? timeout) : base(behaviour, timeout, true)
        {
        }

        protected override async Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            // turn to face someone - allow 5 seconds to be ready to roll
            if (await WaitUntilReadyToInteractAsync(controller.Robot, TimeSpan.FromSeconds(5)))
            {
                var status = await controller.Robot.Behavior.FindFaces();

                // return true if Vector thinks it has succeeded
                return
                    status.Result == Anki.Vector.Types.BehaviorResultCode.Complete &&
                    status.StatusCode == Anki.Vector.Types.StatusCode.Ok;
            }
            else
            {
                return false;
            }

        }
    }
}
