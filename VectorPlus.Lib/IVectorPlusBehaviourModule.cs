using System;
using System.Collections.Generic;

namespace VectorPlus.Lib
{
    public interface IVectorPlusBehaviourModule
    {
        string UniqueReference { get; }
        int Release { get; }

        string Name { get; }
        string Description { get; }
        string Author { get; }
        string AuthorEmail { get; }
        string ModuleWebsite { get; }

        List<IVectorBehaviourPlus> Behaviours { get; }
    }
}
