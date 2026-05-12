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

namespace SWCPaint.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "tools.json");

            IDialogService dialogService = new WpfDialogService();
            IFileManager fileManager = new PhysicalFileManager();
            IProjectSerializer projectSerializer = new JsonProjectSerializer();
            IImageExporter imageExporter = new WpfImageExporter();
            IToolConfigurationService toolConfigurationService = new JsonToolConfigurationService(configPath, fileManager);

            var viewModel = new MainViewModel(
                toolConfigurationService,
                dialogService,
                fileManager,
                projectSerializer,
                imageExporter,
            );

            MainWindow mainWindow = new MainWindow(viewModel); 

            mainWindow.Show();
        }
    }
}
