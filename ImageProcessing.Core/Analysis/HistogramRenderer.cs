using ImageProcessing.Core.Buffers;

namespace ImageProcessing.Core.Analysis;

public static class HistogramRenderer
{
    // Створюємо картинку-графік розміром 256 пікселів у ширину (для кожної яскравості) 
    // і 100 пікселів у висоту.
    public static PixelBuffer Render(int[] histogram, int width = 256, int height = 100)
    {
        var buffer = new PixelBuffer(width, height, PixelFormat.Bgra32);

        // 1. Знаходимо найвищий стовпчик, щоб графік не виліз за межі висоти
        int maxVal = 0;
        for (int i = 0; i < histogram.Length; i++)
        {
            if (histogram[i] > maxVal) maxVal = histogram[i];
        }
        if (maxVal == 0) maxVal = 1; // Захист від ділення на нуль для чорних картинок

        // Кольори для малювання
        var bgColor = new ColorRgba(30, 30, 30);      // Темно-сірий фон графіка
        var barColor = new ColorRgba(100, 200, 255);  // Блакитні стовпчики

        // 2. Заливаємо весь графік фоновим кольором
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                buffer.SetPixel(x, y, bgColor);

        // 3. Малюємо самі стовпчики
        for (int x = 0; x < 256; x++)
        {
            // Вираховуємо висоту конкретного стовпчика у відсотках від найбільшого
            int barHeight = (int)((histogram[x] / (float)maxVal) * height);

            // Малюємо лінію знизу екрана до потрібної висоти стовпчика.
            // Оскільки координата Y=0 - це верх екрана, ми йдемо знизу (height - 1) вгору.
            for (int y = height - 1; y >= height - barHeight; y--)
            {
                if (y >= 0 && y < height && x < width) // Захист від виходу за межі
                {
                    buffer.SetPixel(x, y, barColor);
                }
            }
        }

        return buffer;
    }
}