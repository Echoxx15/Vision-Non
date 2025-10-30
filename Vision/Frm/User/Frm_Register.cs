using System;
using System.Windows.Forms;
using Vision.Auth;

namespace Vision.Frm.User;

/// <summary>
/// 注册窗口：输入用户名、密码与角色，写入 SQLite。
/// </summary>
public partial class Frm_Register : Form
{
    public Frm_Register()
    {
        InitializeComponent();
        Load += Frm_Register_Load;
        btn_Register.Click += Btn_Register_Click;
        btn_Cancel.Click += (_, _) => Close();
    }

    private void Frm_Register_Load(object sender, EventArgs e)
    {
        // 使用中文显示的角色
        cb_Roles.Items.Clear();
        cb_Roles.Items.AddRange(RoleHelper.GetAllDisplays());
        if (cb_Roles.Items.Count > 0) cb_Roles.SelectedIndex = 0;
    }

    private void Btn_Register_Click(object sender, EventArgs e)
    {
        var name = tb_UserName.Text?.Trim();
        var pwd = tb_Password.Text ?? string.Empty;
        var selected = cb_Roles.SelectedItem?.ToString();
        var role = RoleHelper.ParseDisplay(selected ?? string.Empty);
        if (UserManager.Instance.Register(name, pwd, role, out var error))
        {
            MessageBox.Show("注册成功");
            Close();
        }
        else
        {
            MessageBox.Show(error ?? "注册失败");
        }
    }
}