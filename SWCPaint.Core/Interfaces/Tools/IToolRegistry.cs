namespace SWCPaint.Core.Interfaces.Tools;

public interface IToolRegistry
{
    T GetTool<T>() where T : class, ITool;
    IEnumerable<ITool> GetAllTools();
}
