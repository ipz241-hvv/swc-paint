using SWCPaint.Core.Interfaces.Tools;
using SWCPaint.Core.Models.Shapes;
using SWCPaint.Core.Services;
using SWCPaint.Core.Tools;

public class ToolRegistry
{
    private readonly Dictionary<Type, ITool> _tools = new();
    private readonly Dictionary<string, ITool> _toolsByName = new();

    public ToolRegistry(DrawingSettings settings)
    {
        Register(new PencilTool(1, 10));
        Register(new BrushTool(1, 50));
        Register(new ShapeTool<Ellipse>(1, 50));
        Register(new ShapeTool<Rectangle>(1, 50));
        Register(new EraserTool(1, 100));
        Register(new LineTool(1, 50));
    }

    private void Register(ITool tool)
    {
        var type = tool.GetType();
        _tools[type] = tool;

        string name = type.IsGenericType
            ? type.GetGenericArguments()[0].Name
            : type.Name.Replace("Tool", "");

        _toolsByName[name] = tool;
    }

    public T GetTool<T>() where T : class, ITool
    {
        var type = typeof(T);
        if (_tools.TryGetValue(type, out var tool))
        {
            return (T)tool;
        }

        throw new KeyNotFoundException($"Tool of type {type.Name} is not registered in the system.");
    }

    public ITool GetTool(string name)
    {
        if (_toolsByName.TryGetValue(name, out var tool))
        {
            return tool;
        }

        var registeredNames = string.Join(", ", _toolsByName.Keys);
        throw new KeyNotFoundException($"Інструмент '{name}' не знайдено. Доступні інструменти: {registeredNames}");
    }

    public IEnumerable<string> GetRegisteredToolNames() => _toolsByName.Keys;
}