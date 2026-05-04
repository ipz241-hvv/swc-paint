using System.IO;
using System.Windows;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Interfaces.Persistence;
using SWCPaint.Core.Interfaces.Serialization;
using SWCPaint.Infrastructure.Persistence;
using SWCPaint.Infrastructure.Serialization;
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

        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "tools.json");

        IDialogService dialogService = new WpfDialogService();
        IFileManager fileManager = new PhysicalFileManager();
        IProjectSerializer projectSerializer = new JsonProjectSerializer();
        IImageExporter imageExporter = new WpfImageExporter();
        IToolConfigurationService toolConfigurationService = new JsonToolConfigurationService(configPath, fileManager);

        DataContext = new MainViewModel(
            toolConfigurationService, 
            dialogService, 
            fileManager, 
            projectSerializer, 
            imageExporter,
            Close
            );
    }

    public void Exit_Click(object sender, RoutedEventArgs e)
    {   
        Close();
    }
}