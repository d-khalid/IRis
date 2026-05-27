using System.Collections.ObjectModel;
using Avalonia;
using IRis.ViewModels.Main.Canvas.Core;
using System.ComponentModel;
using IRis.Models.Main.Canvas.CircuitObjects;
using IRis.Models.Main.Canvas.Core;


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
        
        _mainInput = new(model.MainInput, TerminalType.Input, true);
        _mainInput.PropertyChanged += OnTerminalPropertyChanged;

        _mainOutput = new(model.MainOutput, TerminalType.Output, true);
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


    public bool Contains(Point pt)
    {
        return Points.Contains(pt);
    }
}
