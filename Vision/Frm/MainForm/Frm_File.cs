using System.Windows.Forms;
using Vision.SaveImage;
using Vision.Settings;

namespace Vision.Frm.MainForm;

public partial class Frm_File : Form
{
    public Frm_File()
    {
        InitializeComponent();
    }

    private void Frm_File_Load(object sender, System.EventArgs e)
    {
        // 先绑定枚举数据源，再根据配置设置 SelectedItem
        cmb_ImageType.DataSource = System.Enum.GetValues(typeof(SaveImageType));
        cmb_ImageToolType.DataSource = System.Enum.GetValues(typeof(SaveImageType));

        LoadSettingsToUI();

        btn_Select.Click += Btn_Select_Click;
        btn_SaveConfig.Click += Btn_SaveConfig_Click;
    }

    private void LoadSettingsToUI()
    {
        var s = FileSettingsManager.Current;
        txt_Path.Text = s.SavePath;
        chk_SaveRawImage.Checked = s.SaveRawImage;
        chk_SaveDealImage.Checked = s.SaveDealImage;
        chk_SaveOKNG.Checked = s.SeparateOkNg;
        // 根据配置设置下拉框选中项
        cmb_ImageType.SelectedItem = s.RawImageType;
        cmb_ImageToolType.SelectedItem = s.DealImageType;
        txt_days.Text = s.RawRetentionDays.ToString();
        txt_days_Deal.Text = s.DealRetentionDays.ToString();
        chk_Delete.Checked = s.EnableAutoDelete;

        rtn_true.Checked = s.EnableDiskCheck;
        rtn_false.Checked = !s.EnableDiskCheck;
        nud_Threshold.Value = System.Math.Max(nud_Threshold.Minimum, System.Math.Min(nud_Threshold.Maximum, s.DiskThresholdMB));
        dtp_PollTime1.Value = System.DateTime.Today.Add(s.PollTime1);
        dtp_PollTime2.Value = System.DateTime.Today.Add(s.PollTime2);
    }

    private void Btn_Select_Click(object sender, System.EventArgs e)
    {
        using var dlg = new FolderBrowserDialog();
        dlg.Description = "选择图片保存路径";
        dlg.SelectedPath = string.IsNullOrWhiteSpace(txt_Path.Text) ? FileSettingsManager.Current.SavePath : txt_Path.Text;
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            txt_Path.Text = dlg.SelectedPath;
        }
    }

    private void Btn_SaveConfig_Click(object sender, System.EventArgs e)
    {
        var s = new FileSettings();
        s.SavePath = txt_Path.Text;
        s.SaveRawImage = chk_SaveRawImage.Checked;
        s.SaveDealImage = chk_SaveDealImage.Checked;
        s.SeparateOkNg = chk_SaveOKNG.Checked;
        s.RawImageType = (SaveImageType)(cmb_ImageType.SelectedItem ?? SaveImageType.png);
        s.DealImageType = (SaveImageType)(cmb_ImageToolType.SelectedItem ?? SaveImageType.jpg);
        s.EnableAutoDelete = chk_Delete.Checked;
        s.EnableDiskCheck = rtn_true.Checked;
        s.DiskThresholdMB = (int)nud_Threshold.Value;
        if (int.TryParse(txt_days.Text, out var d1)) s.RawRetentionDays = d1;
        if (int.TryParse(txt_days_Deal.Text, out var d2)) s.DealRetentionDays = d2;
        s.PollTime1 = dtp_PollTime1.Value.TimeOfDay;
        s.PollTime2 = dtp_PollTime2.Value.TimeOfDay;

        FileSettingsManager.Update(s);
        MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}