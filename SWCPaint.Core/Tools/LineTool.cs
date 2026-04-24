using SWCPaint.Core.Commands;
using SWCPaint.Core.Interfaces.Tools;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Tools;

public class LineTool : ITool
{
    private Line? _currentLine;
    public LayerElement? ActiveElement => _currentLine;

    public double MinThickness { get; }
    public double MaxThickness { get; }

    public LineTool(double minThickness, double maxThickness)
    {
        MinThickness = minThickness;
        MaxThickness = maxThickness;
    }

    public void OnMouseDown(Point point, ToolContext toolContext)
    {
        _currentLine = new Line(point, point)
        {
            StrokeColor = toolContext.Settings.StrokeColor,
            Thickness = toolContext.Settings.Thickness
        };
    }

    public void OnMouseMove(Point point, ToolContext toolContext)
    {
        if (_currentLine == null) return;

        _currentLine.End = point;
    }

    public void OnMouseUp(Point point, ToolContext toolContext)
    {
        if (_currentLine != null)
        {
            if (Math.Abs(_currentLine.Start.X - _currentLine.End.X) < 1 &&
                Math.Abs(_currentLine.Start.Y - _currentLine.End.Y) < 1)
            {
                _currentLine = null;
                return;
            }

            _currentLine.End = point;

            var command = new AddElementCommand(
                toolContext.Project.CurrentLayer,
                _currentLine
            );

            toolContext.History.Execute(command);
        }

        _currentLine = null;
    }
}