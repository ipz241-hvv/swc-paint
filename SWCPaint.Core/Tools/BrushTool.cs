using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Tools;

public class BrushTool : PencilTool
{
    public BrushTool(double minThickness, double maxThickness)
        : base(minThickness, maxThickness)
    {
    }

    public override void OnMouseDown(Point point, ToolContext toolContext)
    {
        CurrentPolyline = new Polyline
        {
            StrokeColor = toolContext.Settings.StrokeColor,
            Thickness = toolContext.Settings.Thickness,
            IsSmooth = true
        };

        CurrentPolyline.Points.Add(point);
    }
}