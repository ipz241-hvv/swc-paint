using System.Collections.ObjectModel;
using System.Windows.Input;
using SWCPaint.Core.Commands;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Wpf.Commands;
using SWCPaint.Wpf.Resources;

namespace SWCPaint.Wpf.ViewModels;

public class LayersViewModel : BaseViewModel
{
    private Project _project;
    private readonly HistoryManager _history;
    private readonly IDialogService _dialogService;
    public ObservableCollection<LayerViewModel> Layers { get; } = new();

    public LayersViewModel(Project project, HistoryManager history, IDialogService dialogService)
    {
        _project = project;
        _history = history;
        _dialogService = dialogService;

        SyncLayers();

        _project.ProjectChanged += () => SyncLayers();

        AddLayerCommand = new RelayCommand(_ => AddLayer());
        RemoveLayerCommand = new RelayCommand(
            p => RemoveLayer(p),
            _ => _project.Layers.Count > 1
        );
        MoveLayerUpCommand = new RelayCommand(_ => MoveLayer(-1), _ => CanMove(-1));
        MoveLayerDownCommand = new RelayCommand(_ => MoveLayer(1), _ => CanMove(1));
        RenameLayerCommand = new RelayCommand(p => RenameLayer(p), _ => SelectedLayer != null);
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
    public ICommand RenameLayerCommand { get; }

    private void SyncLayers()
    {
        Layers.Clear();

        for (int i = _project.Layers.Count - 1; i >= 0; i--)
        {
            var layer = _project.Layers[i];
            Layers.Add(new LayerViewModel(layer, () => _project.RequestRedraw()));
        }

        OnPropertyChanged(nameof(SelectedLayer));
    }

    private void AddLayer()
    {
        string name = $"{Strings.Layer_Name} {Layers.Count + 1}";
        var command = new AddLayerCommand(_project, name);

        _history.Execute(command);
    }

    private void RemoveLayer(object? parameter = null)
    {
        Guid? idToRemove = null;

        if (parameter is Guid id) idToRemove = id;
        else if (SelectedLayer != null) idToRemove = SelectedLayer.Id;

        if (idToRemove == null || _project.Layers.Count <= 1) return;

        var command = new RemoveLayerCommand(_project, idToRemove.Value);
        _history.Execute(command);
    }

    private bool CanMove(int uiDirection)
    {
        if (SelectedLayer == null) return false;

        int modelIndex = _project.Layers.ToList().FindIndex(l => l.Id == SelectedLayer.Id);

        if (modelIndex == -1) return false;

        int modelDirection = -uiDirection;
        int newModelIndex = modelIndex + modelDirection;

        return newModelIndex >= 0 && newModelIndex < _project.Layers.Count;
    }

    private void MoveLayer(int uiDirection)
    {
        if (SelectedLayer == null) return;

        var layersList = _project.Layers.ToList();
        int modelIndex = layersList.FindIndex(l => l.Id == SelectedLayer.Id);
        if (modelIndex == -1) return;

        int modelDirection = -uiDirection;
        int newModelIndex = modelIndex + modelDirection;

        if (newModelIndex >= 0 && newModelIndex < _project.Layers.Count)
        {
            var command = new MoveLayerCommand(_project, SelectedLayer.Id, newModelIndex);

            _history.Execute(command);
            OnPropertyChanged(nameof(SelectedLayer));
        }
    }

    private void RenameLayer(object? parameter = null)
    {
        var target = (parameter as LayerViewModel) ?? SelectedLayer;

        if (target == null) return;

        var newName = _dialogService.ShowInputBox(Strings.Layer_Rename_Title, Strings.Layer_Rename_Prompt, target.Name);

        if (!string.IsNullOrWhiteSpace(newName) && newName != target.Name)
        {
            var layerModel = _project.Layers.First(l => l.Id == target.Id);
            var command = new RenameLayerCommand(layerModel, newName, () => SyncLayers());
            _history.Execute(command);
        }
    }
}