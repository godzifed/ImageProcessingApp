namespace ImageProcessing.Core.Buffers;

// Перелік форматів, у яких ми можемо зберігати картинку.
// У більшості сучасних екранів і фреймворків (включаючи Avalonia) 
// використовується 32-бітний формат Bgra32 (по 1 байту на колір).
public enum PixelFormat
{
    Bgra32 = 0, // Blue, Green, Red, Alpha (прозорість)
    Rgba32 = 1, // Red, Green, Blue, Alpha
    Gray8 = 2   // Відтінки сірого (1 байт на піксель, чорно-біле)
}