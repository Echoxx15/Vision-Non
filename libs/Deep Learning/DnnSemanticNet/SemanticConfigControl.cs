using System;
using System.IO;
using System.Windows.Forms;
using DnnInterfaceNet;

namespace DnnSemanticNet;

/// <summary>
/// 语义分割模型配置控件
/// </summary>
public partial class SemanticConfigControl : UserControl
{
    private readonly DnnSemanticSegmentation _model;

    public SemanticConfigControl(DnnSemanticSegmentation model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        InitializeComponent();
        InitializeEvents();
        LoadModelConfig();
        UpdateUI();
    }

    private void InitializeEvents()
    {
        btnBrowse.Click += BtnBrowse_Click;
        btnLoad.Click += BtnLoad_Click;
        btnUnload.Click += BtnUnload_Click;
        chkLoadOnStartup.CheckedChanged += ChkLoadOnStartup_CheckedChanged;
    }

    private void LoadModelConfig()
    {
        txtName.Text = _model.Name;
        txtModelPath.Text = _model.ModelPath ?? "";

        // 从模型配置加载设置
        if (_model is IConfigurableDnnModel configurable)
        {
            var config = configurable.GetConfig();
            cmbDeviceType.SelectedIndex = config.DeviceType == DnnDeviceType.CPU ? 1 : 0;
            cmbRuntime.SelectedIndex = config.Runtime switch
            {
                DnnRuntime.OpenVINO => 1,
                DnnRuntime.TensorRT => 2,
                _ => 0
            };
            chkLoadOnStartup.Checked = config.LoadOnStartup;
        }
        else
        {
            cmbDeviceType.SelectedIndex = 0;
            cmbRuntime.SelectedIndex = 0;
        }
    }

    private void ChkLoadOnStartup_CheckedChanged(object sender, EventArgs e)
    {
        // 立即更新模型配置
        if (_model is IConfigurableDnnModel configurable)
        {
            var config = configurable.GetConfig();
            config.LoadOnStartup = chkLoadOnStartup.Checked;
            configurable.ApplyConfig(config);
        }
    }

    private void BtnBrowse_Click(object sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog
        {
            Description = "选择模型文件夹（包含 .hdl 和 .hdict 文件）"
        };

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            txtModelPath.Text = dlg.SelectedPath;
        }
    }

    private void BtnLoad_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtModelPath.Text) || !Directory.Exists(txtModelPath.Text))
        {
            MessageBox.Show("请选择有效的模型文件夹", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var deviceType = cmbDeviceType.SelectedIndex == 0 
            ? DnnDeviceType.GPU 
            : DnnDeviceType.CPU;

        var runtime = cmbRuntime.SelectedIndex switch
        {
            1 => DnnRuntime.OpenVINO,
            2 => DnnRuntime.TensorRT,
            _ => DnnRuntime.GC
        };

        this.Cursor = Cursors.WaitCursor;
        try
        {
            if (_model.Load(txtModelPath.Text, deviceType, runtime))
            {
                MessageBox.Show("模型加载成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("模型加载失败，请检查模型文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
            UpdateUI();
        }
    }

    private void BtnUnload_Click(object sender, EventArgs e)
    {
        _model.Unload();
        UpdateUI();
        MessageBox.Show("模型已卸载", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void UpdateUI()
    {
        lblStatusValue.Text = _model.IsLoaded ? "已加载" : "未加载";
        lblStatusValue.ForeColor = _model.IsLoaded ? System.Drawing.Color.Green : System.Drawing.Color.Gray;
        lblClassCountValue.Text = _model.IsLoaded && _model.ClassIDs != null ? _model.ClassIDs.Length.ToString() : "-";
        btnLoad.Enabled = !_model.IsLoaded;
        btnUnload.Enabled = _model.IsLoaded;
    }
}
