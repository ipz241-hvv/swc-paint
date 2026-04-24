using System.Windows.Media;
using Color = SWCPaint.Core.Models.Color;
using ColorConverter = SWCPaint.Core.Converters.ColorConverter;

namespace SWCPaint.Wpf.ViewModels;

public class ColorPickerViewModel : BaseViewModel
{
    private double _h;
    private double _s;
    private double _v;

    public double H { get => _h; set { _h = value; OnPropertyChanged(); UpdatePreview(); } }
    public double S { get => _s; set { _s = value; OnPropertyChanged(); UpdatePreview(); } }
    public double V { get => _v; set { _v = value; OnPropertyChanged(); UpdatePreview(); } }

    public Color SelectedColor => ColorConverter.FromHsv(H, S, V);

    public SolidColorBrush PreviewBrush => new SolidColorBrush(
        System.Windows.Media.Color.FromRgb(SelectedColor.Red, SelectedColor.Green, SelectedColor.Blue));

    public ColorPickerViewModel(Color initialColor)
    {
        H = 0; S = 1; V = 1;
    }

    private void UpdatePreview() => OnPropertyChanged(nameof(PreviewBrush));
}