using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VectorPlus.Lib;
using VectorPlus.Web.Service;

namespace VectorPlus.Web.Models
{
    public class ConfigViewModel
    {
        public List<IVectorPlusBehaviourModule> AllModules { get; set; }
        public List<IVectorBehaviourPlus> ActiveBehaviours { get; set; }

        public string VectorSerial { get; set; }

        public static ConfigViewModel From(IVectorPlusBackgroundService service)
        {
            return new ConfigViewModel()
            {
                AllModules = service.AllModules,
                ActiveBehaviours = service.Controller.Behaviours
            };
        }
    }
}
