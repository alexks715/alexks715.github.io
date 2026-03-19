using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace StudentBodyMassIndex
{
    public partial class Form1 : Form
    {
        // Создаем объект для работы с данными группы
        private GroupData group = new GroupData();

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Загрузка формы - настройка таблицы и диаграммы
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Устанавливаем заголовки вкладок
                tabPage1.Text = "Ввод данных";
                tabPage2.Text = "Таблица данных";
                tabPage3.Text = "Диаграмма";

                // Настройка DataGridView
                dataGridViewStudents.ReadOnly = true;
                dataGridViewStudents.AllowUserToAddRows = false;
                dataGridViewStudents.AutoGenerateColumns = false;
                dataGridViewStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Создаем колонки вручную
                // Колонка для фамилии
                DataGridViewTextBoxColumn colLastName = new DataGridViewTextBoxColumn();
                colLastName.DataPropertyName = "LastName";
                colLastName.HeaderText = "Фамилия";
                colLastName.Name = "LastName";
                dataGridViewStudents.Columns.Add(colLastName);

                // Колонка для веса
                DataGridViewTextBoxColumn colWeight = new DataGridViewTextBoxColumn();
                colWeight.DataPropertyName = "Weight";
                colWeight.HeaderText = "Вес (кг)";
                colWeight.Name = "Weight";
                dataGridViewStudents.Columns.Add(colWeight);

                // Колонка для роста
                DataGridViewTextBoxColumn colHeight = new DataGridViewTextBoxColumn();
                colHeight.DataPropertyName = "Height";
                colHeight.HeaderText = "Рост (м)";
                colHeight.Name = "Height";
                dataGridViewStudents.Columns.Add(colHeight);

                // Колонка для ИМТ (вычисляемое свойство)
                DataGridViewTextBoxColumn colBMI = new DataGridViewTextBoxColumn();
                colBMI.DataPropertyName = "BodyMassIndex";
                colBMI.HeaderText = "ИМТ";
                colBMI.Name = "BMI";
                colBMI.DefaultCellStyle.Format = "F2"; // Формат: два знака после запятой
                dataGridViewStudents.Columns.Add(colBMI);

                // Привязываем данные
                dataGridViewStudents.DataSource = group.GetStudentsList();

                // Первоначальная настройка Chart
                SetupChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке формы: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Настройка внешнего вида диаграммы
        /// </summary>
        private void SetupChart()
        {
            try
            {
                // Полная очистка всего
                chart1.Series.Clear();
                chart1.ChartAreas.Clear();
                chart1.Titles.Clear();

                // 1. Создаем и настраиваем область диаграммы
                ChartArea area = new ChartArea("MainArea");
                area.AxisX.Title = "Студенты";
                area.AxisY.Title = "Индекс массы тела (ИМТ)";
                area.AxisX.Interval = 1;
                area.BackColor = Color.WhiteSmoke;

                // Настройка сетки
                area.AxisX.MajorGrid.Enabled = true;
                area.AxisX.MajorGrid.LineColor = Color.LightGray;
                area.AxisY.MajorGrid.LineColor = Color.LightGray;

                chart1.ChartAreas.Add(area);

                // 2. Создаем серию данных (столбчатую диаграмму)
                Series series = new Series("BMI_Series");
                series.ChartType = SeriesChartType.Column;
                series.ChartArea = "MainArea";
                series.IsValueShownAsLabel = true;
                series.Font = new Font("Microsoft Sans Serif", 8);
                series.Color = Color.SteelBlue;

                // Настройка меток на столбцах
                series.LabelFormat = "F2";
                series.LabelForeColor = Color.Black;

                chart1.Series.Add(series);

                // 3. Добавляем заголовок
                Title title = new Title();
                title.Text = "Нет данных для отображения";
                title.Docking = Docking.Top;
                title.Font = new Font("Microsoft Sans Serif", 10);
                title.ForeColor = Color.Black;
                chart1.Titles.Add(title);

                // Заполняем диаграмму данными
                SafeUpdateChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при настройке диаграммы: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// БЕЗОПАСНОЕ обновление данных на диаграмме (без ошибок с индексами)
        /// </summary>
        private void SafeUpdateChart()
        {
            try
            {
                // Проверяем, что chart1 существует
                if (chart1 == null) return;

                // Проверяем, есть ли серии
                if (chart1.Series.Count == 0)
                {
                    // Если нет серий, создаем заново
                    SetupChart();
                    return;
                }

                // Получаем серию по имени
                Series series = null;
                foreach (Series s in chart1.Series)
                {
                    if (s.Name == "BMI_Series")
                    {
                        series = s;
                        break;
                    }
                }

                // Если серия не найдена, создаем новую
                if (series == null)
                {
                    series = new Series("BMI_Series");
                    series.ChartType = SeriesChartType.Column;
                    series.ChartArea = "MainArea";
                    series.IsValueShownAsLabel = true;
                    chart1.Series.Add(series);
                }

                // Очищаем старые точки
                series.Points.Clear();

                // Проверяем, есть ли данные
                if (group == null || group.Count == 0)
                {
                    // Обновляем заголовок
                    if (chart1.Titles.Count > 0)
                    {
                        chart1.Titles[0].Text = "Нет данных для отображения";
                    }
                    return;
                }

                // Добавляем новые точки данных для каждого студента
                List<Student> students = group.GetStudentsList();
                int pointIndex = 0;

                foreach (Student student in students)
                {
                    if (student != null && !string.IsNullOrEmpty(student.LastName))
                    {
                        // Добавляем точку
                        series.Points.AddXY(student.LastName, student.BodyMassIndex);

                        // Настраиваем точку
                        series.Points[pointIndex].ToolTip = $"{student.LastName}: ИМТ = {student.BodyMassIndex:F2}";

                        // Настраиваем цвет
                        if (student.BodyMassIndex < 17)
                        {
                            series.Points[pointIndex].Color = Color.IndianRed;
                        }

                        pointIndex++;
                    }
                }

                // Обновляем заголовок диаграммы
                if (chart1.Titles.Count > 0)
                {
                    if (!string.IsNullOrEmpty(group.GroupNumber))
                    {
                        chart1.Titles[0].Text = $"Сравнение студентов группы {group.GroupNumber} по ИМТ";
                    }
                    else
                    {
                        chart1.Titles[0].Text = "Сравнение студентов по ИМТ";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении диаграммы: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Добавить студента"
        /// </summary>
        private void buttonAddStudent_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, что все поля заполнены
                if (string.IsNullOrWhiteSpace(textBoxLastName.Text))
                {
                    MessageBox.Show("Введите фамилию студента!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBoxWeight.Text))
                {
                    MessageBox.Show("Введите вес студента!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBoxHeight.Text))
                {
                    MessageBox.Show("Введите рост студента!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получаем номер группы (может быть пустым)
                string groupNumber = textBoxGroupNumber.Text.Trim();
                if (!string.IsNullOrEmpty(groupNumber))
                {
                    group.GroupNumber = groupNumber;
                }

                // Получаем фамилию
                string lastName = textBoxLastName.Text.Trim();

                // Получаем вес (пробуем разные разделители)
                double weight;
                string weightText = textBoxWeight.Text.Trim().Replace('.', ',');
                if (!double.TryParse(weightText, out weight) || weight <= 0)
                {
                    MessageBox.Show("Вес должен быть положительным числом (например: 70,5)", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Получаем рост (пробуем разные разделители)
                double height;
                string heightText = textBoxHeight.Text.Trim().Replace('.', ',');
                if (!double.TryParse(heightText, out height) || height <= 0)
                {
                    MessageBox.Show("Рост должен быть положительным числом (например: 1,75)", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверка на разумные пределы
                if (weight < 20 || weight > 200)
                {
                    DialogResult result = MessageBox.Show("Вы уверены, что вес указан корректно?\n" +
                        "Нажмите 'Да' для продолжения или 'Нет' для исправления.",
                        "Проверка веса", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                        return;
                }

                if (height < 0.5 || height > 2.5)
                {
                    DialogResult result = MessageBox.Show("Вы уверены, что рост указан корректно?\n" +
                        "Нажмите 'Да' для продолжения или 'Нет' для исправления.",
                        "Проверка роста", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                        return;
                }

                // СОЗДАЕМ НОВОГО СТУДЕНТА
                Student newStudent = new Student(lastName, weight, height);

                // ДОБАВЛЯЕМ В СПИСОК
                group.AddStudent(newStudent);

                // Очищаем поля для следующего ввода (кроме номера группы)
                textBoxLastName.Clear();
                textBoxWeight.Clear();
                textBoxHeight.Clear();

                // ВОЗВРАЩАЕМ ФОКУС НА ПОЛЕ ФАМИЛИИ
                textBoxLastName.Focus();

                // ОБНОВЛЯЕМ ТАБЛИЦУ
                dataGridViewStudents.DataSource = null;
                dataGridViewStudents.DataSource = group.GetStudentsList();

                // ОБНОВЛЯЕМ ДИАГРАММУ (безопасно)
                SafeUpdateChart();

                // ПОКАЗЫВАЕМ СООБЩЕНИЕ
                MessageBox.Show($"Студент {lastName} успешно добавлен!\nВсего студентов: {group.Count}",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении студента: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Найти с дефицитом веса"
        /// </summary>
        private void buttonFindUnderweight_Click(object sender, EventArgs e)
        {
            try
            {
                // Очищаем список
                listBoxUnderweight.Items.Clear();

                if (group == null || group.Count == 0)
                {
                    listBoxUnderweight.Items.Add("Нет студентов для поиска");
                    return;
                }

                // Получаем список студентов с дефицитом веса
                List<Student> underweightStudents = group.FindUnderweightStudents();

                if (underweightStudents.Count == 0)
                {
                    listBoxUnderweight.Items.Add("Студенты с дефицитом веса не найдены.");
                    listBoxUnderweight.Items.Add("ИМТ считается дефицитным при значении меньше 17.");
                }
                else
                {
                    listBoxUnderweight.Items.Add($"Найдено студентов с дефицитом веса: {underweightStudents.Count}");
                    listBoxUnderweight.Items.Add(""); // Пустая строка для разделения
                    listBoxUnderweight.Items.Add("Фамилия (ИМТ)");
                    listBoxUnderweight.Items.Add("-------------------");

                    foreach (Student s in underweightStudents)
                    {
                        listBoxUnderweight.Items.Add($"{s.LastName} (ИМТ: {s.BodyMassIndex:F2})");
                    }

                    listBoxUnderweight.Items.Add("");
                    listBoxUnderweight.Items.Add("Рекомендация: проконсультироваться с врачом");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Сохранить в файл"
        /// </summary>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, есть ли что сохранять
                if (group == null || group.Count == 0)
                {
                    MessageBox.Show("Нет данных для сохранения. Сначала добавьте студентов.",
                        "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Используем SaveFileDialog для выбора места сохранения
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.RestoreDirectory = true;
                    saveFileDialog.Title = "Сохранить данные группы";
                    saveFileDialog.DefaultExt = "txt";

                    // Предлагаем имя по умолчанию
                    string defaultName = string.IsNullOrWhiteSpace(group.GroupNumber) ?
                        "Group" : $"Group_{group.GroupNumber}";
                    saveFileDialog.FileName = $"{defaultName}_{DateTime.Now:yyyyMMdd}.txt";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        group.SaveToFile(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Загрузить из файла"
        /// </summary>
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            try
            {
                // Используем OpenFileDialog для выбора файла
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.Title = "Загрузить данные группы";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        group.LoadFromFile(openFileDialog.FileName);

                        // После загрузки данных обновляем интерфейс
                        textBoxGroupNumber.Text = group.GroupNumber;

                        dataGridViewStudents.DataSource = null;
                        dataGridViewStudents.DataSource = group.GetStudentsList();

                        SafeUpdateChart();

                        // Очищаем список найденных студентов
                        listBoxUnderweight.Items.Clear();

                        // Переключаемся на вкладку с таблицей
                        tabControl1.SelectedTab = tabPage2;

                        MessageBox.Show($"Загружено {group.Count} студентов!",
                            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки очистки
        /// </summary>
        private void buttonClear_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Очистить все данные?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    group = new GroupData();
                    textBoxGroupNumber.Clear();
                    textBoxLastName.Clear();
                    textBoxWeight.Clear();
                    textBoxHeight.Clear();
                    listBoxUnderweight.Items.Clear();

                    dataGridViewStudents.DataSource = null;
                    dataGridViewStudents.DataSource = group.GetStudentsList();

                    SafeUpdateChart();

                    MessageBox.Show("Все данные очищены!",
                        "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при очистке: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчики для поддержки ввода чисел
        private void textBoxHeight_TextChanged(object sender, EventArgs e) { }

        private void NumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем цифры, запятую/точку, backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Разрешаем только одну запятую/точку
            if ((e.KeyChar == ',' || e.KeyChar == '.') &&
                ((sender as TextBox).Text.Contains(',') || (sender as TextBox).Text.Contains('.')))
            {
                e.Handled = true;
            }
        }
    }
}