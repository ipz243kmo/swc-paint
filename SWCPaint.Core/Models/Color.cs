using System.Xml;

namespace SWCPaint.Core.Models;

public readonly struct Color
{
    public byte Red { get; }
    public byte Green { get; }
    public byte Blue { get; }
    public byte Alpha { get; }

    public Color(byte red = 0, byte green = 0, byte blue = 0, byte alpha = 255)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    public static Color Black => new(0, 0, 0);
    public static Color White => new(255, 255, 255);
}
