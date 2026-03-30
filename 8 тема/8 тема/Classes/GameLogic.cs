using System;
using System.Collections.Generic;

namespace TicTacToeGame.Classes
{
    /// <summary>
    /// Класс для хранения информации о результате игры
    /// </summary>
    public class GameResult
    {
        public string PlayerName { get; set; }
        public DateTime GameDate { get; set; }
        public int BoardSize { get; set; }
        public string Result { get; set; } // Победа, Поражение, Ничья
        public int MovesCount { get; set; }

        public override string ToString()
        {
            return $"{GameDate:dd.MM.yyyy HH:mm} | {PlayerName} | Поле {BoardSize}x{BoardSize} | {Result} | Ходов: {MovesCount}";
        }
    }

    /// <summary>
    /// Класс логики игры Крестики-нолики
    /// </summary>
    public class GameLogic
    {
        public enum CellState { Empty, X, O }
        public enum GameState { Playing, PlayerWin, ComputerWin, Draw }

        private CellState[,] _board;
        private int _size;
        private GameState _currentState;

        public GameLogic(int size = 3)
        {
            _size = size;
            _board = new CellState[_size, _size];
            _currentState = GameState.Playing;
            InitializeBoard();
        }

        public int Size => _size;
        public GameState CurrentState => _currentState;
        public CellState[,] Board => _board;

        /// <summary>
        /// Инициализация пустого поля
        /// </summary>
        public void InitializeBoard()
        {
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    _board[i, j] = CellState.Empty;
                }
            }
            _currentState = GameState.Playing;
        }

        /// <summary>
        /// Сброс игры с новым размером поля
        /// </summary>
        public void ResetGame(int newSize)
        {
            _size = newSize;
            _board = new CellState[_size, _size];
            InitializeBoard();
        }

        /// <summary>
        /// Сделать ход игрока
        /// </summary>
        public bool MakePlayerMove(int row, int col)
        {
            if (row < 0 || row >= _size || col < 0 || col >= _size)
                return false;

            if (_board[row, col] != CellState.Empty)
                return false;

            if (_currentState != GameState.Playing)
                return false;

            _board[row, col] = CellState.X;
            CheckGameState();
            return true;
        }

        /// <summary>
        /// Сделать ход компьютера
        /// </summary>
        public bool MakeComputerMove()
        {
            if (_currentState != GameState.Playing)
                return false;

            // Находим лучший ход
            var bestMove = GetBestMove();
            if (bestMove.Row != -1 && bestMove.Col != -1)
            {
                _board[bestMove.Row, bestMove.Col] = CellState.O;
                CheckGameState();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Проверка состояния игры
        /// </summary>
        private void CheckGameState()
        {
            // Проверка победы игрока (X)
            if (CheckWin(CellState.X))
            {
                _currentState = GameState.PlayerWin;
                return;
            }

            // Проверка победы компьютера (O)
            if (CheckWin(CellState.O))
            {
                _currentState = GameState.ComputerWin;
                return;
            }

            // Проверка ничьей
            if (IsBoardFull())
            {
                _currentState = GameState.Draw;
                return;
            }
        }

        /// <summary>
        /// Проверка победы для указанного игрока
        /// </summary>
        private bool CheckWin(CellState player)
        {
            // Проверка строк
            for (int i = 0; i < _size; i++)
            {
                bool win = true;
                for (int j = 0; j < _size; j++)
                {
                    if (_board[i, j] != player)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) return true;
            }

            // Проверка столбцов
            for (int j = 0; j < _size; j++)
            {
                bool win = true;
                for (int i = 0; i < _size; i++)
                {
                    if (_board[i, j] != player)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) return true;
            }

            // Проверка главной диагонали
            bool diag1Win = true;
            for (int i = 0; i < _size; i++)
            {
                if (_board[i, i] != player)
                {
                    diag1Win = false;
                    break;
                }
            }
            if (diag1Win) return true;

            // Проверка побочной диагонали
            bool diag2Win = true;
            for (int i = 0; i < _size; i++)
            {
                if (_board[i, _size - 1 - i] != player)
                {
                    diag2Win = false;
                    break;
                }
            }
            if (diag2Win) return true;

            return false;
        }

        /// <summary>
        /// Проверка, заполнено ли всё поле
        /// </summary>
        private bool IsBoardFull()
        {
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (_board[i, j] == CellState.Empty)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Получение лучшего хода для компьютера (минимакс алгоритм)
        /// </summary>
        private (int Row, int Col) GetBestMove()
        {
            int bestScore = int.MinValue;
            int bestRow = -1;
            int bestCol = -1;

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (_board[i, j] == CellState.Empty)
                    {
                        _board[i, j] = CellState.O;
                        int score = Minimax(0, false);
                        _board[i, j] = CellState.Empty;

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestRow = i;
                            bestCol = j;
                        }
                    }
                }
            }

            return (bestRow, bestCol);
        }

        /// <summary>
        /// Алгоритм минимакс для выбора оптимального хода
        /// </summary>
        private int Minimax(int depth, bool isMaximizing)
        {
            // Проверка победы компьютера
            if (CheckWin(CellState.O))
                return 10 - depth;

            // Проверка победы игрока
            if (CheckWin(CellState.X))
                return depth - 10;

            // Проверка ничьей
            if (IsBoardFull())
                return 0;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                for (int i = 0; i < _size; i++)
                {
                    for (int j = 0; j < _size; j++)
                    {
                        if (_board[i, j] == CellState.Empty)
                        {
                            _board[i, j] = CellState.O;
                            int score = Minimax(depth + 1, false);
                            _board[i, j] = CellState.Empty;
                            bestScore = Math.Max(score, bestScore);
                        }
                    }
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                for (int i = 0; i < _size; i++)
                {
                    for (int j = 0; j < _size; j++)
                    {
                        if (_board[i, j] == CellState.Empty)
                        {
                            _board[i, j] = CellState.X;
                            int score = Minimax(depth + 1, true);
                            _board[i, j] = CellState.Empty;
                            bestScore = Math.Min(score, bestScore);
                        }
                    }
                }
                return bestScore;
            }
        }

        /// <summary>
        /// Получение списка пустых клеток
        /// </summary>
        public List<(int Row, int Col)> GetEmptyCells()
        {
            var emptyCells = new List<(int, int)>();
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (_board[i, j] == CellState.Empty)
                        emptyCells.Add((i, j));
                }
            }
            return emptyCells;
        }
    }
}