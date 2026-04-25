using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using SWCPaint.Core.Dtos;
using SWCPaint.Core.Dtos.Shapes;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Factories;

public class ElementFactory
{
    private readonly JsonSerializerOptions _options;
    
    private readonly Dictionary<string, Func<JsonElement, LayerElement>> _creationStrategies;

    public ElementFactory(JsonSerializerOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        
        _creationStrategies = new Dictionary<string, Func<JsonElement, LayerElement>>
        {
            { "Rectangle", CreateRectangle },
            { "Ellipse", CreateEllipse },
            { "Line", CreateLine },
            { "Polyline", CreatePolyline },
            { "Eraser", CreateEraser }
        };
    }

    public LayerElement? CreateElement(JsonElement json)
    {
        if (!json.TryGetProperty("Type", out var typeProp))
            return null;

        string type = typeProp.GetString() ?? string.Empty;

        if (_creationStrategies.TryGetValue(type, out var strategy))
        {
            try 
            {
                return strategy(json);
            }
            catch (JsonException)
            {
               
                return null;
            }
        }

        return null;
    }

    private Rectangle CreateRectangle(JsonElement json)
    {
        var dto = DeserializeDto<RectangleDto>(json);
        var rect = new Rectangle(new Point(dto.X, dto.Y), dto.Width, dto.Height);
        ApplyCommonProperties(rect, dto);
        return rect;
    }

    private Ellipse CreateEllipse(JsonElement json)
    {
        var dto = DeserializeDto<EllipseDto>(json);
        var ellipse = new Ellipse(new Point(dto.X, dto.Y), dto.Width, dto.Height);
        ApplyCommonProperties(ellipse, dto);
        return ellipse;
    }

    private Line CreateLine(JsonElement json)
    {
        var dto = DeserializeDto<LineDto>(json);
        var line = new Line(new Point(dto.Start.X, dto.Start.Y), new Point(dto.End.X, dto.End.Y));
        line.StrokeColor = FromColorDto(dto.StrokeColor);
        line.Thickness = dto.Thickness;
        return line;
    }

    private Polyline CreatePolyline(JsonElement json)
    {
        var dto = DeserializeDto<PolylineDto>(json);
        var points = dto.Points.Select(p => new Point(p.X, p.Y)).ToList();
        var polyline = new Polyline(points);
        polyline.StrokeColor = FromColorDto(dto.StrokeColor);
        polyline.Thickness = dto.Thickness;
        polyline.IsSmooth = dto.IsSmooth;
        return polyline;
    }

    private EraserPath CreateEraser(JsonElement json)
    {
        var dto = DeserializeDto<EraserPathDto>(json);
        var eraser = new EraserPath(dto.Thickness);
        foreach (var p in dto.Points) eraser.AddPoint(new Point(p.X, p.Y));
        return eraser;
    }


    private T DeserializeDto<T>(JsonElement json) => 
        json.Deserialize<T>(_options) ?? throw new JsonException($"Failed to deserialize {typeof(T).Name}");

    private void ApplyCommonProperties(BoxBoundedShape shape, BoxBoundedShapeDto dto)
    {
        shape.StrokeColor = FromColorDto(dto.StrokeColor);
        shape.Thickness = dto.Thickness;

        if (shape is IFillable fillable && dto is IFillableDto fillableDto)
        {
            fillable.FillColor = fillableDto.FillColor != null ? FromColorDto(fillableDto.FillColor) : null;
        }
    }

    private Color FromColorDto(ColorDto dto) =>
        new Color(dto.Red, dto.Green, dto.Blue, dto.Alpha);
}
