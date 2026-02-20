using SWCPaint.Core.Interfaces.Shapes;
using SWCPaint.Core.Models;

namespace SWCPaint.Core.Interfaces.Serialization;

public interface IImageExporter : IShapeVisitor
{
    byte[] Export(Project project);
}
