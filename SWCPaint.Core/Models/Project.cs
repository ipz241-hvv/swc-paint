using SWCPaint.Core.Interfaces;

namespace SWCPaint.Core.Models;

public class Project
{
    private double _width;
    private double _height;
    private readonly List<Layer> _layers = [];
    public event Action? ProjectChanged;

    public double Width
    {
        get
        {
            return _width;
        }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Cannot set canvas width lesser than 1 px wide");
            }
            _width = value;
        }
    }
    public double Height
    {
        get
        {
            return _height;
        }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Cannot set canvas height lesser than 1 px wide");
            }
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

    public Project(int width, int height, string backgroundLayerName)
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

            foreach (var shape in layer.Shapes)
            {
                shape.Draw(context);
            }
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
}
