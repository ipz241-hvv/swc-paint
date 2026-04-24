using System.Text.Json.Serialization;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Models;

public abstract class LayerElement : Entity
{
    public abstract BoundingBox Bounds { get; }

    public abstract void Move(double dx, double dy);

    public abstract void Accept(IElementVisitor visitor);
}