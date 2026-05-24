using Avalonia.Controls;
using Avalonia.Markup.Xaml;


namespace IRis.Views.Main;


public partial class TopMenuView : UserControl
{
    public TopMenuView()
    {
        InitializeComponent();
    }

    private void OnNewClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnOpenClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnSaveClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnSaveAsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnExitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}

    private void OnUndoClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnRedoClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}

    private void OnGenerateFromPromptClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnGenerateFromImageClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}

    private void OnAboutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
