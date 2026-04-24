namespace SWCPaint.Core.Dtos.Shapes;

public class LineDto
{
    public string Type { get; set; } = "Line";
    public PointDto Start { get; set; } = null!;
    public PointDto End { get; set; } = null!;
    public ColorDto StrokeColor { get; set; } = null!;
    public double Thickness { get; set; }
}