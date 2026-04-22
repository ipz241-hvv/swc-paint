using SWCPaint.Core.Models;

namespace SWCPaint.Core.Commands;

public class RenameLayerCommand : IUndoableCommand
{
    private readonly Layer _layer;
    private readonly string _oldName;
    private readonly string _newName;
    private readonly Action _onChanged;

    public RenameLayerCommand(Layer layer, string newName, Action onChanged)
    {
        _layer = layer;
        _oldName = layer.Name;
        _newName = newName;
        _onChanged = onChanged;
    }

    public string Name => "command.layer.rename";

    public void Execute()
    {
        _layer.Name = _newName;
        _onChanged?.Invoke();
    }

    public void Undo()
    {
        _layer.Name = _oldName;
        _onChanged?.Invoke();
    }
}