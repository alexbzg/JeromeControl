namespace WX0B
{
    partial class WX0BControllerPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbController = new System.Windows.Forms.GroupBox();
            this.tbHost = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gbController.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbController
            // 
            this.gbController.Controls.Add(this.tbHost);
            this.gbController.Controls.Add(this.textBox2);
            this.gbController.Controls.Add(this.label1);
            this.gbController.Controls.Add(this.label2);
            this.gbController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbController.Location = new System.Drawing.Point(0, 0);
            this.gbController.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbController.Name = "gbController";
            this.gbController.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbController.Size = new System.Drawing.Size(440, 231);
            this.gbController.TabIndex = 0;
            this.gbController.TabStop = false;
            this.gbController.Text = "Контроллер";
            // 
            // tbHost
            // 
            this.tbHost.Location = new System.Drawing.Point(67, 23);
            this.tbHost.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbHost.Name = "tbHost";
            this.tbHost.Size = new System.Drawing.Size(148, 26);
            this.tbHost.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(269, 23);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(148, 26);
            this.textBox2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(223, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Порт";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Адрес";
            // 
            // WX0BControllerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbController);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "WX0BControllerPanel";
            this.Size = new System.Drawing.Size(440, 231);
            this.gbController.ResumeLayout(false);
            this.gbController.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbController;
        private System.Windows.Forms.TextBox tbHost;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
