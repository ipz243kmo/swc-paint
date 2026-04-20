using SWCPaint.Core.Models;

namespace SWCPaint.Wpf.ViewModels;

public class LayerViewModel : BaseViewModel
{
    private readonly Layer _model;
    private readonly Action _onChanged;

    public LayerViewModel(Layer model, Action onChanged)
    {
        _model = model;
        _onChanged = onChanged;
    }

    public Guid Id => _model.Id;
    public string Name => _model.Name;

    public bool IsVisible
    {
        get => _model.IsVisible;
        set
        {
            if (_model.IsVisible != value)
            {
                _model.IsVisible = value;
                OnPropertyChanged();
                _onChanged?.Invoke();
            }
        }
    }
}