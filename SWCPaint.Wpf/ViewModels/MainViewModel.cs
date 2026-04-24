using System.IO;
using System.Windows.Input;
using SWCPaint.Core.Commands;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Interfaces.Persistence;
using SWCPaint.Core.Interfaces.Serialization;
using SWCPaint.Core.Interfaces.Tools;
using SWCPaint.Core.Models;
using SWCPaint.Core.Services;
using SWCPaint.Core.Tools;
using SWCPaint.Wpf.Commands;
using SWCPaint.Wpf.Models;

namespace SWCPaint.Wpf.ViewModels;

public class MainViewModel : BaseViewModel
{
    private Project _project;
    private HistoryManager _history;
    private readonly IDialogService _dialogService;
    private readonly ToolRegistry _toolRegistry;
    private readonly IProjectSerializer _projectSerializer;
    private readonly IImageExporter _imageExporter;
    private readonly IFileManager _fileManager;
    private ITool _currentTool;
    private readonly List<ToolDisplayItem> _toolInfos = new()
    {
        new() { Name = "Pencil", DisplayName = "Олівець", IconPath = "/Assets/Icons/Tools/pencil.png" },
        new() { Name = "Brush", DisplayName = "Пензель", IconPath = "/Assets/Icons/Tools/brush.png" },
        new() { Name = "Rectangle", DisplayName = "Прямокутник", IconPath = "/Assets/Icons/Tools/rectangle.png" },
        new() { Name = "Ellipse", DisplayName = "Еліпс", IconPath = "/Assets/Icons/Tools/ellipse.png" },
        new() { Name = "Eraser", DisplayName = "Гумка", IconPath = "/Assets/Icons/Tools/eraser.png" },
        new() { Name = "Line", DisplayName = "Лінія", IconPath = "/Assets/Icons/Tools/line.png" }
    };
    private string _statusText = "Готово";

    public HistoryManager History 
    { 
        get => _history; 
        private set
        {
            _history = value;
            OnPropertyChanged();
            _history.HistoryChanged += () => CommandManager.InvalidateRequerySuggested();
        }
    }
    public LayersViewModel LayersContext { get; private set; }

    public Project Project
    {
        get => _project;
        set
        {
            _project = value;
            LayersContext.CurrentProject = value;
            OnPropertyChanged();
        }
    }
    public IEnumerable<ToolDisplayItem> ToolInfos => _toolInfos;
    public ITool CurrentTool
    {
        get => _currentTool;
        set
        {
            _currentTool = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MinThickness));
            OnPropertyChanged(nameof(MaxThickness));

            if (Settings != null)
            {
                Settings.Thickness = Math.Clamp(Settings.Thickness, MinThickness, MaxThickness);
            }
        }
    }
    public string StatusText
    {
        get => _statusText;
        set { _statusText = value; OnPropertyChanged(); }
    }
    public DrawingSettings Settings => DrawingSettings.Instance;
    public double MinThickness => CurrentTool?.MinThickness ?? 1.0;
    public double MaxThickness => CurrentTool?.MaxThickness ?? 50.0;

    public ICommand SelectToolCommand { get; }
    public ICommand NewProjectCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }
    public ICommand SaveProjectCommand { get; }
    public ICommand LoadProjectCommand { get; }
    public ICommand ExportImageCommand { get; }
    public ICommand OpenColorPickerCommand { get; }

    public MainViewModel(
        IDialogService dialogService, 
        IFileManager fileManager, 
        IProjectSerializer projectSerializer,
        IImageExporter imageExporter
        )
    {
        _toolRegistry = new ToolRegistry(Settings);
        _project = new Project(800, 600, "Фон");
        _dialogService = dialogService;
        _history = new HistoryManager();
        _projectSerializer = projectSerializer;
        _imageExporter = imageExporter;
        _fileManager = fileManager;
        _history.HistoryChanged += () => CommandManager.InvalidateRequerySuggested();
        CurrentTool = _toolRegistry.GetTool<PencilTool>();
        Settings.SettingsChanged += () => OnPropertyChanged(nameof(Settings));

        LayersContext = new LayersViewModel(_project, History, _dialogService);

        SelectToolCommand = new RelayCommand(param =>
        {
            string toolName = (param as string) ?? "Pencil";

            try
            {
                CurrentTool = _toolRegistry.GetTool(toolName);
                var displayInfo = _toolInfos.FirstOrDefault(d => d.Name == toolName);

                string displayName = displayInfo?.DisplayName ?? toolName;
                StatusText = $"Інструмент: {displayName}";
            }
            catch (KeyNotFoundException ex)
            {
                StatusText = $"Помилка: {ex.Message}";
            }
        });

        _currentTool = _toolRegistry.GetTool<PencilTool>();

        ExportImageCommand = new RelayCommand(
            ExportImage,
            _ => Project != null
            );
        NewProjectCommand = new RelayCommand(_ => {
            var result = _dialogService.ShowNewProjectDialog();

            if (result != null)
            {
                var (w, h, bgColor) = result.Value;

                History = new HistoryManager();
                Project = new Project(w, h, "Фон");
                Project.BackgroundColor = bgColor;

                LayersContext = new LayersViewModel(Project, History, _dialogService);
                OnPropertyChanged(nameof(LayersContext));

                StatusText = $"Створено новий проєкт {w}x{h}";
            }
        });
        UndoCommand = new RelayCommand(
            _ => {
                History.Undo();
                Project.RequestRedraw();
                StatusText = "Дію відмінено";
            },
            _ => History.CanUndo
        );
        RedoCommand = new RelayCommand(
            _ => {
                History.Redo();
                Project.RequestRedraw();
                StatusText = "Дію повернуто";
            },
            _ => History.CanRedo
        );
        SaveProjectCommand = new RelayCommand(_ => {
            var filePath = _dialogService.SaveFileDialog("Paint Project|*.paint", defaultExt: ".paint");

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            try
            {
                string json = _projectSerializer.Serialize(Project);

                _fileManager.SaveText(filePath, json);

                StatusText = "Проєкт успішно збережено";
            }
            catch (Exception ex)
            {
                StatusText = $"Помилка збереження: {ex.Message}";
            }
        });
        LoadProjectCommand = new RelayCommand(_ =>
        {
            var filePath = _dialogService.OpenFileDialog("Paint Project|*.paint");

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            try
            {
                string json = _fileManager.LoadText(filePath);

                var loadedProject = _projectSerializer.Deserialize(json);

                History = new HistoryManager();
                Project = loadedProject;
                LayersContext = new LayersViewModel(Project, History, _dialogService);

                OnPropertyChanged(nameof(LayersContext));
                StatusText = $"Проєкт завантажено: {Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                StatusText = $"Помилка завантаження: {ex.Message}";
            }
        });
        OpenColorPickerCommand = new RelayCommand(parameter =>
        {
            string type = parameter as string ?? "Stroke";

            var currentColor = type == "Fill"
                ? (Settings.FillColor ?? new Color(0, 0, 0, 0))
                : Settings.StrokeColor;

            var newColor = _dialogService.ShowColorPickerDialog(currentColor);

            if (newColor != null)
            {
                if (type == "Fill")
                {
                    Settings.FillColor = newColor.Value;
                }
                else
                {
                    Settings.StrokeColor = newColor.Value;
                }

                OnPropertyChanged(nameof(Settings));
            }
        });

        History.HistoryChanged += () => {
            CommandManager.InvalidateRequerySuggested();
        };
    }

    private void ExportImage(object? parameter)
    {
        var filePath = _dialogService.SaveFileDialog("PNG Image|*.png", "Unnamed.png");
        if (string.IsNullOrEmpty(filePath)) return;

        try
        {
            byte[] imageData = _imageExporter.Export(Project);

            _fileManager.Save(filePath, imageData);

            StatusText = "Експорт завершено успішно";
        }
        catch (Exception ex)
        {
            StatusText = $"Помилка експорту: {ex.Message}";
        }
    }
}