using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using VectorEditor.Classes;
using VectorEditor.Figures;

namespace VectorEditor
{
    public partial class Form1 : Form
    {
        // Элементы управления
        private MenuStrip mainMenu;
        private ToolStrip toolBar;
        private PictureBox canvas;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        // Данные редактора
        private List<Figure> _figures = new List<Figure>();
        private Figure _selectedFigure = null;
        private Figure _clipboardFigure = null;
        private StackMemory _undoStack;
        private StackMemory _redoStack;
        private Type _currentFigureType = typeof(Hexagon);
        private int _currentStarPoints = 5;

        // Переменные для рисования
        private Point _startPoint;
        private bool _isDrawing = false;

        public Form1()
        {
            // НЕ вызываем InitializeComponent() здесь, так как она будет вызвана в Form1.Designer.cs
            // Вместо этого настраиваем форму
            this.Text = "Векторный редактор - Вариант 19 (Шестиугольник и Звёзды)";
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;
            this.BackColor = SystemColors.Control;

            InitializeCustomComponents();
            SetupEvents();
            _undoStack = new StackMemory(10);
            _redoStack = new StackMemory(10);
            SaveState();
        }

        /// <summary>
        /// Создание и настройка всех элементов управления
        /// </summary>
        private void InitializeCustomComponents()
        {
            // Создание меню
            CreateMenuStrip();

            // Создание панели инструментов
            CreateToolStrip();

            // Создание полотна для рисования
            CreateCanvas();

            // Создание строки состояния
            CreateStatusStrip();
        }

        /// <summary>
        /// Создание меню
        /// </summary>
        private void CreateMenuStrip()
        {
            mainMenu = new MenuStrip();
            mainMenu.Dock = DockStyle.Top;

            // Меню Файл
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Файл");

            ToolStripMenuItem saveItem = new ToolStripMenuItem("Сохранить");
            saveItem.ShortcutKeys = Keys.Control | Keys.S;
            saveItem.Click += SaveToolStripMenuItem_Click;

            ToolStripMenuItem loadItem = new ToolStripMenuItem("Загрузить");
            loadItem.ShortcutKeys = Keys.Control | Keys.O;
            loadItem.Click += LoadToolStripMenuItem_Click;

            ToolStripSeparator separator1 = new ToolStripSeparator();

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Выход");
            exitItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitItem.Click += (s, e) => Application.Exit();

            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { saveItem, loadItem, separator1, exitItem });

            // Меню Правка
            ToolStripMenuItem editMenu = new ToolStripMenuItem("Правка");

            ToolStripMenuItem undoItem = new ToolStripMenuItem("Отменить");
            undoItem.ShortcutKeys = Keys.Control | Keys.Z;
            undoItem.Click += Undo_Click;

            ToolStripMenuItem redoItem = new ToolStripMenuItem("Повторить");
            redoItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoItem.Click += Redo_Click;

            ToolStripSeparator separator2 = new ToolStripSeparator();

            ToolStripMenuItem copyItem = new ToolStripMenuItem("Копировать");
            copyItem.ShortcutKeys = Keys.Control | Keys.C;
            copyItem.Click += Copy_Click;

            ToolStripMenuItem cutItem = new ToolStripMenuItem("Вырезать");
            cutItem.ShortcutKeys = Keys.Control | Keys.X;
            cutItem.Click += Cut_Click;

            ToolStripMenuItem pasteItem = new ToolStripMenuItem("Вставить");
            pasteItem.ShortcutKeys = Keys.Control | Keys.V;
            pasteItem.Click += Paste_Click;

            ToolStripSeparator separator3 = new ToolStripSeparator();

            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Удалить");
            deleteItem.ShortcutKeys = Keys.Delete;
            deleteItem.Click += (s, e) => DeleteSelectedFigure();

            editMenu.DropDownItems.AddRange(new ToolStripItem[] { undoItem, redoItem, separator2, copyItem, cutItem, pasteItem, separator3, deleteItem });

            // Меню Фигура
            ToolStripMenuItem shapeMenu = new ToolStripMenuItem("Фигура");

            ToolStripMenuItem hexagonItem = new ToolStripMenuItem("Шестиугольник");
            hexagonItem.Click += (s, e) => SetCurrentFigureType(typeof(Hexagon));

            ToolStripMenuItem star4Item = new ToolStripMenuItem("Звезда 4-х конечная");
            star4Item.Click += (s, e) => { SetCurrentFigureType(typeof(Star)); _currentStarPoints = 4; };

            ToolStripMenuItem star5Item = new ToolStripMenuItem("Звезда 5-ти конечная");
            star5Item.Click += (s, e) => { SetCurrentFigureType(typeof(Star)); _currentStarPoints = 5; };

            ToolStripMenuItem star6Item = new ToolStripMenuItem("Звезда 6-ти конечная");
            star6Item.Click += (s, e) => { SetCurrentFigureType(typeof(Star)); _currentStarPoints = 6; };

            shapeMenu.DropDownItems.AddRange(new ToolStripItem[] { hexagonItem, star4Item, star5Item, star6Item });

            // Меню Свойства
            ToolStripMenuItem propertiesMenu = new ToolStripMenuItem("Свойства");

            ToolStripMenuItem strokeColorItem = new ToolStripMenuItem("Цвет контура...");
            strokeColorItem.Click += StrokeColor_Click;

            ToolStripMenuItem strokeWidthItem = new ToolStripMenuItem("Толщина контура...");
            strokeWidthItem.Click += StrokeWidth_Click;

            ToolStripSeparator separator4 = new ToolStripSeparator();

            ToolStripMenuItem fillColorItem = new ToolStripMenuItem("Цвет заливки...");
            fillColorItem.Click += FillColor_Click;

            ToolStripMenuItem removeFillItem = new ToolStripMenuItem("Убрать заливку");
            removeFillItem.Click += RemoveFill_Click;

            propertiesMenu.DropDownItems.AddRange(new ToolStripItem[] { strokeColorItem, strokeWidthItem, separator4, fillColorItem, removeFillItem });

            // Меню Справка
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Справка");

            ToolStripMenuItem aboutItem = new ToolStripMenuItem("О программе...");
            aboutItem.Click += (s, e) => MessageBox.Show(
                "Векторный редактор\nВариант 19\n\nФигуры:\n- Правильный шестиугольник\n- Звезда 4-х конечная\n- Звезда 5-ти конечная\n- Звезда 6-ти конечная\n\nДополнительная функция: Заливка фигуры",
                "О программе",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            helpMenu.DropDownItems.Add(aboutItem);

            mainMenu.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, shapeMenu, propertiesMenu, helpMenu });
            this.Controls.Add(mainMenu);
        }

        /// <summary>
        /// Создание панели инструментов
        /// </summary>
        private void CreateToolStrip()
        {
            toolBar = new ToolStrip();
            toolBar.Dock = DockStyle.Top;

            // Кнопка шестиугольника
            ToolStripButton btnHexagon = new ToolStripButton("Шестиугольник");
            btnHexagon.ToolTipText = "Рисовать шестиугольник";
            btnHexagon.Click += (s, e) => SetCurrentFigureType(typeof(Hexagon));
            toolBar.Items.Add(btnHexagon);

            // Кнопка звезды 4
            ToolStripButton btnStar4 = new ToolStripButton("Звезда 4");
            btnStar4.ToolTipText = "Рисовать 4-х конечную звезду";
            btnStar4.Click += (s, e) => { SetCurrentFigureType(typeof(Star)); _currentStarPoints = 4; };
            toolBar.Items.Add(btnStar4);

            // Кнопка звезды 5
            ToolStripButton btnStar5 = new ToolStripButton("Звезда 5");
            btnStar5.ToolTipText = "Рисовать 5-ти конечную звезду";
            btnStar5.Click += (s, e) => { SetCurrentFigureType(typeof(Star)); _currentStarPoints = 5; };
            toolBar.Items.Add(btnStar5);

            // Кнопка звезды 6
            ToolStripButton btnStar6 = new ToolStripButton("Звезда 6");
            btnStar6.ToolTipText = "Рисовать 6-ти конечную звезду";
            btnStar6.Click += (s, e) => { SetCurrentFigureType(typeof(Star)); _currentStarPoints = 6; };
            toolBar.Items.Add(btnStar6);

            // Разделитель
            toolBar.Items.Add(new ToolStripSeparator());

            // Кнопка отмены
            ToolStripButton btnUndo = new ToolStripButton("Отменить");
            btnUndo.ToolTipText = "Отменить действие (Ctrl+Z)";
            btnUndo.Click += (s, e) => Undo_Click(s, e);
            toolBar.Items.Add(btnUndo);

            // Кнопка повтора
            ToolStripButton btnRedo = new ToolStripButton("Повторить");
            btnRedo.ToolTipText = "Повторить действие (Ctrl+Y)";
            btnRedo.Click += (s, e) => Redo_Click(s, e);
            toolBar.Items.Add(btnRedo);

            // Разделитель
            toolBar.Items.Add(new ToolStripSeparator());

            // Кнопка копирования
            ToolStripButton btnCopy = new ToolStripButton("Копировать");
            btnCopy.ToolTipText = "Копировать фигуру (Ctrl+C)";
            btnCopy.Click += (s, e) => Copy_Click(s, e);
            toolBar.Items.Add(btnCopy);

            // Кнопка вырезания
            ToolStripButton btnCut = new ToolStripButton("Вырезать");
            btnCut.ToolTipText = "Вырезать фигуру (Ctrl+X)";
            btnCut.Click += (s, e) => Cut_Click(s, e);
            toolBar.Items.Add(btnCut);

            // Кнопка вставки
            ToolStripButton btnPaste = new ToolStripButton("Вставить");
            btnPaste.ToolTipText = "Вставить фигуру (Ctrl+V)";
            btnPaste.Click += (s, e) => Paste_Click(s, e);
            toolBar.Items.Add(btnPaste);

            // Разделитель
            toolBar.Items.Add(new ToolStripSeparator());

            // Кнопка цвета контура
            ToolStripButton btnStrokeColor = new ToolStripButton("Цвет контура");
            btnStrokeColor.ToolTipText = "Изменить цвет контура выделенной фигуры";
            btnStrokeColor.Click += (s, e) => StrokeColor_Click(s, e);
            toolBar.Items.Add(btnStrokeColor);

            // Кнопка цвета заливки
            ToolStripButton btnFillColor = new ToolStripButton("Цвет заливки");
            btnFillColor.ToolTipText = "Изменить цвет заливки выделенной фигуры";
            btnFillColor.Click += (s, e) => FillColor_Click(s, e);
            toolBar.Items.Add(btnFillColor);

            // Кнопка удалить заливку
            ToolStripButton btnRemoveFill = new ToolStripButton("Убрать заливку");
            btnRemoveFill.ToolTipText = "Убрать заливку выделенной фигуры";
            btnRemoveFill.Click += (s, e) => RemoveFill_Click(s, e);
            toolBar.Items.Add(btnRemoveFill);

            this.Controls.Add(toolBar);
        }

        /// <summary>
        /// Создание полотна для рисования
        /// </summary>
        private void CreateCanvas()
        {
            canvas = new PictureBox();
            canvas.BackColor = Color.White;
            canvas.Dock = DockStyle.Fill;
            canvas.BorderStyle = BorderStyle.Fixed3D;

            // События
            canvas.Paint += Canvas_Paint;
            canvas.MouseClick += Canvas_MouseClick;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;

            this.Controls.Add(canvas);
        }

        /// <summary>
        /// Создание строки состояния
        /// </summary>
        private void CreateStatusStrip()
        {
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            statusLabel.Text = "Готов к работе. Выберите фигуру и начните рисовать на полотне.";
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
        }

        /// <summary>
        /// Настройка событий формы
        /// </summary>
        private void SetupEvents()
        {
            this.KeyDown += Form1_KeyDown;
            this.Resize += (s, e) => canvas.Invalidate();
        }

        private void SetCurrentFigureType(Type type)
        {
            _currentFigureType = type;
            string shapeName = "";
            if (type == typeof(Hexagon)) shapeName = "Шестиугольник";
            else if (type == typeof(Star)) shapeName = $"Звезда ({_currentStarPoints} луча)";
            statusLabel.Text = $"Текущая фигура: {shapeName}";
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Рисуем все фигуры
            foreach (var figure in _figures)
            {
                figure.Draw(e.Graphics);
            }

            // Рисуем маркеры выделения
            foreach (var figure in _figures)
            {
                if (figure.IsSelected)
                {
                    figure.DrawSelectionMarkers(e.Graphics);
                }
            }

            // Показываем предварительный просмотр при рисовании
            if (_isDrawing)
            {
                using (Pen pen = new Pen(Color.Gray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    Rectangle previewRect = new Rectangle(
                        Math.Min(_startPoint.X, this.PointToClient(Cursor.Position).X),
                        Math.Min(_startPoint.Y, this.PointToClient(Cursor.Position).Y),
                        Math.Abs(_startPoint.X - this.PointToClient(Cursor.Position).X),
                        Math.Abs(_startPoint.Y - this.PointToClient(Cursor.Position).Y)
                    );
                    e.Graphics.DrawRectangle(pen, previewRect);
                }
            }
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Проверяем, попали ли в какую-то фигуру
                Figure clickedFigure = null;
                foreach (var figure in _figures.AsEnumerable().Reverse())
                {
                    if (figure.HitTest(e.Location))
                    {
                        clickedFigure = figure;
                        break;
                    }
                }

                // Снимаем выделение со всех
                foreach (var figure in _figures)
                {
                    figure.IsSelected = false;
                }

                if (clickedFigure != null)
                {
                    clickedFigure.IsSelected = true;
                    _selectedFigure = clickedFigure;
                    statusLabel.Text = $"Выделена фигура: {clickedFigure.GetType().Name}";
                }
                else
                {
                    _selectedFigure = null;
                    statusLabel.Text = "Готов к работе. Выберите фигуру и начните рисовать на полотне.";
                }

                canvas.Invalidate();
            }
            else if (e.Button == MouseButtons.Right && _selectedFigure != null)
            {
                // Контекстное меню для удаления фигуры
                var result = MessageBox.Show("Удалить выделенную фигуру?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DeleteSelectedFigure();
                }
            }
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _selectedFigure == null)
            {
                _startPoint = e.Location;
                _isDrawing = true;
                statusLabel.Text = "Нажмите и растяните для создания фигуры";
            }
            else if (e.Button == MouseButtons.Left && _selectedFigure != null)
            {
                // Начинаем перемещение выделенной фигуры
                _startPoint = e.Location;
                _isDrawing = false;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                canvas.Invalidate();
            }
            else if (_selectedFigure != null && e.Button == MouseButtons.Left)
            {
                // Перемещение фигуры
                int dx = e.X - _startPoint.X;
                int dy = e.Y - _startPoint.Y;
                if (dx != 0 || dy != 0)
                {
                    SaveState();
                    _selectedFigure.MoveBy(dx, dy);
                    _startPoint = e.Location;
                    canvas.Invalidate();
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                SaveState();

                Rectangle bounds = new Rectangle(
                    Math.Min(_startPoint.X, e.Location.X),
                    Math.Min(_startPoint.Y, e.Location.Y),
                    Math.Abs(_startPoint.X - e.Location.X),
                    Math.Abs(_startPoint.Y - e.Location.Y)
                );

                if (bounds.Width > 5 && bounds.Height > 5)
                {
                    Figure newFigure = null;

                    if (_currentFigureType == typeof(Hexagon))
                    {
                        newFigure = new Hexagon(bounds);
                    }
                    else if (_currentFigureType == typeof(Star))
                    {
                        newFigure = new Star(bounds, _currentStarPoints);
                    }

                    if (newFigure != null)
                    {
                        _figures.Add(newFigure);
                        canvas.Invalidate();
                        statusLabel.Text = $"Создана новая фигура: {newFigure.GetType().Name}";
                    }
                }

                _isDrawing = false;
            }
        }

        private void DeleteSelectedFigure()
        {
            if (_selectedFigure != null)
            {
                SaveState();
                _figures.Remove(_selectedFigure);
                _selectedFigure = null;
                canvas.Invalidate();
                statusLabel.Text = "Фигура удалена";
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (_selectedFigure != null)
            {
                int step = e.Shift ? 1 : 5;
                bool moved = false;

                switch (e.KeyCode)
                {
                    case Keys.Left:
                        SaveState();
                        _selectedFigure.MoveBy(-step, 0);
                        moved = true;
                        break;
                    case Keys.Right:
                        SaveState();
                        _selectedFigure.MoveBy(step, 0);
                        moved = true;
                        break;
                    case Keys.Up:
                        SaveState();
                        _selectedFigure.MoveBy(0, -step);
                        moved = true;
                        break;
                    case Keys.Down:
                        SaveState();
                        _selectedFigure.MoveBy(0, step);
                        moved = true;
                        break;
                    case Keys.Delete:
                        DeleteSelectedFigure();
                        moved = true;
                        break;
                }

                if (moved)
                {
                    canvas.Invalidate();
                    statusLabel.Text = $"Фигура перемещена на {step} пикселей";
                    e.Handled = true;
                }
            }

            // Глобальные горячие клавиши
            if (e.Control && e.KeyCode == Keys.Z)
            {
                Undo_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                Redo_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                Copy_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.X)
            {
                Cut_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                Paste_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                SaveToolStripMenuItem_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                LoadToolStripMenuItem_Click(sender, e);
                e.Handled = true;
            }
        }

        private void SaveState()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                SaveToStream(ms, _figures);
                _undoStack.Push(ms);
            }
            _redoStack.Clear();
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            if (_undoStack.Count > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    _undoStack.Pop(ms);

                    using (MemoryStream currentMs = new MemoryStream())
                    {
                        SaveToStream(currentMs, _figures);
                        _redoStack.Push(currentMs);
                    }

                    _figures = LoadFromStream(ms).ToList();
                    _selectedFigure = null;
                    canvas.Invalidate();
                    statusLabel.Text = "Отменено последнее действие";
                }
            }
            else
            {
                statusLabel.Text = "Нечего отменять";
            }
        }

        private void Redo_Click(object sender, EventArgs e)
        {
            if (_redoStack.Count > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    _redoStack.Pop(ms);

                    using (MemoryStream currentMs = new MemoryStream())
                    {
                        SaveToStream(currentMs, _figures);
                        _undoStack.Push(currentMs);
                    }

                    _figures = LoadFromStream(ms).ToList();
                    _selectedFigure = null;
                    canvas.Invalidate();
                    statusLabel.Text = "Повторено действие";
                }
            }
            else
            {
                statusLabel.Text = "Нечего повторять";
            }
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            if (_selectedFigure != null)
            {
                _clipboardFigure = _selectedFigure.Clone();
                statusLabel.Text = $"Скопирована фигура: {_selectedFigure.GetType().Name}";
            }
            else
            {
                statusLabel.Text = "Нет выделенной фигуры для копирования";
            }
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            if (_selectedFigure != null)
            {
                SaveState();
                _clipboardFigure = _selectedFigure.Clone();
                _figures.Remove(_selectedFigure);
                _selectedFigure = null;
                canvas.Invalidate();
                statusLabel.Text = "Фигура вырезана";
            }
            else
            {
                statusLabel.Text = "Нет выделенной фигуры для вырезания";
            }
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            if (_clipboardFigure != null)
            {
                SaveState();
                Figure newFigure = _clipboardFigure.Clone();
                newFigure.MoveBy(20, 20);
                _figures.Add(newFigure);
                canvas.Invalidate();
                statusLabel.Text = "Фигура вставлена";
            }
            else
            {
                statusLabel.Text = "Буфер обмена пуст";
            }
        }

        private void StrokeColor_Click(object sender, EventArgs e)
        {
            if (_selectedFigure != null)
            {
                ColorDialog cd = new ColorDialog();
                cd.Color = _selectedFigure.Stroke.Color;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    SaveState();
                    _selectedFigure.Stroke.Color = cd.Color;
                    canvas.Invalidate();
                    statusLabel.Text = "Цвет контура изменён";
                }
            }
            else
            {
                MessageBox.Show("Сначала выделите фигуру!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void StrokeWidth_Click(object sender, EventArgs e)
        {
            if (_selectedFigure != null)
            {
                using (Form inputForm = new Form())
                {
                    inputForm.Text = "Толщина контура";
                    inputForm.Size = new Size(300, 150);
                    inputForm.StartPosition = FormStartPosition.CenterParent;
                    inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    inputForm.MaximizeBox = false;
                    inputForm.MinimizeBox = false;

                    Label label = new Label();
                    label.Text = "Введите толщину контура (1-10):";
                    label.Location = new Point(20, 20);
                    label.AutoSize = true;

                    NumericUpDown numWidth = new NumericUpDown();
                    numWidth.Location = new Point(20, 50);
                    numWidth.Size = new Size(100, 20);
                    numWidth.Minimum = 1;
                    numWidth.Maximum = 10;
                    numWidth.Value = (decimal)_selectedFigure.Stroke.Width;
                    numWidth.DecimalPlaces = 1;
                    numWidth.Increment = 0.5m;

                    Button okButton = new Button();
                    okButton.Text = "OK";
                    okButton.Location = new Point(200, 50);
                    okButton.DialogResult = DialogResult.OK;

                    inputForm.Controls.Add(label);
                    inputForm.Controls.Add(numWidth);
                    inputForm.Controls.Add(okButton);

                    if (inputForm.ShowDialog() == DialogResult.OK)
                    {
                        SaveState();
                        _selectedFigure.Stroke.Width = (float)numWidth.Value;
                        canvas.Invalidate();
                        statusLabel.Text = $"Толщина контура изменена на {numWidth.Value}";
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выделите фигуру!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FillColor_Click(object sender, EventArgs e)
        {
            if (_selectedFigure != null)
            {
                ColorDialog cd = new ColorDialog();
                if (_selectedFigure.Fill.IsFilled)
                    cd.Color = _selectedFigure.Fill.Color;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    SaveState();
                    _selectedFigure.Fill.Color = cd.Color;
                    _selectedFigure.Fill.IsFilled = true;
                    canvas.Invalidate();
                    statusLabel.Text = "Цвет заливки изменён";
                }
            }
            else
            {
                MessageBox.Show("Сначала выделите фигуру!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RemoveFill_Click(object sender, EventArgs e)
        {
            if (_selectedFigure != null)
            {
                SaveState();
                _selectedFigure.Fill.IsFilled = false;
                canvas.Invalidate();
                statusLabel.Text = "Заливка удалена";
            }
            else
            {
                MessageBox.Show("Сначала выделите фигуру!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Vector Editor Files (*.vec)|*.vec|All files (*.*)|*.*";
            sfd.DefaultExt = "vec";
            sfd.Title = "Сохранить рисунок";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        SaveToStream(fs, _figures);
                    }
                    statusLabel.Text = $"Сохранено в файл: {sfd.FileName}";
                    MessageBox.Show("Сохранено успешно!", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Vector Editor Files (*.vec)|*.vec|All files (*.*)|*.*";
            ofd.Title = "Загрузить рисунок";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                    {
                        _figures = LoadFromStream(fs).ToList();
                        _selectedFigure = null;
                        _undoStack.Clear();
                        _redoStack.Clear();
                        canvas.Invalidate();
                    }
                    statusLabel.Text = $"Загружено из файла: {ofd.FileName}";
                    MessageBox.Show("Загружено успешно!", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveToStream(Stream stream, List<Figure> listToSave = null)
        {
            var formatter = new BinaryFormatter();
            var list = (listToSave ?? _figures).ToList();
            formatter.Serialize(stream, list);
            stream.Position = 0;
        }

        private static IEnumerable<Figure> LoadFromStream(Stream stream)
        {
            try
            {
                var formatter = new BinaryFormatter();
                stream.Position = 0;
                return (List<Figure>)formatter.Deserialize(stream);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка десериализации: {e.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Figure>();
            }
        }
    }
}