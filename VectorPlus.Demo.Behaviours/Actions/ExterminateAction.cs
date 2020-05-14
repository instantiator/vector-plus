using System;
using System.Linq;
using System.Threading.Tasks;
using Anki.Vector.Objects;
using VectorPlus.Lib;
using static Anki.Vector.ExternalInterface.ResponseStatus.Types;

namespace VectorPlus.Demo.Behaviour.Actions
{
    public class ExterminateAction : AbstractVectorActionPlus
    {
        private Random random = new Random();

        public ExterminateAction(IVectorBehaviourPlus behaviour, TimeSpan? timeout) : base(behaviour, timeout, true)
        {
        }

        protected override async Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            // initial search for the cube - allow up to 5 seconds to be ready to roll
            await WaitUntilReadyToInteractAsync(controller.Robot, TimeSpan.FromSeconds(5));
            if (ReadyToInteract(controller.Robot))
            {
                await controller.Robot.World.ConnectCube();
                if (controller.Robot.World.LightCube.IsConnected)
                {
                    // there's a cube in sight, so exterminate it
                    await controller.Robot.Behavior.SetLiftHeight(0.5f);
                    await controller.Robot.Behavior.GoToCube(100.0f, 2);
                    await controller.Robot.Behavior.SayText("Extermin8! Extermin8!", true);
                    await controller.Robot.Behavior.GoToCube(50.0f);
                    await PlayEmbeddedSoundAsync(controller, "VectorPlus.Demo.Behaviour.Audio.gun16.wav");
                    await controller.Robot.Behavior.SetLiftHeight(0.0f);
                }
                else
                {
                    // turn towards any face
                    var status = await controller.Robot.Behavior.FindFaces();
                    var faces = controller.Robot.World.Objects.Where(o => o is Face).Select(o => o as Face);
                    var recentSpan = TimeSpan.FromSeconds(20);
                    var recentFaces = faces.Where(f => DateTime.Now - f.LastObservedTime < recentSpan);

                    if (recentFaces.Count() > 0)
                    {
                        int index = random.Next(0, faces.Count());
                        var face = faces.ElementAt(index);
                        var turned = await controller.Robot.Behavior.TurnTowardsFace(face, 2);
                    }

                    // catch phrase instead of extermination
                    await controller.Robot.Behavior.SayText("You would make a good dalek.", true);
                }
            }
            else
            {
                // unable to turn towards a face, just say the line
                await controller.Robot.Behavior.SayText("You would make a good dalek.", true);
            }

            return true;
        }
    }
}
