using System.Collections.ObjectModel;
using Avalonia;
using IRis.ViewModels.Main.Canvas.Core;
using System.ComponentModel;
using IRis.Models.CircuitObjects;
using IRis.Models.Core;
using IRis.Services;
using IRis.Services.Singleton;
using Newtonsoft.Json;
using System;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects;


public partial class WireViewModel() : CircuitObjectViewModel(new Wire())
{
    [JsonIgnore] public ObservableCollection<Point> Points { get; } = [];


    [ObservableProperty] private TerminalViewModel _mainInput = null!;
    partial void OnMainInputChanged(TerminalViewModel value)
    {
        (Model as Wire)!.MainInput = value.GetModel();
        value.PropertyChanged += OnTerminalPropertyChanged;
    }


    [ObservableProperty] private TerminalViewModel _mainOutput = null!;
    partial void OnMainOutputChanged(TerminalViewModel value)
    {
        (Model as Wire)!.MainOutput = value.GetModel();
        value.PropertyChanged += OnTerminalPropertyChanged;
    }


    private void OnTerminalPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(TerminalViewModel.X) or nameof(TerminalViewModel.Y))
            Redraw();
    }


    public void Redraw()
    {
        if (MainInput is null || MainOutput is null) return;

        Points.Clear();
        Points.Add(new Point((int)MainInput.X, (int)MainInput.Y));
        Points.Add(new Point((int)MainOutput.X, (int)MainOutput.Y));
    }


    public override bool Contains(Point pt)
    {
        return Points.Contains(pt);
    }


    public override bool Intersects(Rect rect)
    {
        for (int i = 0; i < Points.Count - 1; i++)
        {
            if (rect.Contains(Points[i]) || rect.Contains(Points[i + 1]))
                return true;
            else if (new Rect(Points[i], Points[i + 1]).Inflate(6).Intersects(rect))
                return true;
        }

        return false;
    }


    public void PointerPressed()
    {
        if (!IsSelected) Selection.GetInstance().Highlight(this);
    }


    public void PointerEntered()
    {
        if (!Preview.GetInstance().IsEmpty() || DragService.IsRunning()) return;
        if (!IsSelected) HoverEffectService.On(this);
    }


    public void PointerExited()
    {
        if (!Preview.GetInstance().IsEmpty() || DragService.IsRunning()) return;
        if (!IsSelected && HoverEffectService.IsRunning()) HoverEffectService.Stop();
    }
}
