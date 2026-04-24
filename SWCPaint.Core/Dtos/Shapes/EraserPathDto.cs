namespace SWCPaint.Core.Dtos.Shapes;

public class EraserPathDto
{
    public string Type { get; set; } = "Eraser";
    public List<PointDto> Points { get; set; } = new();
    public double Thickness { get; set; }
}