using Microsoft.Maui.Graphics;
using System.Collections.Generic;

namespace MauiTicTac;

/// <summary>
/// Единый Drawable для всей игры: сетка, X/O, победная линия
/// </summary>
public class AnimatedLineDrawer : IDrawable
{
    private readonly Dictionary<(int row, int col), string> _symbols = new();
    private readonly Dictionary<(int row, int col), float> _progress = new();

    // Анимация победной линии
    public PointF? WinLineStart { get; private set; }
    public PointF? WinLineEnd { get; private set; }
    public float WinLineProgress { get; private set; } = 0f;
    public bool IsWinLineAnimating => WinLineProgress < 1f;

    /// <summary>
    /// Добавляет символ в ячейку и начинает анимацию
    /// </summary>
    public void AddSymbol(int row, int col, string player)
    {
        _symbols[(row, col)] = player;
        _progress[(row, col)] = 0f;
    }

    /// <summary>
    /// Обновляет прогресс анимации символа
    /// </summary>
    public void SetProgress(int row, int col, float progress)
    {
        if (_symbols.ContainsKey((row, col)))
            _progress[(row, col)] = progress;
    }

    /// <summary>
    /// Запускает анимацию победной линии
    /// </summary>
    public void StartWinLineAnimation(PointF start, PointF end)
    {
        WinLineStart = start;
        WinLineEnd = end;
        WinLineProgress = 0f;
    }

    /// <summary>
    /// Обновляет прогресс анимации линии
    /// </summary>
    /// <param name="deltaTime">Время с последнего кадра</param>
    /// <returns>True, если анимация продолжается</returns>
    public bool UpdateWinLine(float deltaTime)
    {
        if (!IsWinLineAnimating) return false;

        const float duration = 1.0f;
        WinLineProgress += deltaTime / duration;
        WinLineProgress = Math.Clamp(WinLineProgress, 0f, 1f);

        return IsWinLineAnimating;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // 1. Рисуем сетку
        DrawGrid(canvas);

        // 2. Рисуем символы (X и O)
        DrawSymbols(canvas);

        // 3. Рисуем победную линию поверх
        DrawWinLine(canvas);
    }

    private void DrawGrid(ICanvas canvas)
    {
        canvas.StrokeColor = Colors.White;
        canvas.StrokeSize = 1;

        float cellWidth = GridDrawer.GridWidth / 20f;
        float cellHeight = GridDrawer.GridHeight / 20f;

        for (int col = 0; col <= 20; col++)
        {
            float x = col * cellWidth;
            canvas.DrawLine(x, 0, x, GridDrawer.GridHeight);
        }
        for (int row = 0; row <= 20; row++)
        {
            float y = row * cellHeight;
            canvas.DrawLine(0, y, GridDrawer.GridWidth, y);
        }
    }

    private void DrawSymbols(ICanvas canvas)
    {
        foreach (var kvp in _symbols)
        {
            var (row, col) = kvp.Key;
            string player = kvp.Value;
            float progress = _progress.GetValueOrDefault((row, col), 1f);

            if (player == "X")
            {
                GridDrawer.Instance.DrawCrossAnimation(canvas, row, col, progress);
            }
            else if (player == "O")
            {
                GridDrawer.Instance.DrawCircleAnimation(canvas, row, col, progress);
            }
        }
    }

    private void DrawWinLine(ICanvas canvas)
    {
        if (!WinLineStart.HasValue || !WinLineEnd.HasValue || WinLineProgress <= 0)
            return;

        canvas.SaveState();
        canvas.StrokeColor = Colors.Red;
        canvas.StrokeSize = 8;
        canvas.StrokeLineCap = LineCap.Round;

        var start = WinLineStart.Value;
        var end = WinLineEnd.Value;
        var currentX = start.X + (end.X - start.X) * WinLineProgress;
        var currentY = start.Y + (end.Y - start.Y) * WinLineProgress;

        canvas.DrawLine(start.X, start.Y, currentX, currentY);

        canvas.RestoreState();
    }
}