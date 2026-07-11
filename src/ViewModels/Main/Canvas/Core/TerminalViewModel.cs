using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;
using IRis.Services;
using IRis.Services.Singleton;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace IRis.ViewModels.Main.Canvas.Core;

public partial class TerminalViewModel : ObservableObject
{
    private Terminal Model { get; } = new Terminal();

    public Terminal GetModel() => Model;

    public TerminalType Type;

    [JsonIgnore]
    public bool IsOrphan = false;

    [ObservableProperty]
    private double _x = 0;

    [ObservableProperty]
    private double _y = 0;

    [ObservableProperty]
    [property: JsonIgnore]
    private IBrush _color = null!;

    [ObservableProperty]
    [property: JsonIgnore]
    private Cursor _cursor = new(StandardCursorType.Arrow);

    private readonly AppState _appState;
    private readonly Selection _selection;
    private readonly WirePreview _wirePreview;
    private readonly HoverEffectService _hoverEffectService;

    public TerminalViewModel()
    {
        _appState = App.Current.Services.GetRequiredService<AppState>();
        _selection = App.Current.Services.GetRequiredService<Selection>();
        _wirePreview = App.Current.Services.GetRequiredService<WirePreview>();
        _hoverEffectService = App.Current.Services.GetRequiredService<HoverEffectService>();

        Model.PropertyChanged += (_, e) =>
        {
            if (_appState.TerminalColorChangeAllowed && e.PropertyName is nameof(Terminal.State))
                UpdateColor();
        };

        _appState.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(_appState.TerminalColorChangeAllowed))
            {
                UpdateColor();
            }
        };

        UpdateColor();
    }

    private void UpdateColor()
    {
        string resource;

        if (_appState.TerminalColorChangeAllowed)
        {
            resource = Model.State switch
            {
                LogicState.High => "HighStateBrush",
                LogicState.Low => "LowStateBrush",
                LogicState.Unknown => "UnknownStateBrush",
                _ => "UnknownStateBrush",
            };
        }
        else
        {
            resource = "UnknownStateBrush";
        }

        App.Current.TryGetResource(resource, _appState.Theme, out var res);
        Color = (IBrush)res!;
    }

    public void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(sender as Control).Properties.IsLeftButtonPressed)
            return;

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        e.Handled = true;

        if (!_appState.EditingAllowed)
            return;

        if (!_selection.IsEmpty())
            _selection.UnHighlightAll();

        _hoverEffectService.Stop();

        if (_wirePreview.IsEmpty())
            _wirePreview.StartAt(this);
        else
            _wirePreview.EndAt(this);
    }

    public void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        e.Handled = true;

        if (!_appState.EditingAllowed)
            return;

        Cursor = new(StandardCursorType.Cross);
        _hoverEffectService.Hide();
    }

    public void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        e.Handled = true;

        if (!_appState.EditingAllowed)
            return;

        Cursor = new(StandardCursorType.Arrow);
        _hoverEffectService.Show();
    }
}
