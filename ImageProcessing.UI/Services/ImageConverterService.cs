using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ImageProcessing.Core.Buffers;
using SkiaSharp;
using PixelFormat = ImageProcessing.Core.Buffers.PixelFormat;

namespace ImageProcessing.UI.Services;

// Цей сервіс відповідає за "переклад" картинок між жорстким диском, 
// нашим математичним буфером (PixelBuffer) та візуальним інтерфейсом (WriteableBitmap).
public sealed class ImageConverterService
{
    // Метод 1: Зчитуємо файл із диска у наш PixelBuffer
    public PixelBuffer LoadFromFile(string filePath)
    {
        // Відкриваємо файл
        using var stream = File.OpenRead(filePath);
        
        // Створюємо декодер, який розуміє JPG/PNG
        using var codec = SKCodec.Create(stream);
        if (codec is null)
            throw new InvalidDataException("Не вдалося розпізнати формат файлу.");

        // Просимо декодер розпакувати картинку у формат Bgra32 (по 1 байту на колір)
        var info = new SKImageInfo(codec.Info.Width, codec.Info.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
        using var skBitmap = SKBitmap.Decode(codec, info);
        if (skBitmap is null)
            throw new InvalidOperationException("Помилка декодування пікселів.");

        // Створюємо наш математичний буфер потрібного розміру
        var buffer = new PixelBuffer(skBitmap.Width, skBitmap.Height, PixelFormat.Bgra32);
        
        // МАГІЯ ШВИДКОСТІ: Ми не копіюємо пікселі по одному через цикл for.
        // Ми беремо весь блок пам'яті SkiaSharp і миттєво перекидаємо його в наш buffer.Data
        Marshal.Copy(skBitmap.GetPixels(), buffer.Data, 0, buffer.Data.Length);

        return buffer;
    }

    // Метод 2: Конвертуємо наш математичний PixelBuffer у формат, який Avalonia може намалювати на екрані
    public WriteableBitmap ToWriteableBitmap(PixelBuffer buffer)
    {
        // Avalonia вимагає формат Bgra32 для малювання. 
        // Пізніше ми додамо перевірку на чорно-білі картинки, але поки що залишимо так.
        
        // Створюємо порожнє "полотно" для екрана
        var writeableBitmap = new WriteableBitmap(
            new PixelSize(buffer.Width, buffer.Height),
            new Vector(96, 96), // DPI (щільність пікселів, стандарт 96)
            Avalonia.Platform.PixelFormat.Bgra8888,
            AlphaFormat.Premul);

        // Блокуємо полотно в пам'яті відеокарти/процесора і заливаємо туди наші байти
        using (var frameBuffer = writeableBitmap.Lock())
        {
            Marshal.Copy(buffer.Data, 0, frameBuffer.Address, buffer.Data.Length);
        }

        return writeableBitmap;
    }
}