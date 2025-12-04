using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HardwareCameraNet;

namespace Vision.Frm.MainForm;

public partial class Frm_Camera2D : Form
{
    // 维护当前选中的相机实例，用于切换时取消订阅以防重复订阅
    private ICamera currentSelectedCamera;
    
    // 标记是否正在处理行选择，避免重复触发
    private bool _isProcessingRowSelection;


    public Frm_Camera2D()
    {
        InitializeComponent();
        // 表单显示后再初始化显示控件，避免首开阻塞
        //Shown += Frm_Camera2D_Shown;
    }
    private void Frm_Camera2D_Load(object sender, EventArgs e)
    {
        var mans = CameraFactory.Instance.GetAllManufacturers().Cast<object>().ToArray();
        cmb_Manufacturers.Items.AddRange(mans);

        DgvUpdate();

        SetControlState();
        
        // 订阅DGV行选择事件（点击行时选中对应相机）
        dgv_CameraConfig.SelectionChanged += dgv_CameraConfig_SelectionChanged;
        // 设置DGV为单选模式，防止多选
        dgv_CameraConfig.MultiSelect = false;
    }



    private void DgvUpdate()
    {
        dgv_CameraConfig.AutoGenerateColumns = false;
        // 重新绑定数据源，避免对绑定到非 IBindingList 的 DataGridView 直接删行
        dgv_CameraConfig.DataSource = null;
        dgv_CameraConfig.DataSource = CameraFactory.Instance.GetAllConfigs();
    }
    private void dgv_CameraConfig_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
        // 只处理备注列
        if (dgv_CameraConfig.Columns[e.ColumnIndex].Name != "col_Expain")
            return;

        var sn = dgv_CameraConfig.Rows[e.RowIndex].Cells["col_SerialNumber"].Value?.ToString();
        var expain = dgv_CameraConfig.Rows[e.RowIndex].Cells["col_Expain"].Value?.ToString();

        if (string.IsNullOrEmpty(sn)) return;
        var configs = CameraFactory.Instance.GetAllConfigs();
        var config = configs.FirstOrDefault(cfg => cfg.SerialNumber == sn);
        if (config == null) return;
        config.Expain = expain ?? "";
        CameraFactory.Instance.AddOrUpdateConfig(config);

        // 刷新界面
        DgvUpdate();
    }

    private void SetControlState(bool connect = false)
    {
        SetControlText();

        // 参数控件需要相机已连接
        txt_Exposure.Enabled = connect;
        txt_Gain.Enabled = connect;
        chk_HardTrigger.Enabled = connect;
        
        // 采集控件需要相机已连接
        btn_TriggerOnce.Enabled = connect;
        btn_DisConnect.Enabled = connect;
        btn_Continuous.Enabled = connect;
        
        // 添加按钮：只要选择了SN就可以添加（不需要连接）
        btn_Add.Enabled = !string.IsNullOrEmpty(cmb_SnList.Text);

        // 连接按钮：选择了SN且未连接时可用
        btn_Connect.Enabled = !string.IsNullOrEmpty(cmb_SnList.Text) && !connect;
    }
    private void SetControlText()
    {
        try
        {
            if(currentSelectedCamera == null)return;

            if (currentSelectedCamera.IsConnected)
            {
                txt_Exposure.Value = (decimal)currentSelectedCamera.Parameters.ExposureTime;
                txt_Gain.Value = (decimal)currentSelectedCamera.Parameters.Gain;
                txt_MaxExposure.Text = currentSelectedCamera.Parameters.MaxExposureTime.ToString("F5");
                txt_MaxGain.Text = currentSelectedCamera.Parameters.MaxGain.ToString("F5");
                cmb_TriggerSource.Items.Clear();
                var arr = currentSelectedCamera.Parameters.TriggerSoures?.Cast<object>().ToArray() ?? new object[0];
                cmb_TriggerSource.Items.AddRange(arr);
            }
            else
            {
                txt_Exposure.Value = 0;
                txt_Gain.Value = 0;
                txt_MaxExposure.Text = "0";
                txt_MaxGain.Text = "0";
                cmb_TriggerSource.Items.Clear();
            }

        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void cmb_SnList_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            // 防止下拉框初始化时触发（无实际选择）
            var snText = (cmb_SnList.SelectedItem as string) ?? cmb_SnList.Text;
            if (string.IsNullOrEmpty(snText))
                return;

            btn_Connect.Enabled = true;
            // 取消当前相机的事件订阅（避免切换时残留订阅）
            UnsubscribeCurrentCameraEvent();

            // 获取厂商名称和选中的序列号
            var selectedManufacturer = cmb_Manufacturers.Text;
            var selectedSerial = snText.Trim();

            // 通过CameraManager获取相机实例（不再枚举，直接从缓存获取或创建）
            var newCamera = CameraFactory.Instance.GetOrCreateCamera(selectedManufacturer, selectedSerial);
            if (newCamera == null)
            {
                MessageBox.Show($"获取相机{selectedSerial}失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentSelectedCamera = null;
                SetControlState(connect: false); // 更新UI为未连接状态
                return;
            }

            // 订阅新相机的事件
            SubscribeNewCameraEvent(newCamera);

            //更新当前相机状态，并同步UI
            currentSelectedCamera = newCamera;
            SetControlState(currentSelectedCamera.IsConnected);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    /// <summary>
    /// DGV行选择变化事件 - 点击行时选中对应相机
    /// </summary>
    private void dgv_CameraConfig_SelectionChanged(object sender, EventArgs e)
    {
        if (_isProcessingRowSelection) return;
        if (dgv_CameraConfig.SelectedRows.Count == 0) return;
        
        try
        {
            _isProcessingRowSelection = true;
            
            var row = dgv_CameraConfig.SelectedRows[0];
            var sn = row.Cells["col_SerialNumber"].Value?.ToString();
            var manufacturer = row.Cells["col_Manufacturer"].Value?.ToString();
            
            if (string.IsNullOrEmpty(sn) || string.IsNullOrEmpty(manufacturer))
                return;
            
            // 如果已经是当前相机，不重复处理
            if (currentSelectedCamera != null && currentSelectedCamera.SN == sn)
                return;
            
            // 设置厂商下拉框（不触发枚举）
            _isProcessingRowSelection = true;
            var manufacturerIndex = cmb_Manufacturers.Items.IndexOf(manufacturer);
            if (manufacturerIndex >= 0 && cmb_Manufacturers.SelectedIndex != manufacturerIndex)
            {
                // 临时取消事件，设置厂商后恢复
                cmb_Manufacturers.SelectedIndexChanged -= cmb_Manufacturers_SelectedIndexChanged;
                cmb_Manufacturers.SelectedIndex = manufacturerIndex;
                cmb_Manufacturers.SelectedIndexChanged += cmb_Manufacturers_SelectedIndexChanged;
                
                // 手动填充SN列表（不触发枚举）
                cmb_SnList.Items.Clear();
                var list = CameraFactory.Instance.EnumerateDevices(manufacturer) ?? new System.Collections.Generic.List<string>();
                cmb_SnList.Items.AddRange(list.Cast<object>().ToArray());
            }
            
            // 设置SN下拉框
            var snIndex = cmb_SnList.Items.IndexOf(sn);
            if (snIndex >= 0)
            {
                cmb_SnList.SelectedIndex = snIndex;
            }
            else
            {
                // SN不在列表中（可能是离线的相机），直接设置文本并选中
                cmb_SnList.Text = sn;
                SelectCameraDirectly(manufacturer, sn);
            }
        }
        finally
        {
            _isProcessingRowSelection = false;
        }
    }

    /// <summary>
    /// 直接选中相机（不通过下拉框事件）
    /// </summary>
    private void SelectCameraDirectly(string manufacturer, string sn)
    {
        try
        {
            UnsubscribeCurrentCameraEvent();
            
            var newCamera = CameraFactory.Instance.GetOrCreateCamera(manufacturer, sn);
            if (newCamera == null)
            {
                currentSelectedCamera = null;
                SetControlState(connect: false);
                return;
            }
            
            SubscribeNewCameraEvent(newCamera);
            currentSelectedCamera = newCamera;
            SetControlState(currentSelectedCamera.IsConnected);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void cmb_Manufacturers_SelectedIndexChanged(object sender, EventArgs e)
    {
        cmb_SnList.Text = "";
        cmb_SnList.Items.Clear();
        var list = CameraFactory.Instance.EnumerateDevices(cmb_Manufacturers.Text) ?? new System.Collections.Generic.List<string>();
        cmb_SnList.Items.AddRange(list.Cast<object>().ToArray());
    }
    /// <summary>
    /// 订阅新相机的FrameGrabedEvent（确保只订阅一次）
    /// </summary>
    private void SubscribeNewCameraEvent(ICamera newCamera)
    {
        if (newCamera == null) return;

        //订阅前先取消一次（防止相机实例被其他地方订阅过）
        newCamera.FrameGrabedEvent -= UpdateUIImage;
        // 正式订阅
        newCamera.FrameGrabedEvent += UpdateUIImage;
        Console.WriteLine($"订阅相机{newCamera.SN}的图像事件");
    }
    /// <summary>
    /// 取消当前相机的FrameGrabedEvent订阅
    /// </summary>
    private void UnsubscribeCurrentCameraEvent()
    {
        if (currentSelectedCamera != null)
        {
            // 取消订阅
            currentSelectedCamera.FrameGrabedEvent -= UpdateUIImage;
            Console.WriteLine($"取消订阅相机{currentSelectedCamera.SN}的图像事件");
        }
    }

    private void UpdateUIImage(object sender, object img)
    {
        Control dispatcher = pictureEdit_Display != null ? (Control)pictureEdit_Display : this;
        if (dispatcher.InvokeRequired)
        {
            dispatcher.BeginInvoke(new Action<object, object>(UpdateUIImage), sender, img);
            return;
        }
        try
        {
            if (img is Bitmap bmp)
            {
                pictureEdit_Display.Image = bmp;
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void DisConnectEvent(object sender, bool disconnect)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<object, bool>(DisConnectEvent), sender, disconnect);
            return;
        }
        if (cmb_SnList.Text == ((ICamera)sender).SN)
        {
            SetControlState(!disconnect);
        }
    }

    private void txt_Exposure_ValueChanged(object sender, EventArgs e)
    {
        // 检查相机是否存在且已连接
        if (currentSelectedCamera == null || !currentSelectedCamera.IsConnected)
            return;
        
        try
        {
            var val = (double)txt_Exposure.Value;
            var max = currentSelectedCamera.Parameters.MaxExposureTime;
            if (val > max)
            {
                val = max;
                txt_Exposure.Value = (decimal)val;
            }
            currentSelectedCamera.Parameters.ExposureTime = val;
        }
        catch (Exception exception)
        {
            MessageBox.Show("曝光设置失败" + exception, "", MessageBoxButtons.OK);
        }
    }
     
    private void txt_Gain_ValueChanged(object sender, EventArgs e)
    {
        // 检查相机是否存在且已连接
        if (currentSelectedCamera == null || !currentSelectedCamera.IsConnected)
            return;
        
        try
        {
            double val = (double)txt_Gain.Value;
            var max = currentSelectedCamera.Parameters.MaxGain;
            if (val > max)
            {
                val = max;
                txt_Gain.Value = (decimal)val;
            }
            
            currentSelectedCamera.Parameters.Gain = val;
        }
        catch (Exception exception)
        {
            MessageBox.Show("增益设置失败" + exception, "", MessageBoxButtons.OK);
        }
    }
    private void cmb_TriggerSource_SelectedIndexChanged(object sender, EventArgs e)
    {
        // 检查相机是否存在且已连接
        if (currentSelectedCamera == null || !currentSelectedCamera.IsConnected)
            return;
        
        currentSelectedCamera.Parameters.TriggerSoure = cmb_TriggerSource.Text;
    }

    private void btn_Add_Click(object sender, EventArgs e)
    {
        try
        {
            var selectedManufacturer = cmb_Manufacturers.Text;
            var selectedSerial = ((cmb_SnList.SelectedItem as string) ?? cmb_SnList.Text)?.Trim();
            if (string.IsNullOrEmpty(selectedManufacturer) || string.IsNullOrEmpty(selectedSerial))
            {
                MessageBox.Show("请选择厂商和序列号");
                return;
            }

            var configs = CameraFactory.Instance.GetAllConfigs();
            var existConfig = configs.FirstOrDefault(cfg => cfg.SerialNumber == selectedSerial);

            string expain;
            if (existConfig == null)
            {
                // 新建时自动生成备注
                var usedIndexes = configs
                    .Select(cfg => cfg.Expain)
                    .Where(exp => exp != null && exp.StartsWith("相机"))
                    .Select(exp =>
                    {
                        if (int.TryParse(exp.Substring(2), out int idx))
                            return idx;
                        return -1;
                    })
                    .Where(idx => idx >= 0)
                    .ToHashSet();

                int nextIndex = 0;
                while (usedIndexes.Contains(nextIndex))
                    nextIndex++;
                expain = $"相机{nextIndex}";
            }
            else
            {
                // 已存在则保留原备注
                expain = existConfig.Expain;
            }

            // 获取插件信息（通过厂商获取，不需要相机已连接）
            var pluginInfo = CameraPluginServer.Instance.GetPluginInfoByManufacturer(selectedManufacturer);
            if (pluginInfo == null)
            {
                MessageBox.Show($"未找到厂商{selectedManufacturer}对应的插件");
                return;
            }
            
            var cfg = new CameraConfig(selectedSerial, selectedManufacturer, pluginInfo)
            {
                Expain = expain
            };
            CameraFactory.Instance.AddOrUpdateConfig(cfg);
            DgvUpdate();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            MessageBox.Show($"添加配置失败：{exception.Message}");
        }
    }

    private void btn_Remove_Click(object sender, EventArgs e)
    {
        // 判断是否有选中行
        if (dgv_CameraConfig.SelectedRows.Count == 0)
        {
            //MessageBox.Show("请先选择要移除的相机配置行！");
            return;
        }

        // 获取选中行
        var row = dgv_CameraConfig.SelectedRows[0];
        var sn = row.Cells["col_SerialNumber"].Value?.ToString();

        if (string.IsNullOrEmpty(sn))
        {
            MessageBox.Show("选中的行序列号无效！");
            return;
        }
        if(DialogResult.Yes != MessageBox.Show("是否移除相机配置！","",MessageBoxButtons.YesNoCancel))return;
        
        // 如果移除的是当前选中的相机，先清空当前相机
        if (currentSelectedCamera != null && currentSelectedCamera.SN == sn)
        {
            UnsubscribeCurrentCameraEvent();
            currentSelectedCamera = null;
        }
        
        // 先删除管理器中的配置，再整体刷新数据源（不要直接对绑定的 DataGridView 调用 Rows.Remove）
        if (CameraFactory.Instance.RemoveConfig(sn))
        {
            DgvUpdate();
            
            // 刷新整个界面状态
            ResetUIAfterRemove();
        }
        else
        {
            MessageBox.Show("移除失败，未找到该序列号的配置！");
        }
    }

    /// <summary>
    /// 移除配置后重置UI状态
    /// </summary>
    private void ResetUIAfterRemove()
    {
        // 如果还有配置，选中第一个
        if (dgv_CameraConfig.Rows.Count > 0)
        {
            dgv_CameraConfig.Rows[0].Selected = true;
            // SelectionChanged 事件会自动处理选中逻辑
        }
        else
        {
            // 没有配置了，清空当前相机状态，但保留SN下拉框（枚举出来的设备列表）
            currentSelectedCamera = null;
            SetControlState(false);
            
            // 清空显示的图像
            pictureEdit_Display.Image = null;
        }
    }
    private void btn_Connect_Click(object sender, EventArgs e)
    {
        try
        {
            currentSelectedCamera.Open();
            SetControlState(currentSelectedCamera.IsConnected);
            currentSelectedCamera.DisConnetEvent += DisConnectEvent;
            
            // 通知CameraManager更新设备状态（主界面硬件状态会更新）
            if (currentSelectedCamera.IsConnected)
            {
                var config = CameraFactory.Instance.GetAllConfigs()
                    .FirstOrDefault(c => c.SerialNumber == currentSelectedCamera.SN);
                if (config != null)
                {
                    CameraFactory.Instance.NotifyDeviceStateChanged(currentSelectedCamera.SN, config.Expain, true);
                }
            }
        }
        catch (Exception)
        {
            //MessageBox.Show($"打开相机失败,错误码{errCode}," + exception, "", MessageBoxButtons.OK);
        }
    }
    private void btn_DisConnect_Click(object sender, EventArgs e)
    {
        try
        {
            // 先获取配置信息
            var config = CameraFactory.Instance.GetAllConfigs()
                .FirstOrDefault(c => c.SerialNumber == currentSelectedCamera.SN);
            
            currentSelectedCamera.Close();
            SetControlState(currentSelectedCamera.IsConnected);
            currentSelectedCamera.DisConnetEvent -= DisConnectEvent;
            
            // 通知CameraManager更新设备状态（主界面硬件状态会更新）
            if (config != null)
            {
                CameraFactory.Instance.NotifyDeviceStateChanged(currentSelectedCamera.SN, config.Expain, false);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show($"关闭相机失败," + exception, "", MessageBoxButtons.OK);
        }
    }
    private void btn_TriggerOnce_Click(object sender, EventArgs e)
    {
        currentSelectedCamera.SoftwareTriggerOnce();
    }
    private void btn_Continuous_Click(object sender, EventArgs e)
    {
        if (btn_Continuous.Text == "连续采集")
        {
            currentSelectedCamera.ContinuousGrab();
            btn_Continuous.Text = "停止采集";
            btn_DisConnect.Enabled = false;
        }
        else
        {
            currentSelectedCamera.StopContinuousGrab();
            btn_Continuous.Text = "连续采集";
            btn_DisConnect.Enabled = true;
        }
    }
    private void chk_HardTrigger_CheckedChanged(object sender, EventArgs e)
    {
        cmb_TriggerSource.Visible = chk_HardTrigger.Checked;
        if (chk_HardTrigger.Checked && currentSelectedCamera != null)
        {
            // 选中当前触发源字符串
            cmb_TriggerSource.SelectedItem = currentSelectedCamera.Parameters.TriggerSoure;
        }
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 2. 取消相机事件订阅，避免残留引用
            UnsubscribeCurrentCameraEvent();

            // 3. 释放设计器组件（默认逻辑）
            if (components != null)
                components.Dispose();
        }
        base.Dispose(disposing);
    }
}
