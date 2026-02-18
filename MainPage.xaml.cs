using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiTicTac;

/// <summary>
/// Основной класс страницы приложения для игры "Крестики-нолики"
/// Управляет пользовательским интерфейсом, обработкой событий и анимациями
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary>
    /// Количество строк в сетке игры
    /// </summary>
    private const int Rows = 20;

    /// <summary>
    /// Количество столбцов в сетке игры
    /// </summary>
    private const int Cols = 20;

    /// <summary>
    /// Статический экземпляр анимированного drawer для отрисовки игровых элементов
    /// Управляет отрисовкой анимированных крестиков
    /// </summary>
    private static readonly AnimatedGridDrawer animatedDrawer = new();

    /// <summary>
    /// Свойство для доступа к анимированному drawer
    /// Используется для привязки в XAML
    /// </summary>
    private static AnimatedGridDrawer AnimatedDrawer => animatedDrawer;

    /// <summary>
    /// Конструктор класса MainPage
    /// Инициализирует компоненты пользовательского интерфейса
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Обработчик события нажатия на сетку игры
    /// Вызывается при клике пользователя по ячейке сетки
    /// Преобразует координаты клика в индексы ячейки и запускает анимацию крестика
    /// </summary>
    /// <param name="sender">Объект, отправивший событие (GraphicsView)</param>
    /// <param name="e">Аргументы события нажатия, содержащие координаты клика</param>
    private async void OnGridTapped(object sender, TappedEventArgs e)
    {
        if (sender is GraphicsView graphicsView)
        {
            // Get the tap position
            var tapPoint = e.GetPosition(graphicsView);
            
            if (tapPoint != null)
            {
                // Use GridDrawer to convert point to cell
                var (row, col) = GridDrawer.Instance.GetCellFromPoint(tapPoint.Value);
                
                // Calculate cell index (0-399)
                int cellIndex = row * Cols + col;
                
                // Show message with cell number
                // await DisplayAlert("Cell Clicked", $"You clicked cell {cellIndex} (Row: {row}, Col: {col})", "OK");

                // Добавляем крестик для анимации
                animatedDrawer.AddCross(row, col);
                
                // Запускаем анимацию
                await AnimateCross(graphicsView, row, col);
            }
        }
    }

    /// <summary>
    /// Анимирует процесс рисования крестика в указанной ячейке
    /// Разбивает анимацию на несколько шагов для плавного визуального эффекта
    /// </summary>
    /// <param name="graphicsView">Элемент GraphicsView, который нужно обновить</param>
    /// <param name="row">Индекс строки ячейки, где рисуется крестик</param>
    /// <param name="col">Индекс столбца ячейки, где рисуется крестик</param>
    private async Task AnimateCross(GraphicsView graphicsView, int row, int col)
    {
        const int animationSteps = 30;
        const int delayMs = 30;
        
        for (int i = 1; i <= animationSteps; i++)
        {
            float progress = (float)i / animationSteps;
            animatedDrawer.SetCrossProgress(row, col, progress);
            
            // Обновляем GraphicsView
            graphicsView.Invalidate();
            
            // Задержка для плавности анимации
            await Task.Delay(delayMs);
        }
    }

    /// <summary>
    /// Внутренний класс для отрисовки анимированных игровых элементов
    /// Наследуется от GridDrawer и добавляет функциональность анимации крестиков
    /// Управляет прогрессом анимации для каждого крестика на игровом поле
    /// </summary>
    private class AnimatedGridDrawer : GridDrawer
    {
        /// <summary>
        /// Словарь для хранения прогресса анимации для каждого крестика
        /// Ключ: (row, col) - координаты ячейки
        /// Значение: прогресс анимации (0.0 - начало, 1.0 - завершение)
        /// </summary>
        private readonly Dictionary<(int row, int col), float> crossProgress = new();

        /// <summary>
        /// Добавляет новую ячейку для анимации крестика
        /// Инициализирует прогресс анимации с нуля
        /// </summary>
        /// <param name="row">Индекс строки ячейки</param>
        /// <param name="col">Индекс столбца ячейки</param>
        public void AddCross(int row, int col)
        {
            crossProgress[(row, col)] = 0f;
        }

        /// <summary>
        /// Устанавливает текущий прогресс анимации для крестика в указанной ячейке
        /// Используется для анимации процесса рисования крестика
        /// </summary>
        /// <param name="row">Индекс строки ячейки</param>
        /// <param name="col">Индекс столбца ячейки</param>
        /// <param name="progress">Текущий прогресс анимации (0.0 - 1.0)</param>
        public void SetCrossProgress(int row, int col, float progress)
        {
            crossProgress[(row, col)] = progress;
        }

        /// <summary>
        /// Переопределенный метод отрисовки
        /// Сначала рисует основную сетку, затем все анимированные крестики
        /// </summary>
        /// <param name="canvas">Контекст рисования</param>
        /// <param name="dirtyRect">Область, требующая перерисовки</param>
        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Сначала рисуем основную сетку
            base.Draw(canvas, dirtyRect);
            
            // Затем рисуем все анимированные крестики
            foreach (var kvp in crossProgress)
            {
                var (row, col) = kvp.Key;
                float progress = kvp.Value;
                DrawCrossAnimation(canvas, row, col, progress);
            }
        }
    }
}
