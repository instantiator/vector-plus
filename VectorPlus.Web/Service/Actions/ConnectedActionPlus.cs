using System;
using System.Threading.Tasks;
using VectorPlus.Lib;

namespace VectorPlus.Web.Service.Actions
{
    public class ConnectedActionPlus : AbstractVectorActionPlus
    {
        public ConnectedActionPlus() : base(null, TimeSpan.FromSeconds(30), true)
        {
        }

        protected override async Task<bool> ExecuteImplementationAsync(IVectorControllerPlus controller)
        {
            var ok = await WaitUntilReadyToSpeakAsync(controller.Robot, TimeSpan.FromSeconds(20));
            if (ok)
            {
                await controller.Robot.Behavior.SayText("Vector Plus is connected.", true);
                await controller.ReportAsync(VectorBehaviourPlusReport.FromMessage("Connection indicated."));
            }
            else
            {
                await controller.ReportAsync(VectorBehaviourPlusReport.FromMessage("Not ready to speak."));
            }
            return ok;
        }
    }
}
