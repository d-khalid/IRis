# Logic Circuit Simulator

IRis is a circuit simulation software that allows design and simulation of digital logic circuits, along with generation of simulations from hand-drawn sketches without using any LLMs or paid API.

It is currently developed enough to be able to Simulate a Mini-CPU. The sketch-to-simulation system is built in python in a seperate repository and compiled to `.exe` to be used internally. For details, refer to [this](https://github.com/ShahzaibAhmad05/SketchLogic).

![C#](https://img.shields.io/badge/C%23-333333?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-333333?style=for-the-badge&logo=dotnet&logoColor=white)
![Avalonia](https://img.shields.io/badge/Avalonia-333333?style=for-the-badge&logo=avalonia&logoColor=white)

https://github.com/user-attachments/assets/afd92c8a-e5ac-4850-b85a-47a891b0bf08

---

## Distinguishing Features

- Sketch-to-Simulation conversion.
- A more engaging UX than any existing circuit simulation software.
- Convenient [keyboard shortcuts](#key-controls-for-circuit-design) for faster designing.

---

## Try it

Supports but not limited to: _Windows, Linux, and MacOS._

There are no compiled packages/installers yet, but the setup is arguably simple. If you want to run it on your PC, refer to the [Developer Setup](#developer-setup) section for instructions. After following those you will have the simulator running on your system.

However, for Sketch to Simulation Conversion feature, you would have to download the latest `.exe` release from [sketchlogic](https://github.com/ShahzaibAhmad05/SketchLogic/releases). Just put the `.exe` file in the project root and the simulator will pick it up.

---

## Key Controls For Circuit Design

- `R` for rotating components or entire circuits.
- `A` for adding one line of pins to the component.
- `S` for removing one line of pins from the component.
- `Esc` for dropping a component preview.
- `Ctrl+X`, `Ctrl+C`, `Ctrl+V` as shortcuts for cutting, copying, pasting components respectively.
- `Ctrl+Z`, `Ctrl+Y` for undo/redo commands.
- `Arrow keys` or `Ctrl+LeftClick` for canvas control and movement.
- `RightClick` for menu with options from **Simulate tab**.

---

## Developer Notes

All the text bellow is meant for developers who are working, or who want to work on this codebase. It's all human-written, so it's worth a read.

### Local Setup

- Install .NET SDK 9.0 from [here](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).
- Make sure it is installed properly by running `dotnet --version` in a terminal.
- Download the source code from here into a folder, open that folder in a terminal and execute `dotnet build`.
- .NET automatically resolves the dependencies, so it should build with no issues. Execute `dotnet run` to run the program.
- For sketch-to-simulation conversion, download the `sketchlogic.exe` file from [here](https://github.com/ShahzaibAhmad05/SketchLogic/releases/tag/v0.1.0) and place it in the project root. Then look for a `Generate Circuit From Image` option in the top menu of the simulator.

### Code Formatting

`CSharpier.MsBuild` has been configured in `IRis.csproj` for automatic code formatting on builds. Some rules that are not enforced by the code formatter are as follows:

- If a class variable has a multi-line declaration/assigment (including the line it is using for compiler directives), use one empty line after it for spacing.
- Always remove unused dependencies.
- Do not add code that is commented out, other than the chunks that are already there.
- Do not use docstrings/multi-line comments, long explanations are to be done in this `README.md`.
- Add single empty lines in axaml if code blocks isolate perfectly. Follow existing patterns.
- Put comments only where necessary. Try not to remove old comments unless you have to.

With the above in mind, try to keep the code formatting consistent with the existing code when you make changes.

> [!NOTE]
> Any edits to this section have to be repeated for `AGENTS.md`.

### Understanding the Architecture

The sections bellow are details of why and how the codebase architecture was built like this. Every major architectural choice is documented here, every decision was criticized repeatedly until everything came very close to perfection.

#### Dependency Injection

Inspired by the [official documentation: IoC](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/ioc) for `CommunityToolkit.Mvvm` and a few thoughts we had in mind, we implemented **Dependency Injection** for better code structure, maintainability, flexibility, etc.

This involves registering the static and instance-dependent services in `app.axaml.cs` and then calling `App.Current.Services.GetRequiredService<RequesterClass>()` assuming we declare an instance of the _RequesterClass_. This approach is followed for all the Services except for `CommandService` which would otherwise create cycles of service dependencies and hence errors, since services can use other services in their constructors.

It is to be noted that the true DI as mentioned in the documentation involves injecting services through the constructors. we had to take a different approach for components. They involve hierarchies and constructor chaining, so it's common sense that injecting services through the constructors would lead to more rigidity. So instead, we get the services in the constructor code blocks using `App.Current.Services.GetRequiredService<ServiceName>()`.

#### Initializing Components

All kinds of components are only initialized in the `ViewModels/Main/LeftSidebarViewModel.cs` so any changes in the constructors have to be kept consistent with that file.

Furthermore, initializing models has a certain process to it that has to be followed. For instance, adding an **AND Gate** involves declaring an instance with a new Output pin, and then adding two Input pins to it as in:

```csharp
AndGateViewModel gate = new() { Output = new() };
gate.Inputs.Add(new());
gate.Inputs.Add(new());
```

This process has been programmed to facilitate the construction of components during deserialization, which would otherwise cause untraceable bugs. To simplify this, we have already considered using `ComponentFactoryService` to abstract this logic, but that would be redundant as we are only initializing components in the above mentioned ViewModel.

#### ViewModels have Models

Every object in a circuit (except for Terminals) inherits from CircuitObjectViewModel and stores an instance of it's model privately because the logic is needed at various points during the object's lifetime.

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

#### Wire Cloning

Wire cloning is too tricky to be messed with. One IMPORTANT thing when you are working on this codebase would be to always clone a collection of objects together. NEVER EVER think of cloning each object separately. Otherwise their memory references would break, and you would end up with disconnected misbehaving circuit objects.

#### Clipboard Actions: Cut, Copy, Paste

All of these rely on JsonSerialization. When an object is copied it's Json is copied to the system clipboard in text. This can be verified by pasting it anywhere, that is, the json will be pasted. Hence clipboard actions depend on serialization/deserialization.

#### Singleton Pattern for Statefull Services

Singleton pattern is provided to us by `Microsoft.Extensions.DependencyInjection` which we are using for Statefull Services as present in `Services/Singleton/`.

Some services are abolutely static in their lifetimes (and so are their classes) so we are using them directly wherever needed because enforcing dependency injection for these would just be extra boilerplate which is never good for scalability/maintainability.

#### How to add new icons for Context Menus

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
