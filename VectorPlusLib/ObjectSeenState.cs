using System;
using Anki.Vector.Objects;

namespace VectorPlusLib
{
    public class ObjectSeenState
    {
        public int ObjectId { get; set; }
        public ObjectType ObjectType { get; set; }
        public DateTime LastSeen { get; set; }
        public bool BelievedPresent { get; set; }
    }
}
