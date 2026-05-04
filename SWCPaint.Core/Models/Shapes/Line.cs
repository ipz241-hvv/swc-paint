using SWCPaint.Core.Interfaces;

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

    public override void Accept(IElementVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override void Move(double dx, double dy)
    {
        Start = new Point(Start.X + dx, Start.Y + dy);
        End = new Point(End.X + dx, End.Y + dy);
    }
}
