using System;
using System.Drawing;
using DevExpress.XtraPrinting;

namespace EgsReport;

/// <summary>
/// Utility helpers for measuring text with DevExpress padding semantics. The original
/// VB module exposed the same surface area so report builders can keep using the
/// existing measurement behavior when porting layouts to .NET 9.
/// </summary>
public static class ReportingTextUtils
{
    /// <summary>
    /// Measures text using a Graphics surface while accounting for DevExpress <see cref="PaddingInfo"/> values.
    /// Mirrors the VB implementation so callers can request a width-constrained size.
    /// </summary>
    public static Size MeasureText(string? text, Font font, int maxWidth, StringFormat format, PaddingInfo padding)
    {
        if (font is null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        var padWidth = padding.Left + padding.Right;
        var padHeight = padding.Top + padding.Bottom;

        if (string.IsNullOrEmpty(text))
        {
            using var bmp = new Bitmap(1, 1);
            using var graphics = Graphics.FromImage(bmp);
            var baseHeight = Math.Ceiling(graphics.MeasureString("A", font).Height);
            return new Size(padWidth, (int)baseHeight + padHeight);
        }

        using var bitmap = new Bitmap(1, 1);
        using var g = Graphics.FromImage(bitmap);
        g.PageUnit = GraphicsUnit.Pixel;

        var layoutWidth = maxWidth <= 0 ? int.MaxValue : Math.Max(1, maxWidth);
        var measured = g.MeasureString(text, font, layoutWidth, format);

        return new Size(
            (int)Math.Ceiling(measured.Width) + padWidth,
            (int)Math.Ceiling(measured.Height) + padHeight);
    }

    /// <summary>
    /// Measures text while assuming zero padding.
    /// </summary>
    public static Size MeasureText(string? text, Font font, int maxWidth, StringFormat format)
    {
        return MeasureText(text, font, maxWidth, format, new PaddingInfo(0, 0, 0, 0));
    }
}
