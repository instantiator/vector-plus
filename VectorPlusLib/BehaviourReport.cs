using System;
using Anki.Vector;

namespace VectorPlusLib
{
    public class VectorBehaviourPlusReport
    {
        public readonly DateTime Date = DateTime.Now;
        public string Description { get; set; }
        public IVectorControllerPlus Controller { get; set; }
        public Robot Robot { get; set; }
        public IVectorBehaviourPlus Behaviour { get; set; }
    }
}
