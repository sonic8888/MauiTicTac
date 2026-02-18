using Microsoft.Maui.Graphics;


namespace MauiTicTac;

public class GridDrawer : IDrawable
{
    public static readonly GridDrawer Instance = new();

    private const int Rows = 20;
    private const int Cols = 20;
    public const int GridWidth = 800;
    public const int GridHeight = 800;

    public void Draw(ICanvas canvas, RectF dirtyRect)
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
}
