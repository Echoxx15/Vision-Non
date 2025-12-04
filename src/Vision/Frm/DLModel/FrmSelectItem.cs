using System.Windows.Forms;

namespace Vision.Frm.DLModel;

/// <summary>
/// 简单的列表选择对话框
/// </summary>
public class FrmSelectItem : Form
{
    private ListBox listBox;
    private Button btnOk;
    private Button btnCancel;
    
    /// <summary>
    /// 获取用户选择的项
    /// </summary>
    public string SelectedItem { get; private set; }

    public FrmSelectItem(string title, string[] items)
    {
        InitializeComponent();
        Text = title;
        listBox.Items.AddRange(items);
        if (listBox.Items.Count > 0)
            listBox.SelectedIndex = 0;
    }

    private void InitializeComponent()
    {
        listBox = new ListBox();
        btnOk = new Button();
        btnCancel = new Button();
        SuspendLayout();
        
        // listBox
        listBox.Dock = DockStyle.Top;
        listBox.Height = 200;
        listBox.IntegralHeight = false;
        listBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
        listBox.DoubleClick += (s, e) => { if (listBox.SelectedItem != null) { SelectedItem = listBox.SelectedItem.ToString(); DialogResult = DialogResult.OK; Close(); } };
        
        // btnOk
        btnOk.Text = "确定";
        btnOk.DialogResult = DialogResult.OK;
        btnOk.Location = new System.Drawing.Point(60, 210);
        btnOk.Size = new System.Drawing.Size(80, 30);
        btnOk.Click += (s, e) => { SelectedItem = listBox.SelectedItem?.ToString(); };
        
        // btnCancel
        btnCancel.Text = "取消";
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new System.Drawing.Point(160, 210);
        btnCancel.Size = new System.Drawing.Size(80, 30);
        
        // Form
        ClientSize = new System.Drawing.Size(300, 260);
        Controls.Add(listBox);
        Controls.Add(btnOk);
        Controls.Add(btnCancel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        AcceptButton = btnOk;
        CancelButton = btnCancel;
        
        ResumeLayout(false);
    }
}
