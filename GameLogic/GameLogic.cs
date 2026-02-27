using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;

namespace MauiTicTac;

public enum Tactics
{
    Attack,
    Defense
}


/// <summary>
/// Класс, реализующий логику оценки игрового поля в игре "Крестики-нолики".
/// Содержит методы для анализа линий (вертикальных, горизонтальных, диагональных)
/// с целью определения последовательных совпадений символов игрока.
/// </summary>
public class GameLogic
{
    public static Tactics CurrentTactics = Tactics.Attack;
    public static int countWinner = 5;
    public static string O = "O";
    public static string X = "X";
    public static string Empty = " ";

    public delegate void WinnerMessage(BoardCell[] line, string winner, int winnerLostIndex);
    event WinnerMessage? Notify;
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="GameLogic"/>.
    /// </summary>
    public GameLogic()
    {
        Notify += GameLogic.Message;

    }
    /// <summary>
    /// Вычисляет оценку следующего хода для указанной позиции на игровой доске.
    /// Оценка основывается на анализе вертикальных, горизонтальных и диагональных линий
    /// для обоих игроков ("X" и "O"), чтобы определить стратегическую значимость ячейки.
    /// Чем выше значение оценки, тем более приоритетной является ячейка для хода.
    /// </summary>
    /// <param name="board">Двумерный массив, представляющий текущее состояние игровой доски. Не должен быть значением <c>null</c>.</param>
    /// <param name="row">Индекс строки ячейки, для которой вычисляется оценка. Должен быть в пределах [0, GetLength(0)).</param>
    /// <param name="column">Индекс столбца ячейки, для которой вычисляется оценка. Должен быть в пределах [0, GetLength(1)).</param>
    /// <returns>
    /// Новый экземпляр класса <see cref="Cell"/>, представляющий ячейку с координатами <paramref name="row"/>, <paramref name="column"/>
    /// и вычисленным значением оценки <c>result</c>, суммирующим силу линий для обоих игроков.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Метод выполняет следующие шаги:
    /// <list type="number">
    ///   <item><description>Оценивает линии для игрока "X" в четырёх направлениях: вертикаль, горизонталь, левая диагональ, правая диагональ.</description></item>
    ///   <item><description>Добавляет результаты к общей оценке <c>result</c>.</description></item>
    ///   <item><description>Повторяет оценку для игрока "O".</description></item>
    ///   <item><description>Создаёт и возвращает объект <see cref="Cell"/> с суммарной оценкой.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Таким образом, оценка учитывает как возможность сделать выигрышный ход ("X"),
    /// так и необходимость блокировать ход противника ("O"), что делает метод полезным
    /// для алгоритмов принятия решений (например, в AI для игры).
    /// </para>
    /// <note type="warning">
    /// Предполагается, что методы <see cref="VerticalLine"/>, <see cref="HorizontalLine"/>,
    /// <see cref="LeftDiagonalLine"/> и <see cref="RightDiagonalLine"/> корректно реализованы
    /// и возвращают осмысленные значения. Однако, если эти методы не проверяют содержимое
    /// центральной ячейки (например, всегда начинают с 1), это может привести к завышенной оценке
    /// даже для пустых или незначимых ячеек.
    /// </note>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Вызывается, если параметр <paramref name="board"/> равен <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Вызывается, если <paramref name="row"/> или <paramref name="column"/> находятся вне допустимого диапазона индексов доски.
    /// </exception>
    // public EvaluationCell EvaluationNextMove(string[,] board, int row, int column)
    // {
    //     string ticTac = GameLogic.X;
    //     int x = 0;
    //     int o = 0;
    //     var resVertX = VerticalLine(board, row, column, GetValue, ticTac);
    //     var resHorX = HorizontalLine(board, row, column, GetValue, ticTac);
    //     var resLeftDX = LeftDiagonalLine(board, row, column, GetValue, ticTac);
    //     var resRightDX = RightDiagonalLine(board, row, column, GetValue, ticTac);
    //     var maxAx = Math.Max(resVertX, resHorX);
    //     var maxBx = Math.Max(resLeftDX, resRightDX);
    //     var maxX = Math.Max(maxAx, maxBx);
    //     x = resVertX + resHorX + resLeftDX + resRightDX;

    //     ticTac = GameLogic.O;
    //     var resVertO = VerticalLine(board, row, column, GetValue, ticTac);
    //     var resHorO = HorizontalLine(board, row, column, GetValue, ticTac);
    //     var resLeftDO = LeftDiagonalLine(board, row, column, GetValue, ticTac);
    //     var resRightDO = RightDiagonalLine(board, row, column, GetValue, ticTac);
    //     var maxAo = Math.Max(resVertO, resHorO);
    //     var maxBo = Math.Max(resLeftDO, resRightDO);
    //     var maxO = Math.Max(maxAo, maxBo);
    //     o = resVertO + resHorO + resLeftDO + resRightDO;
    //     return new EvaluationCell(row, column, x + o, x, o, maxX, maxO);
    // }


    /// <summary>
    /// Оценивает приоритет следующего хода в указанной ячейке.
    /// </summary>
    /// <param name="board">Игровое поле.</param>
    /// <param name="row">Строка ячейки.</param>
    /// <param name="column">Столбец ячейки.</param>
    /// <returns>Оценка ячейки с учётом потенциала для X и O.</returns>
    public EvaluationCell EvaluationNextMove(string[,] board, int row, int column)
    {
        if (board == null) throw new ArgumentNullException(nameof(board));
        if (GetValue == null) throw new ArgumentNullException(nameof(GetValue));

        int size = board.GetLength(0);
        if (row < 0 || row >= size || column < 0 || column >= size)
            throw new ArgumentOutOfRangeException("row или column вне диапазона.");

        // Локальная функция для оценки по всем направлениям
        (double totalScore, double maxLine) EvaluateForPlayer(string player)
        {
            var resVert = VerticalLine(board, row, column, GetValue, player);
            var resHor = HorizontalLine(board, row, column, GetValue, player);
            var resLeftD = LeftDiagonalLine(board, row, column, GetValue, player);
            var resRightD = RightDiagonalLine(board, row, column, GetValue, player);

            double total = resVert + resHor + resLeftD + resRightD;
            double max = Math.Max(Math.Max(resVert, resHor), Math.Max(resLeftD, resRightD));

            return (total, max);
        }

        var (scoreX, maxX) = EvaluateForPlayer(GameLogic.X);
        var (scoreO, maxO) = EvaluateForPlayer(GameLogic.O);

        return new EvaluationCell(row, column, scoreX + scoreO, scoreX, scoreO, maxX, maxO);
    }

    /// <summary>
    /// Оценивает вертикальную линию на игровой доске, начиная с указанной позиции.
    /// Подсчитывает количество подряд идущих ячеек, содержащих символ текущего игрока,
    /// в направлениях вверх и вниз от заданной позиции. Подсчёт останавливается при встрече
    /// ячейки, не соответствующей символу игрока.
    /// </summary>
    /// <param name="board">Двумерный массив, представляющий игровую доску. Не должен быть значением <c>null</c>.</param>
    /// <param name="row">Индекс строки центральной ячейки, с которой начинается оценка. Должен быть в пределах [0, GetLength(0)).</param>
    /// <param name="column">Индекс столбца центральной ячейки, с которой начинается оценка. Должен быть в пределах [0, GetLength(1)).</param>
    /// <param name="evaluations">Делегат функции, определяющей оценку значения ячейки. Не должен быть значением <c>null</c>.</param>
    /// <param name="ticTac">Символ текущего игрока (например, "X" или "O"), с которым сравниваются значения ячеек.</param>
    /// <returns>
    /// Суммарное количество подряд идущих ячеек, оценённых как 1 (совпадающих с <paramref name="ticTac"/>),
    /// включая центральную ячейку. Минимальное возвращаемое значение — 1.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Вызывается, если параметр <paramref name="board"/> или <paramref name="evaluations"/> равен <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Вызывается, если параметр <paramref name="row"/> или <paramref name="column"/> находится вне допустимого диапазона индексов доски.
    /// </exception>
    /// <remarks>
    /// <note type="warning">
    /// Центральная ячейка (board[row, column]) не проверяется с помощью делегата <paramref name="evaluations"/>. 
    /// Вместо этого метод всегда начинает подсчёт с 1, что может привести к некорректным результатам, 
    /// если центральная ячейка не содержит символ <paramref name="ticTac"/>.
    /// </note>
    /// </remarks>
    private double VerticalLine(string[,] board, int row, int column, Func<string, string, double> evaluations, string ticTac)
    {
        // Валидация
        if (board == null) throw new ArgumentNullException(nameof(board));
        if (evaluations == null) throw new ArgumentNullException(nameof(evaluations));
        if (row < 0 || row >= board.GetLength(0)) throw new ArgumentOutOfRangeException(nameof(row));
        if (column < 0 || column >= board.GetLength(1)) throw new ArgumentOutOfRangeException(nameof(column));

        int temp_row = row;
        double result = 1;
        temp_row--;
        while (temp_row >= 0)
        {
            double value = evaluations(board[temp_row--, column], ticTac);
            if (value == 1)
            {
                result += value;
            }
            else
            {
                if (value > 0)
                {
                    result += value;
                }
                break;
            }
        }
        temp_row = row + 1;
        while (temp_row < board.GetLength(0))
        {
            double value = evaluations(board[temp_row++, column], ticTac);
            if (value == 1)
            {
                result += value;
            }
            else
            {
                if (value > 0)
                {
                    result += value;
                }
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// Оценивает горизонтальную линию на игровой доске, начиная с указанной позиции.
    /// Подсчитывает количество подряд идущих ячеек, содержащих символ текущего игрока,
    /// в направлениях влево и вправо от заданной позиции. Подсчёт останавливается при встрече
    /// ячейки, не соответствующей символу игрока.
    /// </summary>
    /// <param name="board">Двумерный массив, представляющий игровую доску. Не должен быть значением <c>null</c>.</param>
    /// <param name="row">Индекс строки центральной ячейки, с которой начинается оценка. Должен быть в пределах [0, GetLength(0)).</param>
    /// <param name="column">Индекс столбца центральной ячейки, с которой начинается оценка. Должен быть в пределах [0, GetLength(1)).</param>
    /// <param name="evaluations">Делегат функции, определяющей оценку значения ячейки. Не должен быть значением <c>null</c>.</param>
    /// <param name="ticTac">Символ текущего игрока (например, "X" или "O"), с которым сравниваются значения ячеек.</param>
    /// <returns>
    /// Суммарное количество подряд идущих ячеек, оценённых как 1 (совпадающих с <paramref name="ticTac"/>),
    /// включая центральную ячейку. Минимальное возвращаемое значение — 1.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Вызывается, если параметр <paramref name="board"/> или <paramref name="evaluations"/> равен <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Вызывается, если параметр <paramref name="row"/> или <paramref name="column"/> находится вне допустимого диапазона индексов доски.
    /// </exception>
    /// <remarks>
    /// <note type="warning">
    /// Центральная ячейка (board[row, column]) не проверяется с помощью делегата <paramref name="evaluations"/>. 
    /// Вместо этого метод всегда начинает подсчёт с 1, что может привести к некорректным результатам, 
    /// если центральная ячейка не содержит символ <paramref name="ticTac"/>.
    /// </note>
    /// </remarks>
    private double HorizontalLine(string[,] board, int row, int column, Func<string, string, double> evaluations, string ticTac)
    {
        // Валидация
        if (board == null) throw new ArgumentNullException(nameof(board));
        if (evaluations == null) throw new ArgumentNullException(nameof(evaluations));
        if (row < 0 || row >= board.GetLength(0)) throw new ArgumentOutOfRangeException(nameof(row));
        if (column < 0 || column >= board.GetLength(1)) throw new ArgumentOutOfRangeException(nameof(column));

        int temp_column = column;
        double result = 1;
        temp_column--;
        while (temp_column >= 0)
        {
            double value = evaluations(board[row, temp_column--], ticTac);
            if (value == 1)
            {
                result += value;
            }
            else
            {
                if (value > 0)
                {
                    result += value;
                }
                break;
            }
        }
        temp_column = column + 1;
        while (temp_column < board.GetLength(1))
        {
            double value = evaluations(board[row, temp_column++], ticTac);
            if (value == 1)
            {
                result += value;
            }
            else
            {
                if (value > 0)
                {
                    result += value;
                }
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// Оценивает левую диагональную линию (от верхнего левого к нижнему правому углу) на игровой доске.
    /// Подсчитывает количество подряд идущих ячеек, содержащих символ текущего игрока,
    /// в направлениях вверх-влево и вниз-вправо от заданной позиции. Подсчёт останавливается при встрече
    /// ячейки, не соответствующей символу игрока.
    /// </summary>
    /// <param name="board">Двумерный массив, представляющий игровую доску. Не должен быть значением <c>null</c>.</param>
    /// <param name="row">Индекс строки центральной ячейки, с которой начинается оценка. Должен быть в пределах [0, GetLength(0)).</param>
    /// <param name="column">Индекс столбца центральной ячейки, с которой начинается оценка. Должен быть в пределах [0, GetLength(1)).</param>
    /// <param name="evaluations">Делегат функции, определяющей оценку значения ячейки. Не должен быть значением <c>null</c>.</param>
    /// <param name="ticTac">Символ текущего игрока (например, "X" или "O"), с которым сравниваются значения ячеек.</param>
    /// <returns>
    /// Суммарное количество подряд идущих ячеек, оценённых как 1 (совпадающих с <paramref name="ticTac"/>),
    /// включая центральную ячейку. Минимальное возвращаемое значение — 1.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Вызывается, если параметр <paramref name="board"/> или <paramref name="evaluations"/> равен <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Вызывается, если параметр <paramref name="row"/> или <paramref name="column"/> находится вне допустимого диапазона индексов доски.
    /// </exception>
    /// <remarks>
    /// <note type="warning">
    /// Центральная ячейка (board[row, column]) не проверяется с помощью делегата <paramref name="evaluations"/>. 
    /// Вместо этого метод всегда начинает подсчёт с 1, что может привести к некорректным результатам, 
    /// если центральная ячейка не содержит символ <paramref name="ticTac"/>.
    /// </note>
    /// </remarks>
    private double LeftDiagonalLine(string[,] board, int row, int column, Func<string, string, double> evaluations, string ticTac)
    {
        // Валидация
        if (board == null) throw new ArgumentNullException(nameof(board));
        if (evaluations == null) throw new ArgumentNullException(nameof(evaluations));
        if (row < 0 || row >= board.GetLength(0)) throw new ArgumentOutOfRangeException(nameof(row));
        if (column < 0 || column >= board.GetLength(1)) throw new ArgumentOutOfRangeException(nameof(column));

        int temp_column = column;
        int temp_row = row;
        double result = 1;
        temp_row--;
        temp_column--;
        while (temp_column >= 0 && temp_row >= 0)
        {
            double value = evaluations(board[temp_row--, temp_column--], ticTac);
            if (value == 1)
            {
                result += value;
            }
            else
            {
                if (value > 0)
                {
                    result += value;
                }
                break;
            }
        }
        temp_column = column + 1;
        temp_row = row + 1;
        while (temp_column < board.GetLength(1) && temp_row < board.GetLength(0))
        {
            double value = evaluations(board[temp_row++, temp_column++], ticTac);
            if (value == 1)
            {
                result += value;
            }
            else
            {
                if (value > 0)
                {
                    result += value;
                }
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// Оценивает правую диагональную линию (от нижнего левого к верхнему правому углу) на игровой доске.
    /// Подсчитывает количество подряд идущих ячеек, содержащих символ текущего игрока,
    /// в направлениях вниз-влево и вверх-вправо от заданной позиции. Подсчёт останавливается при встрече
    /// ячейки, не соответствующей символу игрока.
    /// </summary>
    /// <param name="board">Двумерный массив, представляющий игровую доску. Не должен быть значением <c>null</c>.</param>
    /// <param name="row">Индекс строки центральной ячейки, с которой начинается оценка. Должен быть в пределах [0, GetLength(0)).</param>
    /// <param name="column">Индекс столбца центральной ячейки, с которой начинается оценка. Должен быть в пределах [0, GetLength(1)).</param>
    /// <param name="evaluations">Делегат функции, определяющей оценку значения ячейки. Не должен быть значением <c>null</c>.</param>
    /// <param name="ticTac">Символ текущего игрока (например, "X" или "O"), с которым сравниваются значения ячеек.</param>
    /// <returns>
    /// Суммарное количество подряд идущих ячеек, оценённых как 1 (совпадающих с <paramref name="ticTac"/>),
    /// включая центральную ячейку. Минимальное возвращаемое значение — 1.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Вызывается, если параметр <paramref name="board"/> или <paramref name="evaluations"/> равен <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Вызывается, если параметр <paramref name="row"/> или <paramref name="column"/> находится вне допустимого диапазона индексов доски.
    /// </exception>
    /// <remarks>
    /// <note type="warning">
    /// Центральная ячейка (board[row, column]) не проверяется с помощью делегата <paramref name="evaluations"/>. 
    /// Вместо этого метод всегда начинает подсчёт с 1, что может привести к некорректным результатам, 
    /// если центральная ячейка не содержит символ <paramref name="ticTac"/>.
    /// </note>
    /// <para>
    /// Обратите внимание, что направления "вниз-влево" и "вверх-вправо" определяются относительно центральной ячейки.
    /// </para>
    /// </remarks>
    private double RightDiagonalLine(string[,] board, int row, int column, Func<string, string, double> evaluations, string ticTac)
    {
        // Валидация
        if (board == null) throw new ArgumentNullException(nameof(board));
        if (evaluations == null) throw new ArgumentNullException(nameof(evaluations));
        if (row < 0 || row >= board.GetLength(0)) throw new ArgumentOutOfRangeException(nameof(row));
        if (column < 0 || column >= board.GetLength(1)) throw new ArgumentOutOfRangeException(nameof(column));

        int temp_column = column;
        int temp_row = row;
        double result = 1;
        temp_row++;
        temp_column--;
        while (temp_column >= 0 && temp_row < board.GetLength(0))
        {
            double value = evaluations(board[temp_row++, temp_column--], ticTac);
            if (value == 1)
            {
                result += value;
            }
            else
            {
                if (value > 0)
                {
                    result += value;
                }
                break;
            }
        }
        temp_column = column + 1;
        temp_row = row - 1;
        while (temp_column < board.GetLength(1) && temp_row >= 0)
        {
            double value = evaluations(board[temp_row--, temp_column++], ticTac);
            if (value == 1)
            {
                result += value;
            }
            else
            {
                if (value > 0)
                {
                    result += value;
                }
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// Базовая функция оценки ячейки игровой доски.
    /// Возвращает 1, если значение ячейки совпадает с символом текущего игрока, иначе возвращает 0.
    /// </summary>
    /// <param name="valueCell">Значение ячейки доски, которое необходимо оценить. Может быть <c>null</c>.</param>
    /// <param name="ticTac">Символ текущего игрока (например, "X" или "O"), с которым выполняется сравнение.</param>
    /// <returns>
    /// Возвращает 1, если <paramref name="valueCell"/> не является значением <c>null</c> и равно <paramref name="ticTac"/>; иначе возвращает 0.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Эта функция предназначена для использования в качестве аргумента в методах оценки линий,
    /// таких как <see cref="VerticalLine"/>, <see cref="HorizontalLine"/> и других.
    /// </para>
    /// <para>
    /// Сравнение выполняется с использованием оператора ==. Для чувствительного к регистру сравнения
    /// убедитесь, что символы в <paramref name="valueCell"/> и <paramref name="ticTac"/> приведены к одному регистру.
    /// </para>
    /// </remarks>
    // private int GetValue(string valueCell, string ticTac) => valueCell == ticTac ? 1 : 0;

    private double GetValue(string valueCell, string ticTac)
    {
        if (valueCell == ticTac) return 1.0;
        if (valueCell == GameLogic.Empty) return 0.5;
        return 0;
    }

    public List<(int row, int column)> GetCellEmpty(string[,] board)
    {
        List<(int row, int column)> emptyCells = new List<(int row, int column)>();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] == " ")
                {

                    emptyCells.Add((i, j));
                }
            }
        }
        return emptyCells;
    }

    /// <summary>
    /// Проверяет, достиг ли один из игроков выигрышной комбинации в заданной линии.
    /// </summary>
    /// <param name="line">Массив строк, представляющий линию на игровом поле (строка, столбец или диагональ).</param>
    /// <param name="tac">Символ первого игрока (например, "X").</param>
    /// <param name="tic">Символ второго игрока (например, "O").</param>
    /// <param name="countWinner">Количество подряд идущих символов, необходимое для победы.</param>
    /// <returns>Возвращает true, если в линии найдено <paramref name="countWinner"/> или более подряд идущих символов одного из игроков; иначе — false.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="line"/> равен null.</exception>
    /// <remarks>
    /// Метод проходит по массиву <paramref name="line"/>, подсчитывая последовательные вхождения символов <paramref name="tac"/> и <paramref name="tic"/>.
    /// При встрече символа другого игрока или пустой ячейки счётчик сбрасывается.
    /// Проверка прерывается, как только один из счётчиков достигает значения <paramref name="countWinner"/>.
    /// </remarks>
    public bool IsWin(BoardCell[] line, string tac, string tic, int countWinner, out string winner, out int winnerLostIndex)
    {
        winner = string.Empty;
        winnerLostIndex = 0;
        // Валидация входных данных
        if (line == null)
            throw new ArgumentNullException(nameof(line));

        // Если линия короче требуемого количества для победы, проверка не имеет смысла
        if (line.Length < countWinner)
            return false;

        int countTac = 0; // Счётчик для символов первого игрока
        int countTic = 0; // Счётчик для символов второго игрока

        // Проход по каждой ячейке в линии
        for (int i = 0; i < line.Length; i++)
        {
            string cell = line[i].Player;
            winner = cell;
            winnerLostIndex = i;
            switch (cell)
            {
                case var c when c == tac:
                    countTac++;
                    countTic = 0; // Сброс счётчика противника
                    break;
                case var c when c == tic:
                    countTic++;
                    countTac = 0; // Сброс счётчика противника
                    break;
                default:
                    // Сброс обоих счётчиков при пустой ячейке или неизвестном символе
                    countTac = 0;
                    countTic = 0;
                    break;
            }

            // Проверка, достиг ли один из игроков выигрышной комбинации
            if (countTac >= countWinner || countTic >= countWinner)
                return true;
        }

        // Ни один игрок не достиг выигрышной комбинации
        return false;
    }


    /// <summary>
    /// Выполняет обход всех линий (горизонтальных, вертикальных и диагональных) на игровом поле,
    /// передавая каждую линию в виде массива <see cref="BoardCell"/> в пользовательскую функцию обработки.
    /// </summary>
    /// <param name="array">Двумерный массив строк, представляющий игровое поле. Не должен быть <see langword="null"/>.</param>
    /// <param name="processLine">Функция, принимающая массив <see cref="BoardCell"/> и возвращающая <see langword="true"/>,
    /// если условие, связанное с линией, выполнено (например, найдена победная комбинация).
    /// Должна быть не <see langword="null"/>.</param>
    /// <returns>
    /// <see langword="true"/>, если хотя бы одна линия удовлетворяет условию, заданному в <paramref name="processLine"/>; иначе — <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Если <paramref name="array"/> или <paramref name="processLine"/> равны <see langword="null"/>.</exception>
    /// <remarks>
    /// Метод проходит по всем возможным линиям длиной от 1 и более:
    /// <list type="bullet">
    ///   <item><description>Каждая строка.</description></item>
    ///   <item><description>Каждый столбец.</description></item>
    ///   <item><description>Все диагонали слева-направо (↖ → ↘).</description></item>
    ///   <item><description>Все диагонали справа-налево (↗ → ↙).</description></item>
    /// </list>
    /// Для повышения производительности рекомендуется, чтобы <paramref name="processLine"/> завершалась как можно быстрее.
    /// Внутренние массивы создаются для каждой линии — избегайте использования на очень больших досках без оптимизации.
    /// </remarks>
    public static bool TraverseBoard(string[,] array, Func<BoardCell[], bool> processLine)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (processLine == null)
            throw new ArgumentNullException(nameof(processLine));

        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        if (rows == 0 || cols == 0)
            return false;

        // Обход по строкам (горизонтали)
        for (int i = 0; i < rows; i++)
        {
            var line = new BoardCell[cols];
            for (int j = 0; j < cols; j++)
                line[j] = new BoardCell(i, j, array[i, j]);
            if (processLine(line))
                return true;
        }

        // Обход по столбцам (вертикали)
        for (int j = 0; j < cols; j++)
        {
            var line = new BoardCell[rows];
            for (int i = 0; i < rows; i++)
                line[i] = new BoardCell(i, j, array[i, j]);
            if (processLine(line))
                return true;
        }

        // Обход диагоналей: слева-направо (↖ → ↘)
        // Диагонали, начинающиеся в первой строке [0, col]
        for (int col = 0; col < cols; col++)
        {
            int length = Math.Min(rows, cols - col);
            var diag = new BoardCell[length];
            for (int i = 0; i < length; i++)
                diag[i] = new BoardCell(i, col + i, array[i, col + i]);
            if (processLine(diag))
                return true;
        }
        // Диагонали, начинающиеся в первом столбце [row, 0], кроме [0,0]
        for (int row = 1; row < rows; row++)
        {
            int length = Math.Min(rows - row, cols);
            var diag = new BoardCell[length];
            for (int i = 0; i < length; i++)
                diag[i] = new BoardCell(row + i, i, array[row + i, i]);
            if (processLine(diag))
                return true;
        }

        // Обход диагоналей: справа-налево (↗ → ↙)
        // Диагонали, начинающиеся в первой строке [0, col]
        for (int col = 0; col < cols; col++)
        {
            int length = Math.Min(rows, col + 1);
            var diag = new BoardCell[length];
            for (int i = 0; i < length && (col - i) >= 0; i++)  // Защита от выхода за границу
                diag[i] = new BoardCell(i, col - i, array[i, col - i]);
            if (processLine(diag))
                return true;
        }
        // Диагонали, начинающиеся в последнем столбце [row, cols-1], кроме [0, cols-1]
        for (int row = 1; row < rows; row++)
        {
            int length = Math.Min(rows - row, cols);
            var diag = new BoardCell[length];
            for (int i = 0; i < length && (cols - 1 - i) >= 0; i++)  // Явная проверка границы
                diag[i] = new BoardCell(row + i, cols - 1 - i, array[row + i, cols - 1 - i]);
            if (processLine(diag))
                return true;
        }

        return false;
    }


    public static void Message(BoardCell[] lineWinner, string winner, int winnerLostIndex)
    {
        System.Console.WriteLine($"Winner:{winner} lostIndex:{winnerLostIndex}");
        foreach (var cell in lineWinner)
            System.Console.WriteLine($"({cell.Row},{cell.Column}) = {cell.Player}");
    }



    /// <summary>
    /// Проверяет, есть ли победитель на текущем игровом поле.
    /// </summary>
    /// <param name="board">Двумерный массив строк, представляющий состояние игрового поля. 
    /// Не должен быть <see langword="null"/>.</param>
    /// <returns>
    /// <see langword="true"/>, если найдена выигрышная комбинация (линия из <see cref="GameLogic.countWinner"/> 
    /// одинаковых символов подряд); иначе — <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="board"/> равен <see langword="null"/>.</exception>
    /// <remarks>
    /// Метод использует обход всех возможных линий (горизонтальных, вертикальных и диагональных) 
    /// с помощью метода <see cref="TraverseBoard(string[,], Func{BoardCell[], bool})"/>.
    /// При обнаружении победной комбинации вызывается событие <see cref="Notify"/>, 
    /// передающее информацию о победителе и позициях.
    /// </remarks>
    /// <seealso cref="TraverseBoard(string[,], Func{BoardCell[], bool})"/>
    /// <seealso cref="CallWinner(BoardCell[])"/>
    /// <seealso cref="Notify"/>
    public bool FindWinner(string[,] board)
    {
        return TraverseBoard(board, CallWinner);
    }

    /// <summary>
    /// Проверяет, содержит ли указанная линия выигрышную комбинацию символов.
    /// </summary>
    /// <param name="line">Массив объектов <see cref="BoardCell"/>, представляющий линию ячеек 
    /// (строку, столбец или диагональ) на игровом поле.</param>
    /// <returns>
    /// <see langword="true"/>, если в линии обнаружена победная комбинация; 
    /// иначе — <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="line"/> равен <see langword="null"/>.</exception>
    /// <remarks>
    /// Метод определяет победителя с помощью вызова <see cref="IsWin(BoardCell[], string, string, int, out string, out int)"/>,
    /// передавая символы игроков X и O из <see cref="GameLogic.X"/> и <see cref="GameLogic.O"/>, 
    /// а также требуемую длину победной комбинации из <see cref="GameLogic.countWinner"/>.
    /// <para>
    /// Если победная комбинация найдена, вызывается делегат <see cref="Notify"/> 
    /// с передачей:
    /// <list type="bullet">
    ///   <item><description>Массива <see cref="BoardCell"/> — линии с победой.</description></item>
    ///   <item><description>Символа победителя (<c>"X"</c> или <c>"O"</c>).</description></item>
    ///   <item><description>Индекса первой ячейки, не входящей в комбинацию (или индекса после последней победной ячейки).</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// Предположим, что <c>line = [(0,0)=X, (0,1)=X, (0,2)=X]</c> и <see cref="GameLogic.countWinner"/> = 3.
    /// Тогда метод вернёт <see langword="true"/>, вызовет <see cref="Notify"/> и передаст "X" как победителя.
    /// </example>
    /// <seealso cref="IsWin(BoardCell[], string, string, int, out string, out int)"/>
    /// <seealso cref="Notify"/>
    /// <seealso cref="FindWinner(string[,])"/>
    public bool CallWinner(BoardCell[] line)
    {
        string winner;
        int winnerLostIndex = 0;
        if (IsWin(line, GameLogic.X, GameLogic.O, GameLogic.countWinner, out winner, out winnerLostIndex))
        {
            Notify?.Invoke(line, winner, winnerLostIndex);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Оценивает все пустые ячейки на доске и возвращает их оценки.
    /// </summary>
    /// <param name="board">Игровое поле.</param>
    /// <returns>Список оценённых ячеек, отсортированный по приоритету (по убыванию).</returns>
    public List<EvaluationCell> Evaluation(string[,] board)
    {
        if (board == null) throw new ArgumentNullException(nameof(board));

        var emptyCells = GetCellEmpty(board);
        var evaluationCells = new List<EvaluationCell>();

        foreach (var cell in emptyCells)
        {
            var evaluation = EvaluationNextMove(board, cell.row, cell.column);
            evaluationCells.Add(evaluation);
        }

        // Сортируем по убыванию MaxX
        // evaluationCells.Sort((a, b) => b.MaxX.CompareTo(a.MaxX));

        return evaluationCells;
    }

    public void PrintEvaluation(string[,] board)
    {
        var list = Evaluation(board);
        foreach (var item in list)
        {
            System.Console.WriteLine(item);
        }
    }

    /// <summary>
    /// Сортирует список оценочных ячеек по убыванию значения <see cref="EvaluationCell.MaxX"/> — 
    /// длины максимальной комбинации для игрока X, проходящей через ячейку.
    /// </summary>
    /// <param name="list">Список объектов <see cref="EvaluationCell"/>, который требуется отсортировать.</param>
    /// <returns>
    /// Отсортированный список <see cref="EvaluationCell"/> в порядке убывания значения <see cref="EvaluationCell.MaxX"/>.
    /// Исходный список модифицируется и возвращается.
    /// </returns>
    /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="list"/> равен <see langword="null"/>.</exception>
    /// <remarks>
    /// Метод использует сравнение по полю <see cref="EvaluationCell.MaxX"/> в порядке убывания,
    /// чтобы определить приоритет ходов, способствующих построению длинных линий для игрока X.
    /// Подходит для сценариев, где необходимо максимизировать атакующие возможности игрока X.
    /// </remarks>
    public static List<EvaluationCell> SortToMaxX(List<EvaluationCell> list)
    {
        list.Sort((a, b) => b.MaxX.CompareTo(a.MaxX));
        return list;
    }
    public static List<EvaluationCell> SortToMaxValue(List<EvaluationCell> list)
    {
        list.Sort((a, b) => b.Value.CompareTo(a.Value));
        return list;
    }



    /// <summary>
    /// Сортирует список оценочных ячеек по убыванию значения <see cref="EvaluationCell.MaxO"/> — 
    /// длины максимальной комбинации для игрока O, проходящей через ячейку.
    /// </summary>
    /// <param name="list">Список объектов <see cref="EvaluationCell"/>, который требуется отсортировать.</param>
    /// <returns>
    /// Отсортированный список <see cref="EvaluationCell"/> в порядке убывания значения <see cref="EvaluationCell.MaxO"/>.
    /// Исходный список модифицируется и возвращается.
    /// </returns>
    /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="list"/> равен <see langword="null"/>.</exception>
    /// <remarks>
    /// Метод используется для определения приоритета ходов, способствующих построению длинной линии для игрока O.
    /// Сортировка по убыванию позволяет выбрать ячейки с наибольшим потенциалом для победы игрока O.
    /// </remarks>
    public static List<EvaluationCell> SortToMaxO(List<EvaluationCell> list)
    {
        list.Sort((a, b) => b.MaxO.CompareTo(a.MaxO));
        return list;
    }

    /// <summary>
    /// Сортирует список оценочных ячеек по убыванию значения <see cref="EvaluationCell.ValueX"/> —
    /// общей оценки значимости ячейки для игрока X.
    /// </summary>
    /// <param name="list">Список объектов <see cref="EvaluationCell"/>, который требуется отсортировать.</param>
    /// <returns>
    /// Отсортированный список <see cref="EvaluationCell"/> в порядке убывания значения <see cref="EvaluationCell.ValueX"/>.
    /// Исходный список модифицируется и возвращается.
    /// </returns>
    /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="list"/> равен <see langword="null"/>.</exception>
    /// <remarks>
    /// Поле <see cref="EvaluationCell.ValueX"/> может учитывать множество факторов: 
    /// количество потенциальных линий, блокировку противника, центральность и т.д.
    /// Этот метод подходит для выбора наилучшего хода с точки зрения общего вклада в стратегию игрока X.
    /// </remarks>
    public static List<EvaluationCell> SortToValueX(List<EvaluationCell> list)
    {
        list.Sort((a, b) => b.ValueX.CompareTo(a.ValueX));
        return list;
    }

    /// <summary>
    /// Сортирует список оценочных ячеек по убыванию значения <see cref="EvaluationCell.ValueO"/> —
    /// общей оценки значимости ячейки для игрока O.
    /// </summary>
    /// <param name="list">Список объектов <see cref="EvaluationCell"/>, который требуется отсортировать.</param>
    /// <returns>
    /// Отсортированный список <see cref="EvaluationCell"/> в порядке убывания значения <see cref="EvaluationCell.ValueO"/>.
    /// Исходный список модифицируется и возвращается.
    /// </returns>
    /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="list"/> равен <see langword="null"/>.</exception>
    /// <remarks>
    /// Используется для определения приоритета ходов, наиболее выгодных для игрока O.
    /// Подходит как для атакующих, так и для оборонительных решений, если оценка учитывает блокировку X.
    /// </remarks>
    public static List<EvaluationCell> SortToValueO(List<EvaluationCell> list)
    {
        list.Sort((a, b) => b.ValueO.CompareTo(a.ValueO));
        return list;
    }

    public EvaluationCell NextMove(string[,] board)
    {
        var list = Evaluation(board);
        // list.Sort();
        // list.Reverse();
        SortToMaxValue(list);
        var cell = SelectCell(list);
        System.Console.WriteLine($"Select Cell:{cell}");
        foreach (var item in list)
        {
            System.Console.WriteLine(item);
        }
        // System.Console.WriteLine(list[0]);
        // System.Console.WriteLine(list[list.Count - 1]);
        return cell;
    }

    /// <summary>
    /// Выбирает ячейку из списка оценённых ячеек на основе максимального значения и текущей тактики игры.
    /// </summary>
    /// <param name="list">Список объектов <see cref="EvaluationCell"/>, представляющих оценённые ячейки игрового поля.</param>
    /// <returns>Объект <see cref="EvaluationCell"/> с наивысшим приоритетом согласно текущей тактике.</returns>
    /// <exception cref="ArgumentNullException">Возникает, если параметр <paramref name="list"/> равен <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Возникает, если параметр <paramref name="list"/> пуст.</exception>
    /// <remarks>
    /// Метод сначала определяет максимальное значение среди всех ячеек в списке. 
    /// Затем формирует список всех ячеек, имеющих это максимальное значение.
    /// Далее, в зависимости от текущей тактики (<see cref="GameLogic.CurrentTactics"/>), 
    /// сортирует этот подсписок либо по приоритету для атаки (<see cref="SortToMaxO(List{EvaluationCell})"/>), 
    /// либо по приоритету для защиты (<see cref="SortToMaxX(List{EvaluationCell})"/>).
    /// Возвращается первый элемент отсортированного списка.
    /// </remarks>
    private EvaluationCell SelectCell(List<EvaluationCell> list)// переделать, не учитывает MaxX ==11
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        if (list.Count == 0)
            throw new ArgumentException("Список не может быть пустым.", nameof(list));


        GameLogic.CurrentTactics = Tactics.Attack;
        double maxX = list[0].MaxX;
        double maxO = list[0].MaxO;
        foreach (var cell in list)
        {
            if (cell.MaxX > maxX)
            {
                maxX = cell.MaxX;
            }
            if (cell.MaxO > maxO)
            {
                maxO = cell.MaxO;
            }
            // if (cell.MaxX >= 5)
            //     GameLogic.CurrentTactics = Tactics.Defense;
            if (maxX <= maxO)
                GameLogic.CurrentTactics = Tactics.Attack;
            else
                GameLogic.CurrentTactics = Tactics.Defense;
        }

        if (GameLogic.CurrentTactics == Tactics.Attack)
        {
            SortToMaxO(list);
        }
        else
        {
            SortToMaxX(list);
        }

        return list[0];
    }
}



