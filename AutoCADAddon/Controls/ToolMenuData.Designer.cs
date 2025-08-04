namespace AutoCADControls.Controls
{
    partial class ToolMenuData
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Open = new System.Windows.Forms.ToolStripLabel();
            this.New = new System.Windows.Forms.ToolStripLabel();
            this.Add = new System.Windows.Forms.ToolStripLabel();
            this.Remove = new System.Windows.Forms.ToolStripLabel();
            this.Properties = new System.Windows.Forms.ToolStripLabel();
            this.Refresh = new System.Windows.Forms.ToolStripLabel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.DrawingTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DrawingName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Open,
            this.New,
            this.Add,
            this.Remove,
            this.Properties,
            this.Refresh});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.toolStrip1.Size = new System.Drawing.Size(978, 34);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Open
            // 
            this.Open.Name = "Open";
            this.Open.Size = new System.Drawing.Size(67, 28);
            this.Open.Text = "Open";
            this.Open.Click += new System.EventHandler(this.Open_Click);
            // 
            // New
            // 
            this.New.Name = "New";
            this.New.Size = new System.Drawing.Size(58, 28);
            this.New.Text = "New";
            this.New.Click += new System.EventHandler(this.New_Click);
            // 
            // Add
            // 
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(53, 28);
            this.Add.Text = "Add";
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // Remove
            // 
            this.Remove.Name = "Remove";
            this.Remove.Size = new System.Drawing.Size(94, 28);
            this.Remove.Text = "Remove";
            this.Remove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // Properties
            // 
            this.Properties.Name = "Properties";
            this.Properties.Size = new System.Drawing.Size(115, 28);
            this.Properties.Text = "Properties";
            this.Properties.Click += new System.EventHandler(this.Properties_Click);
            // 
            // Refresh
            // 
            this.Refresh.Name = "Refresh";
            this.Refresh.Size = new System.Drawing.Size(88, 28);
            this.Refresh.Text = "Refresh";
            this.Refresh.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DrawingTitle,
            this.DrawingName});
            this.dataGridView1.Location = new System.Drawing.Point(6, 56);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 72;
            this.dataGridView1.RowTemplate.Height = 50;
            this.dataGridView1.Size = new System.Drawing.Size(846, 634);
            this.dataGridView1.TabIndex = 1;
            // 
            // DrawingTitle
            // 
            this.DrawingTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DrawingTitle.HeaderText = "Drawing Title";
            this.DrawingTitle.MinimumWidth = 9;
            this.DrawingTitle.Name = "DrawingTitle";
            this.DrawingTitle.ReadOnly = true;
            this.DrawingTitle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // DrawingName
            // 
            this.DrawingName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DrawingName.HeaderText = "Drawing Name";
            this.DrawingName.MinimumWidth = 9;
            this.DrawingName.Name = "DrawingName";
            this.DrawingName.ReadOnly = true;
            this.DrawingName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ToolMenuData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ToolMenuData";
            this.Size = new System.Drawing.Size(978, 1000);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel Open;
        private System.Windows.Forms.ToolStripLabel New;
        private System.Windows.Forms.ToolStripLabel Add;
        private System.Windows.Forms.ToolStripLabel Remove;
        private System.Windows.Forms.ToolStripLabel Properties;
        private System.Windows.Forms.ToolStripLabel Refresh;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn DrawingTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn DrawingName;
    }
}
