using System.Windows.Media;
using SWCPaint.Core.Interfaces;
using CoreColor = SWCPaint.Core.Models.Color;
using CorePoint = SWCPaint.Core.Models.Point;

namespace SWCPaint.Infrastructure.Graphics;

public class WpfDrawingContext : IDrawingContext
{
    private readonly DrawingContext _drawingContext;

    public WpfDrawingContext(DrawingContext drawingContext)
    {
        _drawingContext = drawingContext;
    }

    public void DrawLine(CorePoint start, CorePoint end, CoreColor strokeColor, double thickness)
    {
        var pen = new Pen(ToWpfBrush(strokeColor), thickness);
        _drawingContext.DrawLine(pen, ToWpfPoint(start), ToWpfPoint(end));
    }

    public void DrawRectangle(CorePoint topLeft, double width, double height, CoreColor strokeColor, CoreColor? fillColor, double thickness)
    {
        var pen = new Pen(ToWpfBrush(strokeColor), thickness);
        var brush = fillColor.HasValue ? ToWpfBrush(fillColor.Value) : null;

        _drawingContext.DrawRectangle(brush, pen, new System.Windows.Rect(ToWpfPoint(topLeft), new System.Windows.Size(width, height)));
    }

    public void DrawEllipse(CorePoint center, double radiusX, double radiusY, CoreColor strokeColor, CoreColor? fillColor, double thickness)
    {
        var pen = new Pen(ToWpfBrush(strokeColor), thickness);
        var brush = fillColor.HasValue ? ToWpfBrush(fillColor.Value) : null;

        _drawingContext.DrawEllipse(brush, pen, ToWpfPoint(center), radiusX, radiusY);
    }

    public void DrawPath(IEnumerable<CorePoint> points, CoreColor strokeColor, CoreColor? fillColor, double thickness, bool isClosed)
    {
        using var enumerator = points.GetEnumerator();

        if (!enumerator.MoveNext()) return;

        var streamGeometry = new StreamGeometry();
        using (StreamGeometryContext geometryContext = streamGeometry.Open())
        {
            geometryContext.BeginFigure(ToWpfPoint(enumerator.Current), fillColor.HasValue, isClosed);

            while (enumerator.MoveNext())
            {
                geometryContext.LineTo(ToWpfPoint(enumerator.Current), true, true);
            }
        }

        var pen = new Pen(ToWpfBrush(strokeColor), thickness);
        pen.StartLineCap = PenLineCap.Round;
        pen.EndLineCap = PenLineCap.Round;
        pen.LineJoin = PenLineJoin.Round;
        pen.Freeze();

        var brush = fillColor.HasValue ? ToWpfBrush(fillColor.Value) : null;
        _drawingContext.DrawGeometry(brush, pen, streamGeometry);
    }

    private System.Windows.Point ToWpfPoint(CorePoint p) => new(p.X, p.Y);

    private SolidColorBrush ToWpfBrush(CoreColor c)
    {
        var brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue));
        brush.Freeze();
        return brush;
    }
}