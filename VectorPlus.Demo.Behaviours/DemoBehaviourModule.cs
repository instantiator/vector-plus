using System;
using System.Collections.Generic;
using VectorPlus.Demo.Behaviour.Behaviours;
using VectorPlus.Lib;

namespace VectorPlus.Demo.Behaviour
{
    public class DemoBehaviourModule : IVectorPlusBehaviourModule
    {
        public string UniqueReference => "VectorPlus.Demo.Behaviour.DemoBehaviourModule";

        public int Release => 1;

        public string Name => "Demo behaviours";

        public string Description => "A number of fun behaviours to demonstrate VectorPlus.";

        public string Author => "Lewis Westbury";

        public string AuthorEmail => "lewis@cantab.net";

        public string ModuleWebsite => "https://github.com/instantiator/vector-plus";

        public List<IVectorBehaviourPlus> Behaviours { get; } = new List<IVectorBehaviourPlus>()
        {
            new ExterminateCubeOnSightBehaviour(0),
            new MonitoringEventsBehaviour(2, false),
            new OfferTeaSometimesBehaviour(3)
        };
    }
}
