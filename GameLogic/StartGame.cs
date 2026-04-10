
namespace MauiTicTac;

public static class StartGame
{
    public static string[,] board = new string[20, 20];
    public static void Start()
    {
        // ODO: Implement game start logic
    }

    /// <summary>
    /// Инициализирует все ячейки указанной игровой доски значением пустой ячейки.
    /// </summary>
    /// <param name="board">Двумерный массив строк, представляющий игровое поле. 
    /// Не должен быть <see langword="null"/>.</param>
    /// <exception cref="InvalidOperationException">Возникает, если параметр <paramref name="board"/> равен <see langword="null"/>.
    /// В сообщении исключения указывается, что игровая доска не инициализирована.</exception>
    /// <remarks>
    /// Метод заполняет каждую ячейку двумерного массива значением <c>" "</c> (пробел), 
    /// обозначающим пустую ячейку на игровом поле.
    /// <para>
    /// Используется для сброса состояния доски перед началом новой игры или при её перезапуске.
    /// </para>
    /// <para>
    /// Размеры доски определяются автоматически с помощью <see cref="Array.GetLength(int)"/> — 
    /// метод корректно работает с прямоугольными массивами любой размерности.
    /// </para>
    /// </remarks>
    /// <example>
    /// Инициализация доски 3x3:
    /// <code>
    /// string[,] board = new string[3, 3];
    /// InitBoard(board);
    /// // Теперь все ячейки board[i, j] содержат " "
    /// </code>
    /// </example>
    /// <seealso cref="GameLogic.countWinner"/>
    /// <seealso cref="BoardCell"/>
    public static void InitBoard(string[,] board)
    {
        if (board == null)
            throw new InvalidOperationException("Игровая доска не инициализирована.");

        const string EmptyCell = " ";
        int rows = board.GetLength(0);
        int columns = board.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int k = 0; k < columns; k++)
            {
                board[i, k] = EmptyCell;
            }
        }
    }

    /// <summary>
    /// Устанавливает значение указанной ячейки на игровой доске.
    /// </summary>
    /// <param name="board">Двумерный массив строк, представляющий игровое поле. 
    /// Должен быть инициализирован до вызова метода.</param>
    /// <param name="row">Индекс строки ячейки. Должен быть в допустимом диапазоне для массива <paramref name="board"/>.</param>
    /// <param name="column">Индекс столбца ячейки. Должен быть в допустимом диапазоне для массива <paramref name="board"/>.</param>
    /// <param name="ticTac">Значение, которое необходимо установить в ячейку: 
    /// например, <c>"X"</c>, <c>"O"</c> или <c>" "</c> (пустая ячейка).</param>
    /// <exception cref="ArgumentNullException">Возникает, если <paramref name="board"/> равен <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">Возникает, если <paramref name="row"/> или <paramref name="column"/> 
    /// выходят за пределы размеров массива <paramref name="board"/>.</exception>
    /// <remarks>
    /// Метод напрямую присваивает значение <paramref name="ticTac"/> ячейке с координатами <paramref name="row"/>, <paramref name="column"/>.
    /// <para>
    /// Ответственность за валидацию корректности символов (<c>"X"</c>, <c>"O"</c>) и проверку, 
    /// что ячейка пуста перед установкой, лежит на вызывающем коде.
    /// </para>
    /// </remarks>
    /// <example>
    /// Установка символа "X" в центральную ячейку доски 3x3:
    /// <code>
    /// string[,] board = new string[3, 3];
    /// InitBoard(board); // Сначала инициализируем доску
    /// SetBoard(board, 1, 1, "X"); // Устанавливаем X в центр
    /// </code>
    /// </example>
    /// <seealso cref="InitBoard(string[,])"/>
    /// <seealso cref="GameLogic.X"/>
    /// <seealso cref="GameLogic.O"/>
    /// <summary>
    /// Устанавливает символ игрока в указанную ячейку доски.
    /// </summary>
    /// <param name="board">Игровая доска.</param>
    /// <param name="row">Номер строки.</param>
    /// <param name="column">Номер столбца.</param>
    /// <param name="symbol">Символ игрока ('X' или 'O'), который будет установлен.</param>
    /// <exception cref="ArgumentNullException">Если board — null.</exception>
    /// <exception cref="IndexOutOfRangeException">Если индексы вне диапазона.</exception>
    /// <exception cref="ArgumentException">Если symbol не "X" и не "O".</exception>
    public static void SetBoard(string[,] board, int row, int column, string symbol)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        if (row < 0 || row >= board.GetLength(0) || column < 0 || column >= board.GetLength(1))
            throw new IndexOutOfRangeException("Индексы выходят за границы доски.");

        if (symbol != "X" && symbol != "O")
            throw new ArgumentException("Символ должен быть 'X' или 'O'.", nameof(symbol));

        if (board[row, column] != " ")
            throw new InvalidOperationException("Ячейка уже занята. Выберите пустую ячейку.");

        board[row, column] = symbol;
    }
}