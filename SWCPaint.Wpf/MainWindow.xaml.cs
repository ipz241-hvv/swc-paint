using System.Windows;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Interfaces.Persistence;
using SWCPaint.Core.Interfaces.Serialization;
using SWCPaint.Infrastructure.Persistence;
using SWCPaint.Infrastructure.Services;
using SWCPaint.Wpf.Services;
using SWCPaint.Wpf.ViewModels;

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
        IFileManager fileManager = new PhysicalFileManager();
        IProjectSerializer projectSerializer = new JsonProjectSerializer();
        IImageExporter imageExporter = new WpfImageExporter();

        DataContext = new MainViewModel(dialogService, fileManager, projectSerializer, imageExporter);
    }

    public void Exit_Click(object sender, RoutedEventArgs e)
    {   
        Close();
    }
}