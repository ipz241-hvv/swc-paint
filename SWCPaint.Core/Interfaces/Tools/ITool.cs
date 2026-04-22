using SWCPaint.Core.Models;
using SWCPaint.Core.Tools;

namespace SWCPaint.Core.Interfaces.Tools;

public interface ITool
{
    public LayerElement? ActiveElement { get; }
    public void OnMouseDown(Point point, ToolContext toolContext);
    public void OnMouseMove(Point point, ToolContext toolContext);
    public void OnMouseUp(Point point, ToolContext toolContext);
}