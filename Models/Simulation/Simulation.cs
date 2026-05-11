using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

using IRis.Models.Components;
using IRis.Models.Core;


namespace IRis.Models;


public partial class Simulation : ObservableObject
{
    private readonly Canvas _canvas;

    public readonly List<Component> Components = [];
    public readonly List<Wire> Wires = [];
    private CircuitObject? _previewObject = null;

    private bool _isSimulating;
    private readonly DispatcherTimer _dispatcherTimer;


    [ObservableProperty]
    private Point _currentMousePos = new(0, 0);


    public Simulation(Canvas canvas)
    {
        _canvas = canvas;
        _dispatcherTimer = GetDispatcherTimer();
        DrawGridOnCanvas(canvas);

        _canvas.Cursor = new Cursor(StandardCursorType.Arrow);

        _canvas.PointerPressed += OnPointerPressed;
        _canvas.PointerMoved += OnPointerMoved;
        _canvas.PointerReleased += OnPointerReleased;
        _canvas.PointerWheelChanged += OnPointerWheel;
        _canvas.PointerEntered += OnPointerEnter;
        _canvas.PointerExited += OnPointerExit;
    }
    

    public bool IsSimulating
    {
        get => _isSimulating;
        set 
        {
            _isSimulating = value;

            if (_isSimulating)
            {
                PreviewObject = null;
                _dispatcherTimer.Start();
            }
            else 
            {
                _dispatcherTimer.Stop();

                foreach (Component c in Components)
                {
                    c.NullifyTerminalStates();
                    if (c is LogicProbe lp)
                        lp.State = LogicState.Unknown;
                }

                foreach (Wire wire in Wires)
                {
                    wire.NullifyTerminalStates();
                }
            }
        }
    }


    public CircuitObject? PreviewObject 
    {
        get => _previewObject;
        set 
        {
            _previewObject = value;
        }
    }
}

