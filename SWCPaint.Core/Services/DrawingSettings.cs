using SWCPaint.Core.Models;

namespace SWCPaint.Core.Services;

public sealed class DrawingSettings
{
    private static readonly Lazy<DrawingSettings> _instance =
        new(() => new DrawingSettings());
    private Color _selectedColor = new Color();

    public static DrawingSettings Instance => _instance.Value;
    public Color SelectedColor {
        get 
        {
            return _selectedColor;
        } 
        set
        {
            _selectedColor = value;
            SettingsChanged?.Invoke();
        } 
    } 
    public Color? FillColor { get; set; } = null;
    public double Thickness { get; set; } = 2.0;

    public event Action? SettingsChanged;

    private DrawingSettings() { }
}
