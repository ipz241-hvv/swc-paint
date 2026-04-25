using System;
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
     
        var result = Microsoft.VisualBasic.Interaction.InputBox(message, title, defaultValue);
        
        return string.IsNullOrWhiteSpace(result) ? null : result.Trim();
    }

    public (int width, int height, Color bgColor)? ShowNewProjectDialog()
    {
        var vm = new NewProjectViewModel(this);
        return ShowDialog<NewProjectWindow, (int, int, Color)>(vm, () => 
            (vm.Width, vm.Height, vm.BackgroundColor));
    }

    public string? SaveFileDialog(string filter, string defaultFileName = "New Project", string defaultExt = ".png")
    {
        var dialog = new SaveFileDialog
        {
            Filter = filter,
            FileName = defaultFileName,
            DefaultExt = defaultExt,
            Title = "Зберегти файл"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? OpenFileDialog(string filter) 
    {
        var dialog = new OpenFileDialog
        {
            Filter = filter,
            Multiselect = false,
            CheckFileExists = true,
            CheckPathExists = true,
            Title = "Відкрити файл"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public Color? ShowColorPickerDialog(Color initialColor)
    {
        var vm = new ColorPickerViewModel(initialColor);
        return ShowDialog<ColorPickerWindow, Color>(vm, () => vm.SelectedColor);
    }

    
    private TResult? ShowDialog<TWindow, TResult>(object viewModel, Func<TResult> getResult) where TWindow : Window, new()
    {
        var win = new TWindow
        {
            DataContext = viewModel,
            Owner = Application.Current?.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (win.ShowDialog() == true)
        {
            return getResult();
        }
        return default;
    }
}
