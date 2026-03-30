using System;
using System.Drawing;
using System.Windows.Forms;
using TicTacToeGame.Classes;
using System.Linq;

namespace TicTacToeGame
{
    public partial class Form1 : Form
    {
        // Элементы управления
        private MenuStrip mainMenu;
        private ToolStrip toolBar;
        private Panel gamePanel;
        private Button[,] cellButtons;
        private Label statusLabel;
        private ToolStripLabel playerNameLabel;  // Исправлено: ToolStripLabel вместо Label
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusStripLabel;

        // Логика игры
        private GameLogic _gameLogic;
        private string _currentPlayer;
        private int _currentBoardSize = 3;
        private int _moveCount = 0;

        // Настройки
        private bool _gameActive = true;

        // Для авторизации
        private string _loggedInPlayer = null;

        // Цвета
        private Color _playerColor = Color.Blue;
        private Color _computerColor = Color.Red;
        private Color _cellColor = Color.White;
        private Color _gridColor = Color.Black;

        public Form1()
        {
            // Настройка формы
            this.Text = "Крестики-нолики";
            this.Size = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = SystemColors.Control;
            this.MinimumSize = new Size(600, 500);
            this.KeyPreview = true;

            InitializeCustomComponents();
            SetupEvents();
            NewGame();

            // Запрос авторизации при запуске
            ShowLoginDialog();
        }

        private void InitializeCustomComponents()
        {
            CreateMenuStrip();
            CreateToolStrip();
            CreateGamePanel();
            CreateStatusStrip();
        }

        private void CreateMenuStrip()
        {
            mainMenu = new MenuStrip();
            mainMenu.Dock = DockStyle.Top;

            // Меню Игра
            ToolStripMenuItem gameMenu = new ToolStripMenuItem("Игра");

            ToolStripMenuItem newGameItem = new ToolStripMenuItem("Новая игра");
            newGameItem.ShortcutKeys = Keys.Control | Keys.N;
            newGameItem.Click += (s, e) => NewGame();

            ToolStripMenuItem newGameWithSizeItem = new ToolStripMenuItem("Новая игра с выбором размера...");
            newGameWithSizeItem.Click += (s, e) => ShowBoardSizeDialog();

            ToolStripSeparator sep1 = new ToolStripSeparator();

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Выход");
            exitItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitItem.Click += (s, e) => Application.Exit();

            gameMenu.DropDownItems.AddRange(new ToolStripItem[] { newGameItem, newGameWithSizeItem, sep1, exitItem });

            // Меню Настройки
            ToolStripMenuItem settingsMenu = new ToolStripMenuItem("Настройки");

            ToolStripMenuItem boardSizeItem = new ToolStripMenuItem("Размер поля...");
            boardSizeItem.Click += (s, e) => ShowBoardSizeDialog();

            ToolStripMenuItem colorsItem = new ToolStripMenuItem("Цвета...");
            colorsItem.Click += (s, e) => ShowColorSettingsDialog();

            ToolStripMenuItem changePlayerItem = new ToolStripMenuItem("Сменить игрока...");
            changePlayerItem.Click += (s, e) => ShowLoginDialog();

            settingsMenu.DropDownItems.AddRange(new ToolStripItem[] { boardSizeItem, colorsItem, changePlayerItem });

            // Меню Статистика
            ToolStripMenuItem statsMenu = new ToolStripMenuItem("Статистика");

            ToolStripMenuItem myStatsItem = new ToolStripMenuItem("Моя статистика");
            myStatsItem.Click += (s, e) => ShowPlayerStats();

            ToolStripMenuItem allStatsItem = new ToolStripMenuItem("Все игроки");
            allStatsItem.Click += (s, e) => ShowAllPlayersStats();

            statsMenu.DropDownItems.AddRange(new ToolStripItem[] { myStatsItem, allStatsItem });

            // Меню Справка
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Справка");

            ToolStripMenuItem rulesItem = new ToolStripMenuItem("Правила игры");
            rulesItem.Click += (s, e) => ShowRules();

            ToolStripMenuItem aboutItem = new ToolStripMenuItem("О программе");
            aboutItem.Click += (s, e) => ShowAbout();

            helpMenu.DropDownItems.AddRange(new ToolStripItem[] { rulesItem, aboutItem });

            mainMenu.Items.AddRange(new ToolStripItem[] { gameMenu, settingsMenu, statsMenu, helpMenu });
            this.Controls.Add(mainMenu);
        }

        private void CreateToolStrip()
        {
            toolBar = new ToolStrip();
            toolBar.Dock = DockStyle.Top;

            // Кнопка новой игры
            ToolStripButton btnNewGame = new ToolStripButton("Новая игра");
            btnNewGame.ToolTipText = "Начать новую игру (Ctrl+N)";
            btnNewGame.Click += (s, e) => NewGame();
            toolBar.Items.Add(btnNewGame);

            toolBar.Items.Add(new ToolStripSeparator());

            // Кнопка размера поля
            ToolStripButton btnBoardSize = new ToolStripButton("Размер поля");
            btnBoardSize.ToolTipText = "Выбрать размер поля";
            btnBoardSize.Click += (s, e) => ShowBoardSizeDialog();
            toolBar.Items.Add(btnBoardSize);

            toolBar.Items.Add(new ToolStripSeparator());

            // Метка с именем игрока
            ToolStripLabel lblPlayer = new ToolStripLabel("Игрок: ");
            toolBar.Items.Add(lblPlayer);

            playerNameLabel = new ToolStripLabel("Не авторизован");
            playerNameLabel.ForeColor = Color.Blue;
            toolBar.Items.Add(playerNameLabel);

            this.Controls.Add(toolBar);
        }

        private void CreateGamePanel()
        {
            gamePanel = new Panel();
            gamePanel.Dock = DockStyle.Fill;
            gamePanel.BackColor = Color.White;
            gamePanel.Padding = new Padding(20);
            this.Controls.Add(gamePanel);

            statusLabel = new Label();
            statusLabel.Text = "Ваш ход (X)";
            statusLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            statusLabel.Dock = DockStyle.Top;
            statusLabel.Height = 40;
            statusLabel.BackColor = Color.LightGray;
            gamePanel.Controls.Add(statusLabel);
        }

        private void CreateStatusStrip()
        {
            statusStrip = new StatusStrip();
            statusStripLabel = new ToolStripStatusLabel();
            statusStripLabel.Text = "Готов к игре";
            statusStrip.Items.Add(statusStripLabel);
            this.Controls.Add(statusStrip);
        }

        private void SetupEvents()
        {
            this.Resize += (s, e) => RecreateBoard();
        }

        private void ShowLoginDialog()
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Авторизация";
                dialog.Size = new Size(300, 150);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                Label lblName = new Label();
                lblName.Text = "Введите ваше имя:";
                lblName.Location = new Point(20, 20);
                lblName.AutoSize = true;

                TextBox txtName = new TextBox();
                txtName.Location = new Point(20, 50);
                txtName.Size = new Size(240, 20);

                Button btnOk = new Button();
                btnOk.Text = "OK";
                btnOk.Location = new Point(100, 90);
                btnOk.DialogResult = DialogResult.OK;

                dialog.Controls.Add(lblName);
                dialog.Controls.Add(txtName);
                dialog.Controls.Add(btnOk);

                if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtName.Text))
                {
                    _loggedInPlayer = txtName.Text.Trim();
                    playerNameLabel.Text = _loggedInPlayer;
                    statusStripLabel.Text = $"Игрок: {_loggedInPlayer}";
                }
            }
        }

        private void ShowBoardSizeDialog()
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Выбор размера поля";
                dialog.Size = new Size(300, 150);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                Label lblSize = new Label();
                lblSize.Text = "Размер поля (3-10):";
                lblSize.Location = new Point(20, 20);
                lblSize.AutoSize = true;

                NumericUpDown numSize = new NumericUpDown();
                numSize.Location = new Point(20, 50);
                numSize.Size = new Size(100, 20);
                numSize.Minimum = 3;
                numSize.Maximum = 10;
                numSize.Value = _currentBoardSize;

                Button btnOk = new Button();
                btnOk.Text = "OK";
                btnOk.Location = new Point(200, 50);
                btnOk.DialogResult = DialogResult.OK;

                dialog.Controls.Add(lblSize);
                dialog.Controls.Add(numSize);
                dialog.Controls.Add(btnOk);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _currentBoardSize = (int)numSize.Value;
                    NewGame();
                }
            }
        }

        private void ShowColorSettingsDialog()
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "Настройка цветов";
                dialog.Size = new Size(400, 250);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                int yPos = 20;

                // Цвет игрока
                Label lblPlayerColor = new Label();
                lblPlayerColor.Text = "Цвет крестиков (X):";
                lblPlayerColor.Location = new Point(20, yPos);
                lblPlayerColor.AutoSize = true;

                Button btnPlayerColor = new Button();
                btnPlayerColor.Text = "Выбрать цвет";
                btnPlayerColor.Location = new Point(200, yPos - 3);
                btnPlayerColor.Size = new Size(150, 25);
                btnPlayerColor.BackColor = _playerColor;
                btnPlayerColor.Click += (s, e) =>
                {
                    ColorDialog cd = new ColorDialog();
                    cd.Color = _playerColor;
                    if (cd.ShowDialog() == DialogResult.OK)
                    {
                        _playerColor = cd.Color;
                        btnPlayerColor.BackColor = _playerColor;
                    }
                };

                yPos += 40;

                // Цвет компьютера
                Label lblComputerColor = new Label();
                lblComputerColor.Text = "Цвет ноликов (O):";
                lblComputerColor.Location = new Point(20, yPos);
                lblComputerColor.AutoSize = true;

                Button btnComputerColor = new Button();
                btnComputerColor.Text = "Выбрать цвет";
                btnComputerColor.Location = new Point(200, yPos - 3);
                btnComputerColor.Size = new Size(150, 25);
                btnComputerColor.BackColor = _computerColor;
                btnComputerColor.Click += (s, e) =>
                {
                    ColorDialog cd = new ColorDialog();
                    cd.Color = _computerColor;
                    if (cd.ShowDialog() == DialogResult.OK)
                    {
                        _computerColor = cd.Color;
                        btnComputerColor.BackColor = _computerColor;
                    }
                };

                yPos += 40;

                // Цвет клетки
                Label lblCellColor = new Label();
                lblCellColor.Text = "Цвет клеток:";
                lblCellColor.Location = new Point(20, yPos);
                lblCellColor.AutoSize = true;

                Button btnCellColor = new Button();
                btnCellColor.Text = "Выбрать цвет";
                btnCellColor.Location = new Point(200, yPos - 3);
                btnCellColor.Size = new Size(150, 25);
                btnCellColor.BackColor = _cellColor;
                btnCellColor.Click += (s, e) =>
                {
                    ColorDialog cd = new ColorDialog();
                    cd.Color = _cellColor;
                    if (cd.ShowDialog() == DialogResult.OK)
                    {
                        _cellColor = cd.Color;
                        btnCellColor.BackColor = _cellColor;
                    }
                };

                yPos += 50;

                Button btnOk = new Button();
                btnOk.Text = "Применить";
                btnOk.Location = new Point(150, yPos);
                btnOk.Size = new Size(100, 30);
                btnOk.DialogResult = DialogResult.OK;

                dialog.Controls.Add(lblPlayerColor);
                dialog.Controls.Add(btnPlayerColor);
                dialog.Controls.Add(lblComputerColor);
                dialog.Controls.Add(btnComputerColor);
                dialog.Controls.Add(lblCellColor);
                dialog.Controls.Add(btnCellColor);
                dialog.Controls.Add(btnOk);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    RecreateBoard();
                }
            }
        }

        private void ShowPlayerStats()
        {
            if (string.IsNullOrEmpty(_loggedInPlayer))
            {
                MessageBox.Show("Сначала авторизуйтесь!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ShowLoginDialog();
                return;
            }

            var history = DataManager.GetPlayerHistory(_loggedInPlayer);

            if (history.Count == 0)
            {
                MessageBox.Show($"У игрока {_loggedInPlayer} пока нет сыгранных партий.",
                    "Статистика", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string stats = $"Статистика игрока: {_loggedInPlayer}\n\n";
            stats += $"Всего игр: {history.Count}\n";
            stats += $"Побед: {history.Count(r => r.Result == "Победа")}\n";
            stats += $"Поражений: {history.Count(r => r.Result == "Поражение")}\n";
            stats += $"Ничьих: {history.Count(r => r.Result == "Ничья")}\n\n";
            stats += "Последние игры:\n";
            stats += new string('-', 50) + "\n";

            foreach (var game in history.OrderByDescending(r => r.GameDate).Take(10))
            {
                stats += $"{game.GameDate:dd.MM.yyyy HH:mm} | Поле {game.BoardSize}x{game.BoardSize} | {game.Result} | {game.MovesCount} ходов\n";
            }

            MessageBox.Show(stats, "Моя статистика", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAllPlayersStats()
        {
            var players = DataManager.GetAllPlayers();

            if (players.Count == 0)
            {
                MessageBox.Show("Нет сохранённых данных об игроках.",
                    "Статистика", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string stats = "=== Рейтинг игроков ===\n\n";

            foreach (var player in players.OrderByDescending(p => p.GetWinsCount()))
            {
                stats += $"{player.PlayerName}\n";
                stats += $"  Всего: {player.GameHistory.Count} | Побед: {player.GetWinsCount()} | Поражений: {player.GetLossesCount()} | Ничьих: {player.GetDrawsCount()}\n";
                stats += new string('-', 40) + "\n";
            }

            MessageBox.Show(stats, "Все игроки", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowRules()
        {
            string rules = "Правила игры «Крестики-нолики»\n\n";
            rules += "1. Игра ведётся на квадратном поле.\n";
            rules += "2. Игрок ставит крестики (X), компьютер ставит нолики (O).\n";
            rules += "3. Цель игры — первым выстроить в ряд (по горизонтали, вертикали или диагонали) свои знаки.\n";
            rules += "4. Если все клетки заполнены, а победитель не выявлен, объявляется ничья.\n";
            rules += "5. Размер поля можно выбрать от 3x3 до 10x10.\n";
            rules += "6. Первый ход всегда за игроком.\n\n";
            rules += "Управление:\n";
            rules += "- Нажмите на любую пустую клетку, чтобы сделать ход.\n";
            rules += "- Используйте меню или панель инструментов для настройки игры.";

            MessageBox.Show(rules, "Правила игры", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAbout()
        {
            string about = "Крестики-нолики\n\n";
            about += "Версия: 1.0\n";
            about += "Вариант 19\n\n";
            about += "Особенности:\n";
            about += "- Выбор размера игрового поля от 3x3 до 10x10\n";
            about += "- Интеллектуальный компьютерный противник (алгоритм минимакс)\n";
            about += "- Сохранение результатов игры в бинарный файл\n";
            about += "- Авторизация игроков\n";
            about += "- Настройка цветов\n";
            about += "- Статистика игроков\n\n";
            about += "© 2024";

            MessageBox.Show(about, "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void NewGame()
        {
            _gameLogic = new GameLogic(_currentBoardSize);
            _moveCount = 0;
            _gameActive = true;

            RecreateBoard();
            if (statusLabel != null)
            {
                statusLabel.Text = "Ваш ход (X)";
            }
            if (statusStripLabel != null)
            {
                statusStripLabel.Text = "Игра началась. Ваш ход!";
            }
        }

        private void RecreateBoard()
        {
            // Очищаем панель
            if (gamePanel == null) return;
            gamePanel.Controls.Clear();

            // Добавляем статусную метку обратно
            statusLabel = new Label();
            if (_gameLogic != null && _gameLogic.CurrentState != GameLogic.GameState.Playing)
            {
                statusLabel.Text = GetGameStateMessage();
            }
            else
            {
                statusLabel.Text = "Ваш ход (X)";
            }
            statusLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            statusLabel.Dock = DockStyle.Top;
            statusLabel.Height = 40;
            statusLabel.BackColor = Color.LightGray;
            gamePanel.Controls.Add(statusLabel);

            if (_gameLogic == null) return;

            int boardSize = _gameLogic.Size;
            int panelSize = Math.Min(gamePanel.ClientSize.Width - 40, gamePanel.ClientSize.Height - 60);
            int cellSize = panelSize / boardSize;
            if (cellSize < 10) cellSize = 50;

            // Создаём панель для сетки
            Panel gridPanel = new Panel();
            gridPanel.Size = new Size(cellSize * boardSize, cellSize * boardSize);
            gridPanel.Location = new Point((gamePanel.ClientSize.Width - gridPanel.Width) / 2, 50);
            gridPanel.BackColor = _cellColor;
            gamePanel.Controls.Add(gridPanel);

            cellButtons = new Button[boardSize, boardSize];

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(cellSize - 2, cellSize - 2);
                    btn.Location = new Point(j * cellSize + 1, i * cellSize + 1);
                    btn.Font = new Font("Segoe UI", cellSize / 3, FontStyle.Bold);
                    btn.BackColor = _cellColor;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = _gridColor;

                    int row = i;
                    int col = j;
                    btn.Click += (s, e) => MakeMove(row, col);

                    UpdateButtonText(btn, _gameLogic.Board[i, j]);

                    gridPanel.Controls.Add(btn);
                    cellButtons[i, j] = btn;
                }
            }
        }

        private void UpdateButtonText(Button btn, GameLogic.CellState state)
        {
            if (state == GameLogic.CellState.X)
            {
                btn.Text = "X";
                btn.ForeColor = _playerColor;
                btn.Enabled = false;
            }
            else if (state == GameLogic.CellState.O)
            {
                btn.Text = "O";
                btn.ForeColor = _computerColor;
                btn.Enabled = false;
            }
            else
            {
                btn.Text = "";
                btn.ForeColor = Color.Black;
                if (_gameLogic != null)
                {
                    btn.Enabled = _gameActive && _gameLogic.CurrentState == GameLogic.GameState.Playing;
                }
                else
                {
                    btn.Enabled = false;
                }
            }
        }

        private void UpdateBoardUI()
        {
            if (_gameLogic == null) return;
            for (int i = 0; i < _gameLogic.Size; i++)
            {
                for (int j = 0; j < _gameLogic.Size; j++)
                {
                    if (cellButtons != null && cellButtons[i, j] != null)
                    {
                        UpdateButtonText(cellButtons[i, j], _gameLogic.Board[i, j]);
                    }
                }
            }
        }

        private async void MakeMove(int row, int col)
        {
            if (!_gameActive) return;
            if (_gameLogic == null) return;
            if (_gameLogic.CurrentState != GameLogic.GameState.Playing) return;
            if (_gameLogic.Board[row, col] != GameLogic.CellState.Empty) return;

            // Ход игрока
            if (_gameLogic.MakePlayerMove(row, col))
            {
                _moveCount++;
                UpdateBoardUI();

                // Проверка состояния игры
                if (_gameLogic.CurrentState != GameLogic.GameState.Playing)
                {
                    EndGame();
                    return;
                }

                if (statusLabel != null)
                {
                    statusLabel.Text = "Ход компьютера...";
                }
                if (statusStripLabel != null)
                {
                    statusStripLabel.Text = "Компьютер думает...";
                }
                if (statusLabel != null) statusLabel.Refresh();

                // Небольшая задержка для имитации "мышления"
                await System.Threading.Tasks.Task.Delay(300);

                // Ход компьютера
                if (_gameLogic.MakeComputerMove())
                {
                    _moveCount++;
                    UpdateBoardUI();

                    if (_gameLogic.CurrentState != GameLogic.GameState.Playing)
                    {
                        EndGame();
                        return;
                    }
                }

                if (statusLabel != null)
                {
                    statusLabel.Text = "Ваш ход (X)";
                }
                if (statusStripLabel != null)
                {
                    statusStripLabel.Text = "Ваш ход!";
                }
            }
        }

        private void EndGame()
        {
            _gameActive = false;
            string resultMessage = "";
            string resultType = "";

            if (_gameLogic == null) return;

            switch (_gameLogic.CurrentState)
            {
                case GameLogic.GameState.PlayerWin:
                    resultMessage = "Поздравляем! Вы победили!";
                    resultType = "Победа";
                    if (statusStripLabel != null) statusStripLabel.Text = "Вы выиграли!";
                    break;
                case GameLogic.GameState.ComputerWin:
                    resultMessage = "Компьютер победил. Попробуйте ещё раз!";
                    resultType = "Поражение";
                    if (statusStripLabel != null) statusStripLabel.Text = "Компьютер выиграл...";
                    break;
                case GameLogic.GameState.Draw:
                    resultMessage = "Ничья!";
                    resultType = "Ничья";
                    if (statusStripLabel != null) statusStripLabel.Text = "Ничья!";
                    break;
            }

            if (statusLabel != null)
            {
                statusLabel.Text = resultMessage;
            }

            // Сохраняем результат
            if (!string.IsNullOrEmpty(_loggedInPlayer))
            {
                var result = new GameResult
                {
                    PlayerName = _loggedInPlayer,
                    GameDate = DateTime.Now,
                    BoardSize = _currentBoardSize,
                    Result = resultType,
                    MovesCount = _moveCount
                };
                DataManager.SavePlayerResult(_loggedInPlayer, result);
                if (statusStripLabel != null)
                {
                    statusStripLabel.Text += " Результат сохранён!";
                }
            }

            // Отключаем все кнопки
            if (cellButtons != null && _gameLogic != null)
            {
                for (int i = 0; i < _gameLogic.Size; i++)
                {
                    for (int j = 0; j < _gameLogic.Size; j++)
                    {
                        if (cellButtons[i, j] != null)
                        {
                            cellButtons[i, j].Enabled = false;
                        }
                    }
                }
            }

            // Предложение начать новую игру
            var dialogResult = MessageBox.Show($"{resultMessage}\n\nНачать новую игру?",
                "Игра окончена", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                NewGame();
            }
        }

        private string GetGameStateMessage()
        {
            if (_gameLogic == null) return "Ваш ход (X)";

            switch (_gameLogic.CurrentState)
            {
                case GameLogic.GameState.PlayerWin:
                    return "Вы победили!";
                case GameLogic.GameState.ComputerWin:
                    return "Компьютер победил!";
                case GameLogic.GameState.Draw:
                    return "Ничья!";
                default:
                    return "Ваш ход (X)";
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_gameLogic != null)
            {
                RecreateBoard();
            }
        }
    }
}