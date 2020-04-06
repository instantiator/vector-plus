using System;
using System.Threading.Tasks;
using VectorPlusLib;

namespace VectorPlusDemo.Actions
{
    public class ExterminateAction : AbstractVectorActionPlus
    {
        public ExterminateAction(IVectorBehaviourPlus behaviour, TimeSpan? timeout) : base(behaviour, timeout, true)
        {
        }

        protected override async Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            await controller.Robot.Behavior.SetLiftHeight(0.5f);
            await controller.Robot.Behavior.SayText("Exterminate! Exterminate!", true);

            await controller.Robot.Behavior.GoToCube(50.0f);
            await controller.Robot.Behavior.SetLiftHeight(0.0f);

            return true;
        }
    }
}
