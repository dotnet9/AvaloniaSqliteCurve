using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using SkiaSharp;

namespace AvaloniaSqliteCurve.Controls;

public class CFPlot : Control
{
    private class CustomDrawOp : ICustomDrawOperation
    {
        public Rect Bounds { get; }

        public CustomDrawOp(Rect bounds)
        {
            Bounds = bounds;
        }

        public void Dispose()
        {
            // 根据需要进行资源释放操作
        }

        public bool Equals(ICustomDrawOperation? other)
        {
            return false;
        }

        public bool HitTest(Point p)
        {
            return false;
        }

        public void Render(ImmediateDrawingContext context)
        {
            var width = (int)Bounds.Width;
            var height = (int)Bounds.Height;
            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            using var paint = new SKPaint();
            canvas.DrawColor(SKColors.LightGreen, SKBlendMode.Src);

            canvas.ClipRect(new SKRect(100, 100, 300, 200));
            //将颜色填充当前裁切区域
            canvas.DrawColor(SKColors.Blue, SKBlendMode.SrcOver);
            paint.Color = SKColors.Red;
            paint.TextSize = 24;
            canvas.DrawText($"DrawColor", 95, 150, paint);
        }
    }

    public override void Render(DrawingContext context)
    {
        Rect controlBounds = new(Bounds.Size);
        CustomDrawOp customDrawOp = new(controlBounds);
        context.Custom(customDrawOp);
    }
}