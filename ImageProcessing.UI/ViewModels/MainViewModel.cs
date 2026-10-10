using System;
using System.IO;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageProcessing.Core.Buffers;
using ImageProcessing.UI.Services;

namespace ImageProcessing.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ImageConverterService _converterService = new();
    private PixelBuffer? _originalBuffer;

    [ObservableProperty]
    private WriteableBitmap? _displayedImage;

    [ObservableProperty]
    private string _statusMessage = "Готово до роботи";

    [ObservableProperty]
    private string _imageStats = "Статистика: немає";

    [ObservableProperty]
    private WriteableBitmap? _histogramImage;

    public void LoadImage(string filePath)
    {
        try
     {
         StatusMessage = "Завантаження...";
         _originalBuffer = _converterService.LoadFromFile(filePath);
         DisplayedImage = _converterService.ToWriteableBitmap(_originalBuffer);

         UpdateStatistics(_originalBuffer); // <--- Додаємо виклик нового методу

         StatusMessage = $"Завантажено: {Path.GetFileName(filePath)}";
     }
        catch (Exception ex)
        {
            StatusMessage = $"Помилка: {ex.Message}";
        }
    }

// Допоміжний метод: рахує статистику і малює графік для БУДЬ-ЯКОГО буфера, який ми йому дамо
    private void UpdateStatistics(PixelBuffer buffer)
    {
        // 1. Рахуємо гістограму
        int[] hist = ImageProcessing.Core.Analysis.HistogramCalculator.CalculateLuminance(buffer);
        
        // 2. Рахуємо середню яскравість
        long totalBrightness = 0;
        int totalPixels = buffer.Width * buffer.Height;
        
        for (int i = 0; i < 256; i++)
        {
            totalBrightness += (long)i * hist[i];
        }
        
        int averageBrightness = (int)(totalBrightness / totalPixels);
        
        // 3. Оновлюємо текст і графік на екрані
        ImageStats = $"Роздільна здатність: {buffer.Width}x{buffer.Height} | Середня яскравість: {averageBrightness}/255";
        
        PixelBuffer histBuffer = ImageProcessing.Core.Analysis.HistogramRenderer.Render(hist);
        HistogramImage = _converterService.ToWriteableBitmap(histBuffer);
    }
    // Метод для застосування чорно-білого фільтра
    public void ApplyGrayscale()
    {
        // Якщо користувач ще не завантажив картинку — нічого не робимо
        if (_originalBuffer == null) return;

        try
        {
            StatusMessage = "Обробка: Чорно-білий фільтр...";
            
            // Викликаємо нашу нову математику з Ядра
            PixelBuffer processedBuffer = ImageProcessing.Core.Filters.GrayscaleFilter.Apply(_originalBuffer);
            
            // Оновлюємо картинку на екрані (конвертуємо результат у формат для монітора)
            DisplayedImage = _converterService.ToWriteableBitmap(processedBuffer);
            UpdateStatistics(processedBuffer);
            StatusMessage = "Фільтр застосовано успішно!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Помилка фільтра: {ex.Message}";
        }
    }
    public void ApplyBlur()
    {
        if (_originalBuffer == null) return;

        try
        {
            StatusMessage = "Обробка: Розмиття (Згортка)...";
            
            // Створюємо матрицю для розмиття (Box Blur) розміром 3x3.
            // Щоб картинка не стала білою, сума всіх дробів має дорівнювати 1. (9 разів по 1/9).
            float[,] blurKernel = {
                { 1f/9f, 1f/9f, 1f/9f },
                { 1f/9f, 1f/9f, 1f/9f },
                { 1f/9f, 1f/9f, 1f/9f }
            };

            // Викликаємо наш новий універсальний движок!
            PixelBuffer processed = ImageProcessing.Core.Filters.ConvolutionEngine.Convolve(_originalBuffer, blurKernel);
            
            DisplayedImage = _converterService.ToWriteableBitmap(processed);
            UpdateStatistics(processed);
            StatusMessage = "Розмиття застосовано успішно!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Помилка: {ex.Message}";
        }
    }

    public void ApplyEdgeDetection()
    {
        if (_originalBuffer == null) return;

        try
        {
            StatusMessage = "Обробка: Виділення меж (Собель)...";
            
            // Матриця для виділення ГОРИЗОНТАЛЬНИХ контурів (різниця між верхніми і нижніми пікселями)
            // Зверни увагу: сума всіх чисел дорівнює 0!
            float[,] edgeKernel = {
                { -1f, -2f, -1f },
                {  0f,  0f,  0f },
                {  1f,  2f,  1f }
            };

            // Ми пропускаємо картинку через той самий движок згортки!
            PixelBuffer processed = ImageProcessing.Core.Filters.ConvolutionEngine.Convolve(_originalBuffer, edgeKernel);
            
            DisplayedImage = _converterService.ToWriteableBitmap(processed);
            UpdateStatistics(processed);
            StatusMessage = "Межі виділено успішно!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Помилка: {ex.Message}";
        }
    }
}