using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Interfaces.Tools;
using SWCPaint.Core.Services;

namespace SWCPaint.Core.Tools;

public class ToolRegistry : IToolRegistry
{
    private readonly Dictionary<Type, ITool> _tools = new();
    private readonly Dictionary<string, ITool> _toolsByName = new();

    public ToolRegistry(DrawingSettings settings)
    {
        Register(new PencilTool());
    }

    private void Register(ITool tool)
    {
        var type = tool.GetType();
        _tools[type] = tool;

        string name = type.Name.Replace("Tool", "");
        _toolsByName[name] = tool;
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

    public ITool GetTool(string name)
    {
        if (_toolsByName.TryGetValue(name, out var tool))
        {
            return tool;
        }
        throw new KeyNotFoundException($"Інструмент з назвою '{name}' не знайдено.");
    }

    public IEnumerable<ITool> GetAllTools() => _tools.Values;
}