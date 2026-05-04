using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Interfaces.Shapes;

namespace SWCPaint.Core.Models.Shapes;

public class Ellipse : BoxBoundedShape, IFillable
{
    public double RadiusX => Width / 2;
    public double RadiusY => Height / 2;
    public Color? FillColor { get; set; }

    public Ellipse(Point position, double width, double height) : base(position, width, height) { }
    
    public override void Draw(IDrawingContext canvasContext)
    {
        canvasContext.DrawEllipse(Center, RadiusX, RadiusY, StrokeColor, FillColor, Thickness);
    }

    public override void Accept(IElementVisitor visitor)
    {
        visitor.Visit(this);
    }
}
