using SWCPaint.Core.Interfaces;

namespace SWCPaint.Core.Models;

public class EraserPath : LayerElement
{
    private readonly List<Point> _points = new();
    public IReadOnlyList<Point> Points => _points.AsReadOnly();
    public double Thickness { get; set; }

    public EraserPath(double thickness)
    {
        Thickness = thickness;
    }

    public void AddPoint(Point point)
    {
        if (_points.Count > 0)
        {
            var last = _points[^1];

            if (Math.Sqrt(Math.Pow(point.X - last.X, 2) + Math.Pow(point.Y - last.Y, 2)) < 2)
                return;
        }
        _points.Add(point);
    }

    public override BoundingBox Bounds
    {
        get
        {
            if (_points.Count == 0)
                return new BoundingBox(0, 0, 0, 0);

            double minX = _points.Min(p => p.X);
            double maxX = _points.Max(p => p.X);
            double minY = _points.Min(p => p.Y);
            double maxY = _points.Max(p => p.Y);

            double margin = Thickness / 2;

            double x = minX - margin;
            double y = minY - margin;
            double width = (maxX - minX) + Thickness;
            double height = (maxY - minY) + Thickness;

            return new BoundingBox(x, y, width, height);
        }
    }

    public override void Move(double dx, double dy)
    {
        for (int i = 0; i < _points.Count; i++)
        {
            _points[i] = new Point(_points[i].X + dx, _points[i].Y + dy);
        }
    }

    public override void Accept(IElementVisitor visitor)
    {
        visitor.Visit(this);
    }
}