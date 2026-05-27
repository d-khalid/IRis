using System.Collections.ObjectModel;
using Avalonia;
using IRis.ViewModels.Main.Canvas.Core;
using System.ComponentModel;
using IRis.Models.Main.Canvas.CircuitObjects;
using IRis.Models.Main.Canvas.Core;
using IRis.Models.Core;
using Newtonsoft.Json;
using System;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects;


public partial class WireViewModel() : CircuitObjectViewModel(new Wire())
{
    [JsonIgnore] public ObservableCollection<Point> Points { get; } = [];

    private TerminalViewModel _mainInput = null!;
    public TerminalViewModel MainInput { 
        get => _mainInput;
        set {
            value.Type = TerminalType.Input;
            _mainInput = value;
            (Model as Wire)!.MainInput = value.GetModel();
            _mainInput.PropertyChanged += OnTerminalPropertyChanged;
        }
    }

    private TerminalViewModel _mainOutput = null!;
    public TerminalViewModel MainOutput { 
        get => _mainOutput;
        set {
            value.Type = TerminalType.Output;
            _mainOutput = value;
            (Model as Wire)!.MainOutput = value.GetModel();
            _mainOutput.PropertyChanged += OnTerminalPropertyChanged;
        }
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
        var sel = Selection.GetInstance();
        sel.DitchPartial();
        sel.Focus(this);
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
        Selection.GetInstance().DitchPartial();
    }
}
