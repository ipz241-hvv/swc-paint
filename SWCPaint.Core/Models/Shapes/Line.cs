using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Interfaces.Shapes;

namespace SWCPaint.Core.Models.Shapes;

public class Line : Shape
{
    public Point Start { get; set; }
    public Point End { get; set; }

    public override BoundingBox Bounds
    {
        get
        {
            double x = Math.Min(Start.X, End.X);
            double y = Math.Min(Start.Y, End.Y);
            double width = Math.Abs(Start.X - End.X);
            double height = Math.Abs(Start.Y - End.Y);

            return new BoundingBox(x, y, width, height);
        }
    }

    public Line(Point start, Point end)
    {
        Start = start;
        End = end;
    }

    public override void Draw(IDrawingContext context)
    {
        context.DrawLine(Start, End, StrokeColor, Thickness);
    }

    public override void Accept(IShapeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override bool IsHit(Point point)
    {
        double minX = Math.Min(Start.X, End.X) - 5;
        double maxX = Math.Max(Start.X, End.X) + 5;
        double minY = Math.Min(Start.Y, End.Y) - 5;
        double maxY = Math.Max(Start.Y, End.Y) + 5;

        return point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY;
    }

    public override void Move(double dx, double dy)
    {
        Start = new Point(Start.X + dx, Start.Y + dy);
        End = new Point(End.X + dx, End.Y + dy);
    }
}
