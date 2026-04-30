using SWCPaint.Core.Dtos;

namespace SWCPaint.Core.Interfaces.Serialization;

public interface IToolConfigurationService
{
    IEnumerable<ToolInfo> GetToolsMetadata();
}