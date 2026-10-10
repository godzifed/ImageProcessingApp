using System;
using ImageProcessing.Core.Buffers;

namespace ImageProcessing.Core.Filters;

// Універсальний механізм для застосування будь-яких просторових фільтрів
public static class ConvolutionEngine
{
    public static PixelBuffer Convolve(PixelBuffer source, float[,] kernel)
    {
        var result = new PixelBuffer(source.Width, source.Height, source.Format);
        
        // Дізнаємося розмір матриці (наприклад, 3) і її радіус (наприклад, 1 піксель навколо центру)
        int kernelSize = kernel.GetLength(0);
        int radius = kernelSize / 2;

        // Ковзаємо по кожному пікселю зображення
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                float sumR = 0, sumG = 0, sumB = 0;

                // Перебираємо сусідів пікселя відповідно до розміру матриці
                for (int ky = -radius; ky <= radius; ky++)
                {
                    for (int kx = -radius; kx <= radius; kx++)
                    {
                        // Math.Clamp не дає програмі "вилетіти" за краї картинки
                        int px = Math.Clamp(x + kx, 0, source.Width - 1);
                        int py = Math.Clamp(y + ky, 0, source.Height - 1);

                        // Беремо колір сусіда і множимо його на число з матриці (вагу)
                        ColorRgba color = source.GetPixel(px, py);
                        float weight = kernel[ky + radius, kx + radius];

                        sumR += color.R * weight;
                        sumG += color.G * weight;
                        sumB += color.B * weight;
                    }
                }

                // Обрізаємо результат, щоб колір не вийшов за межі 0-255
                byte r = (byte)Math.Clamp(sumR, 0, 255);
                byte g = (byte)Math.Clamp(sumG, 0, 255);
                byte b = (byte)Math.Clamp(sumB, 0, 255);

                // Записуємо новий піксель, зберігаючи оригінальну прозорість (A)
                byte a = source.GetPixel(x, y).A;
                result.SetPixel(x, y, new ColorRgba(r, g, b, a));
            }
        }

        return result;
    }
}