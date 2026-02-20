using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Models;

public class Layer : Entity
{
    public string Name { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public List<Shape> Shapes { get; set; } = new();
}
