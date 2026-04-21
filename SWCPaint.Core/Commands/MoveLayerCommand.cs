using SWCPaint.Core.Models;

namespace SWCPaint.Core.Commands;

public class MoveLayerCommand : IUndoableCommand
{
    private readonly Project _project;
    private readonly Guid _layerId;
    private readonly int _fromIndex;
    private readonly int _toIndex;

    public string Name => "command.layer.move";

    public MoveLayerCommand(Project project, Guid layerId, int toIndex)
    {
        _project = project;
        _layerId = layerId;
        _toIndex = toIndex;
        _fromIndex = project.Layers.ToList().FindIndex(l => l.Id == layerId);
    }

    public void Execute() => _project.MoveLayer(_layerId, _toIndex);
    public void Undo() => _project.MoveLayer(_layerId, _fromIndex);
}