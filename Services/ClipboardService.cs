using System.Threading.Tasks;
using Avalonia.Collections;
using IRis.ViewModels.Main.Canvas;

namespace IRis.Services;

public class ClipboardService(SerializationService serialization)
{
    private readonly SerializationService _serialization = serialization;

    public async void Copy(AvaloniaList<CircuitObjectViewModel> collection)
    {
        string json = _serialization.Serialize(collection);
        await App.Clipboard.SetTextAsync(json);
    }

    public async Task<AvaloniaList<CircuitObjectViewModel>> Paste()
    {
        var json = await App.Clipboard.GetTextAsync();
        if (json is null)
            return [];

        var pasted = _serialization.Deserialize(json);
        if (pasted is null)
            return [];

        return pasted;
    }
}
