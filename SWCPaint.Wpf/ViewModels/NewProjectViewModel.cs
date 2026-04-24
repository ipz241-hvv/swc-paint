using System.Windows.Input;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Wpf.Commands;

namespace SWCPaint.Wpf.ViewModels;

public class NewProjectViewModel : BaseViewModel
{
    private int _width = 800;
    private int _height = 600;
    private Color _backgroundColor = new Color(255, 255, 255);
    private readonly IDialogService _dialogService;

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
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set { _backgroundColor = value; OnPropertyChanged(); }
    }
    public ICommand ChangeColorCommand { get; }

    public NewProjectViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;

        ChangeColorCommand = new RelayCommand(_ =>
        {
            var newColor = _dialogService.ShowColorPickerDialog(BackgroundColor);
            if (newColor != null)
            {
                BackgroundColor = newColor.Value;
            }
        });
    }
}