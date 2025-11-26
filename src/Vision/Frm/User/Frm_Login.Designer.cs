namespace Vision.Frm.User
{
    partial class Frm_Login
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
      this.label1 = new AntdUI.Label();
      this.cb_Users = new System.Windows.Forms.ComboBox();
      this.label2 = new AntdUI.Label();
      this.tb_Password = new System.Windows.Forms.TextBox();
      this.label3 = new AntdUI.Label();
      this.txt_Role = new AntdUI.Label();
      this.btn_Login = new System.Windows.Forms.Button();
      this.btn_Cancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(27, 76);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(75, 23);
      this.label1.TabIndex = 0;
      this.label1.Text = "用户:";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // cb_Users
      // 
      this.cb_Users.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.cb_Users.FormattingEnabled = true;
      this.cb_Users.Location = new System.Drawing.Point(115, 76);
      this.cb_Users.MaxDropDownItems = 50;
      this.cb_Users.Name = "cb_Users";
      this.cb_Users.Size = new System.Drawing.Size(277, 26);
      this.cb_Users.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(27, 132);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(75, 23);
      this.label2.TabIndex = 2;
      this.label2.Text = "密码:";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // tb_Password
      // 
      this.tb_Password.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.tb_Password.Location = new System.Drawing.Point(115, 127);
      this.tb_Password.Name = "tb_Password";
      this.tb_Password.Size = new System.Drawing.Size(277, 28);
      this.tb_Password.TabIndex = 3;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(27, 190);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(75, 23);
      this.label3.TabIndex = 4;
      this.label3.Text = "角色:";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // txt_Role
      // 
      this.txt_Role.Location = new System.Drawing.Point(115, 190);
      this.txt_Role.Name = "txt_Role";
      this.txt_Role.Size = new System.Drawing.Size(75, 23);
      this.txt_Role.TabIndex = 5;
      this.txt_Role.Text = "";
      this.txt_Role.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // btn_Login
      // 
      this.btn_Login.BackColor = System.Drawing.Color.SeaGreen;
      this.btn_Login.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btn_Login.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_Login.Location = new System.Drawing.Point(115, 320);
      this.btn_Login.Name = "btn_Login";
      this.btn_Login.Size = new System.Drawing.Size(100, 37);
      this.btn_Login.TabIndex = 6;
      this.btn_Login.Text = "登录";
      this.btn_Login.UseVisualStyleBackColor = false;
      // 
      // btn_Cancel
      // 
      this.btn_Cancel.BackColor = System.Drawing.Color.Red;
      this.btn_Cancel.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_Cancel.Location = new System.Drawing.Point(257, 320);
      this.btn_Cancel.Name = "btn_Cancel";
      this.btn_Cancel.Size = new System.Drawing.Size(100, 37);
      this.btn_Cancel.TabIndex = 7;
      this.btn_Cancel.Text = "取消";
      this.btn_Cancel.UseVisualStyleBackColor = false;
      // 
      // Frm_Login
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.ClientSize = new System.Drawing.Size(478, 447);
      this.Controls.Add(this.btn_Cancel);
      this.Controls.Add(this.btn_Login);
      this.Controls.Add(this.txt_Role);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.tb_Password);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.cb_Users);
      this.Controls.Add(this.label1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Frm_Login";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "用户登录";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private AntdUI.Label label1;
        private System.Windows.Forms.ComboBox cb_Users;
        private AntdUI.Label label2;
        private System.Windows.Forms.TextBox tb_Password;
        private AntdUI.Label label3;
        private AntdUI.Label txt_Role;
        private System.Windows.Forms.Button btn_Login;
        private System.Windows.Forms.Button btn_Cancel;
    }
}