using SWCPaint.Core.Models;

namespace SWCPaint.Core.Interfaces;

public interface IDrawingContext
{
    void DrawLine(Point start, Point end, Color strokeColor, double thickness);
    void DrawRectangle(Point topLeft, double width, double height, Color strokeColor, Color? fillColor, double thickness);
    void DrawEllipse(Point center, double radiusX, double radiusY, Color strokeColor, Color? fillColor, double thickness);
    void DrawPath(IEnumerable<Point> points, Color strokeColor, Color? fillColor, double thickness, bool isClosed, bool isSmooth);
    void PushMask(IEnumerable<EraserPath> erasers);
    void PopMask();
    void BeginLayer();
    void EndLayer(IEnumerable<EraserPath> erasers);
}