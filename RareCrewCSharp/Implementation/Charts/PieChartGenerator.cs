using RareCrewCSharp.DTO;
using SkiaSharp;

namespace RareCrewCSharp.Implementation.Charts;

public class PieChartGenerator : IChartGenerator
{
    private const int Padding = 30;
    private const int PieSize = 360;
    private const int LegendX = Padding * 2 + PieSize;
    private const int LegendWidth = 320;
    private const int LegendRowHeight = 26;

    public byte[] GeneratePieChart(List<EmployeeDTO> employees)
    {
        var width = LegendX + LegendWidth;
        var height = Padding * 2 + Math.Max(PieSize, employees.Count * LegendRowHeight);

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        using var textFont = new SKFont(SKTypeface.Default, 14);
        using var labelFont = new SKFont(SKTypeface.Default, 13);
        using var textPaint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
        using var labelPaint = new SKPaint { Color = SKColors.White, IsAntialias = true };
        using var borderPaint = new SKPaint { Color = SKColors.White, IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 2 };

        if (employees.Sum(x => x.TotalHours) <= 0)
        {
            canvas.DrawText("No data available.", Padding, Padding, textFont, textPaint);
            return Encode(surface);
        }

        var pieRect = SKRect.Create(Padding, Padding, PieSize, PieSize);
        var colors = GenerateColors(employees.Count);
        var startAngle = -90f;

        for (var i = 0; i < employees.Count; i++)
        {
            var employee = employees[i];
            var sweep = (float)(employee.Percentage / 100 * 360);

            using var slicePaint = new SKPaint { Color = colors[i], IsAntialias = true };
            canvas.DrawArc(pieRect, startAngle, sweep, true, slicePaint);
            canvas.DrawArc(pieRect, startAngle, sweep, true, borderPaint);

            if (sweep >= 18)
            {
                var middleAngle = (startAngle + sweep / 2) * MathF.PI / 180f;
                var radius = PieSize / 2f * 0.65f;
                var x = pieRect.MidX + radius * MathF.Cos(middleAngle);
                var y = pieRect.MidY + radius * MathF.Sin(middleAngle);
                canvas.DrawText($"{employee.Percentage:0.#}%", x, y + 5, SKTextAlign.Center, labelFont, labelPaint);
            }

            var rowY = Padding + i * LegendRowHeight;
            canvas.DrawRect(SKRect.Create(LegendX, rowY, 16, 16), slicePaint);
            canvas.DrawText($"{employee.Name}: {employee.Percentage:0.##}%", LegendX + 24, rowY + 13, textFont, textPaint);

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
