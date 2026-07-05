using System;
using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;
using Newtonsoft.Json;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class ClockViewModel : ComponentViewModel
{
    [ObservableProperty]
    private TerminalViewModel _output = null!;

    [ObservableProperty]
    [property: JsonIgnore]
    private string _label = "0";

    private readonly DispatcherTimer _timer;

    public ClockViewModel()
        : this(new Clock()) { }

    private ClockViewModel(Clock model)
        : base(model)
    {
        Width = Height = 20;

        _timer = new DispatcherTimer();
        _timer.Tick += (_, _) =>
        {
            var clock = (Model as Clock)!;
            clock.State = clock.State == LogicState.High ? LogicState.Low : LogicState.High;
            clock.Output.State = clock.State;
        };
    }

    partial void OnOutputChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as Clock)!.Output = value.GetModel();

        value.GetModel().PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(Terminal.State))
            {
                Label = value.GetModel().State switch
                {
                    LogicState.High => "1",
                    LogicState.Low => "0",
                    _ => "?",
                };
            }
        };
    }

    public double FrequencyHz
    {
        get => (Model as Clock)!.FrequencyHz;
        set
        {
            (Model as Clock)!.FrequencyHz = value;

            if (_timer.IsEnabled)
                _timer.Interval = TimeSpan.FromMilliseconds(1000 / (2 * value));
        }
    }

    public override void Simulate()
    {
        if (_timer.IsEnabled)
            return;

        _timer.Interval = TimeSpan.FromMilliseconds(1000 / (2 * FrequencyHz));
        _timer.Start();
    }

    public override void Reset()
    {
        _timer.Stop();
        base.Reset();
    }

    public override void UpdateTerminals()
    {
        if (Output is null)
            return;

        double unrotatedX = X + (Width + 10);
        double unrotatedY = Y + (Height / 2);

        Point rotatedPos = _simulationService.RotateTerminalPosition(
            unrotatedX,
            unrotatedY,
            Rotation,
            Width,
            Height,
            X,
            Y
        );

        Output.X = rotatedPos.X;
        Output.Y = rotatedPos.Y;
    }
}
