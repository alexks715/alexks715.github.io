using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TriangleQuarterApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                // Считываем координаты из текстовых полей
                float ax = float.Parse(textBoxAx.Text);
                float ay = float.Parse(textBoxAy.Text);
                float bx = float.Parse(textBoxBx.Text);
                float by = float.Parse(textBoxBy.Text);
                float cx = float.Parse(textBoxCx.Text);
                float cy = float.Parse(textBoxCy.Text);

                // Создаём массив точек
                PointF[] vertices = new PointF[]
                {
                    new PointF(ax, ay),
                    new PointF(bx, by),
                    new PointF(cx, cy)
                };

                // Вызываем методы из нашего класса TriangleHelper
                List<string> vertexQuarters = TriangleHelper.GetQuartersOfVertices(vertices);
                HashSet<string> triangleQuarters = TriangleHelper.GetQuartersOfTriangle(vertices);

                // Формируем текст результата
                string report = "Четверти вершин:\r\n";
                report += $"Вершина A: {vertexQuarters[0]}\r\n";
                report += $"Вершина B: {vertexQuarters[1]}\r\n";
                report += $"Вершина C: {vertexQuarters[2]}\r\n";
                report += "Треугольник пересекает четверти: ";
                report += string.Join(", ", triangleQuarters);

                // Выводим результат
                textBoxResult.Text = report;

                // Рисуем треугольник
                DrawTriangle(vertices);
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка: введите числа! Используйте запятую для дробных чисел.", "Ошибка ввода");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

        private void DrawTriangle(PointF[] vertices)
        {
            // Создаём изображение для рисования
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            // Рисуем координатные оси
            Pen axisPen = new Pen(Color.Black, 1);
            int centerX = pictureBox1.Width / 2;
            int centerY = pictureBox1.Height / 2;
            g.DrawLine(axisPen, centerX, 0, centerX, pictureBox1.Height); // вертикальная ось
            g.DrawLine(axisPen, 0, centerY, pictureBox1.Width, centerY);   // горизонтальная ось

            // Подписи осей
            Font font = new Font("Arial", 8);
            Brush brush = Brushes.Black;
            g.DrawString("X", font, brush, pictureBox1.Width - 15, centerY - 15);
            g.DrawString("Y", font, brush, centerX + 5, 5);

            // Масштаб: 1 единица = 20 пикселей
            float scale = 20;

            // Преобразуем математические координаты в экранные
            PointF[] screenPoints = new PointF[3];
            for (int i = 0; i < 3; i++)
            {
                screenPoints[i] = new PointF(
                    centerX + vertices[i].X * scale,
                    centerY - vertices[i].Y * scale  // минус потому что Y на экране идёт вниз
                );
            }

            // Рисуем треугольник
            Pen trianglePen = new Pen(Color.Red, 2);
            g.DrawPolygon(trianglePen, screenPoints);

            // Подписываем вершины
            Font labelFont = new Font("Arial", 10, FontStyle.Bold);
            Brush labelBrush = Brushes.Blue;
            g.DrawString("A", labelFont, labelBrush, screenPoints[0].X + 2, screenPoints[0].Y - 15);
            g.DrawString("B", labelFont, labelBrush, screenPoints[1].X + 2, screenPoints[1].Y - 15);
            g.DrawString("C", labelFont, labelBrush, screenPoints[2].X + 2, screenPoints[2].Y - 15);

            // Помещаем изображение в PictureBox
            pictureBox1.Image = bmp;
        }
    }
}