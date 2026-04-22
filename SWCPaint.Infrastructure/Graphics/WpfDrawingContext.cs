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

    public WpfDrawingContext(DrawingContext rootContext, double width, double height)
    {
        _currentContext = rootContext;
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
            Geometry maskGeometry = new RectangleGeometry(new Rect(0, 0, _canvasWidth, _canvasHeight));

            foreach (var eraser in erasers)
            {
                var strokeGeometry = CreateStreamGeometry(eraser.Points, false);

                var eraserPen = new Pen(Brushes.Black, eraser.Thickness)
                {
                    StartLineCap = PenLineCap.Round,
                    EndLineCap = PenLineCap.Round,
                    LineJoin = PenLineJoin.Round
                };

                var widenedEraser = strokeGeometry.GetWidenedPathGeometry(eraserPen);

                maskGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, maskGeometry, widenedEraser);
            }

            var brush = new DrawingBrush(new GeometryDrawing(Brushes.Black, null, maskGeometry))
            {
                Viewbox = new Rect(0, 0, _canvasWidth, _canvasHeight),
                ViewboxUnits = BrushMappingMode.Absolute,
                Viewport = new Rect(0, 0, _canvasWidth, _canvasHeight),
                ViewportUnits = BrushMappingMode.Absolute
            };

            layerGroup.OpacityMask = brush;
        }

        _currentContext.DrawDrawing(layerGroup);
    }

    public void DrawLine(CorePoint start, CorePoint end, CoreColor strokeColor, double thickness)
    {
        var pen = new Pen(ToWpfBrush(strokeColor), thickness);
        _currentContext.DrawLine(pen, ToWpfPoint(start), ToWpfPoint(end));
    }

    public void DrawRectangle(CorePoint topLeft, double width, double height, CoreColor strokeColor, CoreColor? fillColor, double thickness)
    {
        var pen = new Pen(ToWpfBrush(strokeColor), thickness);
        var brush = fillColor.HasValue ? ToWpfBrush(fillColor.Value) : null;

        _currentContext.DrawRectangle(brush, pen, new System.Windows.Rect(ToWpfPoint(topLeft), new System.Windows.Size(width, height)));
    }

    public void DrawEllipse(CorePoint center, double radiusX, double radiusY, CoreColor strokeColor, CoreColor? fillColor, double thickness)
    {
        var pen = new Pen(ToWpfBrush(strokeColor), thickness);
        var brush = fillColor.HasValue ? ToWpfBrush(fillColor.Value) : null;

        _currentContext.DrawEllipse(brush, pen, ToWpfPoint(center), radiusX, radiusY);
    }

    public void DrawPath(IEnumerable<CorePoint> points, CoreColor strokeColor, CoreColor? fillColor, double thickness, bool isClosed)
    {
        using var enumerator = points.GetEnumerator();

        if (!enumerator.MoveNext()) return;

        var streamGeometry = new StreamGeometry();
        using (StreamGeometryContext geometryContext = streamGeometry.Open())
        {
            geometryContext.BeginFigure(ToWpfPoint(enumerator.Current), fillColor.HasValue, isClosed);

            while (enumerator.MoveNext())
            {
                geometryContext.LineTo(ToWpfPoint(enumerator.Current), true, true);
            }
        }

        var pen = new Pen(ToWpfBrush(strokeColor), thickness);
        pen.StartLineCap = PenLineCap.Round;
        pen.EndLineCap = PenLineCap.Round;
        pen.LineJoin = PenLineJoin.Round;
        pen.Freeze();

        var brush = fillColor.HasValue ? ToWpfBrush(fillColor.Value) : null;
        _currentContext.DrawGeometry(brush, pen, streamGeometry);
    }

    public void PushMask(IEnumerable<EraserPath> erasers)
    {
        if (!erasers.Any()) return;

        var baseRect = new RectangleGeometry(new Rect(0, 0, _canvasWidth, _canvasHeight));
        var erasersGroup = new GeometryGroup { FillRule = FillRule.Nonzero };

        foreach (var eraser in erasers)
        {
            var pen = new Pen(Brushes.Black, eraser.Thickness)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round,
                LineJoin = PenLineJoin.Round
            };

            var strokeGeometry = CreateStreamGeometry(eraser.Points, false);
            erasersGroup.Children.Add(strokeGeometry.GetWidenedPathGeometry(pen));
        }

        var maskGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, baseRect, erasersGroup);

        var maskBrush = new DrawingBrush(new GeometryDrawing(Brushes.Black, null, maskGeometry))
        {
            Viewbox = new Rect(0, 0, _canvasWidth, _canvasHeight),
            ViewboxUnits = BrushMappingMode.Absolute,
            Viewport = new Rect(0, 0, _canvasWidth, _canvasHeight),
            ViewportUnits = BrushMappingMode.Absolute
        };

        _currentContext.PushOpacityMask(maskBrush);
    }

    public void PopMask() => _currentContext.Pop();

    private StreamGeometry CreateStreamGeometry(IEnumerable<CorePoint> points, bool isClosed)
    {
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            var list = points.ToList();
            if (list.Count > 0)
            {
                ctx.BeginFigure(ToWpfPoint(list[0]), true, isClosed);

                if (list.Count == 1)
                {
                    ctx.LineTo(new System.Windows.Point(list[0].X + 0.01, list[0].Y), true, true);
                }
                else
                {
                    ctx.PolyLineTo(list.Skip(1).Select(ToWpfPoint).ToList(), true, true);
                }
            }
        }
        geometry.Freeze();
        return geometry;
    }

    private System.Windows.Point ToWpfPoint(CorePoint p) => new(p.X, p.Y);

    private SolidColorBrush ToWpfBrush(CoreColor c)
    {
        var brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue));
        brush.Freeze();
        return brush;
    }
}