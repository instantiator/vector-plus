using System;
using System.Linq;
using System.Threading.Tasks;
using Anki.Vector.Objects;
using VectorPlus.Lib;

namespace VectorPlus.Demo.Behaviour.Actions
{
    public class FaceSomeoneAction : AbstractVectorActionPlus
    {
        private Random random = new Random();

        public FaceSomeoneAction(IVectorBehaviourPlus behaviour, TimeSpan? timeout) : base(behaviour, timeout, true)
        {
        }

        protected override async Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            // turn to face someone - allow 5 seconds to be ready to roll
            if (await WaitUntilReadyToInteractAsync(controller.Robot, TimeSpan.FromSeconds(5)))
            {
                var status = await controller.Robot.Behavior.FindFaces();
                var faces = controller.Robot.World.Objects.Where(o => o is Face).Select(o => o as Face);
                var recentSpan = TimeSpan.FromSeconds(20);
                var recentFaces = faces.Where(f => DateTime.Now - f.LastObservedTime < recentSpan);

                if (recentFaces.Count() > 0)
                {
                    int index = random.Next(0, faces.Count());
                    var face = faces.ElementAt(index);
                    var turned = await controller.Robot.Behavior.TurnTowardsFace(face, 2);
                    return
                        turned.Result == Anki.Vector.Types.ActionResultCode.ActionResultSuccess;
                }
                else
                {
                    return false; // no faces
                }
            }
            else
            {
                return false; // not ready to interact
            }

        }
    }
}
