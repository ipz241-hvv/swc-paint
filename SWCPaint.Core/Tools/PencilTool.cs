using SWCPaint.Core.Commands;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;
using SWCPaint.Core.Services;

namespace SWCPaint.Core.Tools;

public class PencilTool : ITool
{
    private Polyline? _currentPolyline;
    public Shape? ActiveShape => _currentPolyline;

    public PencilTool() {}

    public void OnMouseDown(Point point, ToolContext toolContext)
    {
        _currentPolyline = new Polyline
        {
            StrokeColor = toolContext.Settings.StrokeColor,
            Thickness = toolContext.Settings.Thickness
        };

        _currentPolyline.Points.Add(point);
    }

    public void OnMouseMove(Point point, ToolContext toolContext)
    {
        _currentPolyline?.Points.Add(point);
    }

    public void OnMouseUp(Point point, ToolContext toolContext)
    {
        if (_currentPolyline != null) 
        {
            _currentPolyline.Points.Add(point);

            var command = new DrawShapeCommand(
                toolContext.Project.CurrentLayer,
                _currentPolyline
            );
            toolContext.History.Execute(command);
        }
        
        _currentPolyline = null;
    }
}
