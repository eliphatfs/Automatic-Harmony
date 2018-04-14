namespace ConcordV3
{
    partial class FormTest
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonTestLittleStar = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonTest2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonTestLittleStar
            // 
            this.buttonTestLittleStar.Location = new System.Drawing.Point(12, 12);
            this.buttonTestLittleStar.Name = "buttonTestLittleStar";
            this.buttonTestLittleStar.Size = new System.Drawing.Size(140, 23);
            this.buttonTestLittleStar.TabIndex = 0;
            this.buttonTestLittleStar.Text = "TestLittleStar";
            this.buttonTestLittleStar.UseVisualStyleBackColor = true;
            this.buttonTestLittleStar.Click += new System.EventHandler(this.buttonTestLittleStar_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 42);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(526, 219);
            this.textBox1.TabIndex = 1;
            // 
            // buttonTest2
            // 
            this.buttonTest2.Location = new System.Drawing.Point(158, 12);
            this.buttonTest2.Name = "buttonTest2";
            this.buttonTest2.Size = new System.Drawing.Size(75, 23);
            this.buttonTest2.TabIndex = 2;
            this.buttonTest2.Text = "Test2";
            this.buttonTest2.UseVisualStyleBackColor = true;
            this.buttonTest2.Click += new System.EventHandler(this.buttonTest2_Click);
            // 
            // FormTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 273);
            this.Controls.Add(this.buttonTest2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonTestLittleStar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormTest";
            this.Text = "Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonTestLittleStar;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonTest2;
    }
}

