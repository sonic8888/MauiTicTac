using Microsoft.Maui.Graphics;


namespace MauiTicTac;

public class GridDrawer : IDrawable
{
    public static readonly GridDrawer Instance = new();

    private const int Rows = 20;
    private const int Cols = 20;
    public const int GridWidth = 800;
    public const int GridHeight = 800;

    public virtual void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Colors.Black;
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

    public PointF GetCellCenter(int row, int col)
    {
        float cellWidth = GridWidth / Cols;
        float cellHeight = GridHeight / Rows;

        float centerX = (col + 0.5f) * cellWidth;
        float centerY = (row + 0.5f) * cellHeight;

        return new PointF(centerX, centerY);
    }

    public void DrawCrossAnimation(ICanvas canvas, int row, int col, float progress)
    {
        // Анимация крестика - постепенное появление двух линий
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
        
        // Первая диагональ (слева сверху - направо вниз)
        float diag1End = halfSize * progress;
        canvas.DrawLine(
            center.X - halfSize, center.Y - halfSize,
            center.X - halfSize + diag1End, center.Y - halfSize + diag1End);
        
        // Вторая диагональ (справа сверху - налево вниз)
        // Запускается после первой, создавая эффект последовательного рисования
        if (progress > 0.5f)
        {
            float adjustedProgress = (progress - 0.5f) * 2; // Нормализуем прогресс для второй линии
            float diag2End = halfSize * adjustedProgress;
            canvas.DrawLine(
                center.X + halfSize, center.Y - halfSize,
                center.X + halfSize - diag2End, center.Y - halfSize + diag2End);
        }
    }

    public void DrawCircleAnimation(ICanvas canvas, int row, int col, float progress)
    {
        // Анимация нолика - постепенное рисование окружности
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
