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
    public static PointF GetCellCenter(int row, int col)
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
        canvas.StrokeColor = Colors.YellowGreen;
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
    /// Анимированная отрисовка нолика в виде овала (эллипса) в заданной ячейке.
    /// Анимация начинается сверху и развивается по часовой стрелке.
    /// Размеры овала соответствуют размерам ячейки с небольшими отступами.
    /// </summary>
    /// <param name="canvas">Контекст рисования</param>
    /// <param name="row">Индекс строки ячейки</param>
    /// <param name="col">Индекс столбца ячейки</param>
    /// <param name="progress">Прогресс анимации (0.0 - начало, 1.0 - завершение)</param>
    public void DrawCircleAnimation(ICanvas canvas, int row, int col, float progress)
    {
        // Ограничиваем прогресс в диапазоне [0, 1]
        progress = Math.Clamp(progress, 0f, 0.99f);

        // Размеры ячейки
        float cellWidth = GridWidth / Cols;
        float cellHeight = GridHeight / Rows;

        // Центр ячейки
        var center = GetCellCenter(row, col);

        // Отступы: чтобы овал не прижимался к краям
        float paddingX = cellWidth * 0.2f;  // 10% слева/справа
        float paddingY = cellHeight * 0.1f; // 10% сверху/снизу

        // Ширина и высота овала
        float ovalWidth = cellWidth - 2 * paddingX;
        float ovalHeight = cellHeight - 2 * paddingY;

        // Параметры линии
        float strokeWidth = 8;

        // Настройка стиля
        canvas.StrokeColor = Colors.Blue;
        canvas.StrokeSize = strokeWidth;
        canvas.StrokeLineCap = LineCap.Round; // Плавные концы — ключ к плавности

        // Рисуем только если прогресс > 0
        if (progress > 0)
        {
            // Прямоугольник, в который вписан овал
            float x = center.X - ovalWidth / 2;
            float y = center.Y - ovalHeight / 2;


            float startAngle = 90f;
            // Конечный угол: зависит от прогресса
            float endAngle = startAngle + 360 * progress;

            // Рисуем дугу по часовой стрелке, без замыкания);

            // Рисуем дугу по часовой стрелке, без замыкания
            canvas.DrawArc(
                x: x,
                y: y,
                width: ovalWidth,
                height: ovalHeight,
                startAngle: startAngle,
                endAngle: endAngle,
                clockwise: false,
                closed: false      // не замыкаем в центр
            );
        }
    }

    /// <summary>
/// Анимированная отрисовка линии от начальной до конечной точки.
/// Линия появляется постепенно, согласно прогрессу анимации.
/// </summary>
/// <param name="canvas">Контекст рисования</param>
/// <param name="startPoint">Начальная точка линии</param>
/// <param name="endPoint">Конечная точка линии</param>
/// <param name="progress">Прогресс анимации (0.0 - не видно, 1.0 - полностью нарисовано)</param>
public void DrawLineAnimation(ICanvas canvas, PointF startPoint, PointF endPoint, float progress)
{
    // Ограничиваем прогресс диапазоном [0, 1]
    progress = Math.Clamp(progress, 0f, 1f);
    
    // Прекращаем отрисовку, если анимация не началась
    if (progress == 0) return;

    // Настраиваем стиль линии
    canvas.StrokeColor = Colors.Red;
    canvas.StrokeSize = 8;
    canvas.StrokeLineCap = LineCap.Round; // Плавные концы линии

    // Вычисляем текущую конечную точку в зависимости от прогресса
    float currentX = startPoint.X + (endPoint.X - startPoint.X) * progress;
    float currentY = startPoint.Y + (endPoint.Y - startPoint.Y) * progress;

    // Рисуем линию от начальной точки до промежуточной (зависит от progress)
    canvas.DrawLine(startPoint.X, startPoint.Y, currentX, currentY);
}

}
