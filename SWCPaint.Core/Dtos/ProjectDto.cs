using SWCPaint.Core.Models;

namespace SWCPaint.Core.Dtos;

public class ProjectDto
{
    public double Width { get; set; }
    public double Height { get; set; }
    public ColorDto BackgroundColor { get; set; } = null!;
    public List<LayerDto> Layers { get; set; } = new();
}