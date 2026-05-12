using SWCPaint.Core.Dtos;
using SWCPaint.Core.Dtos.Shapes;
using SWCPaint.Core.Factories.ElementFactories;
using SWCPaint.Core.Interfaces.Shapes;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SWCPaint.Core.Factories;

public class ElementFactory
{
    private readonly Dictionary<string, IElementFactory> _factories;

    public ElementFactory(JsonSerializerOptions options)
    {
        _factories = new Dictionary<string, IElementFactory>
        {
            { "Rectangle", new RectangleFactory(options) },
            { "Ellipse",   new EllipseFactory(options)   },
            { "Line",      new LineFactory(options)      },
            { "Polyline",  new PolylineFactory(options)  },
            { "Eraser",    new EraserFactory(options)    }
        };
    }

    public LayerElement? CreateElement(JsonElement json)
    {
        if (!json.TryGetProperty("Type", out var typeProp))
            return null;

        string type = typeProp.GetString() ?? string.Empty;

        if (_factories.TryGetValue(type, out var factory))
        {
            try { return factory.Create(json); }
            catch (JsonException) { return null; }
        }

        return null;
    }
}