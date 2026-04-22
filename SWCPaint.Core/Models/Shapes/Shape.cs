using SWCPaint.Core.Interfaces;

namespace SWCPaint.Core.Models.Shapes;

public abstract class Shape : LayerElement
{
    private double _thickness = 1;

    public Color StrokeColor { get; set; } = new Color();

    public double Thickness
    {
        get => _thickness;
        set
        {
            if (value < 0) throw new ArgumentException("Thickness cannot be lower than 0");
            _thickness = value;
        }
    }

    public abstract void Draw(IDrawingContext context);

    public abstract bool IsHit(Point point);
}