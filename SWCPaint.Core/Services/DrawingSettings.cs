using SWCPaint.Core.Models;

namespace SWCPaint.Core.Services;

public sealed class DrawingSettings
{
    private static readonly Lazy<DrawingSettings> _instance =
        new(() => new DrawingSettings());
    private Color _strokeColor = Color.Black;
    private Color? _fillColor = Color.Black;

    public static DrawingSettings Instance => _instance.Value;
    public Color StrokeColor {
        get 
        {
            return _strokeColor;
        } 
        set
        {
            _strokeColor = value;
            SettingsChanged?.Invoke();
        } 
    }
    public Color? FillColor
    {
        get => _fillColor;
        set
        {
            _fillColor = value;
            SettingsChanged?.Invoke();
        }
    }
    public double Thickness { get; set; } = 2.0;

    public event Action? SettingsChanged;

    private DrawingSettings() { }
}