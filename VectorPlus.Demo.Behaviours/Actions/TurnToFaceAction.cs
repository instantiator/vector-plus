using System;
using System.Linq;
using System.Threading.Tasks;
using Anki.Vector.Objects;
using VectorPlus.Lib;

namespace VectorPlus.Demo.Behaviour.Actions
{
    public class TurnToFaceAction : AbstractVectorActionPlus
    {
        private Face face;

        public TurnToFaceAction(IVectorBehaviourPlus behaviour, TimeSpan? timeout, Face face) : base(behaviour, timeout, true)
        {
            this.face = face;
        }

        protected override async Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            if (face == null) { return false; }

            // allow 5 seconds to be ready to roll
            if (await WaitUntilReadyToInteractAsync(controller.Robot, TimeSpan.FromSeconds(5)))
            {
                var turned = await controller.Robot.Behavior.TurnTowardsFace(face, 2);
                return
                    turned.Result == Anki.Vector.Types.ActionResultCode.ActionResultSuccess;
            }
            else
            {
                return false; // not ready to interact
            }

        }
    }
}
