namespace SWCPaint.Core.Dtos.Shapes;

public class PolylineDto
{
    public string Type { get; set; } = "Polyline";
    public List<PointDto> Points { get; set; } = new();
    public ColorDto StrokeColor { get; set; } = null!;
    public double Thickness { get; set; }
    public bool IsSmooth { get; set; }
}