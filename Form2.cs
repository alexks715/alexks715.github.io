using System;
using System.Drawing;
using System.Windows.Forms;

namespace TriangleAnimation
{
    public partial class Form2 : Form
    {
        public Color SelectedColor { get; private set; }
        public int SpeedInterval { get; private set; }

        public Form2(Color currentColor, int currentSpeed)
        {
            InitializeComponent();

            SelectedColor = currentColor;
            SpeedInterval = currentSpeed;

            trackSpeed.Value = currentSpeed;
            lblSpeedValue.Text = currentSpeed.ToString();
        }

        private void btnSelectColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedColor = colorDialog.Color;
            }
        }

        private void trackSpeed_Scroll(object sender, EventArgs e)
        {
            lblSpeedValue.Text = trackSpeed.Value.ToString();
            SpeedInterval = trackSpeed.Value;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}