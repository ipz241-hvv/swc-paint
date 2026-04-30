using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models.Shapes;

namespace SWCPaint.Core.Models;

public class Project
{
    public const double MIN_DIMENSION = 1.0;
    public const double MAX_DIMENSION = 10000.0;
    private double _width;
    private double _height;
    private readonly List<Layer> _layers = [];
    public event Action? ProjectChanged;

    public double Width
    {
        get => _width;
        set
        {
            if (value < MIN_DIMENSION || value > MAX_DIMENSION)
                throw new ArgumentOutOfRangeException(nameof(Width), $"Width must be between {MIN_DIMENSION} and {MAX_DIMENSION} px.");
            _width = value;
        }
    }
    public double Height
    {
        get => _height;
        set
        {
            if (value < MIN_DIMENSION || value > MAX_DIMENSION)
                throw new ArgumentOutOfRangeException(nameof(Height), $"Height must be between {MIN_DIMENSION} and {MAX_DIMENSION} px.");
            _height = value;
        }
    }
    public Color BackgroundColor { get; set; } = Color.White;
    public Guid CurrentLayerId { get; set; }
    public Layer CurrentLayer
    {
        get
        {
            if (_layers.Count == 0) return null!;

            return _layers.Find(l => l.Id == CurrentLayerId) ?? Layers[0];
        }
    }
    public IReadOnlyList<Layer> Layers => _layers.AsReadOnly();

    public Project(double width, double height, string backgroundLayerName)
    {
        Width = width;
        Height = height;

        var defaultLayer = new Layer(backgroundLayerName);
        AddLayer(defaultLayer);
        CurrentLayerId = defaultLayer.Id;
    }

    public void Render(IDrawingContext context)
    {
        context.DrawRectangle(
            new Point(0, 0),
            Width,
            Height,
            BackgroundColor,
            BackgroundColor,
            0
        );

        foreach (var layer in Layers)
        {
            if (!layer.IsVisible) continue;

            context.BeginLayer();

            var layerErasers = layer.Elements.OfType<EraserPath>().ToList();

            foreach (var element in layer.Elements)
            {
                if (element is Shape shape)
                {
                    var relevantErasers = layerErasers.Where(e => layer.Elements.IndexOf(e) > layer.Elements.IndexOf(element)).ToList();

                    if (relevantErasers.Any())
                    {
                        context.PushMask(relevantErasers);
                        shape.Draw(context);
                        context.PopMask();
                    }
                    else
                    {
                        shape.Draw(context);
                    }
                }
            }

            context.EndLayer(Enumerable.Empty<EraserPath>());
        }
    }

    public void RequestRedraw()
    {
        ProjectChanged?.Invoke();
    }

    public void AddLayer(Layer layer)
    {
        _layers.Add(layer);
        RequestRedraw();
    }

    public void RemoveLayer(Guid id)
    {
        if (_layers.Count <= 1) return;

        var layer = _layers.Find(l => l.Id == id);

        if (layer != null)
        {
            _layers.Remove(layer);

            if (CurrentLayerId == id)
            {
                CurrentLayerId = _layers[0].Id;
            }
        }

        RequestRedraw();
    }

    public void InsertLayer(int index, Layer layer)
    {
        _layers.Insert(index, layer);
    }

    public void MoveLayer(Guid id, int toIndex)
    {
        if (toIndex < 0 || toIndex >= _layers.Count) return;

        var layer = _layers.Find(l => l.Id == id);

        if (layer == null) return;

        _layers.Remove(layer);
        _layers.Insert(toIndex, layer);

        RequestRedraw();
    }

    public void ClearLayers()
    {
        _layers.Clear();
    }
}
