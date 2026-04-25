using System;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Commands;

public class AddElementCommand : IUndoableCommand
{
    private readonly Layer _layer;
    private readonly LayerElement _element;
    private bool _isExecuted; // Прапор для контролю стану команди

    public string Name => $"Додати {_element?.GetType().Name ?? "елемент"} на шар {_layer?.Id}";

    public AddElementCommand(Layer layer, LayerElement element)
    {
      
        _layer = layer ?? throw new ArgumentNullException(nameof(layer), "Шар не може бути порожнім.");
        _element = element ?? throw new ArgumentNullException(nameof(element), "Елемент для додавання не може бути порожнім.");
    }

    public void Execute()
    {
    
        if (_isExecuted) return;

        if (!_layer.Elements.Contains(_element))
        {
            _layer.Elements.Add(_element);
            _isExecuted = true;
        }
    }

    public void Undo()
    {
        
        if (!_isExecuted) return;

        if (_layer.Elements.Contains(_element))
        {
            _layer.Elements.Remove(_element);
            _isExecuted = false;
        }
    }

    // Додатковий метод для логування або діагностики (збільшує обсяг коду)
    public override string ToString() => $"[Command] {Name} (Виконано: {_isExecuted})";
}
