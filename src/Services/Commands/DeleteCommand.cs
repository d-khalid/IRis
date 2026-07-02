using Avalonia.Collections;
using IRis.ViewModels.Main.Canvas;

namespace IRis.Services.Commands;

public class DeleteCommand(
    AvaloniaList<CircuitObjectViewModel> originalList,
    AvaloniaList<CircuitObjectViewModel> toRemove
) : CommandBase
{
    private readonly AvaloniaList<CircuitObjectViewModel> _originalList = originalList;
    private readonly AvaloniaList<CircuitObjectViewModel> _collection = [.. toRemove];

    public override void Execute() => _originalList.RemoveAll(_collection);

    public override void Undo() => _originalList.AddRange(_collection);
}
