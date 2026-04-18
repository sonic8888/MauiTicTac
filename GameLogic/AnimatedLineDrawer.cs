using Microsoft.Maui.Graphics;

namespace MauiTicTac;

/// <summary>
/// Класс для отрисовки анимированной линии между двумя точками
/// Наследуется от GridDrawer, чтобы сохранить функциональность сетки и других элементов
/// </summary>
public class AnimatedLineDrawer : GridDrawer
{
    // Точки начала и конца линии
    public PointF? StartPoint { get; private set; }
    public PointF? EndPoint { get; private set; }

    // Прогресс анимации (0.0 - 1.0)
    public float AnimationProgress { get; private set; } = 0f;

    // Флаг: идёт ли анимация
    public bool IsAnimating => AnimationProgress < 1f;

    /// <summary>
    /// Запускает анимацию линии между двумя точками
    /// </summary>
    /// <param name="start">Начальная точка</param>
    /// <param name="end">Конечная точка</param>
    public void StartLineAnimation(PointF start, PointF end)
    {
        StartPoint = start;
        EndPoint = end;
        AnimationProgress = 0f;
    }

    /// <summary>
    /// Обновляет прогресс анимации (вызывается, например, через DispatcherTimer)
    /// </summary>
    /// <param name="deltaTime">Время в секундах с последнего кадра (например, 0.016 для 60 FPS)</param>
    /// <returns>True, если анимация продолжается</returns>
    public bool UpdateAnimation(float deltaTime)
    {
        if (!IsAnimating || !StartPoint.HasValue || !EndPoint.HasValue)
            return false;

        // Скорость анимации: например, за 1 секунду
        const float duration = 1.0f;
        AnimationProgress += deltaTime / duration;
        AnimationProgress = Math.Clamp(AnimationProgress, 0f, 1f);

        return IsAnimating;
    }

    /// <summary>
    /// Отрисовка: рисуем сетку + анимированную линию
    /// </summary>
    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Рисуем базовую сетку (родительский метод)
        base.Draw(canvas, dirtyRect);

        // Рисуем анимированную линию, если она активна
        if (StartPoint.HasValue && EndPoint.HasValue && AnimationProgress > 0)
        {
            DrawLineAnimation(canvas, StartPoint.Value, EndPoint.Value, AnimationProgress);
        }
    }
}