using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects;
using IRis.Services;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects;

public abstract partial class ComponentViewModel : CircuitObjectViewModel
{
    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    [ObservableProperty]
    [property: JsonIgnore]
    private double _width;

    [ObservableProperty]
    [property: JsonIgnore]
    private double _height;

    [ObservableProperty]
    private double _rotation;

    [JsonIgnore]
    public double TextRotation => -Rotation;

    partial void OnXChanged(double value) => UpdateTerminals();

    partial void OnYChanged(double value) => UpdateTerminals();

    partial void OnWidthChanged(double value) => UpdateTerminals();

    partial void OnHeightChanged(double value) => UpdateTerminals();

    partial void OnRotationChanged(double value)
    {
        UpdateTerminals();
        OnPropertyChanged(nameof(TextRotation));
    }

    private readonly AppState _appState;
    private readonly Selection _selection;
    private readonly WirePreview _wirePreview;
    private readonly Preview _preview;
    protected readonly SimulationService _simulationService;
    protected readonly DragService _dragService;
    protected readonly HoverEffectService _hoverEffectService;

    public ComponentViewModel(Component model)
        : base(model)
    {
        ZIndex = 1;

        _appState = App.Current.Services.GetRequiredService<AppState>();
        _selection = App.Current.Services.GetRequiredService<Selection>();
        _wirePreview = App.Current.Services.GetRequiredService<WirePreview>();
        _preview = App.Current.Services.GetRequiredService<Preview>();
        _simulationService = App.Current.Services.GetRequiredService<SimulationService>();
        _dragService = App.Current.Services.GetRequiredService<DragService>();
        _hoverEffectService = App.Current.Services.GetRequiredService<HoverEffectService>();
    }

    public override bool Intersects(Rect rect) => rect.Intersects(new Rect(X, Y, Width, Height));

    public override bool Contains(Point pt) => new Rect(X, Y, Width, Height).Contains(pt);

    public void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(sender as Control).Properties.IsLeftButtonPressed)
            return;

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        e.Handled = true;

        if (_appState.EditingAllowed)
        {
            if (!IsSelected)
            {
                if (!_selection.IsEmpty())
                    _selection.UnHighlightAll();

                _selection.Highlight(this);
            }

            _dragService.StartAt(_appState.MousePosition);
        }
        else if (this is ToggleViewModel t)
        {
            t.Toggle();
        }
    }

    public void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        e.Handled = true;

        if (!_appState.EditingAllowed)
            return;

        if (_dragService.IsRunning())
            _dragService.Stop();
    }

    public void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        if (!_wirePreview.IsEmpty() || !_preview.IsEmpty() || _dragService.IsRunning())
            return;

        if (!IsSelected)
            _hoverEffectService.On(this);
    }

    public void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        if (!_wirePreview.IsEmpty() || !_preview.IsEmpty() || _dragService.IsRunning())
            return;

        if (!IsSelected && _hoverEffectService.IsRunning())
            _hoverEffectService.Stop();
    }

    public abstract void UpdateTerminals();
}
