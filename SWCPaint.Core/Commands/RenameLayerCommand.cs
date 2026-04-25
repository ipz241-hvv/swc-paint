using System;
using SWCPaint.Core.Models;

namespace SWCPaint.Core.Commands;

public class RenameLayerCommand : IUndoableCommand
{
    private readonly Layer _layer;
    private readonly string _oldName;
    private readonly string _newName;
    private readonly Action _onChanged;

    
    public string Name => $"Перейменувати шар з '{_oldName}' на '{_newName}'";

    public RenameLayerCommand(Layer layer, string newName, Action onChanged)
    {
      
        _layer = layer ?? throw new ArgumentNullException(nameof(layer), "Шар не може бути null.");
      
        _newName = newName ?? string.Empty;
        _oldName = layer.Name;
        _onChanged = onChanged;
    }

    public void Execute()
    {
       
        if (_layer.Name == _newName) return;

        _layer.Name = _newName;
        _onChanged?.Invoke();
    }

    public void Undo()
    {
       
        if (_layer.Name == _oldName) return;

        _layer.Name = _oldName;
        _onChanged?.Invoke();
    }

    // Додатково для кращої налагодження
    public override string ToString() => Name;
}
