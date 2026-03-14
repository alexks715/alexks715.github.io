using System;
using System.Windows.Forms;
using System.Globalization;

namespace SeriesCalculation_Var19
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.textBoxEps.TextChanged += textBoxEps_TextChanged;
            this.textBoxX.TextChanged += textBoxX_TextChanged;
            this.buttonCalculate.Click += buttonCalculate_Click;
            CheckFieldsAndEnableButton();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckFieldsAndEnableButton();
        }

        // Точное значение функции ln(x)
        private double CalculateExactFunction(double x)
        {
            return Math.Log(x);
        }

        // Вычисление суммы ряда для ln(x)
        private double CalculateSeriesSum(double x, double e, out int count)
        {
            double sum = 0;
            double currentMember;
            count = 0;
            int n = 0;

            do
            {
                double numerator = Math.Pow(x - 1, n + 1);
                double denominator = (n + 1) * Math.Pow(x, n + 1);
                currentMember = numerator / denominator;

                sum += currentMember;
                count++;
                n++;

                if (count > 1000000) break;

            } while (Math.Abs(currentMember) >= e * Math.Abs(sum) && count < 1000000);

            return sum;
        }

        private void CheckFieldsAndEnableButton()
        {
            string epsText = textBoxEps.Text;
            string xText = textBoxX.Text;

            bool epsOk = !string.IsNullOrWhiteSpace(epsText);
            bool xOk = !string.IsNullOrWhiteSpace(xText);

            buttonCalculate.Enabled = epsOk && xOk;

            if (!buttonCalculate.Enabled)
            {
                if (!epsOk && !xOk)
                    labelResult.Text = "Введите точность e и аргумент X";
                else if (!epsOk)
                    labelResult.Text = "Введите точность e (0 < e < 1)";
                else if (!xOk)
                    labelResult.Text = "Введите аргумент X (X > 0.5)";
            }
            else
            {
                labelResult.Text = "Нажмите 'Вычислить'";
            }
        }

        private void textBoxEps_TextChanged(object sender, EventArgs e)
        {
            CheckFieldsAndEnableButton();
        }

        private void textBoxX_TextChanged(object sender, EventArgs e)
        {
            CheckFieldsAndEnableButton();
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                CultureInfo culture = new CultureInfo("ru-RU");

                double x = Convert.ToDouble(textBoxX.Text, culture);
                double eps = Convert.ToDouble(textBoxEps.Text, culture);

                if (eps <= 0 || eps >= 1)
                {
                    labelResult.Text = "Ошибка: Точность e должна быть между 0 и 1.";
                    return;
                }

                if (x <= 0.5)
                {
                    labelResult.Text = $"Ошибка: X должен быть больше 0.5 (x > 1/2). Вы ввели: {x}";
                    return;
                }

                double exactValue = CalculateExactFunction(x);
                int n;
                double seriesSum = CalculateSeriesSum(x, eps, out n);

                labelResult.Text = $"ln({x:F3}) = {exactValue:F6}\n" +
                                   $"Сумма ряда = {seriesSum:F6}\n" +
                                   $"Количество членов = {n}\n" +
                                   $"Погрешность = {Math.Abs(exactValue - seriesSum):E2}";
            }
            catch (FormatException)
            {
                labelResult.Text = "Ошибка: Введите корректные числа (используйте запятую)!";
            }
            catch (Exception ex)
            {
                labelResult.Text = $"Ошибка: {ex.Message}";
            }
        }

        // ИСПРАВЛЕННЫЙ обработчик для поля точности
        private void textBoxEps_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;
            TextBox tb = sender as TextBox;

            // Разрешаем Backspace
            if (key == 8) return;

            // Разрешаем цифры
            if (char.IsDigit(key)) return;

            // Разрешаем запятую (проверяем через Contains со строкой)
            if (key == ',' && !tb.Text.Contains(",")) return;

            // Все остальное запрещаем
            e.Handled = true;
        }

        // ИСПРАВЛЕННЫЙ обработчик для поля X
        private void textBoxX_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;
            TextBox tb = sender as TextBox;

            // Разрешаем Backspace
            if (key == 8) return;

            // Разрешаем минус (только в начале)
            if (key == '-' && tb.SelectionStart == 0 && !tb.Text.Contains("-")) return;

            // Разрешаем цифры
            if (char.IsDigit(key)) return;

            // Разрешаем запятую (проверяем через Contains со строкой)
            if (key == ',' && !tb.Text.Contains(",")) return;

            // Все остальное запрещаем
            e.Handled = true;
        }
    }
}