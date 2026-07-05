using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;

namespace IRis.ViewModels.Main;

public partial class RightSidebarViewModel : ViewModelBase
{
    private readonly Selection _selection;

    [ObservableProperty]
    private bool _isVisible;

    [ObservableProperty]
    private ComponentViewModel? _selectedComponent;

    [ObservableProperty]
    private string _componentType = "";

    [ObservableProperty]
    private bool _isMultiInputVisible;

    [ObservableProperty]
    private string _inputCountText = "";

    [ObservableProperty]
    private bool _isClockVisible;

    [ObservableProperty]
    private string _frequencyText = "";

    public RightSidebarViewModel(Selection selection)
    {
        _selection = selection;

        _selection.Objects.CollectionChanged += (_, _) =>
        {
            IsVisible = false;
            IsMultiInputVisible = false;
            IsClockVisible = false;

            if (_selection.Objects.Count == 1 && _selection.Objects[0] is ComponentViewModel co)
            {
                IsVisible = true;
                SelectedComponent = co;
                ComponentType = co.GetType().Name.Replace("ViewModel", "");

                if (co is MultiInputGateViewModel mig)
                {
                    InputCountText = mig.Inputs.Count.ToString();
                    IsMultiInputVisible = true;

                    mig.Inputs.CollectionChanged += (_, _) =>
                    {
                        InputCountText = mig.Inputs.Count.ToString();
                    };
                }
                else if (co is ClockViewModel clock)
                {
                    IsClockVisible = true;
                    FrequencyText = clock.FrequencyHz.ToString();
                }
            }
        };
    }

    partial void OnInputCountTextChanged(string value)
    {
        if (int.TryParse(value, out int count) && count >= 2 && count <= 50)
        {
            if (SelectedComponent is not MultiInputGateViewModel mig || count == mig.Inputs.Count)
                return;

            if (count > mig.Inputs.Count)
            {
                while (mig.Inputs.Count < count)
                {
                    mig.AddInput();
                }
            }
            else
            {
                while (mig.Inputs.Count > count)
                {
                    mig.RemoveInput();
                }
            }
        }
    }

    partial void OnFrequencyTextChanged(string value)
    {
        if (
            double.TryParse(value, out double hz)
            && hz >= 0.01
            && hz <= 100
            && SelectedComponent is ClockViewModel clock
        )
        {
            clock.FrequencyHz = hz;
        }
    }
}
