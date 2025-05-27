// AIGenerationWindow.axaml.cs

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using IRis.ViewModels;

namespace IRis.Views;

public partial class AIGenerationWindow : Window
{
    public AIGenerationWindow()
    {
        InitializeComponent();
        DataContext = new AIGenerationWindowViewModel(this);
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}