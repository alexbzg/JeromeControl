namespace EncRotator
{
    partial class FSetNorth
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FSetNorth));
            this.bRotate0 = new System.Windows.Forms.Button();
            this.bRotate1 = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.bStop0 = new System.Windows.Forms.Button();
            this.bStop1 = new System.Windows.Forms.Button();
            this.bNorth = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bDeleteStops = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // bRotate0
            // 
            this.bRotate0.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bRotate0.Image = ((System.Drawing.Image)(resources.GetObject("bRotate0.Image")));
            this.bRotate0.Location = new System.Drawing.Point(167, 27);
            this.bRotate0.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bRotate0.Name = "bRotate0";
            this.bRotate0.Size = new System.Drawing.Size(70, 70);
            this.bRotate0.TabIndex = 0;
            this.bRotate0.UseVisualStyleBackColor = true;
            this.bRotate0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bRotate0_MouseDown);
            this.bRotate0.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bRotate_MouseUp);
            // 
            // bRotate1
            // 
            this.bRotate1.Image = ((System.Drawing.Image)(resources.GetObject("bRotate1.Image")));
            this.bRotate1.Location = new System.Drawing.Point(70, 27);
            this.bRotate1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bRotate1.Name = "bRotate1";
            this.bRotate1.Size = new System.Drawing.Size(70, 70);
            this.bRotate1.TabIndex = 1;
            this.bRotate1.UseVisualStyleBackColor = true;
            this.bRotate1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bRotate1_MouseDown);
            this.bRotate1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bRotate_MouseUp);
            // 
            // bOK
            // 
            this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOK.Location = new System.Drawing.Point(129, 314);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(82, 32);
            this.bOK.TabIndex = 2;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            // 
            // bStop0
            // 
            this.bStop0.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bStop0.Image = ((System.Drawing.Image)(resources.GetObject("bStop0.Image")));
            this.bStop0.Location = new System.Drawing.Point(204, 25);
            this.bStop0.Name = "bStop0";
            this.bStop0.Size = new System.Drawing.Size(90, 70);
            this.bStop0.TabIndex = 4;
            this.bStop0.Text = "Концевик";
            this.bStop0.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.bStop0.UseVisualStyleBackColor = true;
            this.bStop0.Visible = false;
            this.bStop0.Click += new System.EventHandler(this.bStop0_Click);
            // 
            // bStop1
            // 
            this.bStop1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bStop1.Image = ((System.Drawing.Image)(resources.GetObject("bStop1.Image")));
            this.bStop1.Location = new System.Drawing.Point(12, 25);
            this.bStop1.Name = "bStop1";
            this.bStop1.Size = new System.Drawing.Size(90, 70);
            this.bStop1.TabIndex = 5;
            this.bStop1.Text = "Концевик";
            this.bStop1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.bStop1.UseVisualStyleBackColor = true;
            this.bStop1.Visible = false;
            this.bStop1.Click += new System.EventHandler(this.bStop1_Click);
            // 
            // bNorth
            // 
            this.bNorth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bNorth.Image = ((System.Drawing.Image)(resources.GetObject("bNorth.Image")));
            this.bNorth.Location = new System.Drawing.Point(108, 25);
            this.bNorth.Name = "bNorth";
            this.bNorth.Size = new System.Drawing.Size(90, 70);
            this.bNorth.TabIndex = 6;
            this.bNorth.Text = "Север";
            this.bNorth.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.bNorth.UseVisualStyleBackColor = true;
            this.bNorth.Click += new System.EventHandler(this.bNorth_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bRotate1);
            this.groupBox1.Controls.Add(this.bRotate0);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(305, 107);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Вращение антенны";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bStop1);
            this.groupBox2.Controls.Add(this.bNorth);
            this.groupBox2.Controls.Add(this.bStop0);
            this.groupBox2.Location = new System.Drawing.Point(12, 135);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(305, 109);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Задать";
            // 
            // bDeleteStops
            // 
            this.bDeleteStops.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bDeleteStops.Location = new System.Drawing.Point(24, 265);
            this.bDeleteStops.Name = "bDeleteStops";
            this.bDeleteStops.Size = new System.Drawing.Size(282, 30);
            this.bDeleteStops.TabIndex = 0;
            this.bDeleteStops.Text = "Сброс настроек концевиков";
            this.bDeleteStops.UseVisualStyleBackColor = true;
            this.bDeleteStops.Click += new System.EventHandler(this.bDeleteStops_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(217, 314);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 32);
            this.button1.TabIndex = 9;
            this.button1.Text = "Oтменить";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // FSetNorth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 354);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.bDeleteStops);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FSetNorth";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки антенны";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bRotate0;
        private System.Windows.Forms.Button bRotate1;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bStop0;
        private System.Windows.Forms.Button bStop1;
        private System.Windows.Forms.Button bNorth;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bDeleteStops;
        private System.Windows.Forms.Button button1;
    }
}