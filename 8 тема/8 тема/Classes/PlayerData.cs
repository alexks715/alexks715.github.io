using System;
using System.Collections.Generic;

namespace TicTacToeGame.Classes
{
    /// <summary>
    /// Класс для хранения данных игрока
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string PlayerName { get; set; }
        public List<GameResult> GameHistory { get; set; }

        public PlayerData()
        {
            GameHistory = new List<GameResult>();
        }

        public PlayerData(string name) : this()
        {
            PlayerName = name;
        }

        public void AddGameResult(GameResult result)
        {
            GameHistory.Add(result);
        }

        public List<GameResult> GetResultsByBoardSize(int size)
        {
            return GameHistory.FindAll(r => r.BoardSize == size);
        }

        public int GetWinsCount()
        {
            return GameHistory.FindAll(r => r.Result == "Победа").Count;
        }

        public int GetLossesCount()
        {
            return GameHistory.FindAll(r => r.Result == "Поражение").Count;
        }

        public int GetDrawsCount()
        {
            return GameHistory.FindAll(r => r.Result == "Ничья").Count;
        }

        public override string ToString()
        {
            return $"{PlayerName} - Игр: {GameHistory.Count} (В: {GetWinsCount()}, П: {GetLossesCount()}, Н: {GetDrawsCount()})";
        }
    }
}