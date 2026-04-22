using System.Windows;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Wpf.ViewModels;

namespace SWCPaint.Wpf.Views.Dialogs;

public class WpfDialogService : IDialogService
{
    public string? ShowInputBox(string title, string message, string defaultValue = "")
    {
        string result = Microsoft.VisualBasic.Interaction.InputBox(message, title, defaultValue);
        return string.IsNullOrEmpty(result) ? null : result;
    }

    public (int w, int h, Color color)? ShowNewProjectDialog()
    {
        var vm = new NewProjectViewModel();
        var win = new NewProjectWindow { DataContext = vm, Owner = Application.Current.MainWindow };

        if (win.ShowDialog() == true)
        {
            var wpfColor = (System.Windows.Media.Color)vm.SelectedBackgroundColor.GetValue(null)!;
            var coreColor = new Color(wpfColor.R, wpfColor.G, wpfColor.B, wpfColor.A);
            return (vm.Width, vm.Height, coreColor);
        }
        return null;
    }
}