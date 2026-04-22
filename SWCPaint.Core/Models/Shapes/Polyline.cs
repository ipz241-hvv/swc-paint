using SWCPaint.Core.Interfaces;

namespace SWCPaint.Core.Models.Shapes;

public class Polyline : Shape
{
    public List<Point> Points { get; set; } = new List<Point>();

    public override BoundingBox Bounds
    {
        get
        {
            if (Points.Count == 0) return new BoundingBox(0, 0, 0, 0);

            double minX = Points.Min(p => p.X);
            double maxX = Points.Max(p => p.X);
            double minY = Points.Min(p => p.Y);
            double maxY = Points.Max(p => p.Y);

            return new BoundingBox(minX, minY, maxX - minX, maxY - minY);
        }
    }

    public Polyline() { }

    public override void Draw(IDrawingContext context)
    {
        if (Points.Count < 2) return;

        context.DrawPath(Points, StrokeColor, null, Thickness, false);
    }

    public override void Accept(IElementVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override bool IsHit(Point point)
    {
        throw new NotImplementedException();
    }

    public override void Move(double dx, double dy)
    {
        for (int i = 0; i < Points.Count; i++)
        {
            Points[i] = new Point(Points[i].X + dx, Points[i].Y + dy);
        }
    }
}
