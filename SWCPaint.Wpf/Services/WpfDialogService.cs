using System.Windows;
using Microsoft.Win32;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Wpf.ViewModels;
using SWCPaint.Wpf.Views.Dialogs;

namespace SWCPaint.Wpf.Services;

public class WpfDialogService : IDialogService
{
    public string? ShowInputBox(string title, string message, string defaultValue = "")
    {
        string result = Microsoft.VisualBasic.Interaction.InputBox(message, title, defaultValue);
        return string.IsNullOrEmpty(result) ? null : result;
    }

    public (int width, int height, Color bgColor)? ShowNewProjectDialog()
    {
        var vm = new NewProjectViewModel(this);
        var win = new NewProjectWindow { DataContext = vm, Owner = Application.Current.MainWindow };

        if (win.ShowDialog() == true)
        {
            return (vm.Width, vm.Height, vm.BackgroundColor);
        }
        return null;
    }

    public string? SaveFileDialog(string filter, string defaultFileName = "New Project", string defaultExt = ".png")
    {
        var dialog = new SaveFileDialog
        {
            Filter = filter,
            FileName = defaultFileName,
            DefaultExt = defaultExt
        };

        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }

        return null;
    }

    public string OpenFileDialog(string filter)
    {
        var dialog = new OpenFileDialog
        {
            Filter = filter,
            Multiselect = false,
            CheckFileExists = true,
            CheckPathExists = true
        };

        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }

        return null!;
    }

    public Color? ShowColorPickerDialog(Color initialColor)
    {
        var vm = new ColorPickerViewModel(initialColor);
        var win = new ColorPickerWindow
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };

        if (win.ShowDialog() == true)
        {
            return vm.SelectedColor;
        }

        return null;
    }
}