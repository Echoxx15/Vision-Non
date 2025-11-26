using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HardwareCommNet.UI
{
 /// <summary>
 /// 触发值编辑小窗体
 /// </summary>
 public partial class Frm_TriggerValuesEditor : Form
 {
 private readonly List<string> _values;
 
 public List<string> TriggerValues => _values;

 public Frm_TriggerValuesEditor(List<string> initialValues)
 {
 _values = new List<string>(initialValues ?? new List<string>());
 InitializeComponent();
 LoadValues();
 }

 private void InitializeComponent()
 {
 this.listView1 = new ListView();
 this.btn_Add = new Button();
 this.btn_Remove = new Button();
 this.btn_OK = new Button();
 this.btn_Cancel = new Button();
 
 this.SuspendLayout();
 
 // listView1
 this.listView1.Location = new System.Drawing.Point(12, 12);
 this.listView1.Size = new System.Drawing.Size(360, 250);
 this.listView1.View = View.Details;
 this.listView1.FullRowSelect = true;
 this.listView1.GridLines = true;
 this.listView1.Columns.Add("触发值", 330);
 this.listView1.LabelEdit = true;
 this.listView1.AfterLabelEdit += ListView1_AfterLabelEdit;
 
 // btn_Add
 this.btn_Add.Location = new System.Drawing.Point(12, 270);
 this.btn_Add.Size = new System.Drawing.Size(75, 30);
 this.btn_Add.Text = "添加";
 this.btn_Add.Click += Btn_Add_Click;
 
 // btn_Remove
 this.btn_Remove.Location = new System.Drawing.Point(93, 270);
 this.btn_Remove.Size = new System.Drawing.Size(75, 30);
 this.btn_Remove.Text = "删除";
 this.btn_Remove.Click += Btn_Remove_Click;
 
 // btn_OK
 this.btn_OK.Location = new System.Drawing.Point(216, 270);
 this.btn_OK.Size = new System.Drawing.Size(75, 30);
 this.btn_OK.Text = "确定";
 this.btn_OK.DialogResult = DialogResult.OK;
 
 // btn_Cancel
 this.btn_Cancel.Location = new System.Drawing.Point(297, 270);
 this.btn_Cancel.Size = new System.Drawing.Size(75, 30);
 this.btn_Cancel.Text = "取消";
 this.btn_Cancel.DialogResult = DialogResult.Cancel;
 
 // Form
 this.ClientSize = new System.Drawing.Size(384, 312);
 this.Controls.Add(this.listView1);
 this.Controls.Add(this.btn_Add);
 this.Controls.Add(this.btn_Remove);
 this.Controls.Add(this.btn_OK);
 this.Controls.Add(this.btn_Cancel);
 this.FormBorderStyle = FormBorderStyle.FixedDialog;
 this.MaximizeBox = false;
 this.MinimizeBox = false;
 this.StartPosition = FormStartPosition.CenterParent;
 this.Text = "编辑触发值";
 this.AcceptButton = this.btn_OK;
 this.CancelButton = this.btn_Cancel;
 
 this.ResumeLayout(false);
 }

 private ListView listView1;
 private Button btn_Add;
 private Button btn_Remove;
 private Button btn_OK;
 private Button btn_Cancel;

 private void LoadValues()
 {
 listView1.Items.Clear();
 foreach (var v in _values)
 {
 listView1.Items.Add(v);
 }
 }

 private void Btn_Add_Click(object sender, EventArgs e)
 {
 var item = listView1.Items.Add("新值");
 item.BeginEdit();
 }

 private void Btn_Remove_Click(object sender, EventArgs e)
 {
 if (listView1.SelectedItems.Count > 0)
 {
 listView1.SelectedItems[0].Remove();
 SyncToList();
 }
 }

 private void ListView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
 {
 if (e.Label != null)
 {
 listView1.Items[e.Item].Text = e.Label;
 }
 SyncToList();
 }

 private void SyncToList()
 {
 _values.Clear();
 foreach (ListViewItem item in listView1.Items)
 {
 if (!string.IsNullOrWhiteSpace(item.Text))
 _values.Add(item.Text);
 }
 }
 
 protected override void OnFormClosing(FormClosingEventArgs e)
 {
 if (DialogResult == DialogResult.OK)
 {
 SyncToList();
 }
 base.OnFormClosing(e);
 }
 }
}
