using System;
using System.Threading.Tasks;
using VectorPlus.Lib;
using static Anki.Vector.ExternalInterface.ResponseStatus.Types;

namespace VectorPlus.Demo.Behaviour.Actions
{
    public class ExterminateAction : AbstractVectorActionPlus
    {
        public ExterminateAction(IVectorBehaviourPlus behaviour, TimeSpan? timeout) : base(behaviour, timeout, true)
        {
        }

        protected override async Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            // initial search for the cube - allow up to 5 seconds to be ready to roll
            if (await WaitUntilReadyToInteractAsync(controller.Robot, TimeSpan.FromSeconds(5)))
            {
                await controller.Robot.Behavior.FindCube();
            }

            if (ReadyToInteract(controller.Robot) && controller.Robot.World.LightCube.IsVisible)
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
                // couldn't find the cube - so respond with a dalek catch phrase instead
                if (ReadyToInteract(controller.Robot))
                {
                    // able to turn towards a face
                    await controller.Robot.Behavior.FindFaces();
                    await controller.Robot.Behavior.SayText("You would make a good dalek.", true);
                }
                else if (ReadyToSpeak(controller.Robot))
                {
                    // unable to turn towards a face, just say the line
                    await controller.Robot.Behavior.SayText("You would make a good dalek.", true);
                }
                else
                {
                    // couldn't move or speak - this action failed
                    return false;
                }
            }

            return true;
        }
    }
}
