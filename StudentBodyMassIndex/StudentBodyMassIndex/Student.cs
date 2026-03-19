using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentBodyMassIndex
{
    /// <summary>
    /// Класс, представляющий одного студента
    /// </summary>
    public class Student
    {
        // Приватные поля
        private string lastName;
        private double weight; // в кг
        private double height; // в метрах

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Student()
        {
            this.lastName = "";
            this.weight = 0;
            this.height = 0;
        }

        /// <summary>
        /// Конструктор для создания нового студента
        /// </summary>
        /// <param name="lastName">Фамилия</param>
        /// <param name="weight">Вес (кг)</param>
        /// <param name="height">Рост (м)</param>
        public Student(string lastName, double weight, double height)
        {
            this.lastName = lastName;
            this.weight = weight;
            this.height = height;
        }

        // Свойства для доступа к полям
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public double Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Свойство для вычисления индекса массы тела (ИМТ)
        /// </summary>
        public double BodyMassIndex
        {
            get
            {
                if (height > 0)
                    return weight / (height * height);
                else
                    return 0;
            }
        }

        /// <summary>
        /// Переопределение метода ToString для удобного отображения
        /// </summary>
        public override string ToString()
        {
            return $"{lastName}: вес={weight} кг, рост={height} м, ИМТ={BodyMassIndex:F2}";
        }
    }
}