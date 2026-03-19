using System;
using System.Drawing;
using System.Windows.Forms;

namespace TriangleAnimation
{
    public partial class Form1 : Form
    {
        private Triangle _triangle;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.ResizeRedraw = true;
            this.DoubleBuffered = true;
        }

        private void InitializeTriangle()
        {
            float startX = 50;
            float startY = (this.ClientSize.Height - 50) / 2;
            float size = 50;
            _triangle = new Triangle(startX, startY, size, Color.Blue);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeTriangle();
            animationTimer.Enabled = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            _triangle?.Draw(e.Graphics);
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            _triangle?.Move(this.ClientSize.Width, this.ClientSize.Height);
            this.Invalidate();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.Close();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            Form2 settingsForm = new Form2(_triangle.Color, animationTimer.Interval);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                _triangle.Color = settingsForm.SelectedColor;
                animationTimer.Interval = settingsForm.SpeedInterval;
                this.Invalidate();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (_triangle != null)
            {
                _triangle.Y = (this.ClientSize.Height - _triangle.Height) / 2;
            }
        }
    }
}
