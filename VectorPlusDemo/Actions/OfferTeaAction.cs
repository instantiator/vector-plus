using System;
using System.Threading.Tasks;
using Anki.Vector;
using VectorPlusLib;

namespace VectorPlusDemo.Actions
{
    public class OfferTeaAction : AbstractVectorActionPlus
    {
        public OfferTeaAction(IVectorBehaviourPlus behaviour, TimeSpan? timeout) : base(behaviour, timeout, true)
        {
        }

        protected override async Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            await controller.Robot.Behavior.SetLiftHeight(92f); // this is in BehaviorComponent.MaxLiftHeight, but it's private
            await controller.Robot.Behavior.SayText("Would you like a cup of tea?", true);
            await controller.Robot.Behavior.SetLiftHeight(32f); // this is in BehaviorComponent.MinLiftHeight, but it's private
            return true;
        }
    }
}
