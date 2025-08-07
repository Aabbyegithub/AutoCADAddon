namespace AutoCADAddon
{
    partial class RoomEditForm
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
            this.panel = new System.Windows.Forms.Panel();
            this.Prorate = new System.Windows.Forms.ComboBox();
            this.Length = new System.Windows.Forms.TextBox();
            this.Area = new System.Windows.Forms.TextBox();
            this.FloorCode = new AutoCADAddon.Controls.TableDropdownBox();
            this.RoomCodeBox = new AutoCADAddon.Controls.TableDropdownBox();
            this.RoomStanard = new AutoCADAddon.Controls.TableDropdownBox();
            this.DivisionCode = new AutoCADAddon.Controls.TableDropdownBox();
            this.RoomCategory = new AutoCADAddon.Controls.TableDropdownBox();
            this.RoomType = new AutoCADAddon.Controls.TableDropdownBox();
            this.DepartmentCode = new AutoCADAddon.Controls.TableDropdownBox();
            this.BuildingCode = new AutoCADAddon.Controls.TableDropdownBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExtensions = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.Prorate);
            this.panel.Controls.Add(this.Length);
            this.panel.Controls.Add(this.Area);
            this.panel.Controls.Add(this.FloorCode);
            this.panel.Controls.Add(this.RoomCodeBox);
            this.panel.Controls.Add(this.RoomStanard);
            this.panel.Controls.Add(this.DivisionCode);
            this.panel.Controls.Add(this.RoomCategory);
            this.panel.Controls.Add(this.RoomType);
            this.panel.Controls.Add(this.DepartmentCode);
            this.panel.Controls.Add(this.BuildingCode);
            this.panel.Controls.Add(this.label15);
            this.panel.Controls.Add(this.label14);
            this.panel.Controls.Add(this.label13);
            this.panel.Controls.Add(this.label12);
            this.panel.Controls.Add(this.label11);
            this.panel.Controls.Add(this.label10);
            this.panel.Controls.Add(this.label9);
            this.panel.Controls.Add(this.label8);
            this.panel.Controls.Add(this.label7);
            this.panel.Controls.Add(this.label6);
            this.panel.Controls.Add(this.btnCancel);
            this.panel.Controls.Add(this.btnSave);
            this.panel.Controls.Add(this.btnExtensions);
            this.panel.Controls.Add(this.label5);
            this.panel.Controls.Add(this.label4);
            this.panel.Controls.Add(this.label3);
            this.panel.Controls.Add(this.label2);
            this.panel.Controls.Add(this.label1);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(1018, 710);
            this.panel.TabIndex = 3;
            // 
            // Prorate
            // 
            this.Prorate.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Prorate.FormattingEnabled = true;
            this.Prorate.Items.AddRange(new object[] {
            "None",
            "FLOOR",
            "BUILDING",
            "SITE"});
            this.Prorate.Location = new System.Drawing.Point(227, 511);
            this.Prorate.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Prorate.Name = "Prorate";
            this.Prorate.Size = new System.Drawing.Size(569, 36);
            this.Prorate.TabIndex = 36;
            // 
            // Length
            // 
            this.Length.Font = new System.Drawing.Font("宋体", 14.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Length.Location = new System.Drawing.Point(227, 632);
            this.Length.Margin = new System.Windows.Forms.Padding(4);
            this.Length.Name = "Length";
            this.Length.ReadOnly = true;
            this.Length.Size = new System.Drawing.Size(572, 45);
            this.Length.TabIndex = 35;
            // 
            // Area
            // 
            this.Area.Font = new System.Drawing.Font("宋体", 14.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Area.Location = new System.Drawing.Point(227, 570);
            this.Area.Margin = new System.Windows.Forms.Padding(4);
            this.Area.Name = "Area";
            this.Area.ReadOnly = true;
            this.Area.Size = new System.Drawing.Size(572, 45);
            this.Area.TabIndex = 34;
            // 
            // FloorCode
            // 
            this.FloorCode.Location = new System.Drawing.Point(227, 80);
            this.FloorCode.Margin = new System.Windows.Forms.Padding(4);
            this.FloorCode.Name = "FloorCode";
            this.FloorCode.Size = new System.Drawing.Size(572, 47);
            this.FloorCode.TabIndex = 32;
            // 
            // RoomCodeBox
            // 
            this.RoomCodeBox.Location = new System.Drawing.Point(227, 142);
            this.RoomCodeBox.Margin = new System.Windows.Forms.Padding(4);
            this.RoomCodeBox.Name = "RoomCodeBox";
            this.RoomCodeBox.Size = new System.Drawing.Size(572, 47);
            this.RoomCodeBox.TabIndex = 31;
            // 
            // RoomStanard
            // 
            this.RoomStanard.Location = new System.Drawing.Point(227, 203);
            this.RoomStanard.Margin = new System.Windows.Forms.Padding(4);
            this.RoomStanard.Name = "RoomStanard";
            this.RoomStanard.Size = new System.Drawing.Size(572, 47);
            this.RoomStanard.TabIndex = 30;
            // 
            // DivisionCode
            // 
            this.DivisionCode.Location = new System.Drawing.Point(227, 264);
            this.DivisionCode.Margin = new System.Windows.Forms.Padding(4);
            this.DivisionCode.Name = "DivisionCode";
            this.DivisionCode.Size = new System.Drawing.Size(572, 47);
            this.DivisionCode.TabIndex = 29;
            this.DivisionCode.SelectedItemChanged += new System.EventHandler<AutoCADAddon.Controls.ItemData>(this.DivisionCode_SelectedItemChanged);
            // 
            // RoomCategory
            // 
            this.RoomCategory.Location = new System.Drawing.Point(227, 387);
            this.RoomCategory.Margin = new System.Windows.Forms.Padding(4);
            this.RoomCategory.Name = "RoomCategory";
            this.RoomCategory.Size = new System.Drawing.Size(572, 47);
            this.RoomCategory.TabIndex = 28;
            this.RoomCategory.SelectedItemChanged += new System.EventHandler<AutoCADAddon.Controls.ItemData>(this.RoomCategory_SelectedItemChanged);
            // 
            // RoomType
            // 
            this.RoomType.Location = new System.Drawing.Point(227, 446);
            this.RoomType.Margin = new System.Windows.Forms.Padding(4);
            this.RoomType.Name = "RoomType";
            this.RoomType.Size = new System.Drawing.Size(572, 47);
            this.RoomType.TabIndex = 27;
            // 
            // DepartmentCode
            // 
            this.DepartmentCode.Location = new System.Drawing.Point(227, 326);
            this.DepartmentCode.Margin = new System.Windows.Forms.Padding(4);
            this.DepartmentCode.Name = "DepartmentCode";
            this.DepartmentCode.Size = new System.Drawing.Size(572, 47);
            this.DepartmentCode.TabIndex = 26;
            // 
            // BuildingCode
            // 
            this.BuildingCode.Location = new System.Drawing.Point(227, 19);
            this.BuildingCode.Margin = new System.Windows.Forms.Padding(4);
            this.BuildingCode.Name = "BuildingCode";
            this.BuildingCode.Size = new System.Drawing.Size(572, 47);
            this.BuildingCode.TabIndex = 25;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.SystemColors.Control;
            this.label15.ForeColor = System.Drawing.Color.Red;
            this.label15.Location = new System.Drawing.Point(136, 156);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(21, 21);
            this.label15.TabIndex = 24;
            this.label15.Text = "*";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.SystemColors.Control;
            this.label14.ForeColor = System.Drawing.Color.Red;
            this.label14.Location = new System.Drawing.Point(147, 94);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(21, 21);
            this.label14.TabIndex = 23;
            this.label14.Text = "*";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.SystemColors.Control;
            this.label13.ForeColor = System.Drawing.Color.Red;
            this.label13.Location = new System.Drawing.Point(106, 522);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(21, 21);
            this.label13.TabIndex = 22;
            this.label13.Text = "*";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.SystemColors.Control;
            this.label12.ForeColor = System.Drawing.Color.Red;
            this.label12.Location = new System.Drawing.Point(174, 33);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(21, 21);
            this.label12.TabIndex = 21;
            this.label12.Text = "*";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 642);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(163, 21);
            this.label11.TabIndex = 20;
            this.label11.Text = "Perimeter ft：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 583);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(174, 21);
            this.label10.TabIndex = 19;
            this.label10.Text = "Room Area ft²：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 522);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(108, 21);
            this.label9.TabIndex = 18;
            this.label9.Text = "Prorate：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 460);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(130, 21);
            this.label8.TabIndex = 17;
            this.label8.Text = "Room Type：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 399);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(174, 21);
            this.label7.TabIndex = 16;
            this.label7.Text = "Room Category：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 338);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(196, 21);
            this.label6.TabIndex = 15;
            this.label6.Text = "Department Code：";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(829, 86);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(138, 40);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(829, 19);
            this.btnSave.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(138, 40);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "OK";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnExtensions
            // 
            this.btnExtensions.Location = new System.Drawing.Point(829, 156);
            this.btnExtensions.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnExtensions.Name = "btnExtensions";
            this.btnExtensions.Size = new System.Drawing.Size(161, 40);
            this.btnExtensions.TabIndex = 12;
            this.btnExtensions.Text = "扩展字段...";
            this.btnExtensions.UseVisualStyleBackColor = true;
            this.btnExtensions.Click += new System.EventHandler(this.BtnExtensions_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 276);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(174, 21);
            this.label5.TabIndex = 9;
            this.label5.Text = "Division Code：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 215);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(163, 21);
            this.label4.TabIndex = 8;
            this.label4.Text = "Room Stanard：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 156);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 21);
            this.label3.TabIndex = 6;
            this.label3.Text = "Room Code:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 94);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "Floor Code：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Building Code：";
            // 
            // RoomEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 710);
            this.Controls.Add(this.panel);
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RoomEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Data(Asset Type:Rooms)";
            this.TopMost = true;
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnExtensions;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox Length;
        private System.Windows.Forms.TextBox Area;
        private Controls.TableDropdownBox FloorCode;
        private Controls.TableDropdownBox RoomCodeBox;
        private Controls.TableDropdownBox RoomStanard;
        private Controls.TableDropdownBox DivisionCode;
        private Controls.TableDropdownBox RoomCategory;
        private Controls.TableDropdownBox RoomType;
        private Controls.TableDropdownBox DepartmentCode;
        private Controls.TableDropdownBox BuildingCode;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox Prorate;
    }
}