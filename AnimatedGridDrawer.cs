using Microsoft.Maui.Graphics;
using System.Collections.Generic;

namespace MauiTicTac;

/// <summary>
/// Анимированный drawer, наследующий функциональность GridDrawer
/// Отображает плавную анимацию крестиков и ноликов при ходе игрока
/// </summary>
public class AnimatedGridDrawer : GridDrawer
{
    /// <summary>
    /// Перечисление игровых символов
    /// </summary>
    public enum Symbol
    {
        Cross,
        Circle
    }

    /// <summary>
    /// Хранит тип символа и прогресс анимации для каждой ячейки
    /// Ключ: (row, col), Значение: (symbol, progress)
    /// </summary>
    private readonly Dictionary<(int row, int col), (Symbol symbol, float progress)> animations = new();

    /// <summary>
    /// Добавляет новый символ в указанную ячейку в зависимости от текущего игрока
    /// </summary>
    /// <param name="row">Индекс строки</param>
    /// <param name="col">Индекс столбца</param>
    public void AddSymbol(int row, int col)
    {
        var symbol = MainPage.currentPlayer == MainPage.Player.X ? Symbol.Cross : Symbol.Circle;
        animations[(row, col)] = (symbol, 0f);
    }

    /// <summary>
    /// Устанавливает прогресс анимации для указанной ячейки
    /// </summary>
    /// <param name="row">Строка</param>
    /// <param name="col">Столбец</param>
    /// <param name="progress">Прогресс анимации (0.0 - 1.0)</param>
    public void SetProgress(int row, int col, float progress)
    {
        if (animations.TryGetValue((row, col), out var value))
        {
            animations[(row, col)] = (value.symbol, progress);
        }
    }

    /// <summary>
    /// Переопределённый метод отрисовки
    /// Сначала рисуем сетку, затем все анимированные символы
    /// </summary>
    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Рисуем основную сетку (из базового класса)
        base.Draw(canvas, dirtyRect);

        // Рисуем все анимированные символы
        foreach (var kvp in animations)
        {
            var (row, col) = kvp.Key;
            var (symbol, progress) = kvp.Value;

            if (progress > 0)
            {
                if (symbol == Symbol.Cross)
                {
                    DrawCrossAnimation(canvas, row, col, progress);
                }
                else
                {
                    DrawCircleAnimation(canvas, row, col, progress);
                }
            }
        }
    }
}