using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace TicTacToeGame.Classes
{
    /// <summary>
    /// Класс для управления сохранением и загрузкой данных
    /// </summary>
    public class DataManager
    {
        private static string DataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "players.dat");

        static DataManager()
        {
            // Создание папки Data, если её нет
            string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
        }

        /// <summary>
        /// Сохранение списка игроков в файл
        /// </summary>
        public static void SavePlayers(List<PlayerData> players)
        {
            try
            {
                using (FileStream fs = new FileStream(DataPath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, players);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Загрузка списка игроков из файла
        /// </summary>
        public static List<PlayerData> LoadPlayers()
        {
            try
            {
                if (File.Exists(DataPath))
                {
                    using (FileStream fs = new FileStream(DataPath, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        return (List<PlayerData>)formatter.Deserialize(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            return new List<PlayerData>();
        }

        /// <summary>
        /// Добавление или обновление игрока
        /// </summary>
        public static void SavePlayerResult(string playerName, GameResult result)
        {
            var players = LoadPlayers();
            var player = players.FirstOrDefault(p => p.PlayerName == playerName);

            if (player == null)
            {
                player = new PlayerData(playerName);
                players.Add(player);
            }

            player.AddGameResult(result);
            SavePlayers(players);
        }

        /// <summary>
        /// Получение истории игрока
        /// </summary>
        public static List<GameResult> GetPlayerHistory(string playerName)
        {
            var players = LoadPlayers();
            var player = players.FirstOrDefault(p => p.PlayerName == playerName);
            return player?.GameHistory ?? new List<GameResult>();
        }

        /// <summary>
        /// Получение всех игроков
        /// </summary>
        public static List<PlayerData> GetAllPlayers()
        {
            return LoadPlayers();
        }
    }
}