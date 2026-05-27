using System;
using IRis.Models.Main.Canvas.CircuitObjects;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;
using Newtonsoft.Json;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects;


public abstract partial class ComponentViewModel : CircuitObjectViewModel
{
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;
    [ObservableProperty] [property: JsonIgnore] private double _width;
    [ObservableProperty] [property: JsonIgnore] private double _height;
    [ObservableProperty] private double _rotation;
    [JsonIgnore] public double TextRotation => -Rotation;

    partial void OnXChanged(double value) => UpdateTerminals();
    partial void OnYChanged(double value) => UpdateTerminals();
    partial void OnWidthChanged(double value) => UpdateTerminals();
    partial void OnHeightChanged(double value) => UpdateTerminals();


    partial void OnRotationChanged(double value)
    {
        UpdateTerminals();
        OnPropertyChanged(nameof(TextRotation));
    }


    public ComponentViewModel(Component model) : base(model)
    {
        ZIndex = 1;
    }


    public override bool Intersects(Rect rect)
    {
        return rect.Intersects(new Rect(X, Y, Width, Height));
    }


    public override bool Contains(Point pt)
    {
        return new Rect(X, Y, Width, Height).Contains(pt);
    }


    public void PointerPressed()
    {
        var sel = Selection.GetInstance();
        var drag = Drag.GetInstance();
        sel.DitchPartial();

        if (IsSelected)
            drag.StartWith(sel.Objects);
        else
        {
            sel.Focus(this);
            drag.StartWith(this);
        }
    }


    public void PointerReleased()
    {
        var drag = Drag.GetInstance();
        var sel = Selection.GetInstance();

        if (drag.HasObjects())
        {
            if (drag.Used)
                sel.AddCollection(drag.Objects);
            else
                sel.Focus(this);

            drag.End();
        }
    }


    public void PointerEntered()
    {
        var drag = Drag.GetInstance();
        var sel = Selection.GetInstance();
        var prev = Preview.GetInstance();

        if (prev.HasObjects() || drag.HasObjects()) return;
        if (!sel.Objects.Contains(this))
            sel.AddPartial(this);
    }


    public void PointerExited()
    {
        var sel = Selection.GetInstance();
        sel.DitchPartial();
    }


    protected abstract void UpdateTerminals();
}
