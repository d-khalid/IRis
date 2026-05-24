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
