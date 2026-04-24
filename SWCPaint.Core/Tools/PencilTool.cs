using SWCPaint.Core.Commands;
using SWCPaint.Core.Interfaces.Tools;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Tools;

public class PencilTool : ITool
{
    private Polyline? _currentPolyline;
    public double MinThickness { get; }
    public double MaxThickness { get; }
    public LayerElement? ActiveElement => _currentPolyline;
    protected Polyline? CurrentPolyline
    {
        get => _currentPolyline;
        set => _currentPolyline = value;
    }

    public PencilTool(double minThickness, double maxThickness)
    {
        MinThickness = minThickness;
        MaxThickness = maxThickness;
    }

    public virtual void OnMouseDown(Point point, ToolContext toolContext)
    {
        _currentPolyline = new Polyline
        {
            StrokeColor = toolContext.Settings.StrokeColor,
            Thickness = toolContext.Settings.Thickness,
            IsSmooth = false
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

            var command = new AddElementCommand(
                toolContext.Project.CurrentLayer,
                _currentPolyline
            );
            toolContext.History.Execute(command);
        }
        
        _currentPolyline = null;
    }
}
