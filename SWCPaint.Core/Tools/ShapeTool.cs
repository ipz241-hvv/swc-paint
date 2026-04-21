using SWCPaint.Core.Commands;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Interfaces.Shapes;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Tools;

public class ShapeTool<TShape> : ITool where TShape : BoxBoundedShape
{
    private TShape? _currentShape;
    private Point _startPoint;

    public Shape? ActiveShape => _currentShape;

    public void OnMouseDown(Point point, ToolContext toolContext)
    {
        _startPoint = point;

        _currentShape = (TShape)Activator.CreateInstance(
            typeof(TShape),
            point, 0.0, 0.0)!;

        _currentShape.StrokeColor = toolContext.Settings.StrokeColor;
        _currentShape.Thickness = toolContext.Settings.Thickness;

        if (_currentShape is IFillable fillable)
        {
            fillable.FillColor = toolContext.Settings.FillColor;
        }
    }

    public void OnMouseMove(Point point, ToolContext toolContext)
    {
        if (_currentShape == null) return;

        double newX = Math.Min(_startPoint.X, point.X);
        double newY = Math.Min(_startPoint.Y, point.Y);
        double newWidth = Math.Abs(point.X - _startPoint.X);
        double newHeight = Math.Abs(point.Y - _startPoint.Y);

        double dx = newX - _currentShape.Position.X;
        double dy = newY - _currentShape.Position.Y;

        _currentShape.Move(dx, dy);
        _currentShape.Width = newWidth;
        _currentShape.Height = newHeight;
    }

    public void OnMouseUp(Point point, ToolContext toolContext)
    {
        if (_currentShape != null && (_currentShape.Width > 1 || _currentShape.Height > 1))
        {
            var command = new DrawShapeCommand(
                toolContext.Project.CurrentLayer,
                _currentShape
            );
            toolContext.History.Execute(command);
        }
        _currentShape = null;
    }
}