namespace SWCPaint.Core.Models;

public class Color
{
    byte _alpha;

    public byte Red { get; set; }
    public byte Green { get; set; }
    public byte Blue { get; set; }
    public byte Alpha
    { 
        get
        {
            return _alpha;
        }
        set 
        { 
            if (value > 1)
            {
                throw new ArgumentException("Alpha cannot be higher than 1");
            }
        }
    }

    public Color(byte red = 0, byte green = 0, byte blue = 0, byte alpha = 1)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }
}
