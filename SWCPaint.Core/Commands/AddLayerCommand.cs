using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;

namespace SWCPaint.Core.Commands;

public class AddLayerCommand : IUndoableCommand
{
    private readonly Project _project;
    private readonly Layer _newLayer;
    private readonly Guid _previousLayerId;

    public string Name => "command.layer.add";

    public AddLayerCommand(Project project, string layerName)
    {
        _project = project;
        _previousLayerId = project.CurrentLayerId;
        _newLayer = new Layer(layerName);
    }

    public void Execute()
    {
        _project.AddLayer(_newLayer);
        _project.CurrentLayerId = _newLayer.Id;
    }

    public void Undo()
    {
        _project.RemoveLayer(_newLayer.Id);
        _project.CurrentLayerId = _previousLayerId;
    }
}