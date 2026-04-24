using System.Text.Json.Serialization;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Interfaces.Shapes;

namespace SWCPaint.Core.Models.Shapes;

public class Rectangle : BoxBoundedShape, IFillable
{
    public Color? FillColor { get; set; }

    public Rectangle(Point position, double width, double height) : base(position, width, height) { }

    public override void Accept(IElementVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override void Draw(IDrawingContext context)
    {
        context.DrawRectangle(Position, Width, Height, StrokeColor, FillColor, Thickness);
    }

    public override bool IsHit(Point point)
    {
        return Bounds.Contains(point);
    }
}
