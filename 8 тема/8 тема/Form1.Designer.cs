namespace TicTacToeGame
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Метод InitializeComponent - только для совместимости с конструктором
        // Он пустой, так как вся инициализация происходит в коде
        private void InitializeComponent()
        {
            // Все элементы управления создаются программно в Form1.cs
            // Этот метод оставлен пустым для совместимости
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 700);
            this.Name = "Form1";
            this.Text = "Крестики-нолики";
            this.ResumeLayout(false);
        }
    }
}