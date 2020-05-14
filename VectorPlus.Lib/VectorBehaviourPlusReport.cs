using System;
using Anki.Vector;

namespace VectorPlus.Lib
{
    public class VectorBehaviourPlusReport
    {
        public readonly DateTime Date = DateTime.Now;
        public string Description { get; set; }
        public IVectorControllerPlus Controller { get; set; }
        public Robot Robot { get; set; }
        public IVectorBehaviourPlus Behaviour { get; set; }

        public static VectorBehaviourPlusReport FromMessage(String message, Robot robot = null, IVectorControllerPlus controller = null, IVectorBehaviourPlus behaviour = null)
        {
            return new VectorBehaviourPlusReport()
            {
                Description = message,
                Controller = controller,
                Robot = robot,
                Behaviour = behaviour
            };
        }

        public static VectorBehaviourPlusReport FromException(Exception e, Robot robot = null, IVectorControllerPlus controller = null, IVectorBehaviourPlus behaviour = null)
        {
            return new VectorBehaviourPlusReport()
            {
                Description = e.Message,
                Controller = controller,
                Robot = robot,
                Behaviour = behaviour
            };
        }
    }
}
