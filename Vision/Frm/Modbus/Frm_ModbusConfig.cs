using HslCommunication.Core;
using Logger;
using System;
using System.Drawing;
using System.Windows.Forms;
using Vision.Comm.Modbus;
using Vision.Solutions.Models;

namespace Vision.Frm.Modbus;

/// <summary>
/// Modbus配置界面（集成测试功能）
/// 左侧：连接配置 + 数据读写测试
/// 右侧：输入/输出变量管理
/// </summary>
public partial class Frm_ModbusConfig : Form
{
    private ModbusConfig _config;
    private ModbusAccessor _accessor;

    public Frm_ModbusConfig()
    {
        InitializeComponent();
        InitializeForm();
        BindEvents();
    }

    private void InitializeForm()
    {
        LoadConfigFromSolution();
        LoadDataToUI();
    }

    #region 数据加载

    private void LoadConfigFromSolution()
    {
        var solution = SolutionManager.Instance.Current;

        // 单设备模式：获取或创建默认配置
        _config = ModbusManager.Instance.GetConfig();
        if (_config == null)
        {
            _config = new ModbusConfig
            {
                Name = "ModbusClient",
                Enabled = true,
                IpAddress = "127.0.0.1",
                Port = 502,
                Station = 1,
                ConnectTimeout = 5000,
                ReceiveTimeout = 10000,
                DataFormat = "CDAB",
                StringReverse = false
            };
            solution.ModbusConfig = _config;
        }
        else
        {
            _accessor = ModbusManager.Instance.GetAccessor();
        }
    }

    private void LoadDataToUI()
    {
        // 加载连接配置
        txtIpAddress.Text = _config.IpAddress;
        numPort.Value = _config.Port;
        numStation.Value = _config.Station;
        numConnectTimeout.Value = _config.ConnectTimeout;
        numReceiveTimeout.Value = _config.ReceiveTimeout;
        cmbDataFormat.SelectedItem = _config.DataFormat;

        if (cmbDataFormat.SelectedIndex == -1)
            cmbDataFormat.SelectedIndex = 1; // 默认CDAB

        SetControlState();
    }

    private void SetControlState()
    {
        btnConnect.Text = _accessor.IsConnected ? "断开连接" : "连接";
        //txtIpAddress.Enabled = !_accessor.IsConnected;
        //numPort.Enabled = !_accessor.IsConnected;
    }

    #endregion


    #region 事件绑定

    private void BindEvents()
    {
        // 连接按钮
        btnConnect.Click += OnConnect;

        // 读取测试按钮
        btnReadBool.Click += (_, _) => OnRead(ModbusDataType.Bool);
        btnReadShort.Click += (_, _) => OnRead(ModbusDataType.Short);
        btnReadFloat.Click += (_, _) => OnRead(ModbusDataType.Float);
        btnReadString.Click += (_, _) => OnRead(ModbusDataType.String);

        // 写入测试按钮
        btnWriteBool.Click += (_, _) => OnWrite(ModbusDataType.Bool);
        btnWriteShort.Click += (_, _) => OnWrite(ModbusDataType.Short);
        btnWriteFloat.Click += (_, _) => OnWrite(ModbusDataType.Float);
        btnWriteString.Click += (_, _) => OnWrite(ModbusDataType.String);

        // 保存按钮
        btnSave.Click += OnSave;

    }

    #endregion

    #region 连接管理

    private void OnConnect(object sender, EventArgs e)
    {
        //if (_accessor.IsConnected)
        //{
        //    // 断开连接
        //    Disconnect();
        //}
        //else
        //{
        //    // 建立连接
        //    Connect();
        //}
        // 建立连接
        Connect();
        var a= _accessor.IsConnected;
    }

    private void Connect()
    {
        try
        {
            Cursor = Cursors.WaitCursor;
            ModbusManager.Instance.SetClientConfig(txtIpAddress.Text, (int)numPort.Value);

            _accessor = ModbusManager.Instance.GetAccessor();
            var result = _accessor.Connect();
            
            if (result.IsSuccess)
            {
                btnConnect.BackColor = Color.LimeGreen;
                btnConnect.Text = "断开连接";
                AppendLog("✓ 连接成功", Color.Green);
                LogHelper.Info($"Modbus连接成功: {txtIpAddress.Text}:{numPort.Value}");

                // 启用测试按钮
                EnableTestButtons(true);
            }
            else
            {
                btnConnect.BackColor = Color.LightCoral;
                btnConnect.Text = "连接";
                AppendLog($"✗ 连接失败: {result.Message}", Color.Red);
                LogHelper.Warn($"Modbus连接失败: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            btnConnect.BackColor = Color.LightCoral;
            btnConnect.Text = "连接";
            AppendLog($"✗ 连接异常: {ex.Message}", Color.Red);
            LogHelper.Error(ex, "Modbus连接异常");
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void Disconnect()
    {
        try
        {
            _accessor?.Close();
            btnConnect.BackColor = SystemColors.Control;
            btnConnect.Text = "连接";
            AppendLog("已断开连接", Color.Blue);
            LogHelper.Info("Modbus已断开连接");

            // 禁用测试按钮
            EnableTestButtons(false);
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "断开Modbus连接异常");
        }
    }

    private void EnableTestButtons(bool enabled)
    {
        btnReadBool.Enabled = enabled;
        btnReadShort.Enabled = enabled;
        btnReadFloat.Enabled = enabled;
        btnReadString.Enabled = enabled;
        btnWriteBool.Enabled = enabled;
        btnWriteShort.Enabled = enabled;
        btnWriteFloat.Enabled = enabled;
        btnWriteString.Enabled = enabled;
    }

    #endregion

    #region 数据读写测试

    private void OnRead(ModbusDataType type)
    {
        if (!CheckConnection()) return;

        string address = txtReadAddress.Text.Trim();
        if (string.IsNullOrEmpty(address))
        {
            AppendLog("✗ 请输入读取地址", Color.Red);
            return;
        }

        int length = (int)numReadLength.Value;

        try
        {
            Cursor = Cursors.WaitCursor;

            object value = type switch
            {
                ModbusDataType.Bool => _accessor.ReadBool(address, 1)[0],
                ModbusDataType.Short => _accessor.ReadShort(address),
                ModbusDataType.Float => _accessor.ReadFloat(address),
                ModbusDataType.String => _accessor.ReadString(address, (ushort)length),
                _ => null
            };

            if (value != null)
            {
                txtReadResult.Text = value.ToString();
                AppendLog($"✓ 读取成功: {address} = {value}", Color.Green);
            }
        }
        catch (Exception ex)
        {
            AppendLog($"✗ 读取失败: {ex.Message}", Color.Red);
            LogHelper.Error(ex, "Modbus读取失败");
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void OnWrite(ModbusDataType type)
    {
        if (!CheckConnection()) return;

        string address = txtWriteAddress.Text.Trim();
        string valueStr = txtWriteValue.Text.Trim();

        if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(valueStr))
        {
            AppendLog("✗ 请输入地址和值", Color.Red);
            return;
        }

        try
        {
            Cursor = Cursors.WaitCursor;

            bool success = type switch
            {
                ModbusDataType.Bool => _accessor.WriteBool(address, bool.Parse(valueStr)),
                ModbusDataType.Short => _accessor.WriteShort(address, short.Parse(valueStr)),
                ModbusDataType.Float => _accessor.WriteFloat(address, float.Parse(valueStr)),
                ModbusDataType.String => _accessor.WriteString(address, valueStr),
                _ => false
            };

            if (success)
            {
                AppendLog($"✓ 写入成功: {address} = {valueStr}", Color.Green);
            }
            else
            {
                AppendLog($"✗ 写入失败: {address}", Color.Red);
            }
        }
        catch (Exception ex)
        {
            AppendLog($"✗ 写入异常: {ex.Message}", Color.Red);
            LogHelper.Error(ex, "Modbus写入失败");
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private bool CheckConnection()
    {
        if ( _accessor.IsConnected)
            return true;

        AppendLog("✗ 请先连接Modbus", Color.Red);
        return false;
    }

    #endregion


    #region 保存配置

    private void OnSave(object sender, EventArgs e)
    {
        try
        {
            // 保存连接配置
            _config.IpAddress = txtIpAddress.Text.Trim();
            _config.Port = (int)numPort.Value;
            _config.Station = (byte)numStation.Value;
            _config.ConnectTimeout = (int)numConnectTimeout.Value;
            _config.ReceiveTimeout = (int)numReceiveTimeout.Value;
            _config.DataFormat = cmbDataFormat.SelectedItem?.ToString() ?? "CDAB";

            // 保存到方案
            SolutionManager.Instance.SaveCurrent();

            MessageBox.Show("Modbus配置已保存", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LogHelper.Info("Modbus配置已保存");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LogHelper.Error(ex, "保存Modbus配置失败");
        }
    }

    #endregion

    #region 日志

    private void AppendLog(string message, Color? color = null)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        int startIndex = txtLog.TextLength;
        txtLog.AppendText($"[{timestamp}] {message}\r\n");

        if (color.HasValue)
        {
            txtLog.Select(startIndex, txtLog.TextLength - startIndex);
            txtLog.SelectionColor = color.Value;
        }

        txtLog.ScrollToCaret();
    }

    #endregion
}
