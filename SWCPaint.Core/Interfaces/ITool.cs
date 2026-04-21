using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;
using SWCPaint.Core.Tools;

namespace SWCPaint.Core.Interfaces;

public interface ITool
{
    public Shape? ActiveShape { get; }
    public void OnMouseDown(Point point, ToolContext toolContext);
    public void OnMouseMove(Point point, ToolContext toolContext);
    public void OnMouseUp(Point point, ToolContext toolContext);
}