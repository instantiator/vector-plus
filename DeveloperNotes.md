# VectorPlus Developer Notes

## Dependencies

* Visual Studio or a .NET Core installation (to build, run and test)
* Docker (for containerising the app).

### OS X

* See: [Docker on OS X](https://medium.com/@yutafujii_59175/a-complete-one-by-one-guide-to-install-docker-on-your-mac-os-using-homebrew-e818eb4cfc3) notes
* See: [Docker for multiple project solution](https://www.softwaredeveloper.blog/multi-project-dotnet-core-solution-in-docker-image)

NB. On OS X, using `docker-machine` you may need to specify an upper limit for the disk that's a little larger than the default when you first initialise it - I've been working with 40Gb:

```
docker-machine create --driver virtualbox --virtualbox-disk-size "40000" default
eval $(docker-machine env default)
```

## Concepts

When a __Behaviour__ wishes to issue commands to the robot, it can place __Actions__ in a queue to be run when the robot is free. This helps to distinguish between background __Behaviour__ activity (ie. waiting for an event, such as an object being observed) and foreground __Actions__ (such as moving the robot).

Unfortunately, there's a slight naming clash between Behaviours as defined by VectorPlus and Behaviours (reasonably complex pre-programmed actions that you can ask Vector to do through the SDK). To avoid confusion, the VectorPlus framework names key classes with a 'Plus' suffix.

VectorPlus will only take full control of the robot to execute individual __Actions__, or if a __Behaviour__ is added that explicitly requires full control.

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

## Project structure

These are the core projects in the solution:

* `VectorPlus.Lib` - core functionality including a controller, and abstract implementations of action and behaviour.
* `VectorPlus.Demo.Behaviour` - a library built to demonstrate how to build new behaviours and actions for VectorPlus.
* `VectorPlus.Capabilities.Vision` - a library with some machine vision extensions built in.
* `VectorPlus.Console` - a console tool able to launch a Controller, and mix in some of the demo behaviours.
* `VectorPlus.Web` - a web application with a background service that administers a `VectorControllerPlus`.

There are also some testing projects:

* `VectorPlus.Capabilities.Tests`
* `VectorPlus.Lib.Tests`

## Database setup

The project uses EF Core with SQLite.

* See: [Getting started with EF Core](https://docs.microsoft.com/en-us/ef/core/get-started/?tabs=visual-studio)

* `VectorPlus.Web` maintains a database context in: `VectorPlus.Web.Service.Db.VectorPlusBackgroundServiceDbContext`
* It's driven by SQLite, and the database file is: `vectorplus.db`

It currently contains 2 tables:

* `ModuleConfig` - details of the various behavioural modules the application is aware of.
* `RoboConfig` - should contain 1 or 0 records, with details of the current robot connection config.

### Modifying the database

To create a fresh instance of the database, or to modify the existing database, you'll need to create a new migration and/or update the database. Use the Nuget Package Manager console to perform these actions for the VectorPlus.Web project.

#### Creating a migration

Either modify `VectorPlusBackgroundServiceDbContext`, add new classes, or change the classes that currently map to database tables: `ModuleConfig`, `RobotConfig`

Once ready, create a migration using the Package Manager Console:

* `Add-Migration GiveYourMigrationANameHere -Context VectorPlus.Web.Service.Db.VectorPlusBackgroundServiceDbContext`

#### Applying migrations / updating the database

To run the migration changes to the database, run the following in the Package Manager Console:

* `Update-Database`

#### OS X

Visual Studio for Mac needs a little extra help, as the Nuget Package Manager Console doesn't seem to be easily available. First install this [Nuget Package Manager Console](https://github.com/mrward/monodevelop-nuget-extensions) extension.

## Building Behaviours

Behaviours are designed to run in the background, waiting for triggers or events before they cause the robot to take action.

Behaviours should inherit from `AbstractVectorBehaviourPlus`, and implement `IVectorBehaviourPlus`.

To build a new Behaviour, inherit `AbstractVectorBehaviourPlus` and implement the abstract methods and properties. See the `IVectorBehaviourPlus` interface for details of the purpose of these methods and properties.

There are plenty of examples illustrating the lifecycle of a Behaviour in the VectorPlus.Demo.Behaviours project.

When enabled, a Behaviour will be assigned to the running VectorPlus controller, using `SetControllerAsync`. For Behaviours that inherit from `AbstractVectorBehaviourPlus` the following then occurs:

* The Behaviour has an opportunity to register with the Robot's events (implement `RegisterWithRobotEventsAsync`).
* The Behaviour has an opportunity to issue commands to the Robot or Controller as soon as it connects (implement `IssueCommandsOnConnectionAsync`).
* The Behaviour's main loop is called. This doesn't have to do anything, but the Behaviour needs a main loop, this runs on a separate thread (implement `MainLoopAsync`).

When running, the Behaviour may interact with the Robot directly through `controller.Robot`, but it is normally expected to enqueue an `IVectorActionPlus` which can then be scheduled by the controller (which can then ensure that it has full control of the robot if required, and manage other shared resources such as Frame Processors or object tracking).

A normal (if simple) implementation might first register for an event (such as a seen face), and then when the delegate method provided to the event is called, to enqueue an action (such as some spoken words or a gesture).

On disable or disconnection from the robot, the following occurs:

* The robot's main loop receives a Cancel on its CancelToken.
* The Behaviour should unregister from any of the Robot's events that it was registered for (implement `UnregisterFromRobotEventsAsync`).

## Building a BehaviourModule

VectorPlus will scan provided Module DLLs for classes that implement `IVectorPlusBehaviourModule`. These will then each be loaded as a behavioural module - with any number of assigned Behaviours. Details in the module will be made visible to the user through the UI.
