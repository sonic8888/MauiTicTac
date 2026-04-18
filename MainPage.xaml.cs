using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace MauiTicTac;

/// <summary>
/// Основной класс страницы приложения для игры "Крестики-нолики"
/// Управляет пользовательским интерфейсом, обработкой событий и анимациями
/// </summary>
public partial class MainPage : ContentPage
{

    public enum Player
    {
        X,
        O,
    }

    public static Player currentPlayer = Player.X;
    /// <summary>
    /// Количество строк в сетке игры
    /// </summary>
    private const int Rows = 20;

    /// <summary>
    /// Количество столбцов в сетке игры
    /// </summary>
    private const int Cols = 20;
    private GameLogic gameLogic;

    /// <summary>
    /// Статический экземпляр анимированного drawer для отрисовки игровых элементов
    /// Управляет отрисовкой анимированных крестиков
    /// </summary>
    // private static readonly AnimatedGridDrawerCross animatedDrawerCross = new();


    // private static readonly AnimatedGridDrawerCircle animatedDrawerCircle = new();

    /// <summary>
    /// Свойство для доступа к анимированному drawer
    /// Используется для привязки в XAML
    /// </summary>

    /// <summary>
    /// Свойство для доступа к анимированному drawer
    /// Используется для привязки в XAML
    /// </summary>
    // private static AnimatedGridDrawerCross AnimatedDrawerCross => animatedDrawerCross;
    // private static AnimatedGridDrawerCircle AnimatedDrawerCircle => animatedDrawerCircle;

    /// <summary>
    /// Конструктор класса MainPage
    /// Инициализирует компоненты пользовательского интерфейса
    /// </summary>
    /// 
    /// 
    private static readonly AnimatedGridDrawer animatedDrawer = new();
    private static AnimatedGridDrawer AnimatedDrawer => animatedDrawer;
    public MainPage()
    {
        InitializeComponent();
        StartGame.InitBoard(StartGame.board);
        gameLogic = new GameLogic();
        // StartGame.SetBoard(StartGame.board, 20, 20, GameLogic.O);
    }

    /// <summary>
    /// Обработчик события нажатия на сетку игры
    /// Вызывается при клике пользователя по ячейке сетки
    /// Преобразует координаты клика в индексы ячейки и запускает анимацию крестика
    /// </summary>
    /// <param name="sender">Объект, отправивший событие (GraphicsView)</param>
    /// <param name="e">Аргументы события нажатия, содержащие координаты клика</param>
    private async void OnGridTapped(object sender, TappedEventArgs e)
    {

        if (sender is GraphicsView graphicsView)
        {
            var tapPoint = e.GetPosition(graphicsView);
            if (tapPoint != null)
            {
                var (row, col) = GridDrawer.Instance.GetCellFromPoint(tapPoint.Value);
                int cellIndex = row * Cols + col;

                // Добавляем символ в зависимости от currentPlayer
                animatedDrawer.AddSymbol(row, col);

                // Запускаем анимацию
                await AnimateSymbol(graphicsView, row, col);

                StartGame.SetBoard(StartGame.board, row, col, GameLogic.X);
                if (gameLogic.CheckWiner(StartGame.board)){
                    System.Console.WriteLine("Winer X");
                }

                // Меняем игрока после хода
                currentPlayer = currentPlayer == Player.X ? Player.O : Player.X;
                NextStep(StartGame.board, graphicsView);
            }
        }
    }


    private async void NextStep(string[,] board, GraphicsView graphicsView)
    {
        var nextCell = gameLogic.NextMove(board);
        StartGame.SetBoard(board, nextCell.Row, nextCell.Column, GameLogic.O);
            if (gameLogic.CheckWiner(StartGame.board)){
                    System.Console.WriteLine("Winer O");
                }
        animatedDrawer.AddSymbol(nextCell.Row, nextCell.Column);
        await AnimateSymbol(graphicsView, nextCell.Row, nextCell.Column);
        currentPlayer = currentPlayer == Player.X ? Player.O : Player.X;
    }
    /// <summary>
    /// Анимирует процесс рисования крестика в указанной ячейке
    /// Разбивает анимацию на несколько шагов для плавного визуального эффекта
    /// </summary>
    /// <param name="graphicsView">Элемент GraphicsView, который нужно обновить</param>
    /// <param name="row">Индекс строки ячейки, где рисуется крестик</param>
    /// <param name="col">Индекс столбца ячейки, где рисуется крестик</param>
    // private async Task AnimateCross(GraphicsView graphicsView, int row, int col)
    // {
    //     const int animationSteps = 30;
    //     const int delayMs = 30;

    //     for (int i = 1; i <= animationSteps; i++)
    //     {
    //         float progress = (float)i / animationSteps;
    //         animatedDrawerCross.SetCrossProgress(row, col, progress);

    //         // Обновляем GraphicsView
    //         graphicsView.Invalidate();

    //         // Задержка для плавности анимации
    //         await Task.Delay(delayMs);
    //     }
    // }

    /// <summary>
    /// Анимирует процесс рисования нолика в указанной ячейке
    /// Разбивает анимацию на несколько шагов для плавного визуального эффекта
    /// </summary>
    /// <param name="graphicsView">Элемент GraphicsView, который нужно обновить</param>
    /// <param name="row">Индекс строки ячейки, где рисуется крестик</param>
    /// <param name="col">Индекс столбца ячейки, где рисуется крестик</param>
    // private async Task AnimateCircle(GraphicsView graphicsView, int row, int col)
    // {
    //     const int animationSteps = 30;
    //     const int delayMs = 30;
    //     for (int i = 1; i <= animationSteps; i++)
    //     {
    //         float progress = (float)i / animationSteps;
    //         animatedDrawerCircle.SetCircleProgress(row, col, progress);

    //         // Обновляем GraphicsView
    //         graphicsView.Invalidate();

    //         // Задержка для плавности анимации
    //         await Task.Delay(delayMs);
    //     }
    // }

    private async Task AnimateSymbol(GraphicsView graphicsView, int row, int col)
    {
        const int animationSteps = 30;
        const int delayMs = 30;

        for (int i = 1; i <= animationSteps; i++)
        {
            float progress = (float)i / animationSteps;
            animatedDrawer.SetProgress(row, col, progress);
            graphicsView.Invalidate();
            await Task.Delay(delayMs);
        }
    }


    /// <summary>
    /// Внутренний класс для отрисовки анимированных игровых элементов
    /// Наследуется от GridDrawer и добавляет функциональность анимации крестиков
    /// Управляет прогрессом анимации для каждого крестика на игровом поле
    /// </summary>
    private class AnimatedGridDrawerCross : GridDrawer
    {
        /// <summary>
        /// Словарь для хранения прогресса анимации для каждого крестика
        /// Ключ: (row, col) - координаты ячейки
        /// Значение: прогресс анимации (0.0 - начало, 1.0 - завершение)
        /// </summary>
        private readonly Dictionary<(int row, int col), float> crossProgress = new();

        /// <summary>
        /// Добавляет новую ячейку для анимации крестика
        /// Инициализирует прогресс анимации с нуля
        /// </summary>
        /// <param name="row">Индекс строки ячейки</param>
        /// <param name="col">Индекс столбца ячейки</param>
        public void AddCross(int row, int col)
        {
            crossProgress[(row, col)] = 0f;
        }

        /// <summary>
        /// Устанавливает текущий прогресс анимации для крестика в указанной ячейке
        /// Используется для анимации процесса рисования крестика
        /// </summary>
        /// <param name="row">Индекс строки ячейки</param>
        /// <param name="col">Индекс столбца ячейки</param>
        /// <param name="progress">Текущий прогресс анимации (0.0 - 1.0)</param>
        public void SetCrossProgress(int row, int col, float progress)
        {
            crossProgress[(row, col)] = progress;
        }

        /// <summary>
        /// Переопределенный метод отрисовки
        /// Сначала рисует основную сетку, затем все анимированные крестики
        /// </summary>
        /// <param name="canvas">Контекст рисования</param>
        /// <param name="dirtyRect">Область, требующая перерисовки</param>
        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Сначала рисуем основную сетку
            base.Draw(canvas, dirtyRect);

            // Затем рисуем все анимированные крестики
            foreach (var kvp in crossProgress)
            {
                var (row, col) = kvp.Key;
                float progress = kvp.Value;
                DrawCrossAnimation(canvas, row, col, progress);
            }
        }
    }

    /// <summary>
    /// Внутренний класс для отрисовки анимированных игровых элементов
    /// Наследуется от GridDrawer и добавляет функциональность анимации ноликов
    /// Управляет прогрессом анимации для каждого крестика на игровом поле
    /// </summary>
    private class AnimatedGridDrawerCircle : GridDrawer
    {
        /// <summary>
        /// Словарь для хранения прогресса анимации для каждого нолика
        /// Ключ: (row, col) - координаты ячейки
        /// Значение: прогресс анимации (0.0 - начало, 1.0 - завершение)
        /// </summary>
        private readonly Dictionary<(int row, int col), float> circleProgress = new();


        /// <summary>
        /// Добавляет новую ячейку для анимации нолика
        /// Инициализирует прогресс анимации с нуля
        /// </summary>
        /// <param name="row">Индекс строки ячейки</param>
        /// <param name="col">Индекс столбца ячейки</param>
        public void AddCircle(int row, int col)
        {
            circleProgress[(row, col)] = 0f;
        }

        /// <summary>
        /// Устанавливает текущий прогресс анимации для нолика в указанной ячейке
        /// Используется для анимации процесса рисования нолика
        /// </summary>
        /// <param name="row">Индекс строки ячейки</param>
        /// <param name="col">Индекс столбца ячейки</param>
        /// <param name="progress">Текущий прогресс анимации (0.0 - 1.0)</param>
        public void SetCircleProgress(int row, int col, float progress)
        {
            circleProgress[(row, col)] = progress;
        }

        /// <summary>
        /// Переопределенный метод отрисовки
        /// Сначала рисует основную сетку, затем все анимированные нолики
        /// </summary>
        /// <param name="canvas">Контекст рисования</param>
        /// <param name="dirtyRect">Область, требующая перерисовки</param>
        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Сначала рисуем основную сетку
            base.Draw(canvas, dirtyRect);

            // Затем рисуем все анимированные нолики
            foreach (var kvp in circleProgress)
            {
                var (row, col) = kvp.Key;
                float progress = kvp.Value;
                DrawCircleAnimation(canvas, row, col, progress);
            }
        }
    }


}

