using System.IO;
using System.Text.Json;
using SWCPaint.Core.Dtos;
using SWCPaint.Core.Interfaces.Persistence;
using SWCPaint.Core.Interfaces.Serialization;

namespace SWCPaint.Infrastructure.Serialization;

public class JsonToolConfigurationService : IToolConfigurationService
{
    private readonly string _fullPath;
    private readonly IFileManager _fileManager;

    public JsonToolConfigurationService(string fullPath, IFileManager fileManager)
    {
        _fileManager = fileManager;

        if (string.IsNullOrWhiteSpace(fullPath))
            throw new ArgumentException("Path to config file cannot be empty", nameof(fullPath));

        _fullPath = fullPath;
    }

    public IEnumerable<ToolInfo> GetToolsMetadata()
    {
        if (!File.Exists(_fullPath))
            return Enumerable.Empty<ToolInfo>();

        try
        {
            string json = _fileManager.LoadText(_fullPath);
            return JsonSerializer.Deserialize<List<ToolInfo>>(json) ?? new List<ToolInfo>();
        }
        catch (Exception)
        {
            return Enumerable.Empty<ToolInfo>();
        }
    }
}