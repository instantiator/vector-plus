using System;
using System.Collections.Generic;

namespace VectorPlus.Lib
{
    public interface IVectorPlusBehaviourModule
    {
        /// <summary>
        /// A unique reference for the instance of this module.
        /// </summary>
        string UniqueReference { get; }

        /// <summary>
        /// The release version of this module.
        /// </summary>
        int Release { get; }

        /// <summary>
        /// A user-readable name for this module.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A more detailed description of the collection of behaviours provided
        /// by this module.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The Author's name.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// A contact email address for the Author.
        /// </summary>
        string AuthorEmail { get; }

        /// <summary>
        /// A website where the user can learn more about this module.
        /// </summary>
        string ModuleWebsite { get; }

        /// <summary>
        /// Instances of the behaviours that this module provides.
        /// Not expected to change over time.
        /// </summary>
        List<IVectorBehaviourPlus> Behaviours { get; }
    }
}
