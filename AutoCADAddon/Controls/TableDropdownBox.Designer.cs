using System.Drawing;
using System.Windows.Forms;

namespace AutoCADAddon.Controls
{
    partial class TableDropdownBox
    {
        private System.ComponentModel.IContainer components = null;
        private Button mainButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.mainButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainButton
            // 
            this.mainButton.Dock = DockStyle.Fill;
            this.mainButton.Text = "▼";
            this.mainButton.TextAlign = ContentAlignment.MiddleRight;
            this.mainButton.UseVisualStyleBackColor = true;
            this.mainButton.Click += new System.EventHandler(this.MainButton_Click);
            // 
            // TableDropdownBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainButton);
            this.Name = "TableDropdownBox";
            this.Size = new System.Drawing.Size(200, 26);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
