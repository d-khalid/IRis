using System;
using System.Runtime.Serialization;
using IRis.Models.CircuitObjects;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using IRis.Services.Singleton;
using IRis.Services;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects;


public abstract partial class ComponentViewModel : CircuitObjectViewModel
{
    [ObservableProperty] [property: JsonIgnore] private double _x;
    [ObservableProperty] [property: JsonIgnore] private double _y;
    [ObservableProperty] [property: JsonIgnore] private double _width;
    [ObservableProperty] [property: JsonIgnore] private double _height;
    [ObservableProperty] private double _rotation;
    [JsonIgnore] public double TextRotation => -Rotation;

    public double CenterX
    {
        get => X + Width / 2.0;
        set => X = value - Width / 2.0;
    }

    public double CenterY
    {
        get => Y + Height / 2.0;
        set => Y = value - Height / 2.0;
    }

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
        if (!IsSelected)
        {
            if (!Selection.Get().IsEmpty()) 
                Selection.Get().UnHighlightAll();

            Selection.Get().Highlight(this);
        }

        DragService.StartAt(AppState.Get().MousePosition);
    }


    public static void PointerReleased()
    {
        if (DragService.IsRunning())
            DragService.Stop();
    }


    public void PointerEntered()
    {
        if (!WirePreview.Get().IsEmpty() || !Preview.Get().IsEmpty() || 
            DragService.IsRunning()) 
            return;

        if (!IsSelected) HoverEffectService.On(this);
    }


    public void PointerExited()
    {
        if (!WirePreview.Get().IsEmpty() || !Preview.Get().IsEmpty() || 
            DragService.IsRunning()) 
            return;

        if (!IsSelected && HoverEffectService.IsRunning()) 
            HoverEffectService.Stop();
    }


    public abstract void UpdateTerminals();
}
