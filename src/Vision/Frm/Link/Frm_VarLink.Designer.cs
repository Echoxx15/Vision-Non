using System.Windows.Forms;

namespace Vision.Frm.Link
{
  partial class Frm_VarLink
  {
    private System.ComponentModel.IContainer components = null;
    private SplitContainer split;
    private TreeView tree_Stations;
    private DataGridView dgv_List;
    private Button btn_OK;
    private Button btn_Cancel;

    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null)) components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
      split = new SplitContainer();
      tree_Stations = new TreeView();
      dgv_List = new DataGridView();
      btn_OK = new Button();
      btn_Cancel = new Button();
      ((System.ComponentModel.ISupportInitialize)split).BeginInit();
      split.Panel1.SuspendLayout();
      split.Panel2.SuspendLayout();
      split.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)dgv_List).BeginInit();
      SuspendLayout();

      split.Dock = DockStyle.Top;
      split.Height = 360;
      split.SplitterDistance = 220;
      split.Panel1.Controls.Add(tree_Stations);
      split.Panel2.Controls.Add(dgv_List);

      tree_Stations.Dock = DockStyle.Fill;

      dgv_List.Dock = DockStyle.Fill;

      btn_OK.Text = "确定";
      btn_OK.Width = 80;
      btn_OK.Left = 360;
      btn_OK.Top = 370;
      btn_OK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btn_OK.Click += (_, _) => { CommitSelection(); };

      btn_Cancel.Text = "取消";
      btn_Cancel.Width = 80;
      btn_Cancel.Left = 450;
      btn_Cancel.Top = 370;
      btn_Cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      Controls.Add(split);
      Controls.Add(btn_OK);
      Controls.Add(btn_Cancel);
      Text = "变量链接";
      StartPosition = FormStartPosition.CenterParent;
      Width = 560;
      Height = 430;
      Load += Frm_VarLink_Load;

      split.Panel1.ResumeLayout(false);
      split.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)split).EndInit();
      ((System.ComponentModel.ISupportInitialize)dgv_List).EndInit();
      ResumeLayout(false);
    }
  }
}
