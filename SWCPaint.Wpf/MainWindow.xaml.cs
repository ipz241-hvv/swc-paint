using System.Windows;
using SWCPaint.Core.Interfaces;
using SWCPaint.Wpf.ViewModels;
using SWCPaint.Wpf.Views.Dialogs;

namespace SWCPaint.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        IDialogService dialogService = new WpfDialogService();

        DataContext = new MainViewModel(dialogService);
    }

    public void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}