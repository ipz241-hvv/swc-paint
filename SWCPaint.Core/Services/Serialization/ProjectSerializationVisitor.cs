using SWCPaint.Core.Dtos;
using SWCPaint.Core.Dtos.Shapes;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Services.Serialization;

public class ProjectSerializationVisitor : IElementVisitor
{
    public object LastSerialized { get; private set; } = null!;

    public void Visit(Rectangle rect)
    {
        LastSerialized = new RectangleDto
        {
            Type = "Rectangle",
            X = rect.Position.X,
            Y = rect.Position.Y,
            Width = rect.Width,
            Height = rect.Height,
            StrokeColor = ColorMapper.ToDto(rect.StrokeColor),
            FillColor = rect.FillColor.HasValue ? ColorMapper.ToDto(rect.FillColor.Value) : null,
            Thickness = rect.Thickness
        };
    }

    public void Visit(Ellipse ellipse)
    {
        LastSerialized = new EllipseDto
        {
            X = ellipse.Position.X,
            Y = ellipse.Position.Y,
            Width = ellipse.Width,
            Height = ellipse.Height,
            StrokeColor = ColorMapper.ToDto(ellipse.StrokeColor),
            FillColor = ellipse.FillColor.HasValue ? ColorMapper.ToDto(ellipse.FillColor.Value) : null,
            Thickness = ellipse.Thickness
        };
    }

    public void Visit(Line line)
    {
        LastSerialized = new LineDto
        {
            Start = new PointDto { X = line.Start.X, Y = line.Start.Y },
            End = new PointDto { X = line.End.X, Y = line.End.Y },
            StrokeColor = ColorMapper.ToDto(line.StrokeColor),
            Thickness = line.Thickness
        };
    }

    public void Visit(Polyline polyline)
    {
        LastSerialized = new PolylineDto
        {
            Points = polyline.Points.Select(p => new PointDto { X = p.X, Y = p.Y }).ToList(),
            StrokeColor = ColorMapper.ToDto(polyline.StrokeColor),
            Thickness = polyline.Thickness
        };
    }

    public void Visit(EraserPath eraser)
    {
        LastSerialized = new EraserPathDto
        {
            Points = eraser.Points.Select(p => new PointDto { X = p.X, Y = p.Y }).ToList(),
            Thickness = eraser.Thickness
        };
    }
}