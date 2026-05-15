using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SWCPaint.Core.Commands;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Wpf.Commands;
using SWCPaint.Wpf.Resources;

namespace SWCPaint.Wpf.ViewModels;

public class LayersViewModel : BaseViewModel, IDisposable
{
    private Project _project;
    private readonly HistoryManager _history;
    private readonly IDialogService _dialogService;
    private bool _isDisposed;

    public ObservableCollection<LayerViewModel> Layers { get; } = new();

    public LayersViewModel(Project project, HistoryManager history, IDialogService dialogService)
    {
        _project = project ?? throw new ArgumentNullException(nameof(project));
        _history = history ?? throw new ArgumentNullException(nameof(history));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        InitializeCommands();
        SyncLayers();

        _project.ProjectChanged += OnProjectChanged;
    }

    private void InitializeCommands()
    {
        AddLayerCommand = new RelayCommand(_ => AddLayer());
        RemoveLayerCommand = new RelayCommand(p => RemoveLayer(p), _ => _project.Layers.Count > 1);
        MoveLayerUpCommand = new RelayCommand(_ => MoveLayer(-1), _ => CanMove(-1));
        MoveLayerDownCommand = new RelayCommand(_ => MoveLayer(1), _ => CanMove(1));
        RenameLayerCommand = new RelayCommand(p => RenameLayer(p), _ => SelectedLayer != null);
    }

    public Project CurrentProject
    {
        get => _project;
        set
        {
            var newProject = value ?? throw new ArgumentNullException(nameof(value));
            if (ReferenceEquals(_project, newProject)) return;

            _project.ProjectChanged -= OnProjectChanged;
            _project = newProject;
            _project.ProjectChanged += OnProjectChanged;

            SyncLayers();
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedLayer));
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

    public ICommand AddLayerCommand { get; private set; } = null!;
    public ICommand RemoveLayerCommand { get; private set; } = null!;
    public ICommand MoveLayerUpCommand { get; private set; } = null!;
    public ICommand MoveLayerDownCommand { get; private set; } = null!;
    public ICommand RenameLayerCommand { get; private set; } = null!;

    private void OnProjectChanged() => SyncLayers();

    private IReadOnlyList<Layer> GetLayersInUiOrder()
        => _project.Layers.AsEnumerable().Reverse().ToList();

    private int GetSelectedLayerModelIndex()
    {
        var selectedLayer = SelectedLayer;
        if (selectedLayer == null) return -1;

        return _project.Layers.ToList().FindIndex(layer => layer.Id == selectedLayer.Id);
    }

    private int GetTargetModelIndex(int uiDirection)
    {
        var modelIndex = GetSelectedLayerModelIndex();
        if (modelIndex < 0) return -1;

        // UI direction is inverted relative to model order (Z-index)
        return modelIndex - uiDirection;
    }

    private LayerViewModel CreateLayerViewModel(Layer modelLayer)
        => new LayerViewModel(modelLayer, () => _project.RequestRedraw());

    private void SyncLayers()
    {
        Layers.Clear();

        foreach (var modelLayer in GetLayersInUiOrder())
        {
            Layers.Add(CreateLayerViewModel(modelLayer));
        }

        OnPropertyChanged(nameof(SelectedLayer));
    }

    private void AddLayer()
    {
        string name = $"{Strings.Layer_Name} {Layers.Count + 1}";
        _history.Execute(new AddLayerCommand(_project, name));
    }

    private void RemoveLayer(object? parameter)
    {
        Guid? idToRemove = parameter switch
        {
            Guid id => id,
            LayerViewModel vm => vm.Id,
            _ => SelectedLayer?.Id
        };

        if (!idToRemove.HasValue || _project.Layers.Count <= 1) return;

        _history.Execute(new RemoveLayerCommand(_project, idToRemove.Value));
    }

    private bool CanMove(int uiDirection) => GetTargetModelIndex(uiDirection) >= 0;

    private void MoveLayer(int uiDirection)
    {
        var selectedLayer = SelectedLayer;
        if (selectedLayer == null) return;

        var newIndex = GetTargetModelIndex(uiDirection);
        if (newIndex < 0) return;

        _history.Execute(new MoveLayerCommand(_project, selectedLayer.Id, newIndex));
    }

    private void RenameLayer(object? parameter)
    {
        var target = (parameter as LayerViewModel) ?? SelectedLayer;
        if (target == null) return;

        var newName = _dialogService.ShowInputBox(
            Strings.Layer_Rename_Title,
            Strings.Layer_Rename_Prompt,
            target.Name);

        if (string.IsNullOrWhiteSpace(newName) || newName == target.Name) return;

        var layerModel = _project.Layers.FirstOrDefault(l => l.Id == target.Id);
        if (layerModel == null) return;

        _history.Execute(new RenameLayerCommand(layerModel, newName, SyncLayers));
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        _project.ProjectChanged -= OnProjectChanged;
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}
