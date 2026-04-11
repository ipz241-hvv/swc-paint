namespace SWCPaint.Core.Models;

public readonly record struct BoundingBox(double X, double Y, double Width, double Height)
{
    public double Right => X + Width;
    public double Bottom => Y + Height;
    public Point Center => new Point(X + Width / 2, Y + Height / 2);
    public bool Contains(Point point) =>
        point.X >= X && point.X <= Right && point.Y >= Y && point.Y <= Bottom;
}
