using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using IRis.ViewModels.CircuitObjects;
using Avalonia.Input;


namespace IRis.Views.Main;


public partial class MainWindowView : Window
{
    public MainWindowView()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
}
