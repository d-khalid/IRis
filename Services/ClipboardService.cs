using System.Threading.Tasks;
using Avalonia.Collections;
using IRis.ViewModels.Main.Canvas;

namespace IRis.Services;

public static class ClipboardService
{
    public static async void Copy(AvaloniaList<CircuitObjectViewModel> collection)
    {
        string json = SerializationService.Serialize(collection);
        await App.Clipboard.SetTextAsync(json);
    }

    public static async Task<AvaloniaList<CircuitObjectViewModel>> Paste()
    {
        var json = await App.Clipboard.GetTextAsync();
        if (json is null)
            return [];

        var pasted = SerializationService.Deserialize(json);
        if (pasted is null)
            return [];

        return pasted;
    }
}
