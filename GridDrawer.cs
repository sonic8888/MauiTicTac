using Microsoft.Maui.Graphics;


namespace MauiTicTac;

/// <summary>
/// Класс для отрисовки сетки и игровых элементов в игре "Крестики-нолики"
/// Основной функционал: рисование сетки, конвертация координат, анимация крестиков и ноликов
/// </summary>
public class GridDrawer : IDrawable
{
    /// <summary>
    /// Статический экземпляр класса GridDrawer для глобального доступа
    /// Используется для вызова методов рисования из других частей приложения
    /// </summary>
    public static readonly GridDrawer Instance = new();

    /// <summary>
    /// Количество строк в сетке игры
    /// </summary>
    private const int Rows = 20;

    /// <summary>
    /// Количество столбцов в сетке игры
    /// </summary>
    private const int Cols = 20;

    /// <summary>
    /// Ширина сетки в пикселях (800px)
    /// </summary>
    public const int GridWidth = 800;

    /// <summary>
    /// Высота сетки в пикселях (800px)
    /// </summary>
    public const int GridHeight = 800;

    /// <summary>
    /// Отрисовывает основную сетку игры 20x20
    /// Метод рисует вертикальные и горизонтальные линии, формирующие ячейки
    /// </summary>
    /// <param name="canvas">Контекст рисования для отрисовки графических элементов</param>
    /// <param name="dirtyRect">Прямоугольная область, которая нуждается в перерисовке</param>
    public virtual void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Colors.White;
        canvas.StrokeSize = 1;

        float cellWidth = GridWidth / Cols;
        float cellHeight = GridHeight / Rows;

        // Draw vertical lines
        for (int col = 0; col <= Cols; col++)
        {
            float x = col * cellWidth;
            canvas.DrawLine(x, 0, x, GridHeight);
        }

        // Draw horizontal lines
        for (int row = 0; row <= Rows; row++)
        {
            float y = row * cellHeight;
            canvas.DrawLine(0, y, GridWidth, y);
        }
    }

    /// <summary>
    /// Преобразует экранные координаты точки в индексы ячейки сетки
    /// Используется для определения, по какой ячейке был произведен клик
    /// </summary>
    /// <param name="point">Точка с координатами X и Y в пикселях</param>
    /// <returns>Кортеж с индексами строки и столбца ячейки (row, col)</returns>
    public (int row, int col) GetCellFromPoint(PointF point)
    {
        float cellWidth = GridWidth / Cols;
        float cellHeight = GridHeight / Rows;

        int col = (int)(point.X / cellWidth);
        int row = (int)(point.Y / cellHeight);

        // Clamp to valid range
        row = Math.Clamp(row, 0, Rows - 1);
        col = Math.Clamp(col, 0, Cols - 1);

        return (row, col);
    }

    /// <summary>
    /// Вычисляет центральную точку заданной ячейки сетки
    /// Используется для позиционирования игровых элементов (крестиков, ноликов) в центре ячейки
    /// </summary>
    /// <param name="row">Индекс строки ячейки (0-19)</param>
    /// <param name="col">Индекс столбца ячейки (0-19)</param>
    /// <returns>Точка с координатами центра ячейки</returns>
    public PointF GetCellCenter(int row, int col)
    {
        float cellWidth = GridWidth / Cols;
        float cellHeight = GridHeight / Rows;

        float centerX = (col + 0.5f) * cellWidth;
        float centerY = (row + 0.5f) * cellHeight;

        return new PointF(centerX, centerY);
    }

    /// <summary>
    /// Анимированная отрисовка крестика в заданной ячейке
    /// Рисует две пересекающиеся диагональные линии, формирующие букву X
    /// Анимация происходит постепенно: сначала одна диагональ, затем вторая
    /// </summary>
    /// <param name="canvas">Контекст рисования для отрисовки графических элементов</param>
    /// <param name="row">Индекс строки ячейки (0-19)</param>
    /// <param name="col">Индекс столбца ячейки (0-19)</param>
    /// <param name="progress">Прогресс анимации (0.0 - начало, 1.0 - завершение)</param>
    public void DrawCrossAnimation(ICanvas canvas, int row, int col, float progress)
    {
        if (progress <= 0) return;
        
        var center = GetCellCenter(row, col);
        float cellWidth = GridWidth / Cols;
        float cellHeight = GridHeight / Rows;
        float size = Math.Min(cellWidth, cellHeight) * 0.8f;
        float halfSize = size / 2;
        
        // Установка стиля линии
        canvas.StrokeColor = Colors.Red;
        canvas.StrokeSize = 8;
        canvas.StrokeLineCap = LineCap.Round;
        
        // Полные координаты для диагоналей
        float x1 = center.X - halfSize;
        float y1 = center.Y - halfSize;
        float x2 = center.X + halfSize;
        float y2 = center.Y + halfSize;
        
        // Первая диагональ: сверху слева - вниз справа
        float progress1 = Math.Min(1.0f, progress * 2.0f); // Завершается за первую половину анимации
        float endX1 = x1 + (x2 - x1) * progress1;
        float endY1 = y1 + (y2 - y1) * progress1;
        canvas.DrawLine(x1, y1, endX1, endY1);
        
        // Вторая диагональ: сверху справа - вниз слева
        float progress2 = Math.Max(0.0f, (progress - 0.5f) * 2.0f); // Начинается со второй половины анимации
        float startX2 = center.X + halfSize;
        float startY2 = center.Y - halfSize;
        float endX2 = center.X - halfSize;
        float endY2 = center.Y + halfSize;
        float currentX2 = startX2 - (startX2 - endX2) * progress2;
        float currentY2 = startY2 + (endY2 - startY2) * progress2;
        canvas.DrawLine(startX2, startY2, currentX2, currentY2);
    }

    /// <summary>
    /// Анимированная отрисовка нолика в заданной ячейке
    /// Рисует окружность постепенно, создавая эффект рисования по часовой стрелке
    /// Используется для визуализации хода игрока, ставящего нолик
    /// </summary>
    /// <param name="canvas">Контекст рисования для отрисовки графических элементов</param>
    /// <param name="row">Индекс строки ячейки (0-19)</param>
    /// <param name="col">Индекс столбца ячейки (0-19)</param>
    /// <param name="progress">Прогресс анимации (0.0 - начало, 1.0 - завершение)</param>
    public void DrawCircleAnimation(ICanvas canvas, int row, int col, float progress)
    {
        if (progress <= 0) return;
        
        var center = GetCellCenter(row, col);
        float cellWidth = GridWidth / Cols;
        float cellHeight = GridHeight / Rows;
        float radius = Math.Min(cellWidth, cellHeight) * 0.4f;
        
        // Установка стиля линии
        canvas.StrokeColor = Colors.Blue;
        canvas.StrokeSize = 8;
        canvas.StrokeLineCap = LineCap.Round;
        
        // Рисуем дугу, начиная с 0 и заканчивая в progress * 2π
        // Добавляем небольшой начальный угол для визуального эффекта
        float startAngle = 0;
        float endAngle = (float)(Math.PI * 2 * progress);
        
        // Для лучшего визуального эффекта делаем разрыв в начале
        if (progress > 0)
        {
            canvas.DrawArc(
                center.X - radius, center.Y - radius,
                center.X + radius, center.Y + radius,
                startAngle, endAngle, false, false);
        }
    }
}
