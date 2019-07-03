# ElasticRoute for .NET (C#)

![ElasticRoute Logo](http://elasticroute.staging.wpengine.com/wp-content/uploads/2019/02/Elastic-Route-Logo-Text-on-right-e1551344046806.png)

### API for solving large scale travelling salesman/fleet routing problems

You have a fleet of just 10 vehicles to serve 500 spots in the city. Some vehicles are only available in the day. Some stops can only be served at night. How would you solve this problem?

You don't need to. Just throw us a list of stops, vehicles and depots and we will do the heavy lifting for you. _Routing as a Service!_

**BETA RELEASE:**  ElasticRoute is completely free-to-use until 30th April 2020!

**Note regarding target frameworks:** This libray targets .NET Standard 2.0, and thus should be compatible with any modern implementation in the .NET family.

**Note regarding CLS compatibility:** While this library is written with `CLSCompliant` flags to be intended for use with the other CLS Languages (F#, VB), we only offer support in C#.

## Quick Start Guide

Install with dotnet-cli:

    dotnet add package ElasticRoute

Code lives in the `Detrack.ElasticRoute` namespace (note the vendor prefix).

In your code, set your default API Key (this can be retrieved from the dashboard of the web application):

```csharp
using Detrack.ElasticRoute;

Plan.DefaultApiKey = "keygoeshere";
```

Create a new `Plan` object with a name:

```csharp
Plan plan = new Plan("my_first_plan");
```

Create instances of `Stop` and add them to the plan:

```csharp
Stop stop1 = new Stop("Changi Airport", "80 Airport Boulevard (S)819642");
// both human-readable addresses and machine-friendly coordinates work!
Stop stop2 = new Stop("Gardens By The Bay", 1.281407f, 103.865770f);

plan.Stops.Add(stop1);
plan.Stops.Add(stop2);
// add more stops!
```

Create your fleet of `Vehicle`s and add them to your plan:

```csharp
// continued from above...
Vehicle van1 = new Vehicle("Van 1");
Vehicle van2 = new Vehicle("Van 2");

plan.Vehicles.add(van1);
plan.Vehicles.add(van2);
```

Give us a list of `Depot`s (warehouses):

```csharp
// continued from above...
Depot mainWarehouse = new Depot("Main Warehouse");
plan.depots.Add(mainWarehouse);
```

Set your country and timezone (for accurate geocoding):

```csharp
// continued from above...
// for accurate geocoding, you *must* include country
plan.GeneralSettings.Country = "SG";
// for your own convinence, the timestamps (eta etc) will be displayed in your preferred timezone
plan.GeneralSettings.Timezone = "Asia/Singapore";
```

Call `Solve()` (returns an awaitable task)

```csharp
// solve it immediately
// in an asynchronous context
await plan.Solve();

// store the task somewhere for later, if you have alot of plans to solve at once and want to pool them first
Task planSolvingTask = plan.Solve();
// pooling / solving comes later
```

Inspect the solution!

```csharp
using System;

foreach(Stop stop in plan.Stops){
    Console.WriteLine($"Stop {stop.Name} will be served by {stop.AssignTo}");
}
```

Quick notes:
  - `Stop`s and `Depot`s require a form of address before being solved (either a human-readable address or lat/lng coordinates), however a public constructor that allows you to pass only the name is provided for convenience. You **must** set an address before calling `Plan.Solve()` or you would expect a `BadFieldException` to be thrown.
  - You would see in your IDE that many of the properties are nullable ints and floats. In the context of this library, setting such properties to null would either set it to the default value or disable the behaviour completely. Examples:
    - If `Vehicle.ReturnToDepot` was set to null, the API will treat it as the default value of `False`.
    - If `Vehicle.Priority` was set to null, the API will prioritise this vehicle equally with other null priority vehicles.
    - If `Vehicle.AvailFrom` was set to null, the library will copy the value present in `Plan.GeneralSettings.From`.

The library has every public class member documented with codeblocks that will be picked up by Intellisense and other code analysis tools. The above behaviours are all well documented in their respective codeblocks.


## Advanced Usage

### Setting time constraints

Time constraints for Stops and Vehicles can be set with the `From`/`Till` and `AvailFrom`/`AvailTill` properties:

```csharp
Stop morningOnlyStop = new Stop("Morning Delivery 1");
morningOnlyStop.From = 900;
morningOnlyStop.Till = 1200;
// add address and add to plan...

Vehicle morningShiftVan = new Vehicle("Morning Shift Driver");
morningShiftVan.AvailFrom = 900;
morningShiftVan.AvailTill = 1200;
// add to plan and solve...
```

Leaving these properties as null will cause them to be defaulted to the `From` and `Till` properties in `Plan.GeneralSettings`, which in turn is defaulted to `900` and `1700` respectively.

### Setting home depots

A "home depot" can be set for both Stops and Vehicles. A depot for stops indicate where a vehicle must pick up a stop's goods before arriving, and a depot for vehicles indicate the start and end point of a Vehicle's journey (this implicitly assigns the possible jobs a Vehicle can take).
By default, for every stop and vehicle, if the depot field is not specified we will assume it to be the first depot.

```csharp
Stop commonStop = new Stop("Normal Delivery 1")
commonStop.Depot = "Main Warehouse";
// set stop address and add to plan...
Stop rareStop = new Stop("Uncommon Delivery 1");
rareStop.depot = "Auxillary Warehouse";
// set stop address and add to plan...

Vehicle van1 = new Vehicle("Van 1");
van1.Depot = "Main Warehouse";
Vehicle van2 = new Vehicle("Van 2");
van2.Depot = "Main Warehouse";
// add vehicles to plan...

Depot mainWarehouse = new Depot("Main Warehouse", "Somewhere");
Depot auxWarehouse = new Depot("Auxillary Warehouse", "Somewhere else");
// add depots to plan...

// solve and get results...
```

**IMPORTANT:** The value of the `Depot` properties MUST correspond to a matching `Depot.Name` in the same plan!

### Setting load constraints

Each vehicle can be set to have a cumulative maximum weight, volume and (non-cumulative) seating capacity which can be used to determine how many stops it can serve before it has to return to the depot. Conversely, each stop can also be assigned weight, volume and seating loads.
The fields are `WeightLoad`, `VolumeLoad`, `SeatingLoad` for Stops and `WeightCapacity`, `VolumeCapacity` and `SeatingCapacity` for Vehicles.

### Alternative connection types (for large datasets)

By default, all plans are solved in a _synchronous_ manner. Most small to medium-sized datasets can be solved in less than 10 seconds, but for production uses you probably may one to close the HTTP connection first and poll for updates in the following manner:

```csharp
Plan plan = new Plan("poll_plan")
plan.ConnectionType = Plan.ConnectionTypes.poll;
// do the usual stuff
// in an asynchronous context
await plan.Solve();
while(plan.Status != "planned"){
    await plan.Refresh();
    await Task.Delay(1000);
}
```

Setting the `Plan.ConnectionType` to `"Plan.ConnectionTypes.poll"` will cause the server to return you a response immediately after parsing the request data. You can monitor the status with the `Status` and `Progress` properties while fetching updates with the `Plan.Refresh()` method.

In addition, setting the `Plan.ConnectionType` to `"Plan.ConnectionTypes.Webhook"` will also cause the server to post a copy of the response to your said webhook. The exact location of the webhook can be specified with the `Webhook` property of `Plan` objects.
