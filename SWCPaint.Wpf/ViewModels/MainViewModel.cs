using System.Reflection;
using System.Windows.Input;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Core.Services;
using SWCPaint.Core.Tools;
using SWCPaint.Wpf.Commands;

namespace SWCPaint.Wpf.ViewModels;

public class MainViewModel : BaseViewModel
{
    private Project _project;
    private readonly ToolRegistry _toolRegistry;
    private ITool _currentTool;
    private readonly Dictionary<Type, string> _toolNames = new()
    {
        [typeof(PencilTool)] = "Олівець"
    };
    private string _statusText = "Готово";

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

    public MainViewModel()
    {
        _toolRegistry = new ToolRegistry(Settings);
        _project = new Project(800, 600, "Фон");
        CurrentTool = _toolRegistry.GetTool<PencilTool>();
        Settings.SettingsChanged += () => OnPropertyChanged(nameof(Settings));

        LayersContext = new LayersViewModel(_project);
        SelectToolCommand = new RelayCommand(param =>
        {
            string toolType = (string)(param ?? "Pencil");

            CurrentTool = _toolRegistry.GetTool(toolType);
            StatusText = $"Інструмент: {_toolNames[CurrentTool.GetType()]}";
        });

        _currentTool = _toolRegistry.GetTool<PencilTool>();

        NewProjectCommand = new RelayCommand(_ => {
            Project = new Project(800, 600, "Фон");
            StatusText = $"Створено проєкт {Project.Width}x{Project.Height} пікслів";
        });
    }
}