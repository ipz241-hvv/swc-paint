using System.Windows;
using System.Windows.Controls;
using SWCPaint.Wpf.ViewModels;

namespace SWCPaint.Wpf.Views.Dialogs;

/// <summary>
/// Interaction logic for NewProjectWindow.xaml
/// </summary>
public partial class NewProjectWindow : Window
{
    public NewProjectWindow()
    {
        InitializeComponent();
    }

    private void Accept_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is NewProjectViewModel vm && vm.IsValid)
        {
            DialogResult = true;
        }
    }
}
