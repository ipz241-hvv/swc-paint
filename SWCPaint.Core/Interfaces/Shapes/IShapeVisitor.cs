using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Interfaces.Shapes;

public interface IShapeVisitor
{
    void Visit(Line line);
    void Visit(Rectangle rectangle);
    void Visit(Polyline polyline);
    void Visit(Ellipse ellipse);
}
