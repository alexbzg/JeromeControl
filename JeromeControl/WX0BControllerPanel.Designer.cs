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
            this.bDelete = new System.Windows.Forms.Button();
            this.bConnectionParams = new System.Windows.Forms.Button();
            this.cbConnect = new System.Windows.Forms.CheckBox();
            this.lESMHz = new System.Windows.Forms.Label();
            this.tbESMHz = new System.Windows.Forms.TextBox();
            this.gbController.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbController
            // 
            this.gbController.Controls.Add(this.tbESMHz);
            this.gbController.Controls.Add(this.lESMHz);
            this.gbController.Controls.Add(this.cbConnect);
            this.gbController.Controls.Add(this.bConnectionParams);
            this.gbController.Controls.Add(this.bDelete);
            this.gbController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbController.Location = new System.Drawing.Point(0, 0);
            this.gbController.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbController.Name = "gbController";
            this.gbController.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbController.Size = new System.Drawing.Size(440, 98);
            this.gbController.TabIndex = 0;
            this.gbController.TabStop = false;
            this.gbController.Text = "Контроллер";
            // 
            // bDelete
            // 
            this.bDelete.Location = new System.Drawing.Point(326, 20);
            this.bDelete.Name = "bDelete";
            this.bDelete.Size = new System.Drawing.Size(87, 31);
            this.bDelete.TabIndex = 1;
            this.bDelete.Text = "Удалить";
            this.bDelete.UseVisualStyleBackColor = true;
            this.bDelete.Click += new System.EventHandler(this.bDelete_Click);
            // 
            // bConnectionParams
            // 
            this.bConnectionParams.Location = new System.Drawing.Point(7, 20);
            this.bConnectionParams.Name = "bConnectionParams";
            this.bConnectionParams.Size = new System.Drawing.Size(313, 31);
            this.bConnectionParams.TabIndex = 2;
            this.bConnectionParams.Text = "button1";
            this.bConnectionParams.UseVisualStyleBackColor = true;
            this.bConnectionParams.Click += new System.EventHandler(this.bConnectionParams_Click);
            // 
            // cbConnect
            // 
            this.cbConnect.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbConnect.AutoSize = true;
            this.cbConnect.Location = new System.Drawing.Point(287, 57);
            this.cbConnect.Name = "cbConnect";
            this.cbConnect.Size = new System.Drawing.Size(126, 30);
            this.cbConnect.TabIndex = 3;
            this.cbConnect.Text = "Подключение";
            this.cbConnect.UseVisualStyleBackColor = true;
            this.cbConnect.CheckedChanged += new System.EventHandler(this.cbConnect_CheckedChanged);
            // 
            // lESMHz
            // 
            this.lESMHz.AutoSize = true;
            this.lESMHz.Location = new System.Drawing.Point(7, 62);
            this.lESMHz.Name = "lESMHz";
            this.lESMHz.Size = new System.Drawing.Size(127, 20);
            this.lESMHz.TabIndex = 4;
            this.lESMHz.Text = "ExpertSync MHz";
            // 
            // tbESMHz
            // 
            this.tbESMHz.Location = new System.Drawing.Point(140, 59);
            this.tbESMHz.Name = "tbESMHz";
            this.tbESMHz.Size = new System.Drawing.Size(141, 26);
            this.tbESMHz.TabIndex = 5;
            this.tbESMHz.Validated += new System.EventHandler(this.tbESMHz_Validated);
            // 
            // WX0BControllerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbController);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "WX0BControllerPanel";
            this.Size = new System.Drawing.Size(440, 98);
            this.gbController.ResumeLayout(false);
            this.gbController.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbController;
        private System.Windows.Forms.Button bDelete;
        private System.Windows.Forms.Button bConnectionParams;
        private System.Windows.Forms.CheckBox cbConnect;
        private System.Windows.Forms.TextBox tbESMHz;
        private System.Windows.Forms.Label lESMHz;
    }
}
