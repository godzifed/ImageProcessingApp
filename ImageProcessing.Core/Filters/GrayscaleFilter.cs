using ImageProcessing.Core.Buffers;

namespace ImageProcessing.Core.Filters;

public static class GrayscaleFilter
{
    // Метод приймає оригінальну картинку і повертає нову (оброблену)
    public static PixelBuffer Apply(PixelBuffer source)
    {
        // Створюємо порожній математичний буфер такого ж розміру
        var result = new PixelBuffer(source.Width, source.Height, source.Format);

        // Проходимось по кожному рядку (Y) і кожному стовпцю (X) картинки
        for (int y = 0; y < source.Height; y++)
        {
            for (int x = 0; x < source.Width; x++)
            {
                // 1. Беремо колір поточного пікселя оригіналу
                ColorRgba originalColor = source.GetPixel(x, y);

                // 2. Вираховуємо його яскравість (формулу ми писали раніше у структурі ColorRgba)
                byte grayValue = originalColor.ToGrayscale();

                // 3. Створюємо новий сірий колір (канали R, G і B мають однакове значення)
                var grayColor = new ColorRgba(grayValue, grayValue, grayValue, originalColor.A);

                // 4. Записуємо новий колір у новий буфер на ту саму позицію
                result.SetPixel(x, y, grayColor);
            }
        }

        return result;
    }
}