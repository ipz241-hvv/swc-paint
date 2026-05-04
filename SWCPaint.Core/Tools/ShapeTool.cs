using System;
using SWCPaint.Core.Commands;
using SWCPaint.Core.Interfaces.Shapes;
using SWCPaint.Core.Interfaces.Tools;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Tools;

public class ShapeTool<TShape> : ITool where TShape : BoxBoundedShape
{
    private TShape? _currentShape;
    private Point _startPoint;

    public LayerElement? ActiveElement => _currentShape;
    public double MinThickness { get; }
    public double MaxThickness { get; }

    public ShapeTool(double minThickness, double maxThickness)
    {
        MinThickness = minThickness;
        MaxThickness = maxThickness;
    }

    public void OnMouseDown(Point point, ToolContext toolContext)
    {
        _startPoint = point;
        _currentShape = CreateShapeInstance(point);
        
        if (_currentShape == null) return;

        ApplySettings(_currentShape, toolContext.Settings);
    }

    public void OnMouseMove(Point point, ToolContext toolContext)
    {
        if (_currentShape == null) return;

        UpdateShapeBounds(point);
    }

    public void OnMouseUp(Point point, ToolContext toolContext)
    {
        if (_currentShape == null) return;

        var currentLayer = toolContext.Project.CurrentLayer;
        
        if (currentLayer != null && IsValidSize(_currentShape))
        {
            var command = new AddElementCommand(currentLayer, _currentShape);
            toolContext.History.Execute(command);
        }

        _currentShape = null;
    }

    private TShape? CreateShapeInstance(Point point)
    {
        try 
        {
            return (TShape)Activator.CreateInstance(typeof(TShape), point, 0.0, 0.0)!;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void ApplySettings(TShape shape, ToolSettings settings)
    {
        shape.StrokeColor = settings.StrokeColor;
        shape.Thickness = settings.Thickness;

        if (shape is IFillable fillable)
        {
            fillable.FillColor = settings.FillColor;
        }
    }

    private void UpdateShapeBounds(Point currentPoint)
    {
        if (_currentShape == null) return;

        double left = Math.Min(_startPoint.X, currentPoint.X);
        double top = Math.Min(_startPoint.Y, currentPoint.Y);
        double width = Math.Abs(currentPoint.X - _startPoint.X);
        double height = Math.Abs(currentPoint.Y - _startPoint.Y);

        double dx = left - _currentShape.Position.X;
        double dy = top - _currentShape.Position.Y;

        _currentShape.Move(dx, dy);
        _currentShape.Width = width;
        _currentShape.Height = height;
    }

    private bool IsValidSize(TShape shape) => shape.Width > 1 || shape.Height > 1;
}
