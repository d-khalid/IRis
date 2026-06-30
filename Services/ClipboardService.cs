using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using IRis.ViewModels.Main.Canvas;

namespace IRis.Services;

public static class ClipboardService
{
    public static async void Copy(AvaloniaList<CircuitObjectViewModel> collection)
    {
        if (
            Application.Current?.ApplicationLifetime
                is not IClassicDesktopStyleApplicationLifetime app
            || app.MainWindow?.Clipboard is not IClipboard ic
        )
            return;

        string json = SerializationService.Serialize(collection);
        await ic.SetTextAsync(json);
    }

    public static async Task<AvaloniaList<CircuitObjectViewModel>> Paste()
    {
        if (
            Application.Current?.ApplicationLifetime
                is not IClassicDesktopStyleApplicationLifetime app
            || app.MainWindow?.Clipboard is not IClipboard ic
        )
            return [];

        var json = await ic.GetTextAsync();
        if (json is null)
            return [];

        var pasted = SerializationService.Deserialize(json);
        if (pasted is null)
            return [];

        return pasted;
    }
}
