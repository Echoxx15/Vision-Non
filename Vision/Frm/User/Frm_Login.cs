using System;
using System.Linq;
using System.Windows.Forms;
using Logger;
using Vision.Auth;

namespace Vision.Frm.User;

/// <summary>
/// 登录窗口：展示用户列表，输入密码登录。
/// </summary>
public partial class Frm_Login : Form
{
    public Frm_Login()
    {
        InitializeComponent();
        Load += Frm_Login_Load;
        btn_Login.Click += Btn_Login_Click;
        btn_Cancel.Click += (_, _) => this.Close();
        tb_Password.PasswordChar = '•';
    }

    private void Frm_Login_Load(object sender, EventArgs e)
    {
        cb_Users.Items.Clear();
        cb_Users.Items.AddRange(UserManager.Instance.GetAllUsers().Select(u => u.Username).ToArray<object>());
        if (cb_Users.Items.Count > 0) cb_Users.SelectedIndex = 0;
        cb_Users.SelectedIndexChanged += (_, _) => RefreshRole();
        RefreshRole();
        tb_Password.Focus();
    }

    private void RefreshRole()
    {
        var name = cb_Users.SelectedItem as string;
        var user = UserManager.Instance.GetAllUsers().FirstOrDefault(u => string.Equals(u.Username, name, StringComparison.OrdinalIgnoreCase));
        txt_Role.Text = user == null ? string.Empty : RoleHelper.GetDisplay(user.Role);
    }

    private void Btn_Login_Click(object sender, EventArgs e)
    {
        var name = cb_Users.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(name)) { MessageBox.Show("请选择用户"); return; }
        if (!UserManager.Instance.Login(name, tb_Password.Text))
        {
            MessageBox.Show("用户名或密码错误");
            tb_Password.Text = string.Empty;
            return;
        }
        LogHelper.Info($"用户登录成功: {name}");
        DialogResult = DialogResult.OK;
        Close();
    }
}