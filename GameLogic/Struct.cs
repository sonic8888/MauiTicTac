using System;

namespace MauiTicTac;


public readonly struct Cell : IComparable<Cell>
{
    /// <summary>
    /// Представляет ячейку игрового поля с координатами, оценкой и символом владельца.
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Индекс строки ячейки.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Оценка значимости ячейки (используется для определения приоритета хода).
    /// </summary>
    public int Value { get; }

    public int ValueX { get; }
    public int ValueO { get; }

    /// <summary>
    /// Символ владельца ячейки ("X" или "O").
    /// </summary>
    public string TicTac { get; }

    /// <summary>
    /// Создаёт новую ячейку с указанными параметрами.
    /// </summary>
    /// <param name="row">Индекс строки. Должен быть неотрицательным.</param>
    /// <param name="column">Индекс столбца. Должен быть неотрицательным.</param>
    /// <param name="value">Оценка значимости ячейки.</param>
    /// <param name="ticTac">Символ владельца ("X" или "O").</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Вызывается, если <paramref name="row"/> или <paramref name="column"/> отрицательны.
    /// </exception>
    public Cell(int row, int column, int value, string ticTac, int valueX, int valueO)
    {
        if (row < 0) throw new ArgumentOutOfRangeException(nameof(row), "Индекс строки не может быть отрицательным.");
        if (column < 0) throw new ArgumentOutOfRangeException(nameof(column), "Индекс столбца не может быть отрицательным.");

        Row = row;
        Column = column;
        Value = value;
        ValueX = valueX;
        ValueO = valueO;
        TicTac = ticTac ?? string.Empty;
    }

    /// <summary>
    /// Создаёт новую ячейку с указанными координатами и символом владельца.
    /// </summary>
    /// <param name="row">Индекс строки. Должен быть неотрицательным.</param>
    /// <param name="column">Индекс столбца. Должен быть неотрицательным.</param>
    /// <param name="ticTac">Символ владельца ("X" или "O").</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Вызывается, если <paramref name="row"/> или <paramref name="column"/> отрицательны.
    /// </exception>
    public Cell(int row, int column, string ticTac) : this(row, column, 0, ticTac, 0, 0) { }

    /// <summary>
    /// Создаёт новую ячейку с указанными координатами и оценкой.
    /// </summary>
    /// <param name="row">Индекс строки. Должен быть неотрицательным.</param>
    /// <param name="column">Индекс столбца. Должен быть неотрицательным.</param>
    /// <param name="value">Оценка значимости ячейки.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Вызывается, если <paramref name="row"/> или <paramref name="column"/> отрицательны.
    /// </exception>
    public Cell(int row, int column, int value) : this(row, column, value, string.Empty, 0, 0) { }

    /// <summary>
    /// Сравнивает текущую ячейку с другой по полю <see cref="Value"/>.
    /// </summary>
    /// <param name="other">Ячейка для сравнения.</param>
    /// <returns>
    /// Значение, указывающее относительное расположение текущей ячейки по отношению к <paramref name="other"/>:
    /// меньше нуля — текущая ячейка меньше; ноль — равны; больше нуля — текущая ячейка больше.
    /// </returns>
    public int CompareTo(Cell other) => Value.CompareTo(other.Value);

    /// <summary>
    /// Сравнивает ячейку с объектом.
    /// </summary>
    /// <param name="obj">Объект для сравнения.</param>
    /// <returns>
    /// Значение, указывающее относительное расположение текущей ячейки по отношению к <paramref name="obj"/>:
    /// меньше нуля — текущая ячейка меньше; ноль — равны; больше нуля — текущая ячейка больше.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Вызывается, если <paramref name="obj"/> не является типом <see cref="Cell"/>..
    /// </exception>
    public int CompareTo(object? obj)
    {
        if (obj is Cell cell) return CompareTo(cell);
        throw new ArgumentException("Объект должен быть типа Cell", nameof(obj));
    }

    /// <summary>
    /// Возвращает строковое представление ячейки.
    /// </summary>
    /// <returns>Строка формата "(row, column) = value".</returns>
    public override string ToString() => $"({Row}, {Column}) = {Value}";

}






/// <summary>
/// Представляет ячейку игрового поля с позицией и владельцем.
/// </summary>
public readonly struct BoardCell : IEquatable<BoardCell>
{
    public int Row { get; }
    public int Column { get; }
    public string Player { get; }

    /// <summary>
    /// Создаёт новую ячейку игрового поля.
    /// </summary>
    /// <param name="row">Индекс строки (неотрицательный).</param>
    /// <param name="column">Индекс столбца (неотрицательный).</param>
    /// <param name="Player">Владелец ячейки: X, O или " ".</param>
    /// <exception cref="ArgumentOutOfRangeException">Если row или column отрицательны.</exception>
    public BoardCell(int row, int column, string player)
    {
        if (row < 0) throw new ArgumentOutOfRangeException(nameof(row));
        if (column < 0) throw new ArgumentOutOfRangeException(nameof(column));
        if (player != "X" && player != "O" && player != " ") throw new ArgumentException("Player must be 'X', 'O' or empty string", nameof(player));
        Row = row;
        Column = column;
        Player = player;
    }



    public override string ToString() => $"({Row}, {Column}) = {Player}";
    public bool Equals(BoardCell other) => Row == other.Row && Column == other.Column && Player == other.Player;
    public override bool Equals(object? obj) => obj is BoardCell cell && Equals(cell);
    public override int GetHashCode() => HashCode.Combine(Row, Column, Player);
}

/// <summary>
/// Представляет оценку ячейки для алгоритмов принятия решений (например, ИИ).
/// </summary>
public readonly struct EvaluationCell : IComparable<EvaluationCell>
{
    public int Row { get; }
    public int Column { get; }
    public double Value { get; }  // Общая оценка приоритета хода
    public double ValueX { get; } // Оценка значимости для игрока X
    public double ValueO { get; } // Оценка значимости для игрока O

    public double MaxX { get; }// длинна собираемой комбинации X
    public double MaxO { get; } // длинна собираемой комбинации O
    public bool IsEmptyEnds { get; }// true если собираемая комбинация имеет пустые концы

    /// <summary>
    /// Создаёт новую оценочную ячейку.
    /// </summary>
    /// <param name="row">Индекс строки (неотрицательный).</param>
    /// <param name="column">Индекс столбца (неотрицательный).</param>
    /// <param name="value">Общая оценка приоритета хода.</param>
    /// <param name="valueX">Оценка для X.</param>
    /// <param name="valueO">Оценка для O.</param>
    /// <exception cref="ArgumentOutOfRangeException">Если row или column отрицательны.</exception>
    public EvaluationCell(int row, int column, double value = 0, double valueX = 0, double valueO = 0, double maxX = 0, double maxO = 0, bool isEmptyEnds = false)
    {
        if (row < 0) throw new ArgumentOutOfRangeException(nameof(row));
        if (column < 0) throw new ArgumentOutOfRangeException(nameof(column));
        if (row < 0) throw new ArgumentOutOfRangeException(nameof(row));
        if (column < 0) throw new ArgumentOutOfRangeException(nameof(column));

        Row = row;
        Column = column;
        Value = value;
        ValueX = valueX;
        ValueO = valueO;
        MaxX = maxX;
        MaxO = maxO;
        IsEmptyEnds = isEmptyEnds;

    }

    /// <summary>
    /// Сравнивает по общей оценке <see cref="Value"/> (для сортировки по приоритету).
    /// </summary>
    public int CompareTo(EvaluationCell other) => Value.CompareTo(other.Value);

    public override string ToString() => $"({Row}, {Column}) | Value={Value}, X={ValueX}, O={ValueO}, maxX={MaxX}, maxO={MaxO}";
}

// public readonly struct EvaluationCell
// {
//     public int Row { get; }
//     public int Column { get; }
//     public int VerticalLineResult { get; }
//     public bool VerticalLineIsEmpty { get; }

//     public int HorizontalLineResult { get; }
//     public bool HorizontalLineIsEmpty { get; }

//     public int LeftDiagonalLineResult { get; }
//     public bool LeftDiagonalLineIsEmpty { get; }

//     public int RightDiagonalLineResult { get; }
//     public bool RightDiagonalLineIsEmpty { get; }

//     public int MaxLineResult { get; }
//     public bool MaxLineResultEmptyFlag { get; }

//     public int TotalResult { get; }


//     public EvaluationCell(int row, int column, int verticalLineResult, bool verticalLineIsEmpty, int horizontalLineResult, bool horizontalLineIsEmpty, int leftDiagonalLineResult, bool leftDiagonalLineIsEmpty, int rightDiagonalLineResult, bool rightDiagonalLineIsEmpty)
//     {
//         Row = row;
//         Column = column;
//         VerticalLineResult = verticalLineResult;
//         VerticalLineIsEmpty = verticalLineIsEmpty;
//         HorizontalLineResult = horizontalLineResult;
//         HorizontalLineIsEmpty = horizontalLineIsEmpty;
//         LeftDiagonalLineResult = leftDiagonalLineResult;
//         LeftDiagonalLineIsEmpty = leftDiagonalLineIsEmpty;
//         RightDiagonalLineResult = rightDiagonalLineResult;
//         RightDiagonalLineIsEmpty = rightDiagonalLineIsEmpty;
//         TotalResult = VerticalLineResult + HorizontalLineResult + LeftDiagonalLineResult + RightDiagonalLineResult;
//         (MaxLineResult, MaxLineResultEmptyFlag) = GetMaxLineResultWithEmptyFlag();
//     }

//     public readonly (int maxResult, bool isEmpty) GetMaxLineResultWithEmptyFlag()
//     {
//         int maxResult = VerticalLineResult;
//         bool isEmpty = VerticalLineIsEmpty;

//         if (HorizontalLineResult > maxResult)
//         {
//             maxResult = HorizontalLineResult;
//             isEmpty = HorizontalLineIsEmpty;
//         }

//         if (LeftDiagonalLineResult > maxResult)
//         {
//             maxResult = LeftDiagonalLineResult;
//             isEmpty = LeftDiagonalLineIsEmpty;
//         }

//         if (RightDiagonalLineResult > maxResult)
//         {
//             maxResult = RightDiagonalLineResult;
//             isEmpty = RightDiagonalLineIsEmpty;
//         }

//         return (maxResult, isEmpty);
//     }

//     public override string ToString()
//     {
//         return $"Row:{Row}, Column:{Column}, TotalResult:{TotalResult}, VerticalLineResult:{VerticalLineResult}, HorizontalLineResult:{HorizontalLineResult}, LeftDiagonalLineResult:{LeftDiagonalLineResult}, RightDiagonalLineResult:{RightDiagonalLineResult} MaxLineResult:{MaxLineResult}, MaxLineResultEmptyFlag:{MaxLineResultEmptyFlag}";
//     }
// }


// Context snippet 1 from d:\ProjectsC\TicTacToe\GameLogic.cs









