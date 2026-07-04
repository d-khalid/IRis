using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;

namespace IRis.ViewModels.Main;

public partial class RightSidebarViewModel : ViewModelBase
{
    private readonly Selection _selection;

    [ObservableProperty]
    private ComponentViewModel? _selectedComponent;

    [ObservableProperty]
    private string _componentType = "";

    [ObservableProperty]
    private bool _isVisible;

    [ObservableProperty]
    private bool _isMultiInputVisible;

    [ObservableProperty]
    private string _inputCountText = "";

    public RightSidebarViewModel(Selection selection)
    {
        _selection = selection;
        _selection.Objects.CollectionChanged += (_, _) =>
        {
            if (_selection.Objects.Count == 1 && _selection.Objects[0] is ComponentViewModel co)
            {
                SelectedComponent = co;
                ComponentType = co.GetType().Name.Replace("ViewModel", "");
                IsVisible = true;

                if (co is MultiInputGateViewModel mig)
                {
                    InputCountText = mig.Inputs.Count.ToString();
                    IsMultiInputVisible = true;

                    mig.Inputs.CollectionChanged += (_, _) =>
                    {
                        InputCountText = mig.Inputs.Count.ToString();
                    };
                }
            }
            else
            {
                SelectedComponent = null;
                IsVisible = false;
                IsMultiInputVisible = false;
                InputCountText = "";
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
}
