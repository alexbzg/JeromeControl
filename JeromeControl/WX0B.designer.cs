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
        private void InitializeComponent()
        {
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.gbTerminal = new System.Windows.Forms.GroupBox();
            this.cbConnectTerminal = new System.Windows.Forms.CheckBox();
            this.bTerminalConnectionParams = new System.Windows.Forms.Button();
            this.gbTerminal.SuspendLayout();
            this.SuspendLayout();
            // 
            // bCancel
            // 
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(258, 500);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(87, 30);
            this.bCancel.TabIndex = 4;
            this.bCancel.Text = "Отмена";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // bOK
            // 
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOK.Location = new System.Drawing.Point(351, 500);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 30);
            this.bOK.TabIndex = 5;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            // 
            // gbTerminal
            // 
            this.gbTerminal.Controls.Add(this.bTerminalConnectionParams);
            this.gbTerminal.Controls.Add(this.cbConnectTerminal);
            this.gbTerminal.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbTerminal.Location = new System.Drawing.Point(0, 0);
            this.gbTerminal.Name = "gbTerminal";
            this.gbTerminal.Size = new System.Drawing.Size(432, 98);
            this.gbTerminal.TabIndex = 7;
            this.gbTerminal.TabStop = false;
            this.gbTerminal.Text = "Терминал";
            // 
            // cbConnectTerminal
            // 
            this.cbConnectTerminal.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbConnectTerminal.AutoSize = true;
            this.cbConnectTerminal.Location = new System.Drawing.Point(303, 59);
            this.cbConnectTerminal.Name = "cbConnectTerminal";
            this.cbConnectTerminal.Size = new System.Drawing.Size(126, 30);
            this.cbConnectTerminal.TabIndex = 5;
            this.cbConnectTerminal.Text = "Подключение";
            this.cbConnectTerminal.UseVisualStyleBackColor = true;
            // 
            // bTerminalConnectionParams
            // 
            this.bTerminalConnectionParams.Dock = System.Windows.Forms.DockStyle.Top;
            this.bTerminalConnectionParams.Location = new System.Drawing.Point(3, 22);
            this.bTerminalConnectionParams.Name = "bTerminalConnectionParams";
            this.bTerminalConnectionParams.Size = new System.Drawing.Size(426, 31);
            this.bTerminalConnectionParams.TabIndex = 6;
            this.bTerminalConnectionParams.Text = "button1";
            this.bTerminalConnectionParams.UseVisualStyleBackColor = true;
            // 
            // FWX0B
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(432, 542);
            this.Controls.Add(this.gbTerminal);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FWX0B";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "WX0B";
            this.gbTerminal.ResumeLayout(false);
            this.gbTerminal.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.GroupBox gbTerminal;
        private System.Windows.Forms.CheckBox cbConnectTerminal;
        private System.Windows.Forms.Button bTerminalConnectionParams;
    }
}