using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Controls.Primitives;
using Avalonia.Input;


namespace IRis.Views.Main;


public partial class MainWindowView : Window
{
    public MainWindowView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
