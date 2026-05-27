using System.Collections.ObjectModel;
using Avalonia;
using IRis.ViewModels.Main.Canvas.Core;
using System.ComponentModel;
using IRis.Models.Main.Canvas.CircuitObjects;
using IRis.Models.Main.Canvas.Core;
using IRis.Models.Core;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects;


public partial class WireViewModel : CircuitObjectViewModel
{
    public ObservableCollection<Point> Points { get; set; } = [];
    private TerminalViewModel _mainInput;
    private TerminalViewModel _mainOutput;


    public WireViewModel() : this(new Wire()) {}
    private WireViewModel(Wire model) : 
        base(model)
    {
        
        _mainInput = new TerminalViewModel() 
        {
            Type = TerminalType.Input,
            IsOrphan = true
        };
        _mainInput.PropertyChanged += OnTerminalPropertyChanged;

        _mainOutput = new TerminalViewModel()
        {
            Type = TerminalType.Output,
            IsOrphan = true
        };
        _mainOutput.PropertyChanged += OnTerminalPropertyChanged;
    }


    public TerminalViewModel MainInput { 
        get => _mainInput;
        set {
            _mainInput = value;
            (Model as Wire)!.MainInput = value.GetModel();
            _mainInput.PropertyChanged += OnTerminalPropertyChanged;
        }
    }


    public TerminalViewModel MainOutput { 
        get => _mainOutput;
        set {
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


    private void Redraw()
    {
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
