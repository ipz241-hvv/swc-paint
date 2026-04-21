using System.Reflection;
using System.Windows.Input;
using SWCPaint.Core.Commands;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Core.Services;
using SWCPaint.Core.Tools;
using SWCPaint.Wpf.Commands;
using SWCPaint.Wpf.Models;

namespace SWCPaint.Wpf.ViewModels;

public class MainViewModel : BaseViewModel
{
    private Project _project;
    private readonly ToolRegistry _toolRegistry;
    private ITool _currentTool;
    private readonly List<ToolDisplayItem> _toolInfos = new()
    {
        new() { Name = "Pencil", DisplayName = "Олівець", IconPath = "/Assets/Icons/Tools/pencil.png" },
        new() { Name = "Rectangle", DisplayName = "Прямокутник", IconPath = "/Assets/Icons/Tools/rectangle.png" },
        new() { Name = "Ellipse", DisplayName = "Еліпс", IconPath = "/Assets/Icons/Tools/ellipse.png" }
    };
    private string _statusText = "Готово";

    public HistoryManager History { get; } = new HistoryManager();
    public LayersViewModel LayersContext { get; }

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
    public IEnumerable<PropertyInfo> AvailableColors 
    {
        get
        {
            return typeof(System.Windows.Media.Colors).GetProperties();
        }
    }
    public IEnumerable<ToolDisplayItem> ToolInfos => _toolInfos;
    public PropertyInfo SelectedStrokeColor
    {
        get
        {
            var coreColor = Settings.StrokeColor;
            return AvailableColors.FirstOrDefault(p =>
            {
                var wpfColor = (System.Windows.Media.Color)p.GetValue(null)!;
                return wpfColor.R == coreColor.Red && wpfColor.G == coreColor.Green && wpfColor.B == coreColor.Blue;
            }) ?? AvailableColors.First(p => p.Name == "Black");
        }
        set
        {
            if (value != null)
            {
                var wpfColor = (System.Windows.Media.Color)value.GetValue(null)!;
                Settings.StrokeColor = new SWCPaint.Core.Models.Color(wpfColor.R, wpfColor.G, wpfColor.B, wpfColor.A);
                OnPropertyChanged();
            }
        }
    }
    public PropertyInfo SelectedFillColor
    {
        get
        {
            var coreColor = Settings.FillColor;

            if (coreColor == null)
                return AvailableColors.First(p => p.Name == "Transparent");

            var colorValue = coreColor.Value;

            return AvailableColors.FirstOrDefault(p =>
            {
                var wpfColor = (System.Windows.Media.Color)p.GetValue(null)!;
                return wpfColor.R == colorValue.Red 
                    && wpfColor.G == colorValue.Green 
                    && wpfColor.B == colorValue.Blue 
                    && wpfColor.A == colorValue.Alpha;
            }) ?? AvailableColors.First(p => p.Name == "Transparent");
        }
        set
        {
            if (value == null || value.Name == "Transparent")
            {
                Settings.FillColor = null;
            }
            else
            {
                var wpfColor = (System.Windows.Media.Color)value.GetValue(null)!;
                Settings.FillColor = new SWCPaint.Core.Models.Color(wpfColor.R, wpfColor.G, wpfColor.B, wpfColor.A);
            }
            OnPropertyChanged();
        }
    }
    public ITool CurrentTool
    {
        get => _currentTool;
        set { _currentTool = value; OnPropertyChanged(); }
    }
    public string StatusText
    {
        get => _statusText;
        set { _statusText = value; OnPropertyChanged(); }
    }
    public DrawingSettings Settings => DrawingSettings.Instance;

    public ICommand SelectToolCommand { get; }
    public ICommand NewProjectCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }
    public ICommand SaveProjectCommand { get; }

    public MainViewModel()
    {
        _toolRegistry = new ToolRegistry(Settings);
        _project = new Project(800, 600, "Фон");
        CurrentTool = _toolRegistry.GetTool<PencilTool>();
        Settings.SettingsChanged += () => OnPropertyChanged(nameof(Settings));

        LayersContext = new LayersViewModel(_project, History);

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

        NewProjectCommand = new RelayCommand(_ => {
            Project = new Project(800, 600, "Фон");
            StatusText = $"Створено проєкт {Project.Width}x{Project.Height} пікслів";
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
            StatusText = "Збереження...";
        });

        History.HistoryChanged += () => {
            CommandManager.InvalidateRequerySuggested();
        };
    }
}