using SWCPaint.Core.Models;

namespace SWCPaint.Core.Interfaces.Tools;

public interface IToolRegistry
{
    T GetTool<T>() where T : class, ITool;
    ITool GetTool(string name);
    IEnumerable<ITool> GetAllTools();
}
