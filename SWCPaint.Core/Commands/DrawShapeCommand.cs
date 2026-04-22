using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Commands;

public class AddElementCommand : IUndoableCommand
{
    private readonly Layer _layer;
    private readonly LayerElement _element;

    public string Name => "command.element.add";

    public AddElementCommand(Layer layer, LayerElement element)
    {
        _layer = layer;
        _element = element;
    }

    public void Execute()
    {
        _layer.Elements.Add(_element);
    }

    public void Undo()
    {
        _layer.Elements.Remove(_element);
    }
}