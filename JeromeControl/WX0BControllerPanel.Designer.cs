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
            this.SuspendLayout();
            // 
            // gbController
            // 
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
            // WX0BControllerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbController);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "WX0BControllerPanel";
            this.Size = new System.Drawing.Size(440, 98);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbController;
    }
}
