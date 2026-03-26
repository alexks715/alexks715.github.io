using System;
using System.Globalization;
using System.Windows.Forms;

namespace Task1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private double ExactValue(double x)
        {
            return Math.Log((x + 1) / (x - 1));
        }

        private double SeriesSum(double x, double eps, out int iterations)
        {
            double sum = 0.0;
            double term = 2.0 / x;
            iterations = 0;
            int n = 0;

            while (Math.Abs(term) > eps)
            {
                term = 2.0 / ((2 * n + 1) * Math.Pow(x, 2 * n + 1));
                sum += term;
                iterations++;
                n++;

                if (iterations > 1000000) break;
            }

            return sum;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                double eps = double.Parse(textBox1.Text.Replace(',', '.'),
                    CultureInfo.InvariantCulture);
                double x = double.Parse(textBox2.Text.Replace(',', '.'),
                    CultureInfo.InvariantCulture);

                if (eps <= 0 || eps >= 1)
                {
                    label3.Text = "Выход за диапазон: 0 < ε < 1";
                    label3.ForeColor = System.Drawing.Color.DarkRed;
                    return;
                }

                if (x <= 1)
                {
                    label3.Text = "Ошибка: x должен быть > 1";
                    label3.ForeColor = System.Drawing.Color.DarkRed;
                    return;
                }

                double exact = ExactValue(x);
                int count;
                double sum = SeriesSum(x, eps, out count);
                double error = Math.Abs(exact - sum);

                label3.Text = $"Вычисление выполнено успешно!";
                label3.ForeColor = System.Drawing.Color.DarkGreen;
                label4.Text = $"ln((x+1)/(x-1)) равен: {exact:F15}";
                label5.Text = $"Сумма ряда равна: {sum:F15}";
                label6.Text = $"Кол-во членов ряда: {count}";
            }
            catch (FormatException)
            {
                label3.Text = "Ошибка формата числа";
                label3.ForeColor = System.Drawing.Color.DarkRed;
                label4.Text = "ln((x+1)/(x-1)) равен:";
                label5.Text = "Сумма ряда равна:";
                label6.Text = "Кол-во членов ряда:";
            }
            catch (Exception ex)
            {
                label3.Text = $"Ошибка: {ex.Message}";
                label3.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = !string.IsNullOrWhiteSpace(textBox1.Text) &&
                             !string.IsNullOrWhiteSpace(textBox2.Text) &&
                             textBox1.Text.Trim('-', ',', '.') != "" &&
                             textBox2.Text.Trim('-', ',', '.') != "";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            TextBox tb = sender as TextBox;

            if (char.IsDigit(ch) || ch == '\b')
                return;

            if ((ch == ',' || ch == '.') && !tb.Text.Contains(",") && !tb.Text.Contains("."))
                return;

            if (ch == '-' && tb.SelectionStart == 0 && !tb.Text.Contains("-"))
                return;

            if (char.IsControl(ch))
                return;

            e.Handled = true;
        }
    }
}