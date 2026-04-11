namespace SWCPaint.Core.Models.Shapes;

public abstract class BoxBoundedShape : Shape
{
    private double _width;
    private double _height;

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
                throw new ArgumentException("Width cannot be negative");
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
                throw new ArgumentException("Height cannot be negative");
            }
            _height = value;
        }
    }
    public Point Position { get; protected set; }
    protected Point Center
    {
        get 
        {
            return new Point(Position.X + Width / 2, Position.Y + Height / 2);
        }
    }
    public override BoundingBox Bounds
    {
        get
        {
            return new BoundingBox(Position.X, Position.Y, Width, Height);
        }
    }

    public BoxBoundedShape(Point position, double width, double height)
    {
        Position = position;
        Width = width;
        Height = height;
    }

    public override void Move(double dx, double dy)
    {
        Position = new Point(Position.X + dx, Position.Y + dy);
    }
}
