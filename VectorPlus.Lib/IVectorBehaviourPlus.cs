using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VectorPlus.Lib.Vision;

namespace VectorPlus.Lib
{
    public interface IVectorBehaviourPlus
    {
        /// <summary>
        /// A unique Id for the current instance of this Behaviour.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// A unique reference, generated at runtime and assigned to the behaviour during construction.
        /// </summary>
        string UniqueReference { get; }

        /// <summary>
        /// A name for this behaviour that the user will understand.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A more detailed description of this behaviour for the user.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// True if this behaviour requires VectorPlus to take full control of the robot all of the time.
        /// NB. This is False by default. It's unlikely you'll need it for most behaviours - as they
        /// are designed to run in the background and allow the robot to play naturally.
        /// </summary>
        bool NeedsPermanentRobotControl { get; }

        /// <summary>
        /// If True, VectorPlus will track objects that appear and disappear.
        /// </summary>
        bool NeedsPermanentObjectAppearanceMonitoring { get; }

        /// <summary>
        /// A list of FrameProcessor types that this Behaviour needs VectorPlus to create.
        /// VectorPlus will only create 1 instance of each type of Frame Processor.
        /// This can change dynamically during runtime.
        /// </summary>
        ICollection<Type> RequestedFrameProcessors { get; }

        /// <summary>
        /// If True, requests that VectorPlus ensure that the specified Frame Processors
        /// are instantiated and used. This can change dynamically during runtime.
        /// </summary>
        bool NeedsFrameProcessing { get; }

        /// <summary>
        /// Really only for the Console application to be able to test behaviours.
        /// Indicates that the user tapped a specific key.
        /// </summary>
        /// <param name="c">The character passed through.</param>
        /// <returns></returns>
        Task ReceiveKeypressAsync(char c);

        /// <summary>
        /// Connects this Behaviour to the VectorControllerPlus provided.
        /// </summary>
        Task SetControllerAsync(IVectorControllerPlus controller);

        /// <summary>
        /// Initiates the Behaviour's main loop. (This is not required to run continuously.)
        /// </summary>
        void StartMainLoop();

        /// <summary>
        /// Halts the Behaviour's main loop.
        /// </summary>
        void StopMainLoop();
    }
}
