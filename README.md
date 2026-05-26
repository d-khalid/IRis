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

Let's take an example of `AndGateViewModel` for simplicity. We prepare 3X `Terminal` for inputs and output respectively, then we create 1X `AndGate` and 3X `TerminalViewModel` from the 3X `Terminal` we have. Then we just pass everything to the constructor.

#### Baby-Sitting Models

The `TerminalViewModel` for wires and gates can change on runtime (i.e. when a wire is attached to a gate), which is covered by allowing a method `TerminalViewModel.GetModel()` that allows access to the underlying terminal.
