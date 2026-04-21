using SWCPaint.Core.Interfaces.Shapes;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Commands;

public class DrawShapeCommand : IUndoableCommand
{
    private readonly Layer _layer;
    private readonly Shape _shape;

    public string Name => "command.shape.draw";

    public DrawShapeCommand(Layer layer, Shape shape)
    {
        _layer = layer;
        _shape = shape;
    }

    public void Execute()
    {
        _layer.Shapes.Add(_shape);
    }

    public void Undo()
    {
        _layer.Shapes.Remove(_shape);
    }
}