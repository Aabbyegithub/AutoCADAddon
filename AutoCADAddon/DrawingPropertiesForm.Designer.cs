namespace AutoCADAddon
{
    partial class DrawingPropertiesForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.Filename = new System.Windows.Forms.TextBox();
            this.Title = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BtnNewFloor = new System.Windows.Forms.Button();
            this.BtnNewBuilding = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbMet = new System.Windows.Forms.ComboBox();
            this.Metric = new System.Windows.Forms.RadioButton();
            this.Imperial = new System.Windows.Forms.RadioButton();
            this.Path1 = new System.Windows.Forms.Label();
            this.Version = new System.Windows.Forms.Label();
            this.cmbFloor = new AutoCADAddon.Controls.TableDropdownBox();
            this.cmbBuilding = new AutoCADAddon.Controls.TableDropdownBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "File name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 68);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "Title:";
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(656, 14);
            this.BtnOK.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(138, 40);
            this.BtnOK.TabIndex = 2;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(656, 61);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(138, 40);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Filename
            // 
            this.Filename.Location = new System.Drawing.Point(156, 14);
            this.Filename.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Filename.Name = "Filename";
            this.Filename.Size = new System.Drawing.Size(486, 31);
            this.Filename.TabIndex = 4;
            // 
            // Title
            // 
            this.Title.Location = new System.Drawing.Point(156, 61);
            this.Title.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(486, 31);
            this.Title.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbFloor);
            this.groupBox1.Controls.Add(this.cmbBuilding);
            this.groupBox1.Controls.Add(this.BtnNewFloor);
            this.groupBox1.Controls.Add(this.BtnNewBuilding);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(22, 122);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.groupBox1.Size = new System.Drawing.Size(772, 164);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default space hierarchy values";
            // 
            // BtnNewFloor
            // 
            this.BtnNewFloor.Location = new System.Drawing.Point(691, 94);
            this.BtnNewFloor.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.BtnNewFloor.Name = "BtnNewFloor";
            this.BtnNewFloor.Size = new System.Drawing.Size(53, 40);
            this.BtnNewFloor.TabIndex = 5;
            this.BtnNewFloor.Text = "+";
            this.BtnNewFloor.UseVisualStyleBackColor = true;
            this.BtnNewFloor.Click += new System.EventHandler(this.BtnNewFloor_Click);
            // 
            // BtnNewBuilding
            // 
            this.BtnNewBuilding.Location = new System.Drawing.Point(691, 40);
            this.BtnNewBuilding.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.BtnNewBuilding.Name = "BtnNewBuilding";
            this.BtnNewBuilding.Size = new System.Drawing.Size(53, 40);
            this.BtnNewBuilding.TabIndex = 4;
            this.BtnNewBuilding.Text = "+";
            this.BtnNewBuilding.UseVisualStyleBackColor = true;
            this.BtnNewBuilding.Click += new System.EventHandler(this.BtnNewBuilding_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 103);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 21);
            this.label4.TabIndex = 1;
            this.label4.Text = "Floor:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 49);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 21);
            this.label3.TabIndex = 0;
            this.label3.Text = "Building:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbMet);
            this.groupBox2.Controls.Add(this.Metric);
            this.groupBox2.Controls.Add(this.Imperial);
            this.groupBox2.Location = new System.Drawing.Point(22, 298);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.groupBox2.Size = new System.Drawing.Size(772, 131);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Units";
            // 
            // cmbMet
            // 
            this.cmbMet.FormattingEnabled = true;
            this.cmbMet.Location = new System.Drawing.Point(147, 74);
            this.cmbMet.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.cmbMet.Name = "cmbMet";
            this.cmbMet.Size = new System.Drawing.Size(530, 29);
            this.cmbMet.TabIndex = 2;
            // 
            // Metric
            // 
            this.Metric.AutoSize = true;
            this.Metric.Location = new System.Drawing.Point(16, 77);
            this.Metric.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Metric.Name = "Metric";
            this.Metric.Size = new System.Drawing.Size(101, 25);
            this.Metric.TabIndex = 1;
            this.Metric.TabStop = true;
            this.Metric.Text = "Metric";
            this.Metric.UseVisualStyleBackColor = true;
            this.Metric.Click += new System.EventHandler(this.Metric_Click);
            // 
            // Imperial
            // 
            this.Imperial.AutoSize = true;
            this.Imperial.Location = new System.Drawing.Point(16, 37);
            this.Imperial.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Imperial.Name = "Imperial";
            this.Imperial.Size = new System.Drawing.Size(123, 25);
            this.Imperial.TabIndex = 0;
            this.Imperial.TabStop = true;
            this.Imperial.Text = "Imperial";
            this.Imperial.UseVisualStyleBackColor = true;
            this.Imperial.Click += new System.EventHandler(this.Imperial_Click);
            // 
            // Path1
            // 
            this.Path1.AutoSize = true;
            this.Path1.Location = new System.Drawing.Point(24, 453);
            this.Path1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Path1.Name = "Path1";
            this.Path1.Size = new System.Drawing.Size(65, 21);
            this.Path1.TabIndex = 9;
            this.Path1.Text = "Path:";
            // 
            // Version
            // 
            this.Version.AutoSize = true;
            this.Version.Location = new System.Drawing.Point(28, 500);
            this.Version.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(285, 21);
            this.Version.TabIndex = 10;
            this.Version.Text = "AutoCAD Drawing Version :";
            // 
            // cmbFloor
            // 
            this.cmbFloor.Location = new System.Drawing.Point(134, 91);
            this.cmbFloor.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.cmbFloor.Name = "cmbFloor";
            this.cmbFloor.Size = new System.Drawing.Size(546, 46);
            this.cmbFloor.TabIndex = 7;
            // 
            // cmbBuilding
            // 
            this.cmbBuilding.Location = new System.Drawing.Point(134, 37);
            this.cmbBuilding.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.cmbBuilding.Name = "cmbBuilding";
            this.cmbBuilding.Size = new System.Drawing.Size(546, 46);
            this.cmbBuilding.TabIndex = 6;
            this.cmbBuilding.SelectedItemChanged += new System.EventHandler<AutoCADAddon.Controls.ItemData>(this.cmbBuilding_SelectedItemChanged);
            // 
            // DrawingPropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 548);
            this.Controls.Add(this.Version);
            this.Controls.Add(this.Path1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.Filename);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DrawingPropertiesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Drawing Properties";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox Filename;
        private System.Windows.Forms.TextBox Title;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button BtnNewFloor;
        private System.Windows.Forms.Button BtnNewBuilding;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton Metric;
        private System.Windows.Forms.RadioButton Imperial;
        private System.Windows.Forms.ComboBox cmbMet;
        private System.Windows.Forms.Label Path1;
        private System.Windows.Forms.Label Version;
        private Controls.TableDropdownBox cmbFloor;
        private Controls.TableDropdownBox cmbBuilding;
    }
}