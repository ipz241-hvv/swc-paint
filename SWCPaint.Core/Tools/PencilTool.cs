using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;
using SWCPaint.Core.Services;

namespace SWCPaint.Core.Tools;

public class PencilTool : ITool
{
    private Polyline? _currentPolyline;

    public PencilTool() {}

    public void OnMouseDown(Point point, ToolContext toolContext)
    {
        _currentPolyline = new Polyline
        {
            StrokeColor = toolContext.Settings.StrokeColor,
            Thickness = toolContext.Settings.Thickness
        };

        _currentPolyline.Points.Add(point);

        toolContext.Project.CurrentLayer.Shapes.Add(_currentPolyline);
    }

    public void OnMouseMove(Point point, ToolContext toolContext)
    {
        _currentPolyline?.Points.Add(point);
    }

    public void OnMouseUp(Point point, ToolContext toolContext)
    {
        _currentPolyline?.Points.Add(point);
        _currentPolyline = null;
    }
}
