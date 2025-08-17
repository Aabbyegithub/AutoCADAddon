namespace AutoCADAddon
{
    partial class LoginForm
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
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblServer = new System.Windows.Forms.Label();
            this.lbProject = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.IsRenember = new System.Windows.Forms.CheckBox();
            this.Password = new System.Windows.Forms.TextBox();
            this.UserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ProjectList = new System.Windows.Forms.ComboBox();
            this.cboServers = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(67, 218);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 0;
            this.btnLogin.Text = "OK";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(221, 218);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(41, 16);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(47, 12);
            this.lblServer.TabIndex = 2;
            this.lblServer.Text = "Server:";
            // 
            // lbProject
            // 
            this.lbProject.AutoSize = true;
            this.lbProject.Location = new System.Drawing.Point(35, 47);
            this.lbProject.Name = "lbProject";
            this.lbProject.Size = new System.Drawing.Size(53, 12);
            this.lbProject.TabIndex = 3;
            this.lbProject.Text = "Project:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.IsRenember);
            this.groupBox1.Controls.Add(this.Password);
            this.groupBox1.Controls.Add(this.UserName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 73);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(378, 127);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Credentrials";
            // 
            // IsRenember
            // 
            this.IsRenember.AutoSize = true;
            this.IsRenember.Location = new System.Drawing.Point(169, 94);
            this.IsRenember.Name = "IsRenember";
            this.IsRenember.Size = new System.Drawing.Size(15, 14);
            this.IsRenember.TabIndex = 5;
            this.IsRenember.UseVisualStyleBackColor = true;
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(78, 52);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(280, 21);
            this.Password.TabIndex = 4;
            this.Password.UseSystemPasswordChar = true;
            // 
            // UserName
            // 
            this.UserName.Location = new System.Drawing.Point(78, 22);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(280, 21);
            this.UserName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Remember your password:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "User Name:";
            // 
            // ProjectList
            // 
            this.ProjectList.FormattingEnabled = true;
            this.ProjectList.Location = new System.Drawing.Point(88, 42);
            this.ProjectList.Name = "ProjectList";
            this.ProjectList.Size = new System.Drawing.Size(301, 20);
            this.ProjectList.TabIndex = 5;
            // 
            // cboServers
            // 
            this.cboServers.FormattingEnabled = true;
            this.cboServers.Location = new System.Drawing.Point(88, 14);
            this.cboServers.Name = "cboServers";
            this.cboServers.Size = new System.Drawing.Size(301, 20);
            this.cboServers.TabIndex = 6;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 253);
            this.Controls.Add(this.cboServers);
            this.Controls.Add(this.ProjectList);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbProject);
            this.Controls.Add(this.lblServer);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLogin);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sign in";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Label lbProject;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ProjectList;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.TextBox UserName;
        private System.Windows.Forms.CheckBox IsRenember;
        public System.Windows.Forms.ComboBox cboServers;
    }
}