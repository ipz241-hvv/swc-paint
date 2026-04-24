using SWCPaint.Core.Commands;
using SWCPaint.Core.Interfaces.Tools;
using SWCPaint.Core.Models;

namespace SWCPaint.Core.Tools;

public class EraserTool : ITool
{
    private EraserPath? _currentEraser;
    public LayerElement? ActiveElement => _currentEraser;

    public double MinThickness { get; }
    public double MaxThickness { get; }

    public EraserTool(double minThickness, double maxThickness)
    {
        MinThickness = minThickness;
        MaxThickness = maxThickness;
    }

    public void OnMouseDown(Point point, ToolContext toolContext)
    {
        _currentEraser = new EraserPath(toolContext.Settings.Thickness);
        _currentEraser.AddPoint(point);
    }

    public void OnMouseMove(Point point, ToolContext toolContext)
    {
        _currentEraser?.AddPoint(point);
    }

    public void OnMouseUp(Point point, ToolContext toolContext)
    {
        if (_currentEraser != null)
        {
            _currentEraser.AddPoint(point);

            var command = new AddElementCommand(
                toolContext.Project.CurrentLayer,
                _currentEraser
            );
            toolContext.History.Execute(command);
        }
        _currentEraser = null;
    }
}