using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace StudentBodyMassIndex
{
    /// <summary>
    /// Класс для управления списком студентов группы
    /// </summary>
    public class GroupData
    {
        // Приватные поля
        private List<Student> students;
        private string groupNumber;

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public GroupData()
        {
            students = new List<Student>();
            groupNumber = "Не указана";
        }

        /// <summary>
        /// Конструктор для загрузки данных из файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public GroupData(string fileName)
        {
            students = new List<Student>();
            groupNumber = "Не указана";
            LoadFromFile(fileName);
        }

        // Свойства
        public string GroupNumber
        {
            get { return groupNumber; }
            set { groupNumber = value; }
        }

        public int Count
        {
            get { return students.Count; }
        }

        /// <summary>
        /// Индексатор для доступа к студенту по индексу
        /// </summary>
        public Student this[int index]
        {
            get
            {
                if (index >= 0 && index < students.Count)
                    return students[index];
                else
                    return null;
            }
        }

        /// <summary>
        /// Добавление нового студента в группу
        /// </summary>
        public void AddStudent(Student s)
        {
            if (s != null)
            {
                students.Add(s);
            }
        }

        /// <summary>
        /// Удаление студента по индексу
        /// </summary>
        public void RemoveStudent(int index)
        {
            if (index >= 0 && index < students.Count)
                students.RemoveAt(index);
        }

        /// <summary>
        /// Удаление студента по объекту
        /// </summary>
        public void RemoveStudent(Student s)
        {
            if (s != null && students.Contains(s))
            {
                students.Remove(s);
            }
        }

        /// <summary>
        /// Очистка списка студентов
        /// </summary>
        public void Clear()
        {
            students.Clear();
            groupNumber = "Не указана";
        }

        /// <summary>
        /// Получить список всех студентов (для привязки данных)
        /// </summary>
        public List<Student> GetStudentsList()
        {
            return students;
        }

        /// <summary>
        /// Получить массив фамилий всех студентов
        /// </summary>
        public string[] GetLastNames()
        {
            string[] names = new string[students.Count];
            for (int i = 0; i < students.Count; i++)
            {
                names[i] = students[i].LastName;
            }
            return names;
        }

        /// <summary>
        /// Получить массив значений ИМТ всех студентов
        /// </summary>
        public double[] GetBMIValues()
        {
            double[] bmis = new double[students.Count];
            for (int i = 0; i < students.Count; i++)
            {
                bmis[i] = students[i].BodyMassIndex;
            }
            return bmis;
        }

        /// <summary>
        /// Поиск студентов с дефицитом массы тела (ИМТ < 17)
        /// </summary>
        public List<Student> FindUnderweightStudents()
        {
            List<Student> underweight = new List<Student>();
            foreach (Student s in students)
            {
                if (s.BodyMassIndex < 17)
                {
                    underweight.Add(s);
                }
            }
            return underweight;
        }

        /// <summary>
        /// Поиск студентов с нормальным весом (ИМТ 18.5 - 25)
        /// </summary>
        public List<Student> FindNormalWeightStudents()
        {
            List<Student> normal = new List<Student>();
            foreach (Student s in students)
            {
                double bmi = s.BodyMassIndex;
                if (bmi >= 18.5 && bmi <= 25)
                {
                    normal.Add(s);
                }
            }
            return normal;
        }

        /// <summary>
        /// Поиск студентов с избыточным весом (ИМТ > 25)
        /// </summary>
        public List<Student> FindOverweightStudents()
        {
            List<Student> overweight = new List<Student>();
            foreach (Student s in students)
            {
                if (s.BodyMassIndex > 25)
                {
                    overweight.Add(s);
                }
            }
            return overweight;
        }

        /// <summary>
        /// Получить средний ИМТ по группе
        /// </summary>
        public double GetAverageBMI()
        {
            if (students.Count == 0)
                return 0;

            double sum = 0;
            foreach (Student s in students)
            {
                sum += s.BodyMassIndex;
            }
            return sum / students.Count;
        }

        /// <summary>
        /// Получить минимальный ИМТ в группе
        /// </summary>
        public double GetMinBMI()
        {
            if (students.Count == 0)
                return 0;

            double min = students[0].BodyMassIndex;
            foreach (Student s in students)
            {
                if (s.BodyMassIndex < min)
                    min = s.BodyMassIndex;
            }
            return min;
        }

        /// <summary>
        /// Получить максимальный ИМТ в группе
        /// </summary>
        public double GetMaxBMI()
        {
            if (students.Count == 0)
                return 0;

            double max = students[0].BodyMassIndex;
            foreach (Student s in students)
            {
                if (s.BodyMassIndex > max)
                    max = s.BodyMassIndex;
            }
            return max;
        }

        /// <summary>
        /// Сохранение данных в файл
        /// </summary>
        /// <param name="fileName">Имя файла для сохранения</param>
        public void SaveToFile(string fileName)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
                {
                    // Первая строка - номер группы
                    writer.WriteLine(groupNumber);

                    // Вторая строка - количество студентов
                    writer.WriteLine(students.Count);

                    // Далее - данные каждого студента: Фамилия;Вес;Рост
                    foreach (Student s in students)
                    {
                        // Используем точку как разделитель для чисел (культурно-независимый формат)
                        string weightStr = s.Weight.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        string heightStr = s.Height.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        writer.WriteLine($"{s.LastName};{weightStr};{heightStr}");
                    }
                }
                MessageBox.Show("Данные успешно сохранены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Загрузка данных из файла
        /// </summary>
        /// <param name="fileName">Имя файла для загрузки</param>
        public void LoadFromFile(string fileName)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
                {
                    // Очищаем текущие данные
                    students.Clear();

                    // Читаем номер группы
                    string line = reader.ReadLine();
                    if (line != null)
                        groupNumber = line;
                    else
                        groupNumber = "Не указана";

                    // Читаем количество студентов
                    line = reader.ReadLine();
                    if (line == null) return;

                    int count = int.Parse(line);

                    // Читаем и добавляем студентов
                    for (int i = 0; i < count; i++)
                    {
                        line = reader.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] parts = line.Split(';');
                            if (parts.Length == 3)
                            {
                                string lastName = parts[0];

                                // Парсим числа с учетом разных форматов (точка или запятая)
                                double weight;
                                double height;

                                // Пробуем распарсить с учетом культуры
                                if (!double.TryParse(parts[1], System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out weight))
                                {
                                    // Если не получилось, пробуем с текущей культурой
                                    double.TryParse(parts[1], out weight);
                                }

                                if (!double.TryParse(parts[2], System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out height))
                                {
                                    double.TryParse(parts[2], out height);
                                }

                                students.Add(new Student(lastName, weight, height));
                            }
                        }
                    }
                }
                MessageBox.Show("Данные успешно загружены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}\n" +
                    $"Проверьте формат файла.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Сохранение данных в файл с возможностью выбора кодировки
        /// </summary>
        public void SaveToFile(string fileName, Encoding encoding)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName, false, encoding))
                {
                    writer.WriteLine(groupNumber);
                    writer.WriteLine(students.Count);

                    foreach (Student s in students)
                    {
                        string weightStr = s.Weight.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        string heightStr = s.Height.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        writer.WriteLine($"{s.LastName};{weightStr};{heightStr}");
                    }
                }
                MessageBox.Show("Данные успешно сохранены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Экспорт данных в CSV файл (можно открыть в Excel)
        /// </summary>
        public void ExportToCSV(string fileName)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
                {
                    // Заголовки
                    writer.WriteLine("Фамилия;Вес (кг);Рост (м);ИМТ");

                    // Данные
                    foreach (Student s in students)
                    {
                        string weightStr = s.Weight.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        string heightStr = s.Height.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        string bmiStr = s.BodyMassIndex.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                        writer.WriteLine($"{s.LastName};{weightStr};{heightStr};{bmiStr}");
                    }
                }
                MessageBox.Show("Данные экспортированы в CSV!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}