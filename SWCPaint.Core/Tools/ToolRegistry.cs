using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Interfaces.Tools;
using SWCPaint.Core.Services;

namespace SWCPaint.Core.Tools;

public class ToolRegistry : IToolRegistry
{
    private readonly Dictionary<Type, ITool> _tools = new();

    public ToolRegistry(DrawingSettings settings)
    {
        Register(new PencilTool());
    }

    private void Register(ITool tool)
    {
        _tools[tool.GetType()] = tool;
    }

    public T GetTool<T>() where T : class, ITool
    {
        var type = typeof(T);

        if (_tools.TryGetValue(type, out var tool))
        {
            return (T)tool;
        }
        
        throw new KeyNotFoundException($"Tool {type.Name} is not registered.");
    }

    public IEnumerable<ITool> GetAllTools() => _tools.Values;
}