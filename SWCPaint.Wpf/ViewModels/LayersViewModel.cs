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
            if (_project == value) return;
            _project.ProjectChanged -= OnProjectChanged; 
            _project = value;
            _project.ProjectChanged += OnProjectChanged; 
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

    public ICommand AddLayerCommand { get; private set; }
    public ICommand RemoveLayerCommand { get; private set; }
    public ICommand MoveLayerUpCommand { get; private set; }
    public ICommand MoveLayerDownCommand { get; private set; }
    public ICommand RenameLayerCommand { get; private set; }

    private void OnProjectChanged() => SyncLayers();

    private void SyncLayers()
    {
      
        var actualLayers = _project.Layers.AsEnumerable().Reverse().ToList();

  
        var toRemove = Layers.Where(l => actualLayers.All(al => al.Id != l.Id)).ToList();
        foreach (var layer in toRemove) Layers.Remove(layer);

       
        for (int i = 0; i < actualLayers.Count; i++)
        {
            var modelLayer = actualLayers[i];
            if (i >= Layers.Count || Layers[i].Id != modelLayer.Id)
            {
                var newVm = new LayerViewModel(modelLayer, () => _project.RequestRedraw());
                if (i < Layers.Count) Layers.Insert(i, newVm);
                else Layers.Add(newVm);
            }
        }

        OnPropertyChanged(nameof(SelectedLayer));
    }

    private void AddLayer()
    {
        string name = $"{Strings.Layer_Name} {Layers.Count + 1}";
        var command = new AddLayerCommand(_project, name);

        _history.Execute(command);
    }

    private void RemoveLayer(object? parameter)
    {
        Guid? idToRemove = parameter switch
        {
            Guid id => id,
            LayerViewModel vm => vm.Id,
            _ => SelectedLayer?.Id
        };

        if (idToRemove.HasValue && _project.Layers.Count > 1)
        {
            _history.Execute(new RemoveLayerCommand(_project, idToRemove.Value));
        }
    }

    private bool CanMove(int uiDirection)
    {
        if (SelectedLayer == null) return false;
        
        int modelIndex = _project.Layers.ToList().FindIndex(l => l.Id == SelectedLayer.Id);
        int newIndex = modelIndex - uiDirection; // Інверсія для відповідності UI (Z-index)

        return newIndex >= 0 && newIndex < _project.Layers.Count;
    }

    private void MoveLayer(int uiDirection)
    {
        if (!CanMove(uiDirection)) return;

        int modelIndex = _project.Layers.ToList().FindIndex(l => l.Id == SelectedLayer.Id);
        int newIndex = modelIndex - uiDirection;

        _history.Execute(new MoveLayerCommand(_project, SelectedLayer.Id, newIndex));
    }

    private void RenameLayer(object? parameter)
    {
        var target = (parameter as LayerViewModel) ?? SelectedLayer;
        if (target == null) return;

        var newName = _dialogService.ShowInputBox(Strings.Layer_Rename_Title, Strings.Layer_Rename_Prompt, target.Name);

        if (!string.IsNullOrWhiteSpace(newName) && newName != target.Name)
        {
            var layerModel = _project.Layers.FirstOrDefault(l => l.Id == target.Id);
            if (layerModel != null)
            {
                _history.Execute(new RenameLayerCommand(layerModel, newName, SyncLayers));
            }
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _project.ProjectChanged -= OnProjectChanged;
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}
