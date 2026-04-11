namespace SWCPaint.Core.Models;

public class Project
{
    private double _width = 800;
    private double _height = 600;

    public double Width 
    {
        get
        {
            return _width; 
        }
        set
        {
            if (_width < 0)
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
            if (_height < 0)
            {
                throw new ArgumentException("Cannot set canvas height lesser than 1 px wide");
            }
            _height = value;
        }
    }
    public Guid CurrentLayerId { get; set; }
    public Layer CurrentLayer { 
        get 
        {
            return Layers.Find(layer => layer.Id == CurrentLayerId) ?? Layers[0];
        }
    }
    public List<Layer> Layers { get; set; } = [];
}
