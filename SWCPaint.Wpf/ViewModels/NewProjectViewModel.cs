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
        set => SetField(ref _width, value);
    }
    public int Height
    {
        get => _height;
        set => SetField(ref _height, value);
    }
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set => SetField(ref _backgroundColor, value);
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
                    error = ValidateDimension(
                        Width,
                        Strings.Project_Width_Error
                    );
                    break;

                case nameof(Height):
                    error = ValidateDimension(
                        Height,
                        Strings.Project_Height_Error
                    );
                    break;
            }

            return error ?? string.Empty;
        }
    }
    public bool IsValid => string.IsNullOrEmpty(this[nameof(Width)]) &&
                          string.IsNullOrEmpty(this[nameof(Height)]);

    private string? ValidateDimension(int value, string errorMessage)
    {
        if (value < Project.MIN_DIMENSION || value > Project.MAX_DIMENSION)
        {
            return $"{errorMessage} {Project.MIN_DIMENSION}-{Project.MAX_DIMENSION}";
        }

        return null;
    }

    private void SetField<T>(ref T field, T value)
    {
        if (!Equals(field, value))
        {
            field = value;
            OnPropertyChanged();
        }
    }

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