using System;
using System.IO;
using System.Threading.Tasks;
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
        cmbDeviceType.SelectedIndexChanged += CmbDeviceType_SelectedIndexChanged;
        cmbRuntime.SelectedIndexChanged += CmbRuntime_SelectedIndexChanged;
        btnExportOptimized.Click += BtnExportOptimized_Click;
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
            
            // 加载优化设置
            cmbOptimizePrecision.SelectedIndex = config.OptimizePrecision switch
            {
                DnnOptimizePrecision.FP16 => 1,
                DnnOptimizePrecision.INT8 => 2,
                _ => 0
            };
            nudOptimizeBatchSize.Value = config.OptimizeBatchSize;
        }
        else
        {
            cmbDeviceType.SelectedIndex = 0;
            cmbRuntime.SelectedIndex = 0;
        }
        
        // 加载可用优化设备
        LoadOptimizeDevices();
    }

    /// <summary>
    /// 加载可用的优化设备列表
    /// </summary>
    private void LoadOptimizeDevices()
    {
        cmbOptimizeDevice.Items.Clear();
        
        if (_model is IOptimizableDnnModel optimizable)
        {
            var devices = optimizable.GetAvailableOptimizeDevices();
            foreach (var device in devices)
            {
                cmbOptimizeDevice.Items.Add(device);
            }
            
            if (cmbOptimizeDevice.Items.Count > 0)
                cmbOptimizeDevice.SelectedIndex = 0;
        }
    }

    private void ChkLoadOnStartup_CheckedChanged(object sender, EventArgs e)
    {
        UpdateModelConfig();
    }

    private void CmbDeviceType_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateModelConfig();
    }

    private void CmbRuntime_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateModelConfig();
    }

    /// <summary>
    /// 更新模型配置（保存到模型实例）
    /// </summary>
    private void UpdateModelConfig()
    {
        if (_model is IConfigurableDnnModel configurable)
        {
            var config = configurable.GetConfig();
            config.DeviceType = cmbDeviceType.SelectedIndex == 1 ? DnnDeviceType.CPU : DnnDeviceType.GPU;
            config.Runtime = cmbRuntime.SelectedIndex switch
            {
                1 => DnnRuntime.OpenVINO,
                2 => DnnRuntime.TensorRT,
                _ => DnnRuntime.GC
            };
            config.LoadOnStartup = chkLoadOnStartup.Checked;
            config.ModelPath = txtModelPath.Text;
            configurable.ApplyConfig(config);
        }
    }

    private void BtnBrowse_Click(object sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog
        {
            Description = "选择语义分割模型文件夹（包含 .hdl 模型文件）"
        };

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            txtModelPath.Text = dlg.SelectedPath;
            UpdateModelConfig();
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
                
                // 加载成功后刷新优化设备列表
                LoadOptimizeDevices();
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

    /// <summary>
    /// 导出优化模型按钮点击事件
    /// </summary>
    private async void BtnExportOptimized_Click(object sender, EventArgs e)
    {
        if (!_model.IsLoaded)
        {
            MessageBox.Show("请先加载模型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbOptimizeDevice.SelectedItem == null)
        {
            MessageBox.Show("请选择优化设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!(_model is IOptimizableDnnModel optimizable))
        {
            MessageBox.Show("当前模型不支持优化导出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var deviceName = cmbOptimizeDevice.SelectedItem.ToString();
        var precision = cmbOptimizePrecision.SelectedIndex switch
        {
            1 => DnnOptimizePrecision.FP16,
            2 => DnnOptimizePrecision.INT8,
            _ => DnnOptimizePrecision.FP32
        };
        var batchSize = (int)nudOptimizeBatchSize.Value;

        // 禁用界面
        groupBoxOptimize.Enabled = false;
        lblOptimizeStatus.Text = "正在优化...";
        lblOptimizeStatus.ForeColor = System.Drawing.Color.Blue;
        progressBarOptimize.Value = 0;
        progressBarOptimize.Visible = true;

        try
        {
            bool success = await Task.Run(() =>
            {
                return optimizable.OptimizeAndExport(deviceName, precision, batchSize, (progress, message) =>
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        progressBarOptimize.Value = Math.Min(progress, 100);
                        lblOptimizeStatus.Text = message;
                    }));
                });
            });

            if (success)
            {
                lblOptimizeStatus.Text = "优化完成";
                lblOptimizeStatus.ForeColor = System.Drawing.Color.Green;
                MessageBox.Show("模型优化导出成功！\n优化后的模型已保存到原模型文件夹。", "成功", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblOptimizeStatus.Text = "优化失败";
                lblOptimizeStatus.ForeColor = System.Drawing.Color.Red;
                MessageBox.Show("模型优化导出失败，请检查设备和参数设置。", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            lblOptimizeStatus.Text = "优化失败";
            lblOptimizeStatus.ForeColor = System.Drawing.Color.Red;
            MessageBox.Show($"优化过程出错：{ex.Message}", "错误", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            groupBoxOptimize.Enabled = true;
            progressBarOptimize.Visible = false;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        lblStatusValue.Text = _model.IsLoaded ? "已加载" : "未加载";
        lblStatusValue.ForeColor = _model.IsLoaded ? System.Drawing.Color.Green : System.Drawing.Color.Gray;
        btnLoad.Enabled = !_model.IsLoaded;
        btnUnload.Enabled = _model.IsLoaded;
        
        // 优化导出控件状态
        groupBoxOptimize.Enabled = _model.IsLoaded;
        btnExportOptimized.Enabled = _model.IsLoaded && cmbOptimizeDevice.Items.Count > 0;
        
        // 检查优化模型状态
        if (_model is IOptimizableDnnModel optimizable && _model.IsLoaded)
        {
            bool hasTensorRT = optimizable.HasOptimizedModel(DnnRuntime.TensorRT);
            bool hasOpenVINO = optimizable.HasOptimizedModel(DnnRuntime.OpenVINO);
            
            if (hasTensorRT || hasOpenVINO)
            {
                var optimized = new System.Collections.Generic.List<string>();
                if (hasTensorRT) optimized.Add("TensorRT");
                if (hasOpenVINO) optimized.Add("OpenVINO");
                lblOptimizeStatus.Text = $"已优化: {string.Join(", ", optimized)}";
                lblOptimizeStatus.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblOptimizeStatus.Text = "未优化";
                lblOptimizeStatus.ForeColor = System.Drawing.Color.Gray;
            }
        }
        else
        {
            lblOptimizeStatus.Text = "-";
            lblOptimizeStatus.ForeColor = System.Drawing.Color.Gray;
        }
    }
}
