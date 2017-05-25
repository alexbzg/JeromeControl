namespace WX0B
{
    partial class FWX0B
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        public override void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FWX0B));
            this.gbTerminal = new System.Windows.Forms.GroupBox();
            this.bTerminalConnectionParams = new System.Windows.Forms.Button();
            this.cbConnectTerminal = new System.Windows.Forms.CheckBox();
            this.gbControllers = new System.Windows.Forms.GroupBox();
            this.bAddController = new System.Windows.Forms.Button();
            this.gbTerminal.SuspendLayout();
            this.gbControllers.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbTerminal
            // 
            this.gbTerminal.Controls.Add(this.bTerminalConnectionParams);
            this.gbTerminal.Controls.Add(this.cbConnectTerminal);
            this.gbTerminal.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbTerminal.Location = new System.Drawing.Point(0, 0);
            this.gbTerminal.Name = "gbTerminal";
            this.gbTerminal.Size = new System.Drawing.Size(264, 72);
            this.gbTerminal.TabIndex = 7;
            this.gbTerminal.TabStop = false;
            this.gbTerminal.Text = "Терминал";
            // 
            // bTerminalConnectionParams
            // 
            this.bTerminalConnectionParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bTerminalConnectionParams.Location = new System.Drawing.Point(3, 22);
            this.bTerminalConnectionParams.Name = "bTerminalConnectionParams";
            this.bTerminalConnectionParams.Size = new System.Drawing.Size(214, 38);
            this.bTerminalConnectionParams.TabIndex = 6;
            this.bTerminalConnectionParams.Text = "button1";
            this.bTerminalConnectionParams.UseVisualStyleBackColor = true;
            this.bTerminalConnectionParams.Click += new System.EventHandler(this.bTerminalConnectionParams_Click);
            // 
            // cbConnectTerminal
            // 
            this.cbConnectTerminal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbConnectTerminal.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbConnectTerminal.AutoSize = true;
            this.cbConnectTerminal.Image = ((System.Drawing.Image)(resources.GetObject("cbConnectTerminal.Image")));
            this.cbConnectTerminal.Location = new System.Drawing.Point(220, 22);
            this.cbConnectTerminal.Name = "cbConnectTerminal";
            this.cbConnectTerminal.Size = new System.Drawing.Size(38, 38);
            this.cbConnectTerminal.TabIndex = 5;
            this.cbConnectTerminal.UseVisualStyleBackColor = true;
            this.cbConnectTerminal.CheckedChanged += new System.EventHandler(this.cbConnectTerminal_CheckedChanged);
            // 
            // gbControllers
            // 
            this.gbControllers.AutoSize = true;
            this.gbControllers.Controls.Add(this.bAddController);
            this.gbControllers.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbControllers.Location = new System.Drawing.Point(0, 72);
            this.gbControllers.Name = "gbControllers";
            this.gbControllers.Size = new System.Drawing.Size(264, 56);
            this.gbControllers.TabIndex = 8;
            this.gbControllers.TabStop = false;
            this.gbControllers.Text = "Контроллеры";
            // 
            // bAddController
            // 
            this.bAddController.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bAddController.Location = new System.Drawing.Point(3, 22);
            this.bAddController.Name = "bAddController";
            this.bAddController.Size = new System.Drawing.Size(258, 31);
            this.bAddController.TabIndex = 0;
            this.bAddController.Text = "Добавить контроллер";
            this.bAddController.UseVisualStyleBackColor = true;
            this.bAddController.Click += new System.EventHandler(this.bAddController_Click);
            // 
            // FWX0B
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(264, 151);
            this.Controls.Add(this.gbControllers);
            this.Controls.Add(this.gbTerminal);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FWX0B";
            this.Text = "WX0B";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FWX0B_FormClosing);
            this.gbTerminal.ResumeLayout(false);
            this.gbTerminal.PerformLayout();
            this.gbControllers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gbTerminal;
        private System.Windows.Forms.CheckBox cbConnectTerminal;
        private System.Windows.Forms.Button bTerminalConnectionParams;
        private System.Windows.Forms.GroupBox gbControllers;
        private System.Windows.Forms.Button bAddController;
    }
}