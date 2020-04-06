# VectorPlus

A C# library for defining and managing new vector behaviours that can coexist.

## Launch

This library is in the early stages of development. Download the solution, and run the __VectorPlusDemo__ app from 

New behaviours will automatically start.

VectorPlus will only take control of Vector to execute individual actions, or if a behaviour is added that explicitly requires full control.

Press any key in the terminal window to end the session and disconnect from Vector.

## VectorPlusLib

### Concepts

* The VectorControllerPlus is responsible for managing a reasonably stable connection to Vector, until it is Disposed.
  * (Remember to DisposeAsync it once you have finished with it.)
  * It also manages full control of the robot - releasing Vector whenever it isn't needed.
* A VectorBehaviourPlus can be added to the VectorController. Once added, there are 3 ways for it to interact with Vector:
  * It can register itself with any Vector events it is using as triggers.
  * It enqueues some initial Actions when it first connects.
  * Its main loop is started which can also enqueue actions.
* A VectorActionPlus is the smallest unit of instruction passed to Vector.
  * It can contain any number of instructions for the robot.
  * If required, it asks the VectorControllerPlus to take full control for it.

### Implementing your own behaviours

_TODO_
