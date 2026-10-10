using System;
using System.Runtime.CompilerServices;

namespace ImageProcessing.Core.Buffers;

// Високопродуктивний буфер зображення для обробки пікселів.
// sealed означає, що від цього класу не можна успадковуватися (це невелика оптимізація для C#).
public sealed class PixelBuffer
{
    // Одинарний, суцільний масив пам'яті, де лежать усі пікселі підряд.
    private readonly byte[] _data;

    public int Width { get; }
    public int Height { get; }
    public int BytesPerPixel { get; } // Скільки байтів займає 1 піксель (для Bgra32 = 4 байти)
    public int Stride { get; }        // Скільки байтів займає рівно 1 горизонтальний рядок зображення
    public PixelFormat Format { get; }

    // Прямий доступ до нашого масиву (щоб інші фільтри могли швидко по ньому бігати)
    public byte[] Data => _data;

    public PixelBuffer(int width, int height, PixelFormat format = PixelFormat.Bgra32)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        Width = width;
        Height = height;
        Format = format;
        
        // Визначаємо, скільки байтів потрібно на 1 піксель залежно від формату
        BytesPerPixel = format switch
        {
            PixelFormat.Bgra32 => 4,
            PixelFormat.Rgba32 => 4,
            PixelFormat.Gray8 => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        // Обчислюємо розмір одного рядка (Ширина * Кількість байтів на піксель)
        Stride = Width * BytesPerPixel;
        
        // Виділяємо пам'ять під усю картинку (Кількість рядків * Розмір рядка)
        _data = new byte[Stride * Height];
    }

    // Допоміжний метод: отримує колір конкретного пікселя за координатами X та Y.
    // [MethodImpl(MethodImplOptions.AggressiveInlining)] просить компілятор 
    // вставити цей код прямо в місце виклику для швидкості.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ColorRgba GetPixel(int x, int y)
    {
        // Обчислюємо точну адресу (індекс) нашого пікселя у великому масиві _data
        int offset = (y * Stride) + (x * BytesPerPixel);

        return Format switch
        {
            PixelFormat.Bgra32 => new ColorRgba(
                R: _data[offset + 2],
                G: _data[offset + 1],
                B: _data[offset],
                A: _data[offset + 3]),

            PixelFormat.Gray8 => new ColorRgba(
                R: _data[offset],
                G: _data[offset],
                B: _data[offset],
                A: 255),

            _ => default
        };
    }

    // Допоміжний метод: встановлює колір пікселя
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPixel(int x, int y, ColorRgba color)
    {
        int offset = (y * Stride) + (x * BytesPerPixel);

        if (Format == PixelFormat.Bgra32)
        {
            _data[offset] = color.B;
            _data[offset + 1] = color.G;
            _data[offset + 2] = color.R;
            _data[offset + 3] = color.A;
        }
        else if (Format == PixelFormat.Gray8)
        {
            // Якщо картинка чорно-біла, перетворюємо колір у відтінок сірого
            _data[offset] = color.ToGrayscale();
        }
    }

    // Метод для створення точної копії зображення (щоб зберігати оригінал і не псувати його)
    public PixelBuffer Clone()
    {
        var copy = new PixelBuffer(Width, Height, Format);
        Buffer.BlockCopy(_data, 0, copy._data, 0, _data.Length); // Дуже швидке копіювання пам'яті
        return copy;
    }
}