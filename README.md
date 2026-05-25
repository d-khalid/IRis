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

Every object in a circuit inherits from CircuitObjectViewModel and stores an instance of it's model privately.
