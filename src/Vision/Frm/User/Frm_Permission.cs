using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vision.Auth;

namespace Vision.Frm.User;

public partial class Frm_Permission : Form
{
    private readonly List<(string Name, ToolStripItem Item)> _items = new();

    public Frm_Permission(IEnumerable<ToolStripItem> items)
    {
        InitializeComponent();
        foreach (var it in items ?? Enumerable.Empty<ToolStripItem>())
        {
            _items.Add((it.Name, it));
        }
        Load += Frm_Permission_Load;
        btn_Confirm.Click += Btn_Confirm_Click;
        btn_Cancel.Click += (_, _) => Close();
        comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
    }

    private void Frm_Permission_Load(object sender, EventArgs e)
    {
        comboBox1.Items.Clear();
        comboBox1.Items.AddRange(RoleHelper.GetAllDisplays());
        if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
        LoadRoleAssignments();
    }

    private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadRoleAssignments();
    }

    private void LoadRoleAssignments()
    {
        clb_Items.Items.Clear();
        var role = RoleHelper.ParseDisplay(comboBox1.SelectedItem?.ToString() ?? string.Empty);
        var map = PermissionManager.GetRoleAssignments(role).ToDictionary(t => t.Name, t => t.Enabled == 1, StringComparer.OrdinalIgnoreCase);
        foreach (var (name, item) in _items)
        {
            var text = string.IsNullOrWhiteSpace(item.Text) ? name : item.Text;
            var isChecked = map.TryGetValue(name, out var en) ? en : false;
            clb_Items.Items.Add(new ItemWrap { Name = name, Text = text }, isChecked);
        }
        clb_Items.DisplayMember = nameof(ItemWrap.Text);
        clb_Items.ValueMember = nameof(ItemWrap.Name);
    }

    private void Btn_Confirm_Click(object sender, EventArgs e)
    {
        var role = RoleHelper.ParseDisplay(comboBox1.SelectedItem?.ToString() ?? string.Empty);
        var allNames = _items.Select(t => t.Name).ToList();
        var checkedNames = new List<string>();
        foreach (var obj in clb_Items.CheckedItems)
        {
            if (obj is ItemWrap wrap && !string.IsNullOrEmpty(wrap.Name)) checkedNames.Add(wrap.Name);
        }
        var uncheckedNames = allNames.Except(checkedNames, StringComparer.OrdinalIgnoreCase);
        PermissionManager.SetRolePermissions(role, uncheckedNames, false);
        PermissionManager.SetRolePermissions(role, checkedNames, true);
        MessageBox.Show("已应用");
        Close();
    }

    private sealed class ItemWrap
    {
        public string Name { get; set; }
        public string Text { get; set; }
    }
}