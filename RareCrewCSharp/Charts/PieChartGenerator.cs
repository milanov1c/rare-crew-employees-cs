using RareCrewCSharp.Models;
using SkiaSharp;

namespace RareCrewCSharp.Charts;

public class PieChartGenerator : IChartGenerator
{
    private const int Padding = 30;
    private const int PieSize = 360;
    private const int LegendX = Padding * 2 + PieSize;
    private const int LegendWidth = 320;
    private const int LegendRowHeight = 26;
    private const int TitleHeight = 40;

    public byte[] GeneratePieChartPng(IReadOnlyList<EmployeeSummary> employees)
    {
        var width = LegendX + LegendWidth;
        var legendHeight = employees.Count * LegendRowHeight;
        var height = TitleHeight + Padding + Math.Max(PieSize, legendHeight) + Padding;

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        using var titleFont = new SKFont(SKTypeface.Default, 20);
        using var textFont = new SKFont(SKTypeface.Default, 14);
        using var labelFont = new SKFont(SKTypeface.Default, 13);
        using var textPaint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        using var labelPaint = new SKPaint { Color = SKColors.White, IsAntialias = true };

        canvas.DrawText("Percentage of total time worked", Padding, Padding, titleFont, textPaint);

        var total = employees.Sum(e => e.TotalHours);
        if (employees.Count == 0 || total <= 0)
        {
            canvas.DrawText("No data available.", Padding, TitleHeight + Padding, textFont, textPaint);
            return Encode(surface);
        }

        var pieRect = SKRect.Create(Padding, TitleHeight + Padding, PieSize, PieSize);
        var colors = GenerateColors(employees.Count);

        var startAngle = -90f;

        for (var i = 0; i < employees.Count; i++)
        {
            var employee = employees[i];
            var sweep = (float)(employee.Percentage / 100 * 360);

            using var slicePaint = new SKPaint { Color = colors[i], IsAntialias = true, Style = SKPaintStyle.Fill };
            canvas.DrawArc(pieRect, startAngle, sweep, true, slicePaint);

            using var borderPaint = new SKPaint { Color = SKColors.White, IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 2 };
            canvas.DrawArc(pieRect, startAngle, sweep, true, borderPaint);

            if (sweep >= 18)
            {
                var midAngle = (startAngle + sweep / 2) * MathF.PI / 180f;
                var radius = PieSize / 2f * 0.65f;
                var x = pieRect.MidX + radius * MathF.Cos(midAngle);
                var y = pieRect.MidY + radius * MathF.Sin(midAngle);
                canvas.DrawText($"{employee.Percentage:0.#}%", x, y + 5, SKTextAlign.Center, labelFont, labelPaint);
            }

            var rowY = TitleHeight + Padding + i * LegendRowHeight;
            canvas.DrawRect(SKRect.Create(LegendX, rowY, 16, 16), slicePaint);
            canvas.DrawText(
                $"{employee.Name}: {employee.Percentage:0.##}% ({employee.TotalHours:0.#}h)",
                LegendX + 24, rowY + 13, textFont, textPaint);

            startAngle += sweep;
        }

        return Encode(surface);
    }

    private static byte[] Encode(SKSurface surface)
    {
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private static SKColor[] GenerateColors(int count)
    {
        var colors = new SKColor[count];
        for (var i = 0; i < count; i++)
        {
            var hue = 360f * i / count;
            var value = i % 2 == 0 ? 85f : 70f;
            colors[i] = SKColor.FromHsv(hue, 70f, value);
        }
        return colors;
    }
}
