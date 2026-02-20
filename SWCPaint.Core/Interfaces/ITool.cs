using SWCPaint.Core.Models;
using SWCPaint.Core.Services;

namespace SWCPaint.Core.Interfaces;

public interface ITool
{
    public abstract void OnMouseDown(Point point, Project project, DrawingSettings settings);
    public virtual void OnMouseMove(Point point, Project project) { }
    public virtual void OnMouseUp(Point point, Project project) { }
}

