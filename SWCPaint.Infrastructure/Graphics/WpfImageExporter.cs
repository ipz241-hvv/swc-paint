using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SWCPaint.Core.Interfaces.Serialization;
using SWCPaint.Core.Models;
using SWCPaint.Infrastructure.Graphics;

namespace SWCPaint.Infrastructure.Services;

public class WpfImageExporter : IImageExporter
{
    public byte[] Export(Project project)
    {
        var renderTarget = new RenderTargetBitmap(
            (int)project.Width,
            (int)project.Height,
            96, 96,
            PixelFormats.Pbgra32);

        var visual = new DrawingVisual();

        using (var context = visual.RenderOpen())
        {
            var adapter = new WpfDrawingContext(context, project.Width, project.Height);

            var bg = project.BackgroundColor;
            var backgroundBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(bg.Alpha, bg.Red, bg.Green, bg.Blue));
            context.DrawRectangle(backgroundBrush, null, new Rect(0, 0, project.Width, project.Height));

            project.Render(adapter);
        }

        renderTarget.Render(visual);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(renderTarget));

        using var memoryStream = new MemoryStream();
        encoder.Save(memoryStream);

        return memoryStream.ToArray();
    }
}