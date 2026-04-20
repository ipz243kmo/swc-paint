using System.Collections.ObjectModel;
using System.Windows.Input;
using SWCPaint.Core.Models;
using SWCPaint.Wpf.Commands;

namespace SWCPaint.Wpf.ViewModels;

public class LayersViewModel : BaseViewModel
{
    private Project _project;
    public ObservableCollection<LayerViewModel> Layers { get; } = new();

    public LayersViewModel(Project project)
    {
        _project = project;
        SyncLayers();

        AddLayerCommand = new RelayCommand(_ => AddLayer());
        RemoveLayerCommand = new RelayCommand(_ => RemoveLayer(), _ => Layers.Count > 1);
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

    private void SyncLayers()
    {
        Layers.Clear();
        foreach (var layer in _project.Layers)
            Layers.Add(new LayerViewModel(layer, () => _project.RequestRedraw()));

        OnPropertyChanged(nameof(SelectedLayer));
    }

    private void AddLayer()
    {
        var newLayer = new Layer($"Шар {Layers.Count + 1}");
        _project.AddLayer(newLayer);

        var layerViewModel = new LayerViewModel(newLayer, () => _project.RequestRedraw());
        Layers.Add(layerViewModel);
        SelectedLayer = layerViewModel;
    }

    private void RemoveLayer()
    {
        if (SelectedLayer == null || Layers.Count <= 1) return;

        var toRemove = SelectedLayer;
        _project.RemoveLayer(toRemove.Id);
        Layers.Remove(toRemove);
        SelectedLayer = Layers.LastOrDefault();
    }
}