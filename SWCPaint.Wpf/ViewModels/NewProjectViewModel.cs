using System.ComponentModel;
using System.Windows.Input;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Wpf.Commands;
using SWCPaint.Wpf.Resources;

namespace SWCPaint.Wpf.ViewModels;

public class NewProjectViewModel : BaseViewModel, IDataErrorInfo
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

    public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            string? error = null;

            switch (columnName)
            {
                case nameof(Width):
                    if (Width < Project.MIN_DIMENSION || Width > Project.MAX_DIMENSION)
                        error = $"{Strings.Project_Width_Error} {Project.MIN_DIMENSION}-{Project.MAX_DIMENSION}";
                    break;
                case nameof(Height):
                    if (Height < Project.MIN_DIMENSION || Height > Project.MAX_DIMENSION)
                        error = $"{Strings.Project_Height_Error} {Project.MIN_DIMENSION}-{Project.MAX_DIMENSION}";
                    break;
            }

            return error ?? string.Empty;
        }
    }
    public bool IsValid => string.IsNullOrEmpty(this[nameof(Width)]) &&
                          string.IsNullOrEmpty(this[nameof(Height)]);

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