using SWCPaint.Core.Models;

namespace SWCPaint.Core.Interfaces.Serialization;

public interface IImageExporter
{
    byte[] Export(Project project);
}
