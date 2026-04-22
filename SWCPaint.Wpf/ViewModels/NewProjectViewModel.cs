namespace SWCPaint.Wpf.ViewModels;

public class NewProjectViewModel : BaseViewModel
{
    private int _width = 800;
    private int _height = 600;
    private System.Reflection.PropertyInfo _selectedBackgroundColor;

    public int Width
    {
        get => _width;
        set { _width = value; OnPropertyChanged(); }
    }

    public int Height
    {
        get => _height;
        set { _height = value; OnPropertyChanged(); }
    }

    public IEnumerable<System.Reflection.PropertyInfo> AvailableColors =>
        typeof(System.Windows.Media.Colors).GetProperties();

    public System.Reflection.PropertyInfo SelectedBackgroundColor
    {
        get => _selectedBackgroundColor;
        set { _selectedBackgroundColor = value; OnPropertyChanged(); }
    }

    public NewProjectViewModel()
    {
        _selectedBackgroundColor = AvailableColors.First(p => p.Name == "White");
    }
}