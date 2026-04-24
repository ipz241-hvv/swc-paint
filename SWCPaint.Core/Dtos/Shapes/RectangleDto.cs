namespace SWCPaint.Core.Dtos.Shapes;

public class RectangleDto
{
    public string Type { get; set; } = "Rectangle";
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public ColorDto StrokeColor { get; set; } = null!;
    public ColorDto? FillColor { get; set; }
    public double Thickness { get; set; }
}