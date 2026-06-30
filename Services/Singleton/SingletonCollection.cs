using Avalonia.Collections;
using IRis.ViewModels.Main.Canvas;

namespace IRis.Services.Singleton;

/// <summary>
/// This can be used to create a singleton enumerable class. Just inherit from it and pass the name of
/// the child class as type argument. This class adds a public list of Objects to child classes.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class SingletonCollection<T> : SingletonBase<T>
    where T : SingletonBase<T>, new()
{
    public AvaloniaList<CircuitObjectViewModel> Objects { get; } = [];
}
