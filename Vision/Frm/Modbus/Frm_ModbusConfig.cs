using HslCommunication.Core;
using Logger;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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
    private BindingList<ModbusVariable> _inputVars;
    private BindingList<ModbusVariable> _outputVars;
    private ModbusAccessor _accessor;
    private bool _isConnected = false;

    public Frm_ModbusConfig()
    {
        InitializeComponent();
        InitializeForm();
}

    private void InitializeForm()
    {
        LoadConfigFromSolution();
        SetupDataGridViews();
        BindEvents();
     LoadDataToUI();
    }

    #region 数据加载

    private void LoadConfigFromSolution()
    {
        var solution = SolutionManager.Instance.Current;
    
        if (solution.ModbusConfigs == null)
        solution.ModbusConfigs = new ModbusConfigCollection();

     // 单设备模式：获取或创建默认配置
        if (solution.ModbusConfigs.Configs.Count == 0)
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
  solution.ModbusConfigs.Add(_config);
      }
        else
  {
     _config = solution.ModbusConfigs.Configs[0];
  }

        // 分离输入输出变量
    _inputVars = new BindingList<ModbusVariable>(
   _config.Variables.Where(v => v.Direction == ModbusDirection.Input).OrderBy(v => v.Index).ToList());
        _outputVars = new BindingList<ModbusVariable>(
      _config.Variables.Where(v => v.Direction == ModbusDirection.Output).OrderBy(v => v.Index).ToList());
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

        // 绑定变量数据
 dgvInput.DataSource = _inputVars;
        dgvOutput.DataSource = _outputVars;
    }

    #endregion

    #region DataGridView设置

    private void SetupDataGridViews()
    {
     SetupGrid(dgvInput);
        SetupGrid(dgvOutput);
    }

    private void SetupGrid(DataGridView dgv)
    {
        dgv.AutoGenerateColumns = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect = false;
dgv.AllowUserToAddRows = false;
        dgv.RowHeadersVisible = false;

      dgv.Columns.Clear();

        // 序号列（只读）
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
        Name = "colIndex",
       HeaderText = "序号",
        DataPropertyName = "Index",
            Width = 60,
            ReadOnly = true
  });

        // 类型列（只读）
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
    Name = "colType",
      HeaderText = "类型",
 DataPropertyName = "TypeName",
      Width = 80,
  ReadOnly = true
        });

        // 名称列（只读 - 自动生成Value1, Value2...）
        dgv.Columns.Add(new DataGridViewTextBoxColumn
     {
        Name = "colName",
            HeaderText = "名称",
       DataPropertyName = "Name",
       Width = 120,
        ReadOnly = true
        });

        // 地址列（可编辑）
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colAddress",
            HeaderText = "地址",
            DataPropertyName = "Address",
            Width = 100
      });

        // 注释列（可编辑）
        dgv.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colComment",
     HeaderText = "注释",
        DataPropertyName = "Comment",
   Width = 250
        });
    }

    #endregion

    #region 事件绑定

    private void BindEvents()
    {
      // 连接按钮
 btnConnect.Click += OnConnect;

        // 读取测试按钮
        btnReadBool.Click += (s, e) => OnRead(ModbusDataType.Bool);
        btnReadShort.Click += (s, e) => OnRead(ModbusDataType.Short);
        btnReadFloat.Click += (s, e) => OnRead(ModbusDataType.Float);
        btnReadString.Click += (s, e) => OnRead(ModbusDataType.String);

        // 写入测试按钮
        btnWriteBool.Click += (s, e) => OnWrite(ModbusDataType.Bool);
        btnWriteShort.Click += (s, e) => OnWrite(ModbusDataType.Short);
        btnWriteFloat.Click += (s, e) => OnWrite(ModbusDataType.Float);
        btnWriteString.Click += (s, e) => OnWrite(ModbusDataType.String);

  // 添加输入变量按钮
        btnAddInputShort.Click += (s, e) => AddVariable(ModbusDataType.Short, ModbusDirection.Input);
        btnAddInputFloat.Click += (s, e) => AddVariable(ModbusDataType.Float, ModbusDirection.Input);
        btnAddInputBool.Click += (s, e) => AddVariable(ModbusDataType.Bool, ModbusDirection.Input);
        btnAddInputString.Click += (s, e) => AddVariable(ModbusDataType.String, ModbusDirection.Input);
        btnDeleteInput.Click += (s, e) => DeleteVariable(ModbusDirection.Input);

        // 添加输出变量按钮
        btnAddOutputShort.Click += (s, e) => AddVariable(ModbusDataType.Short, ModbusDirection.Output);
        btnAddOutputFloat.Click += (s, e) => AddVariable(ModbusDataType.Float, ModbusDirection.Output);
   btnAddOutputBool.Click += (s, e) => AddVariable(ModbusDataType.Bool, ModbusDirection.Output);
  btnAddOutputString.Click += (s, e) => AddVariable(ModbusDataType.String, ModbusDirection.Output);
  btnDeleteOutput.Click += (s, e) => DeleteVariable(ModbusDirection.Output);

  // 保存按钮
        btnSave.Click += OnSave;

        // DataGridView编辑完成
        dgvInput.CellEndEdit += (s, e) => SaveGridChanges();
        dgvOutput.CellEndEdit += (s, e) => SaveGridChanges();
    }

    #endregion

    #region 连接管理

    private void OnConnect(object sender, EventArgs e)
    {
        if (_isConnected)
     {
      // 断开连接
     Disconnect();
        }
        else
        {
         // 建立连接
       Connect();
  }
    }

    private void Connect()
    {
      try
      {
 this.Cursor = Cursors.WaitCursor;

       // 解析数据格式
  DataFormat dataFormat = cmbDataFormat.SelectedItem?.ToString() switch
      {
  "ABCD" => DataFormat.ABCD,
     "CDAB" => DataFormat.CDAB,
 "BADC" => DataFormat.BADC,
    "DCBA" => DataFormat.DCBA,
    _ => DataFormat.CDAB
        };

     // 创建访问器
            _accessor = ModbusTcp.Get(
 txtIpAddress.Text.Trim(),
     (int)numPort.Value,
          dataFormat,
(byte)numStation.Value,
       (int)numConnectTimeout.Value,
           false);

 var result = _accessor.Connect();

     if (result.IsSuccess)
  {
   _isConnected = true;
     btnConnect.BackColor = Color.LimeGreen;
    btnConnect.Text = "断开连接";
AppendLog("✓ 连接成功", Color.Green);
      LogHelper.Info($"Modbus连接成功: {txtIpAddress.Text}:{numPort.Value}");

        // 启用测试按钮
            EnableTestButtons(true);
      }
         else
  {
          _isConnected = false;
     btnConnect.BackColor = Color.LightCoral;
  btnConnect.Text = "连接";
  AppendLog($"✗ 连接失败: {result.Message}", Color.Red);
 LogHelper.Warn($"Modbus连接失败: {result.Message}");
            }
        }
        catch (Exception ex)
        {
    _isConnected = false;
    btnConnect.BackColor = Color.LightCoral;
     btnConnect.Text = "连接";
       AppendLog($"✗ 连接异常: {ex.Message}", Color.Red);
            LogHelper.Error(ex, "Modbus连接异常");
     }
        finally
   {
            this.Cursor = Cursors.Default;
   }
 }

    private void Disconnect()
    {
   try
 {
       _accessor?.Close();
  _isConnected = false;
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
    this.Cursor = Cursors.WaitCursor;

         object value = type switch
     {
        ModbusDataType.Bool => _accessor.ReadBool(address, 1)[0],
          ModbusDataType.Short => _accessor.ReadInt16(address),
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
 this.Cursor = Cursors.Default;
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
   this.Cursor = Cursors.WaitCursor;

     bool success = type switch
     {
   ModbusDataType.Bool => _accessor.WriteBool(address, bool.Parse(valueStr)),
    ModbusDataType.Short => _accessor.WriteInt16(address, short.Parse(valueStr)),
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
     this.Cursor = Cursors.Default;
  }
    }

    private bool CheckConnection()
    {
    if (_isConnected && _accessor != null)
 return true;

  AppendLog("✗ 请先连接Modbus", Color.Red);
        return false;
    }

    #endregion

    #region 变量管理

    private void AddVariable(ModbusDataType type, ModbusDirection direction)
    {
  var list = direction == ModbusDirection.Input ? _inputVars : _outputVars;

        // 生成唯一名称 Value1, Value2...
        string name = GenerateUniqueName();
    int index = list.Count + 1;

 var variable = new ModbusVariable
  {
            Index = index,
     Type = type,
            Name = name,
          Direction = direction,
            Address = "D100",
            Length = 1,
            Comment = ""
        };

        list.Add(variable);
        _config.Variables.Add(variable);

   LogHelper.Info($"添加Modbus变量: {name} ({type})");
    }

    private void DeleteVariable(ModbusDirection direction)
    {
        var dgv = direction == ModbusDirection.Input ? dgvInput : dgvOutput;
        var list = direction == ModbusDirection.Input ? _inputVars : _outputVars;

        if (dgv.CurrentRow == null)
        {
       MessageBox.Show("请先选择要删除的变量", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    return;
        }

        var variable = dgv.CurrentRow.DataBoundItem as ModbusVariable;
        if (variable != null)
        {
   list.Remove(variable);
  _config.Variables.Remove(variable);
            ReindexVariables(list);
            LogHelper.Info($"删除Modbus变量: {variable.Name}");
        }
    }

    private void SaveGridChanges()
  {
    // DataGridView的更改会自动反映到BindingList
    }

    private string GenerateUniqueName()
    {
        int index = 1;
        while (_config.Variables.Any(v => v.Name == $"Value{index}"))
     {
          index++;
        }
        return $"Value{index}";
    }

    private void ReindexVariables(BindingList<ModbusVariable> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
          list[i].Index = i + 1;
        }
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

    #region 窗体关闭

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
 // 关闭时断开连接
        if (_isConnected)
{
        Disconnect();
  }

        base.OnFormClosing(e);
    }

    #endregion
}
