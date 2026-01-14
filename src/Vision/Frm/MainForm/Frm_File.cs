using System;
using System.Windows.Forms;
using Vision.Localization;
using Vision.SaveImage;
using Vision.Settings;

namespace Vision.Frm.MainForm;

public partial class Frm_File : Form, ILocalizable
{
    public Frm_File()
    {
        InitializeComponent();
        
        // 订阅语言变更事件
        LanguageService.Instance.LanguageChanged += OnLanguageChanged;
        
        // 窗体关闭时取消订阅
        FormClosed += (_, _) => LanguageService.Instance.LanguageChanged -= OnLanguageChanged;
    }
    
    private void OnLanguageChanged(object sender, LanguageChangedEventArgs e)
    {
        if (IsDisposed) return;
        if (InvokeRequired)
            BeginInvoke(new Action(ApplyLanguage));
        else
            ApplyLanguage();
    }

    private void Frm_File_Load(object sender, EventArgs e)
    {
        // 先绑定枚举数据源，再根据配置设置 SelectedItem
        cmb_ImageType.DataSource = Enum.GetValues(typeof(SaveImageType));
        cmb_ImageToolType.DataSource = Enum.GetValues(typeof(SaveImageType));

        LoadSettingsToUI();

        btn_Select.Click += Btn_Select_Click;
        btn_SaveConfig.Click += Btn_SaveConfig_Click;
        
        // 应用当前语言
        ApplyLanguage();
    }
    
    /// <summary>
    /// 应用当前语言到界面控件
    /// </summary>
    public void ApplyLanguage()
    {
        var lang = LanguageService.Instance;
        
        // 窗体标题
        Text = lang.Get(LangKeys.File_Title);
        
        // 分组框
        groupBox3.Text = lang.Get(LangKeys.File_SavePath);
        groupBox1.Text = lang.Get(LangKeys.File_StorageSettings);
        groupBox2.Text = lang.Get(LangKeys.File_DiskAlarmSettings);
        
        // 存储设置
        chk_SaveRawImage.Text = lang.Get(LangKeys.File_SaveRawImage);
        chk_SaveDealImage.Text = lang.Get(LangKeys.File_SaveResultImage);
        chk_Delete.Text = lang.Get(LangKeys.File_DeleteImages);
        chk_SaveOKNG.Text = lang.Get(LangKeys.File_SeparateOKNG);
        label12.Text = lang.Get(LangKeys.File_RawImageRetention);
        label5.Text = lang.Get(LangKeys.File_ResultImageRetention);
        label3.Text = lang.Get(LangKeys.File_RawImageType);
        label23.Text = lang.Get(LangKeys.File_ResultImageType);
        label2.Text = $"{lang.Get(LangKeys.Common_Unit)}/({lang.Get(LangKeys.Common_Day)})";
        label4.Text = $"{lang.Get(LangKeys.Common_Unit)}/({lang.Get(LangKeys.Common_Day)})";
        
        // 磁盘报警设置
        label6.Text = lang.Get(LangKeys.File_DiskMonitorEnabled);
        rtn_true.Text = lang.Get(LangKeys.Common_Yes);
        rtn_false.Text = lang.Get(LangKeys.Common_No);
        label8.Text = lang.Get(LangKeys.File_AlarmThreshold);
        label7.Text = $"{lang.Get(LangKeys.Common_Unit)}/(M)";
        label10.Text = lang.Get(LangKeys.File_CheckTime1);
        label11.Text = lang.Get(LangKeys.File_CheckTime2);
        label9.Text = lang.Get(LangKeys.File_ThresholdNote);
        
        // 按钮
        btn_Select.Text = lang.Get(LangKeys.Common_Select);
        btn_SaveConfig.Text = lang.Get(LangKeys.Common_Save);
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
        nud_Threshold.Value = Math.Max(nud_Threshold.Minimum, Math.Min(nud_Threshold.Maximum, s.DiskThresholdMB));
        dtp_PollTime1.Value = DateTime.Today.Add(s.PollTime1);
        dtp_PollTime2.Value = DateTime.Today.Add(s.PollTime2);
    }

    private void Btn_Select_Click(object sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog();
        dlg.Description = LanguageService.Instance.Get(LangKeys.Common_Select);
        dlg.SelectedPath = string.IsNullOrWhiteSpace(txt_Path.Text) ? FileSettingsManager.Current.SavePath : txt_Path.Text;
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            txt_Path.Text = dlg.SelectedPath;
        }
    }

    private void Btn_SaveConfig_Click(object sender, EventArgs e)
    {
        var lang = LanguageService.Instance;
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
        MessageBox.Show(lang.Get(LangKeys.Msg_SaveSuccess), lang.Get(LangKeys.Common_Info), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
