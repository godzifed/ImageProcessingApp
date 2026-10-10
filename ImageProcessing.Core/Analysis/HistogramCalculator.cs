using ImageProcessing.Core.Buffers;

namespace ImageProcessing.Core.Analysis;

public static class HistogramCalculator
{
    // Повертає масив із 256 чисел, де індекс - це яскравість (0-255), 
    // а значення - кількість пікселів такої яскравості.
    public static int[] CalculateLuminance(PixelBuffer source)
    {
        int[] histogram = new int[256];

        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                // Беремо піксель і дізнаємося його яскравість (від 0 до 255)
                ColorRgba color = source.GetPixel(x, y);
                byte brightness = color.ToGrayscale();
                
                // Збільшуємо лічильник пікселів для цієї конкретної яскравості
                histogram[brightness]++;
            }
        }

        return histogram;
    }
}