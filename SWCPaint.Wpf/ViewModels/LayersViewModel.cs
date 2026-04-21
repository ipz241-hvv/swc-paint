using System.Collections.ObjectModel;
using System.Windows.Input;
using SWCPaint.Core.Commands;
using SWCPaint.Core.Models;
using SWCPaint.Wpf.Commands;

namespace SWCPaint.Wpf.ViewModels;

public class LayersViewModel : BaseViewModel
{
    private Project _project;
    private readonly HistoryManager _history;
    public ObservableCollection<LayerViewModel> Layers { get; } = new();

    public LayersViewModel(Project project, HistoryManager history)
    {
        _project = project;
        _history = history;

        SyncLayers();

        _project.ProjectChanged += () => SyncLayers();

        AddLayerCommand = new RelayCommand(_ => AddLayer());
        RemoveLayerCommand = new RelayCommand(
            _ => RemoveLayer(),
            _ => _project.Layers.Count > 1
        );
        MoveLayerUpCommand = new RelayCommand(_ => MoveLayer(-1), _ => CanMove(-1));
        MoveLayerDownCommand = new RelayCommand(_ => MoveLayer(1), _ => CanMove(1));
    }

    public Project CurrentProject
    {
        get => _project;
        set
        {
            _project = value;
            SyncLayers();
            OnPropertyChanged();
        }
    }

    public LayerViewModel? SelectedLayer
    {
        get => Layers.FirstOrDefault(l => l.Id == _project.CurrentLayerId);
        set
        {
            if (value == null || _project.CurrentLayerId == value.Id) return;
            _project.CurrentLayerId = value.Id;
            OnPropertyChanged();
        }
    }

    public ICommand AddLayerCommand { get; }
    public ICommand RemoveLayerCommand { get; }
    public ICommand MoveLayerUpCommand { get; }
    public ICommand MoveLayerDownCommand { get; }

    private void SyncLayers()
    {
        Layers.Clear();

        foreach (var layer in _project.Layers)
            Layers.Add(new LayerViewModel(layer, () => _project.RequestRedraw()));

        OnPropertyChanged(nameof(SelectedLayer));
    }

    private void AddLayer()
    {
        string name = $"Шар {Layers.Count + 1}";
        var command = new AddLayerCommand(_project, name);

        _history.Execute(command);
    }

    private void RemoveLayer()
    {
        if (SelectedLayer == null || _project.Layers.Count <= 1) return;

        var command = new RemoveLayerCommand(_project, SelectedLayer.Id);

        _history.Execute(command);
    }

    private bool CanMove(int direction)
    {
        if (SelectedLayer == null) return false;
        int currentIndex = _project.Layers.ToList().FindIndex(l => l.Id == SelectedLayer.Id);
        int newIndex = currentIndex + direction;
        return newIndex >= 0 && newIndex < _project.Layers.Count;
    }

    private void MoveLayer(int direction)
    {
        if (SelectedLayer == null) return;

        int currentIndex = _project.Layers.ToList().FindIndex(l => l.Id == SelectedLayer.Id);
        var command = new MoveLayerCommand(_project, SelectedLayer.Id, currentIndex + direction);
        _history.Execute(command);
    }
}