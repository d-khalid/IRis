<div align="center">
    <h1>Logic Circuit Simulator</h1>
    <p>Design on paper, take a picture, Simulate!</p>
</div>

IRis is a desktop app that allows design and simulation of digital logic circuits. It features generation of simulations from hand-drawn sketches without using LLMs or any paid API.

It is currently developed enough to be able to Simulate a Mini-CPU. The sketch-to-simulation system is built by the same developers in a [seperate repository](https://github.com/ShahzaibAhmad05/SketchLogic) and compiled to `.exe` to be used internally. This `README` is entirely human-written and human-maintained; it's worth a read.

<div align="center">
  <img src="https://img.shields.io/github/license/d-khalid/IRis?style=for-the-badge&color=333333" alt="License" />
  <img src="https://img.shields.io/github/actions/workflow/status/d-khalid/IRis/dotnet-desktop.yml?style=for-the-badge&color=333333" alt="Build Status" />
  <img src="https://img.shields.io/codecov/c/github/d-khalid/IRis?style=for-the-badge&color=333333" alt="Code Coverage" />
  <img src="https://img.shields.io/github/last-commit/d-khalid/IRis?style=for-the-badge&color=333333" alt="Last Commit" />
</div>
<br />

[https://github.com/user-attachments/assets/afd92c8a-e5ac-4850-b85a-47a891b0bf08](https://github.com/user-attachments/assets/afd92c8a-e5ac-4850-b85a-47a891b0bf08)

> [!NOTE]
> As evident from the screenshots and demo above, this simulation software has a dramatically better UI than [Logisim Evolution](https://github.com/logisim-evolution/logisim-evolution) and [CircuitVerse](https://circuitverse.org/). It has been built with a modern tech stack involving:
> 
> ![C#](https://img.shields.io/badge/C%23-333333?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-333333?style=for-the-badge&logo=dotnet&logoColor=white)
![Avalonia](https://img.shields.io/badge/Avalonia-333333?style=for-the-badge&logo=avalonia&logoColor=white)

---

## Sketch to Simulation Conversion

This is one of the distinguishing features of this simulator. We support sketch-to-simulation conversion via [sketchlogic](https://github.com/ShahzaibAhmad05/SketchLogic) built by the same developers. 

The user is expected to draw a logic circuit on paper, take a picture and open it in IRis, and it gets converted to an actual simulation that is exactly the same as the one drawn on paper. The software allows them to edit the circuit and simulate it to test different inputs, just like what they would do after a regular circuit design. Details of how this works internally can be checked from the repository mentioned above.

---

## Key Controls For Circuit Design

- `R` for rotating components or entire circuits.
- `A` for adding one line of pins to the component.
- `S` for removing one line of pins from the component.
- `Esc` for dropping a component preview.
- `Ctrl+X`, `Ctrl+C`, `Ctrl+V` as shortcuts for cutting, copying, pasting components respectively.
- `Ctrl+Z`, `Ctrl+Y` for undo/redo commands.
- `Arrow keys` or `Ctrl+LeftClick` for canvas control and movement.

---

## Try it

Supports & Tested on: _Windows, Linux, and MacOS._

To use this software on your PC, just download the latest `.zip` for your system from [releases](https://github.com/d-khalid/IRis/releases). After unzipping, open the folder, double-click on `IRis.exe`, and you will have the simulation software running in a window. **(no .NET required)**

Now for the Sketch to Simulation feature, you would have to download the latest `.exe` release from [sketchlogic](https://github.com/ShahzaibAhmad05/SketchLogic/releases). Just put the `.exe` file in the project root and the simulator will pick it up.

---

## How does the Simulation Work?

Everything in a canvas is described by one `AvaloniaList<CircuitObject>`. This list contains components and wires. All of them have a `Simulate()` function through which they access their logical layer, compute output and update their visual states. On the other hand, wires propagate signals instantaneously, independent of the frequency setting of the simulation.

The Clock components have adjustable frequencies (in Hz) from the right sidebar that opens when the user selects them. Each tick simulates a full cycle (e.g. High state first then low and so on). 

Simulation itself also offers an adjustable frequency (from the left sidebar, in the Canvas tab). This is recommended to be used for:

- learning purposes - that is, to see how a circuit behaves by applying a slower computation speed on each component.
- performance - lower cpu load or a better quality of life.

---

## Do we have limitations?

Yes, seriously. There are some sections of the code that involve AI-generated mathematics (which have the model and "reason for use" explicitly specified with comments) but this approach is deprecated and planned to be replaced. Also, we are currently not handling cycle detection, bad wiring and race-conditions.

Furthermore, we are missing some industry-grade features such as VHDL export support, built-in circuit libraries,  and a few essential components (just a few). 

Note that multiple components are being added every week, so the lack of basic components would be covered up soon, and as for the other features, they are being planned as worked on as you are reading this. They might just be about to get added with the next release.

---

## Why do we have a `AGENTS.md` file?

While it is obvious that just like this `README.md`, this codebase is entirely organic and pest-free (manually typed). Absolutely no AI-Agents have worked on this codebase ever. That is, they haven't made edits in it directly.

The `AGENTS.md` file is there to facilitate the use of AI-agents in **Ask Mode** to query about better approaches, more efficient code, tracing down logic in some places, etc. We believe this is how AI-agents should be used in programming, because only software engineers are smart enough to be making architectural decisions.

Decisions that involve trade-offs, deciding whether something is worth adding or removing, making the experience more seemless for a human, user-experience engineering, (and so on) none of these can ever be done by AI-agents.

The file instructs agents to use the least number of tokens and try to sound like a human. It is to be noted that sometimes AI-agents skip reading this file entirely, if it's the case with your agent too then just remind it to read before every prompt.

---

## Local Setup for Contributors

- Install .NET SDK 9.0 from [here](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).
- Make sure it is installed properly by running `dotnet --version` in a terminal.
- Download the source code from here into a folder, open that folder in a terminal and use `cd src` and `dotnet build`.
- .NET automatically resolves the dependencies, so it should build with no issues. Use `cd src` and `dotnet run` to run the program.
- For sketch-to-simulation conversion, download the `sketchlogic.exe` file from [here](https://github.com/ShahzaibAhmad05/SketchLogic/releases/tag/v0.1.0) and place it in the `src/` directory. Then look for a `Generate Circuit From Image` option in the top menu of the simulator.

---

## Code Formatting

`CSharpier.MsBuild` has been configured in `IRis.csproj` for automatic code formatting on builds. Some rules that are not enforced by the code formatter are as follows:

- If a class variable has a multi-line declaration/assigment (including the line it is using for compiler directives), use one empty line after it for spacing.
- Always remove unused dependencies.
- Do not add code that is commented out, other than the chunks that are already there.
- Do not use docstrings/multi-line comments, long explanations are to be done in this `README.md`.
- Add single empty lines in axaml if code blocks isolate perfectly. Follow existing patterns.
- Put comments only where necessary. Try not to remove old comments unless you have to.

With the above in mind, try to keep the code formatting consistent with the existing code when you make changes.

---

## Debugging

Logger is registered in services. It can be declared using any other service. Common logging levels used in IRis are `DEBUG`, `INFO`, `WARNING` and `ERROR`. More and more logs will be added as the app expands.

As for debugging a compiled app, it's `.exe` has to be run from a terminal which would then print debugging info as the app is running.

---

## CI & Testing

Continuous Integration has been configured in `.github/dotnet-desktop.yml`. This runs a sample build first, and then the testing module. Testing checks all the components against sample inputs and expected outputs using `[Theory]`, `InlineData` and `[Fact]` from `xunit`. To manually run testing, follow this:

```bash
cd tests
dotnet test
```

The same commands run during CI and automatically update code coverage with [CodeCov](https://about.codecov.io). Configuration for this process can be found in in `tests/IRis.Tests.csproj`. `codecov.yml` on the other hand, is there to stop their github bot from commenting on every PR as it is annoying.

---

## Publishing Releases

Move into `src/` directory since we have the code there. Available target systems as runtime identifiers are given in this [catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog).

```bash
dotnet publish IRis.csproj --configuration Release --runtime <RUNTIME_IDENTIFIER> --self-contained true --output ./publish/win
```

Note that we are using `--self-contained` to bundle the .NET runtime into the published folder. We then compress the folder to a `.zip` file to ship it with a release.

Releases have the version formatting of `Major.Minor.Patch`. As for release notes, auto-generate them and only keep the ones responsible for changes visible on the UI. We are currently publishing releases for `win-x64`, `win-x86`, `linux-x64`, `osx-x64` and `osx-arm64`.

Additionally, the sketch-to-simulation feature has to be kept optional, so users who want that feature have to download `sketchlogic.exe` and place it next to `IRis.exe` in the publish folder.

> [!NOTE]
> We prefer having a console alongside the app on windows, for transparency and partially because we consider our users to be smart enough to know that having a console printing state info is often more helpful than just a dead GUI.

---

## Understanding the Architecture

The sections bellow are details of why and how the codebase architecture was built like this. Every developer working on this codebase is requested to read this atleast once.

### Circuit Objects & Inheritance

The first thought that would come across a contributor's mind when browsing the codebase is the deep abstraction chain for the circuit objects. And honestly, that is a fair observation. To justify this, we would like to ask the contributor to look at the code the abstract classes are handling. Each one of them has it's purpose.

`CircuitObject` is a top-level class for every object that is in the circuit (except for the green Terminal dots which are owned by components and hence, is in a separate namespace named `IRis.ViewModels.Main.Canvas.Core`). It handles the properties common to all circuit objects: `Opacity`, `ZIndex`, `IsSelected`, `Simulate()`, `Reset()`, etc. Then we have `Component` and `Wire` and the abstraction goes down till concrete classes.

### Dependency Injection

Inspired by the [official documentation: IoC](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/ioc) for `CommunityToolkit.Mvvm` and a few thoughts we had in mind, we implemented **Dependency Injection** for better code structure, maintainability, flexibility, etc.

This involves registering the static and instance-dependent services in `app.axaml.cs` and then calling `App.Current.Services.GetRequiredService<RequestedService>()` including registration of the requested service as a singleton. This approach is followed for all the Services except for `CommandService` which would otherwise create cycles of service dependencies and hence errors, since services can use other services in their constructors.

It is to be noted that usually DI involves injecting services through the constructors. However, we had to take a different approach for components. They involve hierarchies and constructor chaining, so it's common sense that injecting services through the long constructor-chains is not feasible. So instead, we call the services in the constructors themselves using `App.Current.Services.GetRequiredService<ServiceName>()` and store instances of each service being used.

Furthermore, singleton pattern is provided to us by `Microsoft.Extensions.DependencyInjection` and we are using it for all Services as present in `Services/`. One thing to note here is that the classes here have clear separation of concerns so that there is no one `god class`. For more details, look at the code files for service classes.

### Initializing Components

All kinds of components are only initialized in the `ViewModels/Main/LeftSidebarViewModel.cs` so any changes in the constructors have to be kept consistent with that file.

Furthermore, initializing models has a certain process to it that has to be followed. If you look at the code you would find null forgiving operators at places, but that is intentional as deserialization needs a parameterless constructor path. For instance, adding an **AND Gate** involves declaring an instance with a new Output pin, and then adding two Input pins to it as in:

```csharp
AndGateViewModel gate = new() { Output = new() };
gate.Inputs.Add(new());
gate.Inputs.Add(new());
```

To simplify this approach, we have already considered using `ComponentFactoryService` to abstract this logic, but that would be redundant as we are only initializing components in the above mentioned ViewModel. Another important point here is that all components allow rotations and the mathematics behind it only allows them to be orthogonal, which is intentional. We would like to clarify that we do not plan to add rotations at arbitrary angles.

### ViewModels have Models

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

### Cloning & Clipboard

Wire cloning is too tricky to be messed with. One IMPORTANT thing when you are working on this codebase would be to always clone a collection of objects together. NEVER EVER think of cloning each object separately. Otherwise their memory references would break, and you would end up with disconnected misbehaving circuit objects.

Clipboard actions such as cut, copy, paste rely on JsonSerialization. When an object is copied it's Json is copied to the system clipboard in text. This can be verified by pasting it anywhere, that is, the json will be pasted. Hence clipboard actions depend on serialization/deserialization.

### How to add new icons for Context Menus?

Grab a Path geometry from [fluenticons](https://fluenticons.co/). Add it in a StreamGeometry tag in `app.axaml` as follows:

```xml
<StreamGeometry x:Key="arrow_left_regular">
    M10.295 19.716a1 1 0 0 0 1.404-1.425l-5.37-5.29h13.67a1 1 0 1 0 0-2H6.336L11.7 5.714a1 1 0 0 0-1.404-1.424l-6.924 6.822a1.25 1.25 0 0 0 0 1.78l6.924 6.823Z
</StreamGeometry>
```

Now refer to the key as a Static Resource in MenuIcon:

```xml
<MenuItem.Icon><PathIcon Data="{StaticResource save_edit}" /></MenuItem.Icon>
```

---

## License & Contributions

This project is licensed under `GPL-3.0`. For details, refer to [LICENSE](https://github.com/d-khalid/IRis?tab=GPL-3.0-1-ov-file).

Contributions are welcome, but only after understanding the documentation above, under strict reviewing and absolutely no AI-slop (refer to [Agents Use](#why-do-we-have-a-agentsmd-file)). For details on policies, refer to [CONTRIBUTING](https://github.com/d-khalid/IRis?tab=contributing-ov-file).
