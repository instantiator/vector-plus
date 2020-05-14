using System;
using System.Threading.Tasks;
using VectorPlus.Lib;

namespace VectorPlus.Demo.Behaviour.Actions
{
    public class BirthdaySongAction : AbstractVectorActionPlus
    {
        public BirthdaySongAction(IVectorBehaviourPlus behaviour, TimeSpan? timeout) : base(behaviour, timeout, true)
        {
        }

        protected override async Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            await controller.Robot.Behavior.SetLiftHeight(92f); // this is in BehaviorComponent.MaxLiftHeight, but it's private
            await controller.Robot.Behavior.SayText("Happy birthday to you!", true);
            await controller.Robot.Behavior.SayText("Happy birthday to you!", true);
            await controller.Robot.Behavior.SayText("Happy birthday dear Sheila.", true);
            await controller.Robot.Behavior.SayText("Happy birthday to you!", true);
            // await controller.Robot.Behavior.SetLiftHeight(32f); // this is in BehaviorComponent.MinLiftHeight, but it's private
            return true;
        }
    }
}
