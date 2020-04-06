﻿using System;
using System.Threading.Tasks;
using Anki.Vector;

namespace VectorPlusLib
{
    public interface IVectorBehaviourPlus
    {
        Guid Id { get; }

        bool NeedsPermanentRobotControl { get; }

        Task SetControllerAsync(IVectorControllerPlus controller);

        void StartMainLoop();
        void StopMainLoop();
    }
}
