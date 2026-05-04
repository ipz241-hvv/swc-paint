using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using CoreColor = SWCPaint.Core.Models.Color;
using CorePoint = SWCPaint.Core.Models.Point;

namespace SWCPaint.Infrastructure.Graphics;

public class WpfDrawingContext : IDrawingContext
{
    private DrawingContext _currentContext;
    private readonly double _canvasWidth;
    private readonly double _canvasHeight;
    private readonly Stack<DrawingGroup> _groupStack = new();
    private readonly Stack<DrawingContext> _contextStack = new();
    
    private readonly Dictionary<string, Brush> _brushCache = new();
    private readonly Dictionary<string, Pen> _penCache = new();

    public WpfDrawingContext(DrawingContext rootContext, double width, double height)
    {
        _currentContext = rootContext ?? throw new ArgumentNullException(nameof(rootContext));
        _canvasWidth = width;
        _canvasHeight = height;
    }

    public void BeginLayer()
    {
        var group = new DrawingGroup();
        _groupStack.Push(group);
        _contextStack.Push(_currentContext);
        _currentContext = group.Open();
    }

    public void EndLayer(IEnumerable<EraserPath> erasers)
    {
        _currentContext.Close();
        var layerGroup = _groupStack.Pop();
        _currentContext = _contextStack.Pop();

        if (erasers.Any())
        {
            ApplyEraserMask(layerGroup, erasers);
        }

        _currentContext.DrawDrawing(layerGroup);
    }

    private void ApplyEraserMask(DrawingGroup group, IEnumerable<EraserPath> erasers)
    {
        var maskGeometry = new GeometryGroup();
        maskGeometry.Children.Add(new RectangleGeometry(new Rect(0, 0, _canvasWidth, _canvasHeight)));

        var eraserCombined = new GeometryGroup();
        foreach (var eraser in erasers)
        {
            var pen = GetPen(new CoreColor(0, 0, 0, 255), eraser.Thickness, true);
            var path = CreateStreamGeometry(eraser.Points, false);
            eraserCombined.Children.Add(path.GetWidenedPathGeometry(pen));
        }

        var finalMask = new CombinedGeometry(GeometryCombineMode.Exclude, maskGeometry.Children[0], eraserCombined);
        
        var brush = new DrawingBrush(new GeometryDrawing(Brushes.Black, null, finalMask))
        {
            Viewbox = new Rect(0, 0, _canvasWidth, _canvasHeight),
            ViewboxUnits = BrushMappingMode.Absolute,
            Viewport = new Rect(0, 0, _canvasWidth, _canvasHeight),
            ViewportUnits = BrushMappingMode.Absolute
        };
        brush.Freeze();
        group.OpacityMask = brush;
    }

    public void DrawLine(CorePoint start, CorePoint end, CoreColor strokeColor, double thickness)
    {
        var pen = GetPen(strokeColor, thickness);
        _currentContext.DrawLine(pen, ToWpfPoint(start), ToWpfPoint(end));
    }

    public void DrawRectangle(CorePoint topLeft, double width, double height, CoreColor strokeColor, CoreColor? fillColor, double thickness)
    {
        var pen = GetPen(strokeColor, thickness);
        var brush = fillColor.HasValue ? GetBrush(fillColor.Value) : null;
        _currentContext.DrawRectangle(brush, pen, new Rect(ToWpfPoint(topLeft), new Size(width, height)));
    }

    public void DrawEllipse(CorePoint center, double radiusX, double radiusY, CoreColor strokeColor, CoreColor? fillColor, double thickness)
    {
        var pen = GetPen(strokeColor, thickness);
        var brush = fillColor.HasValue ? GetBrush(fillColor.Value) : null;
        _currentContext.DrawEllipse(brush, pen, ToWpfPoint(center), radiusX, radiusY);
    }

    public void DrawPath(IEnumerable<CorePoint> points, CoreColor strokeColor, CoreColor? fillColor, double thickness, bool isClosed, bool isSmooth)
    {
        var geometry = CreateStreamGeometry(points, isClosed);
        var pen = GetPen(strokeColor, thickness, isSmooth);
        var brush = fillColor.HasValue ? GetBrush(fillColor.Value) : null;
        
        _currentContext.DrawGeometry(brush, pen, geometry);
    }

    private Brush GetBrush(CoreColor color)
    {
        string key = $"{color.Red}-{color.Green}-{color.Blue}-{color.Alpha}";
        if (!_brushCache.TryGetValue(key, out var brush))
        {
            brush = new SolidColorBrush(Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue));
            brush.Freeze();
            _brushCache[key] = brush;
        }
        return brush;
    }

    private Pen GetPen(CoreColor color, double thickness, bool isSmooth = false)
    {
        string key = $"{color.Red}-{color.Green}-{color.Blue}-{color.Alpha}-{thickness}-{isSmooth}";
        if (!_penCache.TryGetValue(key, out var pen))
        {
            pen = new Pen(GetBrush(color), thickness);
            if (isSmooth)
            {
                pen.StartLineCap = pen.EndLineCap = PenLineCap.Round;
                pen.LineJoin = PenLineJoin.Round;
            }
            pen.Freeze();
            _penCache[key] = pen;
        }
        return pen;
    }

    private StreamGeometry CreateStreamGeometry(IEnumerable<CorePoint> points, bool isClosed)
    {
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            var list = points.ToList();
            if (list.Count > 0)
            {
                ctx.BeginFigure(ToWpfPoint(list[0]), true, isClosed);
                if (list.Count > 1)
                    ctx.PolyLineTo(list.Skip(1).Select(ToWpfPoint).ToList(), true, true);
                else
                    ctx.LineTo(new System.Windows.Point(list[0].X + 0.05, list[0].Y), true, true);
            }
        }
        geometry.Freeze();
        return geometry;
    }

    private System.Windows.Point ToWpfPoint(CorePoint p) => new(p.X, p.Y);
}
