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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.gbTerminal = new System.Windows.Forms.GroupBox();
            this.cbConnectTerminal = new System.Windows.Forms.CheckBox();
            this.gbTerminal.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Адрес";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(216, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Порт";
            // 
            // tbAddress
            // 
            this.tbAddress.Location = new System.Drawing.Point(65, 27);
            this.tbAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(146, 26);
            this.tbAddress.TabIndex = 2;
            this.tbAddress.Text = "127.0.0.1";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(272, 27);
            this.tbPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(129, 26);
            this.tbPort.TabIndex = 3;
            this.tbPort.Text = "8080";
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
            this.gbTerminal.Controls.Add(this.cbConnectTerminal);
            this.gbTerminal.Controls.Add(this.tbPort);
            this.gbTerminal.Controls.Add(this.label1);
            this.gbTerminal.Controls.Add(this.label2);
            this.gbTerminal.Controls.Add(this.tbAddress);
            this.gbTerminal.Location = new System.Drawing.Point(6, 4);
            this.gbTerminal.Name = "gbTerminal";
            this.gbTerminal.Size = new System.Drawing.Size(414, 106);
            this.gbTerminal.TabIndex = 7;
            this.gbTerminal.TabStop = false;
            this.gbTerminal.Text = "Терминал";
            // 
            // cbConnectTerminal
            // 
            this.cbConnectTerminal.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbConnectTerminal.AutoSize = true;
            this.cbConnectTerminal.Location = new System.Drawing.Point(275, 61);
            this.cbConnectTerminal.Name = "cbConnectTerminal";
            this.cbConnectTerminal.Size = new System.Drawing.Size(126, 30);
            this.cbConnectTerminal.TabIndex = 5;
            this.cbConnectTerminal.Text = "Подключение";
            this.cbConnectTerminal.UseVisualStyleBackColor = true;
            // 
            // FWX0BConnection
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
            this.Name = "FWX0BConnection";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "WX0B";
            this.gbTerminal.ResumeLayout(false);
            this.gbTerminal.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.GroupBox gbTerminal;
        private System.Windows.Forms.CheckBox cbConnectTerminal;
    }
}