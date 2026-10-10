using System.Runtime.InteropServices;

namespace ImageProcessing.Core.Buffers;

// Ця структура описує один конкретний колір.
// [StructLayout(LayoutKind.Sequential)] гарантує, що байти кольору 
// будуть лежати в оперативній пам'яті рівно один за одним. Це дає високу швидкість.
[StructLayout(LayoutKind.Sequential)]
public readonly record struct ColorRgba(byte R, byte G, byte B, byte A = 255)
{
    // Метод для перетворення кольорового пікселя в чорно-білий (яскравість).
    // Ми використовуємо швидкі побітові зсуви (>> 8) замість ділення, 
    // щоб алгоритми працювали миттєво.
    public byte ToGrayscale() =>
        (byte)((R * 77 + G * 150 + B * 29) >> 8);
}