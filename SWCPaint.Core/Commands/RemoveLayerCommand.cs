using SWCPaint.Core.Models;

namespace SWCPaint.Core.Commands;

public class RemoveLayerCommand : IUndoableCommand
{
    private readonly Project _project;
    private readonly Layer _layerToRemove;
    private readonly int _removedIndex;
    private readonly Guid _previousActiveId;

    public string Name => "command.layer.remove";

    public RemoveLayerCommand(Project project, Guid layerId)
    {
        _project = project;
        _layerToRemove = project.Layers.First(l => l.Id == layerId);
        _removedIndex = project.Layers.ToList().IndexOf(_layerToRemove);
        _previousActiveId = project.CurrentLayerId;
    }

    public void Execute()
    {
        _project.RemoveLayer(_layerToRemove.Id);
    }

    public void Undo()
    {
        _project.InsertLayer(_removedIndex, _layerToRemove);
        _project.CurrentLayerId = _previousActiveId;
    }
}