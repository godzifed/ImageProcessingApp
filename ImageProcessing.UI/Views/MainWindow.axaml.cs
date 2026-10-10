using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ImageProcessing.UI.ViewModels;

namespace ImageProcessing.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Цей метод викликається, коли ми натискаємо кнопку "Відкрити фото..."
    private async void OnOpenFileClicked(object? sender, RoutedEventArgs e)
    {
        // Перевіряємо, чи підключений наш "мізок" (MainViewModel)
        if (DataContext is not MainViewModel vm) return;

        // Відкриваємо стандартне системне вікно Windows/Linux/Mac для вибору файлу
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Оберіть зображення",
            AllowMultiple = false, // Можна вибрати лише одну картинку
            FileTypeFilter = new[]
            {
                // Фільтруємо, щоб показувало лише картинки
                new FilePickerFileType("Зображення") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" } }
            }
        });

        // Якщо користувач вибрав файл (а не натиснув "Скасувати")
        if (files.Count > 0)
        {
            // Беремо шлях до файлу (наприклад, C:\Фото\Кіт.jpg) і відправляємо в наш "мізок"
            vm.LoadImage(files[0].Path.LocalPath);
        }
    }

    private void OnGrayscaleClicked(object? sender, RoutedEventArgs e)
    {
        // Якщо "мізок" вікна підключено, просимо його запустити фільтр
        if (DataContext is MainViewModel vm)
        {
            vm.ApplyGrayscale();
        }
    }

    private void OnBlurClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.ApplyBlur();
        }
    }

    private void OnEdgeClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.ApplyEdgeDetection();
        }
    }
}