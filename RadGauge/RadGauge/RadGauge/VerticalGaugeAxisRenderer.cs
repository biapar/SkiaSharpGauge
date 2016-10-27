﻿using SkiaSharp;
using System.Collections.Generic;

namespace RadGauge
{
    public class VerticalGaugeAxisRenderer : IGaugePartRenderer
    {
        double min = 0;
        double max = 100;
        double step = 20;
        float fontSize = 20f;
        float axisWidth = 1f;
        float tickLength = 5f;
        float tickThickness = 1f;
        SKColor axisColor = (SKColor)0xFF000000;
        SKColor tickColor = (SKColor)0xFF000000;
        SKColor labelColor = (SKColor)0xFF000000;
        float labelsOffset = 5f;
        float tickOffset = 0f;

        readonly Dictionary<string, SKRect> labelRects = new Dictionary<string, SKRect>();

        SKSize maxLabelSize;

        public SKSize MaxLabelSize
        {
            get { return maxLabelSize; }
        }

        public SKSize Measure(SKSize rect)
        {
            labelRects.Clear();

            this.maxLabelSize = GetMaxLabelSize();

            var width = tickOffset + labelsOffset + maxLabelSize.Width + axisWidth + tickLength + 1;

            return new SKSize(width, rect.Height);
        }

        public void Render(SKCanvas canvas, SKRect layoutSlot)
        {
            var top = layoutSlot.Top + maxLabelSize.Height / 2;
            var left = layoutSlot.Left;
            var height = layoutSlot.Height - maxLabelSize.Height;
            var tickLeft = labelsOffset + left + maxLabelSize.Width;

            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.IsStroke = false;
                paint.TextEncoding = SKTextEncoding.Utf32;
                paint.TextSize = fontSize;

                for (var i = min; i <= max; i += step)
                {
                    var position = GaugeRenderHelper.GetRelativePosition(i, min, max, top + height, top);
                    DrawLabel(i.ToString(), left, position, labelColor, paint, canvas);

                    var tickTop = position;
                    var tickRect = new SKRect(tickLeft, tickTop, tickLeft + tickLength, tickTop + tickThickness);
                    DrawRect(tickRect, tickColor, paint, canvas);
                }

                var axisLeft = tickOffset + tickLeft + tickLength;

                DrawRect(new SKRect(axisLeft, top, axisLeft + axisWidth, top + height), axisColor, paint, canvas);
            }
        }

        private void DrawLabel(string text, float x, float y, SKColor color, SKPaint paint, SKCanvas canvas)
        {
            paint.Color = color;

            SKRect bounds = this.labelRects[text];

            canvas.DrawText(text, x + maxLabelSize.Width - bounds.Width, y - bounds.Top / 2, paint);
        }

        private void DrawRect(SKRect rect, SKColor color, SKPaint paint, SKCanvas canvas)
        {
            paint.Color = color;

            canvas.DrawRect(rect, paint);
        }

        private SKSize GetMaxLabelSize()
        {
            // TODO
            float maxWidth = 0;
            float maxHeight = 0;
            float lastLabelHeight = 0;

            using (var paint = new SKPaint
            {
                TextSize = this.fontSize,
                IsAntialias = true,
                IsStroke = false,
                TextEncoding = SKTextEncoding.Utf32
            })
            {
                double i;
                for (i = this.min; i <= this.max; i += step)
                {
                    var bounds = MeasureText(i.ToString(), paint);

                    if (bounds.Width > maxWidth)
                    {
                        maxWidth = bounds.Width;
                    }

                    if (i == min && maxHeight < bounds.Height)
                    {
                        maxHeight = bounds.Height;
                    }

                    lastLabelHeight = bounds.Height;
                }

                if (i > max)
                {
                    var bounds = MeasureText(i.ToString(), paint);
                    if (bounds.Width > maxWidth)
                    {
                        maxWidth = bounds.Width;
                    }

                    lastLabelHeight = bounds.Height;
                }

                if (maxHeight < lastLabelHeight)
                {
                    maxHeight = lastLabelHeight;
                }
            }

            return new SKSize(maxWidth, maxHeight);
        }

        private SKRect MeasureText(string text, SKPaint paint)
        {
            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);


            if (this.labelRects.ContainsKey(text))
            {
                this.labelRects[text] = bounds;
            }
            else
            {
                this.labelRects.Add(text, bounds);
            }

            return bounds;
        }
    }
}
