# IRis - Circuit Simulator

IRis is an AI-powered circuit simulation software made with **Avalonia** and **C#**. It is currently under continuous development. It uses a vibe-coded YOLO model to generate simulations from sketches.

<br>

<img src="https://drive.google.com/uc?export=view&id=1NguAusaeMhvOUsnOimggUou8rtfbO2aD" alt="Image">

---

## Documentation

### Models

#### ManagerBase:

BaseManager class can be used to access and view a list of CircuitObjects. It implements Singleton pattern in itself so a lot of code repetition is saved. Any child class must provide its name as argument when inheriting:

```csharp
public partial class Simulation : ManagerBase<Simulation> { ... }
```

The Show() and Hide() functions of the BaseManager can work differently for each child class, based on how it is wired in the frontend. For instance, in Selection class, the methods are being used to show/hide the SelectionBox. Some child class may not use these methods at all, but they are completely relatable and handy for each.

For getting an instance of a Child class, just call the GetInstance method. For example:

```csharp
var instance = Simulation.GetInstance();
```

### Views

#### Usage of Views

Most of the interactive functionality is dealt with in views, except where either the code file becomes too long/messy or where framework limitations come in our way, then we have to use ViewModels.

This might be lowkey a bad design choice. But I don't plan in changing it soon, so be it.

#### Adding Icons to Menu

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


### ViewModels

#### CircuitObjects Structure

Every object in a circuit inherits from CircuitObjectViewModel and stores an instance of it's model privately just in case it is ever needed.

#### Creating a Wire

For instantiating a WireViewModel, we have to create 2X `Terminal`, 1X `Wire` with those terminals, and 2X `TerminalViewModel` with those terminals. Simple, right?

#### Creating a Component

Let's take an example of `AndGateViewModel` for simplicity. We create a new instance, put in the Output terminal using an object initializer, and then add two inputs to make it look sane:

```csharp
AndGateViewModel gate = new() { Output = new() };
gate.Inputs.Add(new());
gate.Inputs.Add(new());
```

Normally the Type field of the `TerminalViewModel` has to be explicitly mentioned, but in this case, the `AndGateViewModel` Inputs list intercept function assigns the type implicitly. Also, the field **IsOrphan** is set to false by default and is only meant to be used for previews.


### Models

These are frankly the slaves of the entire app. They are held captive by ViewModels and created and managed privately inside ViewModels. None of the outside logic should be referring to these except for the case of `TerminalViewModel`.

The `TerminalViewModel` for wires and gates can change on runtime (i.e. when a wire is attached to a gate), which is covered by allowing a method `TerminalViewModel.GetModel()` that allows access to the underlying terminal.


### Json Serialization

Take a sip of copium please. What you'll read next might be tough to swallow. And small bites are always appreciated.

#### Components

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

#### Wires

Wire cloning is too tricky to be messed with. One IMPORTANT thing if you are working on this codebase would be to always clone a collection of objects together. NEVER EVER think of cloning each object separately. Otherwise their memory references would break, and you would end up with disconnected weird-behaving wires.
