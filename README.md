# IRis - Circuit Simulator

IRis is an AI-powered circuit simulation software made with **Avalonia** and **C#**. It is currently developed enough to be able to Simulate a Mini-CPU. It uses [sketchlogic](https://github.com/ShahzaibAhmad05/SketchLogic) internally for sketch to simulation conversions.

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Avalonia](https://img.shields.io/badge/Avalonia-8B45BF?style=for-the-badge&logo=avalonia&logoColor=white)


---


## UI Snapshot

<img height="380" alt="snapshot of a 1-Bit ALU circuit designed in IRis" src="https://github.com/user-attachments/assets/e2043afb-a54b-42d8-a70e-7e8f389afaf1" />


> snapshot of a 1-Bit ALU circuit designed in IRis


---


## Platform Compatibility

Supports Windows, Linux, and MacOS.


---


## Try it

There are no compiled packages/installers yet, but the setup is arguably simple. If you want to run it on your PC, refer to the [Developer Setup](#developer-setup) section for instructions. After following those you will have the simulator running on your system.

However, for Sketch to Simulation Conversion feature, you would have to download the latest `.exe` release from [sketchlogic](https://github.com/ShahzaibAhmad05/SketchLogic/releases). Just put the `.exe` file in the project root and the simulator will pick it up.


---


## Architectural Notes


### SingletonCollection Class

`SingletonCollection` class can be used to access and view a list of CircuitObjects. It implements Singleton pattern in itself so a lot of code repetition is saved. Any child class must provide its name as argument when inheriting:

```csharp
public partial class Simulation : SingletonCollection<Simulation> { ... }
```

For getting an instance of a Child class, just call the GetInstance method. For example:

```csharp
var instance = Simulation.Get();
```


### Usage of Views

Most of the UI features are dealt with in Views, except where either the code file becomes too long/messy or where framework limitations come in our way, then we have to use ViewModels.


### ViewModels have Models

Every object in a circuit (except for Terminals) inherits from CircuitObjectViewModel and stores an instance of it's model privately just in case it is ever needed.

The `TerminalViewModel` for wires and gates can change on runtime (i.e. when a wire is attached to a gate), which is covered by allowing a method `TerminalViewModel.GetModel()` that allows access to the underlying terminal.

For each component, taking an example of a gate, lets say, we have X input terminals and 1 output terminal. We would need to store the terminal memory references, which `Newtonsoft.Json` currently does with id numbers. This helps mapping them back to connections during deserialization.

Furthermore, the component needs it's X,Y coordinates to be mapped back to it's position properly. It would also be necessary to keep the Rotation as that is also important during deserialization. Rest of the component's visual properties can be created on runtime.

One important thing here is that for `ToggleViewModel`, the State has to be kept in the json because it would be annoying to have to set the state of each of the toggles of our circuit each time we load it from a file. But the State is stored in the model which we are not serializing right? That's exactly the reason we have this wrapper for it in the `ToggleViewModel`:

```csharp
public LogicState State
{
    get => (Model as Toggle)!.State;
    set => (Model as Toggle)!.State = value;
}
```


### Wire Cloning/Copying

Wire cloning is too tricky to be messed with. One IMPORTANT thing if you are working on this codebase would be to always clone a collection of objects together. NEVER EVER think of cloning each object separately. Otherwise their memory references would break, and you would end up with disconnected weird-behaving circuit objects.


### Cloning Service

Currently it relies on JsonSerialization. One thing to note is that the entire app's functionality depend on cloning, and cloning depends on serialization. If serialization/deserialization breaks, nothing will behave as expected.


### Why did we use Singleton Pattern in Services?

We need Static classes to share states between different parts of IRis, but neither can static classes inherit from `ObservableObject` (considering we need those static classes to have observable properties) nor can they implement `INotifyPropertyChanged` which forces us to find a workaround.

We have chosen this workaround to be the singleton pattern.


### Adding Icons to Any Context Menu (globally)

Grab a Path geometry from [fluenticons](https://fluenticons.co/). Add it in a StreamGeometry tag in `app.axaml` as follows:

```xml
<StreamGeometry x:Key="arrow_left_regular">
    M10.295 19.716a1 1 0 0 0 1.404-1.425l-5.37-5.29h13.67a1 1 0 1 0 0-2H6.336L11.7 5.714a1 1 0 0 0-1.404-1.424l-6.924 6.822a1.25 1.25 0 0 0 0 1.78l6.924 6.823Z
</StreamGeometry>
```

Now refer to it as a Static Resource in MenuIcon:

```xml
<MenuItem.Icon><PathIcon Data="{StaticResource save_edit}" /></MenuItem.Icon>
```

---

## Developer Setup

- Install .NET SDK 9.0 from [here](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).
- Make sure it is installed properly by running `dotnet --version` in a terminal.  
- Download the source code from here into a folder, open that folder in a terminal and execute `dotnet build`.
- .NET automatically resolves the dependencies, so it should build with no issues. Execute `dotnet run` to run the program.
