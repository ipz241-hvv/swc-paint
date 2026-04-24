namespace SWCPaint.Core.Dtos;

public class LayerDto
{
    public string Name { get; set; } = null!;
    public bool IsVisible { get; set; }
    public List<object> Elements { get; set; } = new();
}
