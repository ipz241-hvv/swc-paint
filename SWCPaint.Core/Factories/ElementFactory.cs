using System.Text.Json;
using SWCPaint.Core.Dtos;
using SWCPaint.Core.Dtos.Shapes;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Factories;

public class ElementFactory
{
    private readonly JsonSerializerOptions _options;

    public ElementFactory(JsonSerializerOptions options)
    {
        _options = options;
    }

    public LayerElement? CreateElement(JsonElement json)
    {
        if (!json.TryGetProperty("Type", out var typeProp))
            return null;

        string type = typeProp.GetString() ?? string.Empty;

        return type switch
        {
            "Rectangle" => CreateRectangle(json),
            "Ellipse" => CreateEllipse(json),
            "Line" => CreateLine(json),
            "Polyline" => CreatePolyline(json),
            "Eraser" => CreateEraser(json),
            _ => null
        };
    }

    private Rectangle CreateRectangle(JsonElement json)
    {
        var dto = json.Deserialize<RectangleDto>(_options)!;
        return new Rectangle(new Point(dto.X, dto.Y), dto.Width, dto.Height)
        {
            StrokeColor = FromColorDto(dto.StrokeColor),
            FillColor = dto.FillColor != null ? FromColorDto(dto.FillColor) : null,
            Thickness = dto.Thickness
        };
    }

    private Ellipse CreateEllipse(JsonElement json)
    {
        var dto = json.Deserialize<EllipseDto>(_options)!;
        return new Ellipse(new Point(dto.X, dto.Y), dto.Width, dto.Height)
        {
            StrokeColor = FromColorDto(dto.StrokeColor),
            FillColor = dto.FillColor != null ? FromColorDto(dto.FillColor) : null,
            Thickness = dto.Thickness
        };
    }

    private Line CreateLine(JsonElement json)
    {
        var dto = json.Deserialize<LineDto>(_options)!;
        return new Line(new Point(dto.Start.X, dto.Start.Y), new Point(dto.End.X, dto.End.Y))
        {
            StrokeColor = FromColorDto(dto.StrokeColor),
            Thickness = dto.Thickness
        };
    }

    private Polyline CreatePolyline(JsonElement json)
    {
        var dto = json.Deserialize<PolylineDto>(_options)!;
        var points = dto.Points.Select(p => new Point(p.X, p.Y)).ToList();
        return new Polyline(points)
        {
            StrokeColor = FromColorDto(dto.StrokeColor),
            Thickness = dto.Thickness,
            IsSmooth = dto.IsSmooth
        };
    }

    private EraserPath CreateEraser(JsonElement json)
    {
        var dto = json.Deserialize<EraserPathDto>(_options)!;
        var eraser = new EraserPath(dto.Thickness);

        foreach (var p in dto.Points)
        {
            eraser.AddPoint(new Point(p.X, p.Y));
        }

        return eraser;
    }

    private Color FromColorDto(ColorDto dto) =>
        new Color(dto.Red, dto.Green, dto.Blue, dto.Alpha);
}