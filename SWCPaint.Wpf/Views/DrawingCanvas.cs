using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SWCPaint.Core.Interfaces;
using SWCPaint.Core.Models;
using SWCPaint.Core.Services;
using SWCPaint.Core.Tools;
using SWCPaint.Infrastructure.Graphics;

namespace SWCPaint.Wpf.Views;

public class DrawingCanvas : FrameworkElement
{
    public static readonly DependencyProperty ProjectProperty =
    DependencyProperty.Register(
        nameof(Project),
        typeof(Project),
        typeof(DrawingCanvas),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnProjectChanged
        ));

    public static readonly DependencyProperty ActiveToolProperty =
        DependencyProperty.Register(
            nameof(ActiveTool), 
            typeof(ITool), 
            typeof(DrawingCanvas),
            new FrameworkPropertyMetadata(null)
            );

    public Project Project
    {
        get => (Project)GetValue(ProjectProperty);
        set => SetValue(ProjectProperty, value);
    }

    public ITool ActiveTool
    {
        get => (ITool)GetValue(ActiveToolProperty);
        set => SetValue(ActiveToolProperty, value);
    }

    public DrawingCanvas()
    {
        Focusable = true;
        ClipToBounds = true;
    }

    private static void OnProjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var canvas = (DrawingCanvas)d;

        if (e.OldValue is Project oldProject)
            oldProject.ProjectChanged -= canvas.OnProjectRequestRedraw;

        if (e.NewValue is Project newProject)
            newProject.ProjectChanged += canvas.OnProjectRequestRedraw;

        canvas.InvalidateVisual();
    }

    private void OnProjectRequestRedraw()
    {
        Dispatcher.Invoke(InvalidateVisual);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        if (Project == null) return;

        var adapter = new WpfDrawingContext(drawingContext);

        drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, Project.Width, Project.Height));

        Project.Render(adapter);
    }

    #region Mouse Events Handling

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (ActiveTool == null || Project == null) return;

        var context = new ToolContext(Project, DrawingSettings.Instance);

        CaptureMouse();
        var position = e.GetPosition(this);
        var corePoint = new SWCPaint.Core.Models.Point(position.X, position.Y);

        ActiveTool.OnMouseDown(corePoint, context);

        InvalidateVisual();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (!IsMouseCaptured || ActiveTool == null || Project == null) return;

        var context = new ToolContext(Project, DrawingSettings.Instance);
        var position = e.GetPosition(this);
        var corePoint = new SWCPaint.Core.Models.Point(position.X, position.Y);

        ActiveTool.OnMouseMove(corePoint, context);

        InvalidateVisual();
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        if (!IsMouseCaptured) return;

        var context = new ToolContext(Project, DrawingSettings.Instance);
        var position = e.GetPosition(this);
        var corePoint = new SWCPaint.Core.Models.Point(position.X, position.Y);

        ActiveTool?.OnMouseUp(corePoint, context);

        ReleaseMouseCapture();
        InvalidateVisual();
    }

    #endregion
}