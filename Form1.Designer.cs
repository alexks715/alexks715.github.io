namespace TriangleQuarterApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            textBoxAx = new TextBox();
            textBoxAy = new TextBox();
            textBoxBx = new TextBox();
            textBoxBy = new TextBox();
            textBoxCx = new TextBox();
            textBoxCy = new TextBox();
            buttonCalculate = new Button();
            textBoxResult = new TextBox();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 15);
            label1.Name = "label1";
            label1.Size = new Size(41, 20);
            label1.TabIndex = 0;
            label1.Text = "A(X):";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 84);
            label2.Name = "label2";
            label2.Size = new Size(40, 20);
            label2.TabIndex = 1;
            label2.Text = "A(Y):";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 146);
            label3.Name = "label3";
            label3.Size = new Size(40, 20);
            label3.TabIndex = 2;
            label3.Text = "B(X):";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(601, 15);
            label4.Name = "label4";
            label4.Size = new Size(39, 20);
            label4.TabIndex = 3;
            label4.Text = "B(Y):";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(601, 84);
            label5.Name = "label5";
            label5.Size = new Size(40, 20);
            label5.TabIndex = 4;
            label5.Text = "C(X):";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(601, 146);
            label6.Name = "label6";
            label6.Size = new Size(39, 20);
            label6.TabIndex = 5;
            label6.Text = "C(Y):";
            // 
            // textBoxAx
            // 
            textBoxAx.Location = new Point(92, 12);
            textBoxAx.Name = "textBoxAx";
            textBoxAx.Size = new Size(125, 27);
            textBoxAx.TabIndex = 6;
            // 
            // textBoxAy
            // 
            textBoxAy.Location = new Point(92, 84);
            textBoxAy.Name = "textBoxAy";
            textBoxAy.Size = new Size(125, 27);
            textBoxAy.TabIndex = 7;
            // 
            // textBoxBx
            // 
            textBoxBx.Location = new Point(92, 146);
            textBoxBx.Name = "textBoxBx";
            textBoxBx.Size = new Size(125, 27);
            textBoxBx.TabIndex = 8;
            // 
            // textBoxBy
            // 
            textBoxBy.Location = new Point(663, 12);
            textBoxBy.Name = "textBoxBy";
            textBoxBy.Size = new Size(125, 27);
            textBoxBy.TabIndex = 9;
            // 
            // textBoxCx
            // 
            textBoxCx.Location = new Point(663, 77);
            textBoxCx.Name = "textBoxCx";
            textBoxCx.Size = new Size(125, 27);
            textBoxCx.TabIndex = 10;
            // 
            // textBoxCy
            // 
            textBoxCy.Location = new Point(663, 143);
            textBoxCy.Name = "textBoxCy";
            textBoxCy.Size = new Size(125, 27);
            textBoxCy.TabIndex = 11;
            // 
            // buttonCalculate
            // 
            buttonCalculate.Location = new Point(302, 49);
            buttonCalculate.Name = "buttonCalculate";
            buttonCalculate.Size = new Size(198, 29);
            buttonCalculate.TabIndex = 12;
            buttonCalculate.Text = "Определить четверти";
            buttonCalculate.UseVisualStyleBackColor = true;
            buttonCalculate.Click += buttonCalculate_Click;
            // 
            // textBoxResult
            // 
            textBoxResult.Location = new Point(288, 110);
            textBoxResult.Multiline = true;
            textBoxResult.Name = "textBoxResult";
            textBoxResult.ReadOnly = true;
            textBoxResult.ScrollBars = ScrollBars.Vertical;
            textBoxResult.Size = new Size(236, 97);
            textBoxResult.TabIndex = 13;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.HighlightText;
            pictureBox1.Location = new Point(-17, 232);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(831, 232);
            pictureBox1.TabIndex = 14;
            pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBoxResult);
            Controls.Add(buttonCalculate);
            Controls.Add(textBoxCy);
            Controls.Add(textBoxCx);
            Controls.Add(textBoxBy);
            Controls.Add(textBoxBx);
            Controls.Add(textBoxAy);
            Controls.Add(textBoxAx);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        // ВАЖНО: Этот метод БЫЛ внутри региона, я его полностью удалил
        // private void textBox3_TextChanged(object sender, EventArgs e)
        // {
        //     throw new NotImplementedException();
        // }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox textBoxAx;
        private TextBox textBoxAy;
        private TextBox textBoxBx;
        private TextBox textBoxBy;
        private TextBox textBoxCx;
        private TextBox textBoxCy;
        private Button buttonCalculate;
        private TextBox textBoxResult;
        private PictureBox pictureBox1;

        // ВАЖНО: Это свойство БЫЛО в конце, я его полностью удалил
        // public EventHandler Form1_Load { get; private set; }
    }
}