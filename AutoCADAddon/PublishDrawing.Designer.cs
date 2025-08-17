namespace AutoCADAddon
{
    partial class PublishDrawing
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AllDrawing = new System.Windows.Forms.RadioButton();
            this.ThisDrawingOnly = new System.Windows.Forms.RadioButton();
            this.Publish = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.Options = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.NamedView = new System.Windows.Forms.RadioButton();
            this.CurrentView = new System.Windows.Forms.RadioButton();
            this.FloorBoundary = new System.Windows.Forms.RadioButton();
            this.Extents = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.PublishingRule = new System.Windows.Forms.ProgressBar();
            this.PublishingDrawing = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.AllDrawing);
            this.groupBox1.Controls.Add(this.ThisDrawingOnly);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(271, 90);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Process Which Drawings";
            // 
            // AllDrawing
            // 
            this.AllDrawing.AutoSize = true;
            this.AllDrawing.Location = new System.Drawing.Point(24, 57);
            this.AllDrawing.Name = "AllDrawing";
            this.AllDrawing.Size = new System.Drawing.Size(89, 16);
            this.AllDrawing.TabIndex = 5;
            this.AllDrawing.Text = "All Drawing";
            this.AllDrawing.UseVisualStyleBackColor = true;
            this.AllDrawing.CheckedChanged += new System.EventHandler(this.AllDrawing_CheckedChanged);
            // 
            // ThisDrawingOnly
            // 
            this.ThisDrawingOnly.AutoSize = true;
            this.ThisDrawingOnly.Checked = true;
            this.ThisDrawingOnly.Location = new System.Drawing.Point(24, 27);
            this.ThisDrawingOnly.Name = "ThisDrawingOnly";
            this.ThisDrawingOnly.Size = new System.Drawing.Size(125, 16);
            this.ThisDrawingOnly.TabIndex = 4;
            this.ThisDrawingOnly.TabStop = true;
            this.ThisDrawingOnly.Text = "This Drawing Only";
            this.ThisDrawingOnly.UseVisualStyleBackColor = true;
            this.ThisDrawingOnly.CheckedChanged += new System.EventHandler(this.ThisDrawingOnly_CheckedChanged);
            // 
            // Publish
            // 
            this.Publish.Location = new System.Drawing.Point(300, 14);
            this.Publish.Name = "Publish";
            this.Publish.Size = new System.Drawing.Size(75, 23);
            this.Publish.TabIndex = 1;
            this.Publish.Text = "Publish";
            this.Publish.UseVisualStyleBackColor = true;
            this.Publish.Click += new System.EventHandler(this.Publish_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(300, 48);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Options
            // 
            this.Options.Location = new System.Drawing.Point(300, 84);
            this.Options.Name = "Options";
            this.Options.Size = new System.Drawing.Size(75, 23);
            this.Options.TabIndex = 3;
            this.Options.Text = "Options";
            this.Options.UseVisualStyleBackColor = true;
            this.Options.Click += new System.EventHandler(this.Options_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox1);
            this.groupBox2.Controls.Add(this.NamedView);
            this.groupBox2.Controls.Add(this.CurrentView);
            this.groupBox2.Controls.Add(this.FloorBoundary);
            this.groupBox2.Controls.Add(this.Extents);
            this.groupBox2.Location = new System.Drawing.Point(12, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(363, 155);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "View to Publish";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(149, 122);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(208, 20);
            this.comboBox1.TabIndex = 4;
            // 
            // NamedView
            // 
            this.NamedView.AutoSize = true;
            this.NamedView.Location = new System.Drawing.Point(24, 124);
            this.NamedView.Name = "NamedView";
            this.NamedView.Size = new System.Drawing.Size(83, 16);
            this.NamedView.TabIndex = 3;
            this.NamedView.Text = "Named View";
            this.NamedView.UseVisualStyleBackColor = true;
            this.NamedView.CheckedChanged += new System.EventHandler(this.NamedView_CheckedChanged);
            // 
            // CurrentView
            // 
            this.CurrentView.AutoSize = true;
            this.CurrentView.Location = new System.Drawing.Point(24, 92);
            this.CurrentView.Name = "CurrentView";
            this.CurrentView.Size = new System.Drawing.Size(95, 16);
            this.CurrentView.TabIndex = 2;
            this.CurrentView.Text = "Current View";
            this.CurrentView.UseVisualStyleBackColor = true;
            this.CurrentView.CheckedChanged += new System.EventHandler(this.CurrentView_CheckedChanged);
            // 
            // FloorBoundary
            // 
            this.FloorBoundary.AutoSize = true;
            this.FloorBoundary.Location = new System.Drawing.Point(24, 60);
            this.FloorBoundary.Name = "FloorBoundary";
            this.FloorBoundary.Size = new System.Drawing.Size(107, 16);
            this.FloorBoundary.TabIndex = 1;
            this.FloorBoundary.Text = "Floor Boundary";
            this.FloorBoundary.UseVisualStyleBackColor = true;
            this.FloorBoundary.CheckedChanged += new System.EventHandler(this.FloorBoundary_CheckedChanged);
            // 
            // Extents
            // 
            this.Extents.AutoSize = true;
            this.Extents.Checked = true;
            this.Extents.Location = new System.Drawing.Point(24, 28);
            this.Extents.Name = "Extents";
            this.Extents.Size = new System.Drawing.Size(65, 16);
            this.Extents.TabIndex = 0;
            this.Extents.TabStop = true;
            this.Extents.Text = "Extents";
            this.Extents.UseVisualStyleBackColor = true;
            this.Extents.CheckedChanged += new System.EventHandler(this.Extents_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.PublishingRule);
            this.groupBox3.Controls.Add(this.PublishingDrawing);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(12, 285);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(363, 153);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Progress";
            // 
            // PublishingRule
            // 
            this.PublishingRule.Location = new System.Drawing.Point(13, 113);
            this.PublishingRule.Name = "PublishingRule";
            this.PublishingRule.Size = new System.Drawing.Size(344, 19);
            this.PublishingRule.TabIndex = 3;
            // 
            // PublishingDrawing
            // 
            this.PublishingDrawing.Location = new System.Drawing.Point(13, 50);
            this.PublishingDrawing.Name = "PublishingDrawing";
            this.PublishingDrawing.Size = new System.Drawing.Size(344, 19);
            this.PublishingDrawing.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Publishing Rule:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Publishing Drawing:";
            // 
            // PublishDrawing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 450);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.Options);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Publish);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PublishDrawing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Publish Enterprise Craphics";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Publish;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Options;
        private System.Windows.Forms.RadioButton AllDrawing;
        private System.Windows.Forms.RadioButton ThisDrawingOnly;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.RadioButton NamedView;
        private System.Windows.Forms.RadioButton CurrentView;
        private System.Windows.Forms.RadioButton FloorBoundary;
        private System.Windows.Forms.RadioButton Extents;
        private System.Windows.Forms.ProgressBar PublishingRule;
        private System.Windows.Forms.ProgressBar PublishingDrawing;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}