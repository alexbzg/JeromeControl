namespace NetPA
{
    partial class FNetPA
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FNetPA));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.ddbSettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.miControl = new System.Windows.Forms.ToolStripMenuItem();
            this.tssConnectionsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.miConnectionsList = new System.Windows.Forms.ToolStripMenuItem();
            this.miRelaySettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miModuleSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lRotation = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.toolStrip.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ddbSettings});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip.Location = new System.Drawing.Point(23, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip.Size = new System.Drawing.Size(42, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // ddbSettings
            // 
            this.ddbSettings.AutoSize = false;
            this.ddbSettings.BackColor = System.Drawing.SystemColors.Control;
            this.ddbSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ddbSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miControl,
            this.tssConnectionsSeparator,
            this.miConnectionsList,
            this.miRelaySettings,
            this.miModuleSettings});
            this.ddbSettings.Image = ((System.Drawing.Image)(resources.GetObject("ddbSettings.Image")));
            this.ddbSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddbSettings.Name = "ddbSettings";
            this.ddbSettings.Size = new System.Drawing.Size(42, 20);
            this.ddbSettings.ToolTipText = "Настройки";
            // 
            // miControl
            // 
            this.miControl.Name = "miControl";
            this.miControl.Size = new System.Drawing.Size(209, 22);
            this.miControl.Text = "Управление";
            // 
            // tssConnectionsSeparator
            // 
            this.tssConnectionsSeparator.Name = "tssConnectionsSeparator";
            this.tssConnectionsSeparator.Size = new System.Drawing.Size(206, 6);
            // 
            // miConnectionsList
            // 
            this.miConnectionsList.Name = "miConnectionsList";
            this.miConnectionsList.Size = new System.Drawing.Size(209, 22);
            this.miConnectionsList.Text = "Подключения";
            this.miConnectionsList.Click += new System.EventHandler(this.miConnectionsList_Click);
            // 
            // miRelaySettings
            // 
            this.miRelaySettings.Name = "miRelaySettings";
            this.miRelaySettings.Size = new System.Drawing.Size(209, 22);
            this.miRelaySettings.Text = "Настройки реле";
            this.miRelaySettings.Click += new System.EventHandler(this.miRelaySettings_Click);
            // 
            // miModuleSettings
            // 
            this.miModuleSettings.Name = "miModuleSettings";
            this.miModuleSettings.Size = new System.Drawing.Size(209, 22);
            this.miModuleSettings.Text = "Настройки модуля";
            this.miModuleSettings.Click += new System.EventHandler(this.miModuleSettings_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lRotation});
            this.statusStrip1.Location = new System.Drawing.Point(0, 345);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(120, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lRotation
            // 
            this.lRotation.Image = global::JeromeControl.Properties.Resources.rotate_icon;
            this.lRotation.Name = "lRotation";
            this.lRotation.Size = new System.Drawing.Size(16, 17);
            this.lRotation.Visible = false;
            // 
            // FNetPA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(120, 367);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip);
            this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FNetPA";
            this.Text = "Ant Comm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FNetPA_FormClosed);
            this.Load += new System.EventHandler(this.FMain_Load);
            this.SizeChanged += new System.EventHandler(this.FMain_ResizeEnd);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FMain_KeyDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.form_MouseClick);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton ddbSettings;
        private System.Windows.Forms.ToolStripMenuItem miConnectionsList;
        private System.Windows.Forms.ToolStripSeparator tssConnectionsSeparator;
        private System.Windows.Forms.ToolStripMenuItem miModuleSettings;
        private System.Windows.Forms.ToolStripMenuItem miControl;
        private System.Windows.Forms.ToolStripMenuItem miRelaySettings;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lRotation;
    }
}

