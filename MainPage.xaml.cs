using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiTicTac;

public partial class MainPage : ContentPage
{
    private const int Rows = 20;
    private const int Cols = 20;
    private static readonly AnimatedGridDrawer animatedDrawer = new();
    private static AnimatedGridDrawer AnimatedDrawer => animatedDrawer;

    public MainPage()
    {
        InitializeComponent();
    }

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

    // Класс для отрисовки анимированных элементов
    private class AnimatedGridDrawer : GridDrawer
    {
        private readonly Dictionary<(int row, int col), float> crossProgress = new();

        public void AddCross(int row, int col)
        {
            crossProgress[(row, col)] = 0f;
        }

        public void SetCrossProgress(int row, int col, float progress)
        {
            crossProgress[(row, col)] = progress;
        }

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
