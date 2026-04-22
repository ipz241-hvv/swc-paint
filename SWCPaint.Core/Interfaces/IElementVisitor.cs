using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Interfaces;

public interface IElementVisitor
{
    void Visit(Line line);
    void Visit(Rectangle rectangle);
    void Visit(Polyline polyline);
    void Visit(Ellipse ellipse);
    void Visit(EraserPath eraserPath);
}
