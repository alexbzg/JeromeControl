namespace AntennaeRotator
{
    partial class fConnectionParams
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fConnectionParams));
            this.bOK = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbHost = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.nudIntervalOff = new System.Windows.Forms.NumericUpDown();
            this.lIntOff = new System.Windows.Forms.Label();
            this.cbDeviceType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chbHwLimits = new System.Windows.Forms.CheckBox();
            this.pIcon = new System.Windows.Forms.Panel();
            this.bIconPrev = new System.Windows.Forms.Button();
            this.bIconNext = new System.Windows.Forms.Button();
            this.lIntOn = new System.Windows.Forms.Label();
            this.nudIntervalOn = new System.Windows.Forms.NumericUpDown();
            this.tbUSARTPort = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbESMHz = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalOff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalOn)).BeginInit();
            this.SuspendLayout();
            // 
            // bOK
            // 
            this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bOK.Location = new System.Drawing.Point(124, 387);
            this.bOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(112, 37);
            this.bOK.TabIndex = 0;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bCancel.Location = new System.Drawing.Point(246, 387);
            this.bCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(112, 37);
            this.bCancel.TabIndex = 1;
            this.bCancel.Text = "Отмена";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(70, 200);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Порт";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(51, 165);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Пароль";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(61, 125);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Адрес";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(125, 194);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(227, 26);
            this.tbPort.TabIndex = 5;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(125, 159);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(227, 26);
            this.tbPassword.TabIndex = 6;
            // 
            // tbHost
            // 
            this.tbHost.Location = new System.Drawing.Point(125, 122);
            this.tbHost.Name = "tbHost";
            this.tbHost.Size = new System.Drawing.Size(227, 26);
            this.tbHost.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(90, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Название";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(179, 22);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(173, 26);
            this.tbName.TabIndex = 9;
            // 
            // nudIntervalOff
            // 
            this.nudIntervalOff.Location = new System.Drawing.Point(282, 335);
            this.nudIntervalOff.Name = "nudIntervalOff";
            this.nudIntervalOff.Size = new System.Drawing.Size(70, 26);
            this.nudIntervalOff.TabIndex = 14;
            this.nudIntervalOff.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudIntervalOff.ThousandsSeparator = true;
            this.nudIntervalOff.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudIntervalOff.Validating += new System.ComponentModel.CancelEventHandler(this.nudInterval_Validating);
            // 
            // lIntOff
            // 
            this.lIntOff.AutoSize = true;
            this.lIntOff.Location = new System.Drawing.Point(99, 341);
            this.lIntOff.Name = "lIntOff";
            this.lIntOff.Size = new System.Drawing.Size(183, 20);
            this.lIntOff.TabIndex = 15;
            this.lIntOff.Text = "Интервал выключения";
            // 
            // cbDeviceType
            // 
            this.cbDeviceType.FormattingEnabled = true;
            this.cbDeviceType.Items.AddRange(new object[] {
            "Net Rotator 5.0"});
            this.cbDeviceType.Location = new System.Drawing.Point(179, 54);
            this.cbDeviceType.Name = "cbDeviceType";
            this.cbDeviceType.Size = new System.Drawing.Size(173, 28);
            this.cbDeviceType.TabIndex = 16;
            this.cbDeviceType.SelectedIndexChanged += new System.EventHandler(this.cbDeviceType_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(137, 62);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 20);
            this.label6.TabIndex = 17;
            this.label6.Text = "Тип";
            // 
            // chbHwLimits
            // 
            this.chbHwLimits.AutoSize = true;
            this.chbHwLimits.Location = new System.Drawing.Point(146, 88);
            this.chbHwLimits.Name = "chbHwLimits";
            this.chbHwLimits.Size = new System.Drawing.Size(205, 24);
            this.chbHwLimits.TabIndex = 18;
            this.chbHwLimits.Text = "Аппаратные концевики";
            this.chbHwLimits.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chbHwLimits.UseVisualStyleBackColor = true;
            // 
            // pIcon
            // 
            this.pIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pIcon.Location = new System.Drawing.Point(12, 12);
            this.pIcon.Name = "pIcon";
            this.pIcon.Size = new System.Drawing.Size(66, 78);
            this.pIcon.TabIndex = 19;
            // 
            // bIconPrev
            // 
            this.bIconPrev.Location = new System.Drawing.Point(12, 89);
            this.bIconPrev.Name = "bIconPrev";
            this.bIconPrev.Size = new System.Drawing.Size(30, 30);
            this.bIconPrev.TabIndex = 0;
            this.bIconPrev.Text = "<";
            this.bIconPrev.UseVisualStyleBackColor = true;
            this.bIconPrev.Click += new System.EventHandler(this.bIconPrev_Click);
            // 
            // bIconNext
            // 
            this.bIconNext.Location = new System.Drawing.Point(45, 89);
            this.bIconNext.Name = "bIconNext";
            this.bIconNext.Size = new System.Drawing.Size(30, 30);
            this.bIconNext.TabIndex = 1;
            this.bIconNext.Text = ">";
            this.bIconNext.UseVisualStyleBackColor = true;
            this.bIconNext.Click += new System.EventHandler(this.bIconNext_Click);
            // 
            // lIntOn
            // 
            this.lIntOn.AutoSize = true;
            this.lIntOn.Location = new System.Drawing.Point(98, 308);
            this.lIntOn.Name = "lIntOn";
            this.lIntOn.Size = new System.Drawing.Size(172, 20);
            this.lIntOn.TabIndex = 21;
            this.lIntOn.Text = "Интервал включения";
            // 
            // nudIntervalOn
            // 
            this.nudIntervalOn.Location = new System.Drawing.Point(281, 302);
            this.nudIntervalOn.Name = "nudIntervalOn";
            this.nudIntervalOn.Size = new System.Drawing.Size(70, 26);
            this.nudIntervalOn.TabIndex = 20;
            this.nudIntervalOn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudIntervalOn.ThousandsSeparator = true;
            this.nudIntervalOn.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // tbUSARTPort
            // 
            this.tbUSARTPort.Location = new System.Drawing.Point(124, 228);
            this.tbUSARTPort.Name = "tbUSARTPort";
            this.tbUSARTPort.Size = new System.Drawing.Size(227, 26);
            this.tbUSARTPort.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(8, 231);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 20);
            this.label8.TabIndex = 22;
            this.label8.Text = "Порт USART";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 263);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(124, 20);
            this.label9.TabIndex = 24;
            this.label9.Text = "ExpertSync Mhz";
            // 
            // tbESMHz
            // 
            this.tbESMHz.Location = new System.Drawing.Point(141, 260);
            this.tbESMHz.Name = "tbESMHz";
            this.tbESMHz.Size = new System.Drawing.Size(210, 26);
            this.tbESMHz.TabIndex = 25;
            // 
            // fConnectionParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 446);
            this.Controls.Add(this.tbESMHz);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tbUSARTPort);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lIntOn);
            this.Controls.Add(this.nudIntervalOn);
            this.Controls.Add(this.bIconPrev);
            this.Controls.Add(this.bIconNext);
            this.Controls.Add(this.pIcon);
            this.Controls.Add(this.chbHwLimits);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbDeviceType);
            this.Controls.Add(this.lIntOff);
            this.Controls.Add(this.nudIntervalOff);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbHost);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "fConnectionParams";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Параметры подключения";
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalOff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalOn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox tbPort;
        public System.Windows.Forms.TextBox tbPassword;
        public System.Windows.Forms.TextBox tbHost;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox tbName;
        public System.Windows.Forms.NumericUpDown nudIntervalOff;
        private System.Windows.Forms.Label lIntOff;
        public System.Windows.Forms.ComboBox cbDeviceType;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.CheckBox chbHwLimits;
        private System.Windows.Forms.Panel pIcon;
        private System.Windows.Forms.Button bIconPrev;
        private System.Windows.Forms.Button bIconNext;
        private System.Windows.Forms.Label lIntOn;
        public System.Windows.Forms.NumericUpDown nudIntervalOn;
        public System.Windows.Forms.TextBox tbUSARTPort;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox tbESMHz;
    }
}