using System;
using System.Threading.Tasks;
using VectorPlusLib;

namespace VectorPlusDemo.Actions
{
    public class StopAllMotorsAction : AbstractVectorActionPlus
    {
        public StopAllMotorsAction(IVectorBehaviourPlus behaviour, TimeSpan? timeout) : base(behaviour, timeout, true)
        {
        }

        protected async override Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            await controller.Robot.Motors.StopAllMotors();
            return true;
        }
    }
}
