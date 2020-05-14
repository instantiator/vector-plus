using System;
using VectorPlus.Web.Service;

namespace VectorPlus.Web.Models
{
    public class ConnectionViewModel
    {
        public RoboConfig RoboConfig { get; set; }

        public static ConnectionViewModel From(IVectorPlusBackgroundService service)
        {
            return new ConnectionViewModel()
            {
                RoboConfig = service.RetrieveRoboConfig()
            };
        }
    }
}
