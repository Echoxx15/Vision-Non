using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Vision.Solutions.IOTable;
using Vision.Solutions.Models;

namespace Vision.Frm.Link
{
 public class Frm_IOTableEditor : Form
 {
 private readonly ProcessStation _station;
 private IOTableConfig _config;

 private readonly BindingList<IOVariableConfig> _vars;
 private readonly BindingList<TriggerConfig> _trigs;

 private readonly TabControl _tabs = new TabControl();
 private readonly DataGridView _dgvVars = new DataGridView();
 private readonly DataGridView _dgvTrigs = new DataGridView();
 private readonly Button _btnOk = new Button();
 private readonly Button _btnCancel = new Button();
 private readonly ToolStrip _tsVar = new ToolStrip();
 private readonly ToolStrip _tsTrig = new ToolStrip();

 public Frm_IOTableEditor(ProcessStation station)
 {
 _station = station ?? throw new ArgumentNullException(nameof(station));
 _config = station.IOTableConfig ?? new IOTableConfig { Name = station.Name + "_IOTable" };

 // 深拷贝一份到编辑集合，避免实时更改
 _vars = new BindingList<IOVariableConfig>((_config.Variables ?? new List<IOVariableConfig>()).Select(CloneVar).ToList());
 _trigs = new BindingList<TriggerConfig>((_config.Triggers ?? new List<TriggerConfig>()).Select(CloneTrig).ToList());

 Text = "通讯表编辑器";
 StartPosition = FormStartPosition.CenterParent;
 MinimumSize = new Size(900,560);
 Width =1000;
 Height =640;

 BuildUi();
 }

 private static IOVariableConfig CloneVar(IOVariableConfig v) => new IOVariableConfig
 {
 Name = v?.Name,
 Description = v?.Description,
 DataType = v?.DataType ?? IODataType.Int,
 AccessType = v?.AccessType ?? IOAccessType.Input,
 DeviceName = v?.DeviceName,
 Address = v?.Address,
 DefaultValue = v?.DefaultValue,
 ToolBlockOutputName = v?.ToolBlockOutputName
 };

 private static TriggerConfig CloneTrig(TriggerConfig t) => new TriggerConfig
 {
 Name = t?.Name,
 VariableName = t?.VariableName,
 ConditionType = t?.ConditionType ?? TriggerConditionType.Equal,
 TriggerValue = t?.TriggerValue,
 IsEnabled = t?.IsEnabled ?? true,
 DebounceDelayMs = t?.DebounceDelayMs ??50,
 ValueType = t?.ValueType ?? TriggerValueType.Auto
 };

 private void BuildUi()
 {
 // Tabs
 _tabs.Dock = DockStyle.Fill;
 var pageVars = new TabPage("变量");
 var pageTrig = new TabPage("触发器");
 _tabs.TabPages.Add(pageVars);
 _tabs.TabPages.Add(pageTrig);

 // Vars toolbar
 var btnAddVar = new ToolStripButton("新增") { DisplayStyle = ToolStripItemDisplayStyle.Text };
 var btnDelVar = new ToolStripButton("删除") { DisplayStyle = ToolStripItemDisplayStyle.Text };
 var btnSave = new ToolStripButton("保存") { DisplayStyle = ToolStripItemDisplayStyle.Text, ToolTipText = "立即保存配置（不关闭窗口）" };
 var btnImportCsv = new ToolStripButton("导入CSV") { DisplayStyle = ToolStripItemDisplayStyle.Text };
 var btnExportCsv = new ToolStripButton("导出CSV") { DisplayStyle = ToolStripItemDisplayStyle.Text };
 var btnRefreshOutputs = new ToolStripButton("刷新输出") { DisplayStyle = ToolStripItemDisplayStyle.Text, ToolTipText = "从检测工具刷新可用的输出端子列表" };
 btnAddVar.Click += (_, __) => AddVar();
 btnDelVar.Click += (_, __) => RemoveCurrentVar();
 btnSave.Click += (_, __) => SaveWithoutClosing();
 btnImportCsv.Click += (_, __) => ImportVariablesFromCsv();
 btnExportCsv.Click += (_, __) => ExportVariablesToCsv();
 btnRefreshOutputs.Click += (_, __) => RefreshOutputsList();
 _tsVar.Items.AddRange(new ToolStripItem[] { btnAddVar, btnDelVar, btnSave, new ToolStripSeparator(), btnImportCsv, btnExportCsv, new ToolStripSeparator(), btnRefreshOutputs });
 _tsVar.Dock = DockStyle.Top;

 // Vars grid
 _dgvVars.Dock = DockStyle.Fill;
 _dgvVars.AutoGenerateColumns = false;
 _dgvVars.AllowUserToAddRows = false;
 _dgvVars.AllowUserToDeleteRows = false;
 _dgvVars.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
 _dgvVars.RowHeadersVisible = false;
 _dgvVars.DataError += (_, e) => { e.ThrowException = false; };

 _dgvVars.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(IOVariableConfig.Name), HeaderText = "名称", Width =140 });
 _dgvVars.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(IOVariableConfig.Description), HeaderText = "描述", Width =160 });

 var colType = new DataGridViewComboBoxColumn { DataPropertyName = nameof(IOVariableConfig.DataType), HeaderText = "数据类型", Width =90, DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton };
 colType.DataSource = Enum.GetValues(typeof(IODataType));
 _dgvVars.Columns.Add(colType);

 var colAcc = new DataGridViewComboBoxColumn { DataPropertyName = nameof(IOVariableConfig.AccessType), HeaderText = "访问", Width =70, DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton };
 colAcc.DataSource = Enum.GetValues(typeof(IOAccessType));
 _dgvVars.Columns.Add(colAcc);

 _dgvVars.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(IOVariableConfig.DeviceName), HeaderText = "设备名", Width =120 });
 _dgvVars.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(IOVariableConfig.Address), HeaderText = "地址", Width =100 });
 
 // 关联输出：改为下拉列表，从ToolBlock.Outputs动态读取
 var colOutput = new DataGridViewComboBoxColumn 
 { 
 DataPropertyName = nameof(IOVariableConfig.ToolBlockOutputName), 
 HeaderText = "关联输出", 
 Width =120,
 DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
 };
 _dgvVars.Columns.Add(colOutput);
 
 _dgvVars.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(IOVariableConfig.DefaultValue), HeaderText = "默认值", Width =100 });

 _dgvVars.DataSource = _vars;

 // 动态刷新输出端子列表
 _dgvVars.CellEnter += DgvVars_CellEnter;

 var pnlVars = new Panel { Dock = DockStyle.Fill };
 pnlVars.Controls.Add(_dgvVars);
 pnlVars.Controls.Add(_tsVar);
 pageVars.Controls.Add(pnlVars);

 // Triggers toolbar
 var btnAddTrig = new ToolStripButton("新增") { DisplayStyle = ToolStripItemDisplayStyle.Text };
 var btnDelTrig = new ToolStripButton("删除") { DisplayStyle = ToolStripItemDisplayStyle.Text };
 btnAddTrig.Click += (_, __) => AddTrig();
 btnDelTrig.Click += (_, __) => RemoveCurrentTrig();
 _tsTrig.Items.AddRange(new ToolStripItem[] { btnAddTrig, btnDelTrig });
 _tsTrig.Dock = DockStyle.Top;

 // Triggers grid
 _dgvTrigs.Dock = DockStyle.Fill;
 _dgvTrigs.AutoGenerateColumns = false;
 _dgvTrigs.AllowUserToAddRows = false;
 _dgvTrigs.AllowUserToDeleteRows = false;
 _dgvTrigs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
 _dgvTrigs.RowHeadersVisible = false;
 _dgvTrigs.DataError += (_, e) => { e.ThrowException = false; };

 _dgvTrigs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TriggerConfig.Name), HeaderText = "名称", Width =160 });

 var colVarName = new DataGridViewComboBoxColumn { DataPropertyName = nameof(TriggerConfig.VariableName), HeaderText = "变量名", Width =160, DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton };
 colVarName.FlatStyle = FlatStyle.Standard;
 colVarName.DataSource = _vars;
 colVarName.ValueMember = nameof(IOVariableConfig.Name);
 colVarName.DisplayMember = nameof(IOVariableConfig.Name);
 _dgvTrigs.Columns.Add(colVarName);

 var colCond = new DataGridViewComboBoxColumn { DataPropertyName = nameof(TriggerConfig.ConditionType), HeaderText = "条件", Width =120, DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton };
 colCond.DataSource = Enum.GetValues(typeof(TriggerConditionType));
 _dgvTrigs.Columns.Add(colCond);

 var colVt = new DataGridViewComboBoxColumn { DataPropertyName = nameof(TriggerConfig.ValueType), HeaderText = "值类型", Width =100, DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton };
 colVt.DataSource = Enum.GetValues(typeof(TriggerValueType));
 _dgvTrigs.Columns.Add(colVt);

 _dgvTrigs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TriggerConfig.TriggerValue), HeaderText = "触发值", Width =140 });
 _dgvTrigs.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(TriggerConfig.IsEnabled), HeaderText = "启用", Width =60 });
 _dgvTrigs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(TriggerConfig.DebounceDelayMs), HeaderText = "去抖(ms)", Width =90 });

 _dgvTrigs.DataSource = _trigs;

 var pnlTrigs = new Panel { Dock = DockStyle.Fill };
 pnlTrigs.Controls.Add(_dgvTrigs);
 pnlTrigs.Controls.Add(_tsTrig);
 pageTrig.Controls.Add(pnlTrigs);

 // Buttons
 _btnOk.Text = "确定";
 _btnCancel.Text = "取消";
 _btnOk.DialogResult = DialogResult.OK;
 _btnCancel.DialogResult = DialogResult.Cancel;
 _btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
 _btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
 _btnOk.Width =90; _btnCancel.Width =90;
 _btnOk.Height =28; _btnCancel.Height =28;
 _btnCancel.Left = ClientSize.Width -100;
 _btnOk.Left = _btnCancel.Left -100;
 _btnOk.Top = ClientSize.Height -50;
 _btnCancel.Top = ClientSize.Height -50;
 _btnOk.Click += (_, __) => CommitAndClose();
 _btnCancel.Click += (_, __) => { DialogResult = DialogResult.Cancel; Close(); };

 Controls.Add(_tabs);
 Controls.Add(_btnOk);
 Controls.Add(_btnCancel);
 Resize += (_, __) => LayoutButtons();
 }

 private void LayoutButtons()
 {
 _btnCancel.Left = ClientSize.Width -100 -10;
 _btnOk.Left = _btnCancel.Left - _btnOk.Width -10;
 _btnOk.Top = ClientSize.Height - _btnOk.Height -10;
 _btnCancel.Top = ClientSize.Height - _btnCancel.Height -10;
 }

 private void AddVar()
 {
 var v = new IOVariableConfig
 {
 Name = MakeUniqueVarName("Var"),
 Description = string.Empty,
 DataType = IODataType.Int,
 AccessType = IOAccessType.Input,
 DeviceName = string.IsNullOrWhiteSpace(_station.CommDeviceName) ? string.Empty : _station.CommDeviceName,
 Address = string.Empty,
 DefaultValue = string.Empty
 };
 _vars.Add(v);
 }

 private void RemoveCurrentVar()
 {
 if (_dgvVars.CurrentRow == null) return;
 var row = _dgvVars.CurrentRow.Index;
 if (row >=0 && row < _vars.Count) _vars.RemoveAt(row);
 }

 private string MakeUniqueVarName(string baseName)
 {
 int i =0;
 while (true)
 {
 var name = i ==0 ? baseName : baseName + i;
 if (!_vars.Any(v => string.Equals(v.Name, name, StringComparison.OrdinalIgnoreCase))) return name;
 i++;
 }
 }

 private void AddTrig()
 {
 var t = new TriggerConfig
 {
 Name = MakeUniqueTrigName("Trig"),
 VariableName = _vars.FirstOrDefault()?.Name,
 ConditionType = TriggerConditionType.Equal,
 ValueType = TriggerValueType.Auto,
 TriggerValue = "1",
 IsEnabled = true,
 DebounceDelayMs =50
 };
 _trigs.Add(t);
 }

 private void RemoveCurrentTrig()
 {
 if (_dgvTrigs.CurrentRow == null) return;
 var row = _dgvTrigs.CurrentRow.Index;
 if (row >=0 && row < _trigs.Count) _trigs.RemoveAt(row);
 }

 private string MakeUniqueTrigName(string baseName)
 {
 int i =0;
 while (true)
 {
 var name = i ==0 ? baseName : baseName + i;
 if (!_trigs.Any(v => string.Equals(v.Name, name, StringComparison.OrdinalIgnoreCase))) return name;
 i++;
 }
 }

 private void CommitAndClose()
 {
 // 写回站位的 IOTableConfig
 _config ??= new IOTableConfig();
 _config.Variables = _vars.ToList();
 _config.Triggers = _trigs.ToList();
 if (string.IsNullOrWhiteSpace(_config.Name)) _config.Name = _station.Name + "_IOTable";

 _station.IOTableConfig = _config;

 bool deviceSaved = false;
 bool solutionSaved = false;

 // 若绑定了设备名，保存到设备级文件
 try
 {
 if (!string.IsNullOrWhiteSpace(_station.CommDeviceName) && _config != null)
 {
 DeviceIOTableStore.Save(_station.CommDeviceName, _config);
 deviceSaved = true;
 }
 }
 catch (Exception ex)
 {
 MessageBox.Show($"保存设备配置文件失败: {ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
 }

 // 保存当前方案到磁盘
 try
 {
 SolutionManager.Instance.SaveCurrent();
 solutionSaved = true;
 }
 catch (Exception ex)
 {
 MessageBox.Show($"保存方案文件失败: {ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
 }

 // 显示保存结果
 if (deviceSaved || solutionSaved)
 {
 var msg = "通讯表配置已保存：\n";
 if (deviceSaved) msg += $"? 设备文件: {_station.CommDeviceName}.iot.xml\n";
 if (solutionSaved) msg += "? 方案文件已更新\n";
 MessageBox.Show(msg, "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
 }

 DialogResult = DialogResult.OK;
 Close();
 }
 
 private void ImportVariablesFromCsv()
 {
 using var ofd = new OpenFileDialog
 {
 Filter = "CSV文件|*.csv|所有文件|*.*",
 Title = "导入变量配置"
 };
 
 if (ofd.ShowDialog(this) != DialogResult.OK) return;

 try
 {
 var lines = System.IO.File.ReadAllLines(ofd.FileName, System.Text.Encoding.UTF8);
 if (lines.Length < 2)
 {
 MessageBox.Show("CSV文件格式不正确", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
 return;
 }

 // 跳过表头
 var imported = 0;
 for (int i = 1; i < lines.Length; i++)
 {
 var line = lines[i].Trim();
 if (string.IsNullOrWhiteSpace(line)) continue;

 var parts = line.Split(',');
 if (parts.Length < 7) continue;

 try
 {
 var varCfg = new IOVariableConfig
 {
 Name = parts[0].Trim(),
 Description = parts[1].Trim(),
 DataType = Enum.TryParse<IODataType>(parts[2].Trim(), out var dt) ? dt : IODataType.Int,
 AccessType = Enum.TryParse<IOAccessType>(parts[3].Trim(), out var at) ? at : IOAccessType.Input,
 DeviceName = parts[4].Trim(),
 Address = parts[5].Trim(),
 ToolBlockOutputName = parts[6].Trim(),
 DefaultValue = parts.Length > 7 ? parts[7].Trim() : string.Empty
 };

 // 检查是否已存在同名变量
 if (_vars.Any(v => string.Equals(v.Name, varCfg.Name, StringComparison.OrdinalIgnoreCase)))
 {
 continue; // 跳过重复
 }

 _vars.Add(varCfg);
 imported++;
 }
 catch
 {
 // 跳过解析失败的行
 }
 }

 MessageBox.Show($"成功导入 {imported} 个变量", "导入完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
 }
 catch (Exception ex)
 {
 MessageBox.Show($"导入失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
 }
 }

 private void ExportVariablesToCsv()
 {
 if (_vars.Count == 0)
 {
 MessageBox.Show("没有可导出的变量", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
 return;
 }

 using var sfd = new SaveFileDialog
 {
 Filter = "CSV文件|*.csv",
 Title = "导出变量配置",
 FileName = $"IOTable_{_station.Name}_{DateTime.Now:yyyyMMddHHmmss}.csv"
 };

 if (sfd.ShowDialog(this) != DialogResult.OK) return;

 try
 {
 var lines = new List<string>
 {
 "名称,描述,数据类型,访问类型,设备名,地址,关联输出,默认值"
 };

 foreach (var v in _vars)
 {
 var line = $"{v.Name},{v.Description},{v.DataType},{v.AccessType},{v.DeviceName},{v.Address},{v.ToolBlockOutputName},{v.DefaultValue}";
 lines.Add(line);
 }

 System.IO.File.WriteAllLines(sfd.FileName, lines, System.Text.Encoding.UTF8);
 MessageBox.Show($"成功导出 {_vars.Count} 个变量", "导出完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
 }
 catch (Exception ex)
 {
 MessageBox.Show($"导出失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
 }
 }
 
 private void DgvVars_CellEnter(object sender, DataGridViewCellEventArgs e)
 {
 if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

 // 检查是否为"关联输出"列
 var col = _dgvVars.Columns[e.ColumnIndex];
 if (col.DataPropertyName != nameof(IOVariableConfig.ToolBlockOutputName)) return;

 // 获取当前行的访问类型
 if (_dgvVars.Rows[e.RowIndex].DataBoundItem is not IOVariableConfig varCfg) return;

 // 只有Output类型才需要关联输出
 if (varCfg.AccessType != IOAccessType.Output && varCfg.AccessType != IOAccessType.InputOutput)
 {
 return;
 }

 // 从ToolBlock获取输出端子列表
 var outputNames = GetToolBlockOutputNames();
 
 if (col is DataGridViewComboBoxColumn comboCol)
 {
 comboCol.DataSource = outputNames;
 }
 }

 private List<string> GetToolBlockOutputNames()
 {
 var list = new List<string> { string.Empty }; // 空选项

 try
 {
 var toolBlock = _station?.DetectionTool?.ToolBlock;
 if (toolBlock?.Outputs != null)
 {
 for (int i = 0; i < toolBlock.Outputs.Count; i++)
 {
 var terminal = toolBlock.Outputs[i];
 if (!string.IsNullOrWhiteSpace(terminal?.Name))
 {
 list.Add(terminal.Name);
 }
 }
 }
 }
 catch
 {
 // 获取失败时返回空列表
 }

 return list;
 }

 private void RefreshOutputsList()
 {
 var outputNames = GetToolBlockOutputNames();
 
 // 更新"关联输出"列的数据源
 foreach (DataGridViewColumn col in _dgvVars.Columns)
 {
 if (col.DataPropertyName == nameof(IOVariableConfig.ToolBlockOutputName) && col is DataGridViewComboBoxColumn comboCol)
 {
 comboCol.DataSource = null; // 先清空
 comboCol.DataSource = outputNames;
 break;
 }
 }

 // 刷新显示
 _dgvVars.Refresh();
 
 MessageBox.Show($"已刷新输出端子列表，共 {outputNames.Count - 1} 个可用端子", "刷新完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
 }

 private void SaveWithoutClosing()
 {
 // 写回工位的 IOTableConfig
 _config ??= new IOTableConfig();
 _config.Variables = _vars.ToList();
 _config.Triggers = _trigs.ToList();
 if (string.IsNullOrWhiteSpace(_config.Name)) _config.Name = _station.Name + "_IOTable";

 _station.IOTableConfig = _config;

 bool deviceSaved = false;
 bool solutionSaved = false;
 string errorMsg = string.Empty;

 // 保存到设备文件
 try
 {
 if (!string.IsNullOrWhiteSpace(_station.CommDeviceName) && _config != null)
 {
 DeviceIOTableStore.Save(_station.CommDeviceName, _config);
 deviceSaved = true;
 }
 }
 catch (Exception ex)
 {
 errorMsg += $"设备文件保存失败: {ex.Message}\n";
 }

 // 保存到方案文件
 try
 {
 SolutionManager.Instance.SaveCurrent();
 solutionSaved = true;
 }
 catch (Exception ex)
 {
 errorMsg += $"方案文件保存失败: {ex.Message}\n";
 }

 // 显示结果
 if (!string.IsNullOrEmpty(errorMsg))
 {
 MessageBox.Show(errorMsg, "保存警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
 }
 else if (deviceSaved || solutionSaved)
 {
 var msg = $"? 通讯表配置已保存\n\n变量: {_vars.Count} 个\n触发器: {_trigs.Count} 个\n";
 if (deviceSaved) msg += $"\n已保存到设备文件: {_station.CommDeviceName}.iot.xml";
 if (solutionSaved) msg += "\n已保存到方案文件";
 MessageBox.Show(msg, "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
 }
 }
 }
}
