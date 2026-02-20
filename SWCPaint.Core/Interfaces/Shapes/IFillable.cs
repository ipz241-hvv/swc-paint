using SWCPaint.Core.Models;

namespace SWCPaint.Core.Interfaces.Shapes;

public interface IFillable
{
    public Color? FillColor { get; set; }
}
