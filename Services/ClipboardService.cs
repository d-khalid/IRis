using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas;


namespace IRis.Services;


/// <summary>
/// Sets and Gets the circuit from the system's clipboard. Might not work
/// if the system doesn't have an active clipboard.
/// 
/// Additionally, it implicitly pastes into Preview and gets the Mouse Position 
/// from AppState to update the Preview position once.
/// </summary>
public static class ClipboardService
{
    public static async void Copy(AvaloniaList<CircuitObjectViewModel> collection)
    {
        if (Application.Current?.ApplicationLifetime is not
            IClassicDesktopStyleApplicationLifetime app ||
            app.MainWindow?.Clipboard is not IClipboard ic) return;

        string json = SerializationService.Serialize(collection);
        await ic.SetTextAsync(json);
    }


    public static async void Paste()
    {
        if (Application.Current?.ApplicationLifetime is not
            IClassicDesktopStyleApplicationLifetime app ||
            app.MainWindow?.Clipboard is not IClipboard ic) return;

        var json = await ic.GetTextAsync();
        if (json is null) return;

        var pasted = SerializationService.Deserialize(json);
        if (pasted is null) return;

        var prev = Preview.Get();
        prev.Pick(pasted);
    }
}
