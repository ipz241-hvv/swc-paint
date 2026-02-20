using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Interfaces.Shapes;

namespace SWCPaint.Core.Models.Shapes;

public abstract class Shape : Entity
{
    double _thickness = 1;

    public Color StrokeColor { get; set; } = new Color();
    public double Thickness {
        get
        {
            return _thickness;
        }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Thickness cannot be lower than 0");
            }
            _thickness = value;
        }
    }
    public Point Position { get; set; }

    public Shape(Point position)
    {
        Position = position;
    }

    public abstract void Accept(IShapeVisitor visitor);

    public abstract void Draw(IDrawingContext context);

    public abstract bool IsHit(Point point);
}
