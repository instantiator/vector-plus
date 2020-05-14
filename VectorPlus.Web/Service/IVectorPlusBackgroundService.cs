using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VectorPlus.Lib;

namespace VectorPlus.Web.Service
{
    public interface IVectorPlusBackgroundService
    {
        IVectorControllerPlus Controller { get; }
        List<IVectorPlusBehaviourModule> AllModules { get; }

        RoboConfig RetrieveRoboConfig();
        Task<ActionResponseMessage> SetRoboConfigAsync(string robotName, string robotSerial, string email, string password, string ipOverride = null);
        Task<ActionResponseMessage> EraseRoboConfigAsync();
        Task<ActionResponseMessage> ReconnectAsync();
        Task<ActionResponseMessage> DisconnectAsync();

        ActionResponseMessage AddModuleDLL(byte[] bytes);
        ActionResponseMessage RemoveModule(string reference);
        Task<ActionResponseMessage> ActivateBehaviourAsync(string reference);
        Task<ActionResponseMessage> DeactivateBehaviourAsync(string reference);
    }
}
