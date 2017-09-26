namespace WX0B
{
    partial class FWX0BStatus
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lTerminal = new System.Windows.Forms.Label();
            this.lController = new System.Windows.Forms.Label();
            this.pTerminalStatus = new System.Windows.Forms.Panel();
            this.pControllerStatus = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lTerminal
            // 
            this.lTerminal.Location = new System.Drawing.Point(2, 11);
            this.lTerminal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lTerminal.Name = "lTerminal";
            this.lTerminal.Size = new System.Drawing.Size(207, 20);
            this.lTerminal.TabIndex = 0;
            this.lTerminal.Text = "Терминал";
            // 
            // lController
            // 
            this.lController.Location = new System.Drawing.Point(2, 55);
            this.lController.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lController.Name = "lController";
            this.lController.Size = new System.Drawing.Size(207, 20);
            this.lController.TabIndex = 1;
            this.lController.Text = "Контроллер";
            // 
            // pTerminalStatus
            // 
            this.pTerminalStatus.BackgroundImage = global::JeromeControl.Properties.Resources.icon_connect;
            this.pTerminalStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pTerminalStatus.Location = new System.Drawing.Point(170, 2);
            this.pTerminalStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pTerminalStatus.Name = "pTerminalStatus";
            this.pTerminalStatus.Size = new System.Drawing.Size(38, 38);
            this.pTerminalStatus.TabIndex = 2;
            // 
            // pControllerStatus
            // 
            this.pControllerStatus.BackgroundImage = global::JeromeControl.Properties.Resources.icon_connect;
            this.pControllerStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pControllerStatus.Location = new System.Drawing.Point(170, 46);
            this.pControllerStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pControllerStatus.Name = "pControllerStatus";
            this.pControllerStatus.Size = new System.Drawing.Size(38, 38);
            this.pControllerStatus.TabIndex = 0;
            // 
            // FWX0BStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(209, 84);
            this.Controls.Add(this.pControllerStatus);
            this.Controls.Add(this.pTerminalStatus);
            this.Controls.Add(this.lController);
            this.Controls.Add(this.lTerminal);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimizeBox = false;
            this.Name = "FWX0BStatus";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "WX0B";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FWX0BStatus_FormClosed);
            this.Resize += new System.EventHandler(this.FWX0BStatus_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Label lTerminal;
        internal System.Windows.Forms.Label lController;
        internal System.Windows.Forms.Panel pTerminalStatus;
        internal System.Windows.Forms.Panel pControllerStatus;
    }
}