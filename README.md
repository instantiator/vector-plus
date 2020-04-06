# VectorPlus

A C# library and framework for defining and managing 'cooperative' vector behaviours through the (unofficial) C# SDK.

## Context

The SDK allows programs to access and control the Vector robot. Some capabilities require that the SDK take full control of the robot. While that is happening, the robot is locked.

__VectorPlus__ creates behaviours that only take control of the robot as they need it, and release that control afterwards.

## Using this framework

This library is in the early stages of development. Download the solution, and run the __VectorPlusDemo__ app with Visual Studio or .NET Core. New behaviours will automatically start.

VectorPlus will only take control of Vector to execute individual actions, or if a behaviour is added that explicitly requires full control.

Press any key in the terminal window to end the session and disconnect from Vector.

## VectorPlusLib

### Concepts

When a Behaviour wishes to issue commands to the robot, it can place Actions in a queue to be run when the robot is free. This helps to distinguish between background BehaviourPlus activity (ie. waiting for an event, such as an object being observed) and foreground ActionPlus (such as moving the robot).

Unfortunately, there's a slight clash between Behaviours as defined by VectorPlus and Behaviours (reasonably complex pre-programmed actions that you can ask Vector to do through the SDK). To distinguish, the VectorPlus framework names key classes with a 'Plus' suffix.

* __The VectorControllerPlus__ is responsible for managing a reasonably stable connection to Vector, until it is Disposed.
  * (Remember to DisposeAsync it once you have finished with it.)
  * It also manages full control of the robot - releasing Vector whenever it isn't needed.
* __A VectorBehaviourPlus__ can be added to the VectorControllerPlus. Once added, there are 3 ways for it to interact with Vector:
  * It can register itself with any Vector events it is using as triggers.
  * It enqueues some initial ActionPlus when it first connects.
  * Its main loop is started which can also enqueue actions.
* __A VectorActionPlus__ is the smallest unit of instruction passed to Vector.
  * It can contain any number of instructions for the robot.
  * If required (likely) it asks the VectorControllerPlus to take full control for it.

### Implementing your own behaviours

_TODO: document this_
