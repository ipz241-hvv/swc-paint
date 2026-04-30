using System.Diagnostics.CodeAnalysis;
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
using SWCPaint.Wpf.Resources;

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
    private readonly List<ToolDisplayItem> _toolInfos = new();
    private string _statusText;

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
                OnPropertyChanged(nameof(Settings));
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

    public Action? CloseAction { get; private set; }
    public ICommand SelectToolCommand { get; private set; }
    public ICommand NewProjectCommand { get; private set; }
    public ICommand UndoCommand { get; private set; }
    public ICommand RedoCommand { get; private set; }
    public ICommand SaveProjectCommand { get; private set; }
    public ICommand LoadProjectCommand { get; private set; }
    public ICommand ExportImageCommand { get; private set; }
    public ICommand OpenColorPickerCommand { get; private set; }
    public ICommand ExitCommand { get; private set; }

    public MainViewModel(
        IToolConfigurationService toolConfigService,
        IDialogService dialogService,
        IFileManager fileManager,
        IProjectSerializer projectSerializer,
        IImageExporter imageExporter,
        Action closeAction
    )
    {
        _dialogService = dialogService;
        _fileManager = fileManager;
        _projectSerializer = projectSerializer;
        _imageExporter = imageExporter;
        CloseAction = closeAction;

        _toolRegistry = new ToolRegistry(Settings);
        _history = new HistoryManager();
        _project = new Project(800, 600, Strings.Layer_NewProject_Background);
        LayersContext = new LayersViewModel(Project, History, _dialogService);

        InitializeTools(toolConfigService);
        InitializeCommands();
        InitializeSubscribers();

        _statusText = Strings.Paint_Status_Ready;
    }

    private void InitializeSubscribers()
    {
        Settings.SettingsChanged += () => OnPropertyChanged(nameof(Settings));
        History.HistoryChanged += () => CommandManager.InvalidateRequerySuggested();
    }

    [MemberNotNull(nameof(_currentTool))]
    private void InitializeTools(IToolConfigurationService toolConfigService)
    {
        var metadata = toolConfigService.GetToolsMetadata();

        foreach (var m in metadata)
        {
            _toolInfos.Add(new ToolDisplayItem
            {
                Name = m.Name,
                LocalizationKey = m.LocalizationKey,
                IconPath = $"/Assets/Icons/Tools/{m.Name.ToLower()}.png",
                DisplayName = Strings.ResourceManager.GetString(m.LocalizationKey) ?? m.Name
            });
        }
        _currentTool = _toolRegistry.GetTool<PencilTool>();
    }

    [MemberNotNull(nameof(SelectToolCommand), nameof(ExitCommand), nameof(ExportImageCommand),
               nameof(NewProjectCommand), nameof(UndoCommand), nameof(RedoCommand),
               nameof(SaveProjectCommand), nameof(LoadProjectCommand), nameof(OpenColorPickerCommand))]
    private void InitializeCommands()
    {
        SelectToolCommand = new RelayCommand(param =>
        {
            string toolName = (param as string) ?? "Pencil";

            try
            {
                CurrentTool = _toolRegistry.GetTool(toolName);
                var displayInfo = _toolInfos.FirstOrDefault(d => d.Name == toolName);

                string displayName = displayInfo?.DisplayName ?? toolName;
                StatusText = $"{Strings.Tool_Select_Status}: {displayName}";
            }
            catch (Exception)
            {
                StatusText = Strings.Tool_SelectFailed_Status;
            }
        });

        ExitCommand = new RelayCommand(_ =>
        {
            CloseAction?.Invoke();
        });

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
                Project = new Project(w, h, Strings.Layer_NewProject_Background);
                Project.BackgroundColor = bgColor;

                LayersContext = new LayersViewModel(Project, History, _dialogService);
                OnPropertyChanged(nameof(LayersContext));

                StatusText = $"{Strings.Project_New_Status} {w}x{h}";
            }
        });

        UndoCommand = new RelayCommand(
            _ => {
                History.Undo();
                Project.RequestRedraw();
                StatusText = Strings.Paint_Undo_Status;
            },
            _ => History.CanUndo
        );

        RedoCommand = new RelayCommand(
            _ => {
                History.Redo();
                Project.RequestRedraw();
                StatusText = Strings.Paint_Redo_Status;
            },
            _ => History.CanRedo
        );

        SaveProjectCommand = new RelayCommand(_ => {
            var filter = $"{Strings.Project_Open_FileType}|*.paint";
            var filePath = _dialogService.SaveFileDialog(filter, defaultExt: ".paint");

            if (string.IsNullOrWhiteSpace(filePath)) return;

            try
            {
                string json = _projectSerializer.Serialize(Project);
                _fileManager.SaveText(filePath, json);
                StatusText = Strings.Project_Save_Status;
            }
            catch (Exception)
            {
                StatusText = Strings.Project_SaveFailed_Status;
            }
        });

        LoadProjectCommand = new RelayCommand(_ =>
        {
            var filter = $"{Strings.Project_Open_FileType}|*.paint";
            var filePath = _dialogService.OpenFileDialog(filter);

            if (string.IsNullOrWhiteSpace(filePath)) return;

            try
            {
                string json = _fileManager.LoadText(filePath);
                var loadedProject = _projectSerializer.Deserialize(json);

                History = new HistoryManager();
                Project = loadedProject;
                LayersContext = new LayersViewModel(Project, History, _dialogService);

                OnPropertyChanged(nameof(LayersContext));
                StatusText = $"{Strings.Project_Load_Status}: {Path.GetFileName(filePath)}";
            }
            catch (Exception)
            {
                StatusText = Strings.Project_LoadFailed_Status;
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
    }

    private void ExportImage(object? parameter)
    {
        var filePath = _dialogService.SaveFileDialog($"{Strings.Project_ExportAsImage_FileType}|*.png", 
            $"{Strings.Project_ExportAsImage_FileName}.png");
        if (string.IsNullOrEmpty(filePath)) return;

        try
        {
            byte[] imageData = _imageExporter.Export(Project);

            _fileManager.Save(filePath, imageData);

            StatusText = Strings.Project_ExportAsImage_Status;
        }
        catch (Exception ex)
        {
            StatusText = $"{Strings.Project_ExportAsImageFailed_Status}: {ex.Message}";
        }
    }
}