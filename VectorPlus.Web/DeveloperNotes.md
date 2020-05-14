# VectorPlus Developer Notes

## Concepts

* __Controller__ (`IVectorControllerPlus`, `VectorControllerPlus`) - manages connections to Vector, registered Behaviours, and a queue of Actions.
* __Behaviour__ (`IVectorBehaviourPlus`, `AbstractVectorBehaviourPlus`) - a long running behavioural trait, able to monitor Vector and add Actions to the Controller's queue.
* __Action__ (`IVectorActionPlus`, `AbstractVectorActionPlus`) - individual units of Vector activity, added to a queue in the Controller.

## Project structure

* `VectorPlus.Lib` - core functionality including a controller, and abstract implementations of action and behaviour.
* `VectorPlus.Demo.Behaviour` - a library built to demonstrate how to build new behaviours and actions for VectorPlus.
* `VectorPlus.Console` - a console tool able to launch a Controller, and mix in the demo behaviours.
* `VectorPlus.Web` - a web application with a background service that administers a `VectorControllerPlus`.

## Database setup

* `VectorPlus.Web` maintains a database context in: `VectorPlus.Web.Service.Db.VectorPlusBackgroundServiceDbContext`
* It's driven by SQLite, and the data file is: `vectorplus.db`

It currently contains:

* A list of `ModuleConfig` with details of the various behavioural modules the application is aware of.

### Database modifications

The project uses EF Core with SQLite, see: [Getting started with EF Core](https://docs.microsoft.com/en-us/ef/core/get-started/?tabs=visual-studio)

Visual Studio for Mac needs a little extra help, as the Nuget Package Manager Console doesn't seem to be easily available.

First install this [Nuget Package Manager Console](https://github.com/mrward/monodevelop-nuget-extensions) extension.

* Creating a migration:
  `Add-Migration InitialCreate -Context VectorPlus.Web.Service.Db.VectorPlusBackgroundServiceDbContext`

* Applying and updating the database:
  `Update-Database`
