using SWCPaint.Core.Models;

namespace SWCPaint.Core.Interfaces.Serialization;

public interface IImageExporter : IElementVisitor
{
    byte[] Export(Project project);
}
