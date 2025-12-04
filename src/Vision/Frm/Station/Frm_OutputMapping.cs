using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Vision.Solutions.Models;
using Logger;
using HardwareCommNet;
using Cognex.VisionPro.ToolBlock;

namespace Vision.Frm.Station;

/// <summary>
/// 输出映射配置窗体
/// 
/// 功能：
/// - 左侧树显示三个工具（棋盘格、九点、检测）
/// - 右上方显示选中工具的所有输出端子
/// - 右下方显示已配置的输出映射
/// - 双击输出端子可添加映射
/// </summary>
public partial class Frm_OutputMapping : Form
{
    private readonly StationConfig _station;
    private readonly BindingList<MappingRow> _mappings = new();
    
    // 当前选中的工具类型
    private string _selectedToolType;
    private StationConfig.Tools _selectedTool;

    /// <summary>
    /// 映射行数据
    /// </summary>
    private class MappingRow
    {
        public string 工具 { get; set; }
        public string 工具输出 { get; set; }
        public string 输出类型 { get; set; }
        public string 通讯输出 { get; set; }
        public bool 每次发送 { get; set; } = true;
        public string 备注 { get; set; }
    }

    public Frm_OutputMapping(StationConfig station)
    {
        InitializeComponent();
        _station = station ?? throw new ArgumentNullException(nameof(station));
        
        InitializeTree();
        InitializeOutputsGrid();
        InitializeMappingsGrid();
        LoadMappings();
        
        // 延迟初始化 ComboBox（等待 DataGridView 完成绑定）
        this.Load += (s, e) =>
        {
            InitializeComboBoxCells();
        };
        
        WireEvents();
    }

    /// <summary>
    /// 初始化工具树
    /// </summary>
    private void InitializeTree()
    {
        treeView_Tools.Nodes.Clear();
        
        // 检测工具节点
        var detectNode = new TreeNode("检测工具") { Tag = ("Detection", _station.DetectionTool) };
        treeView_Tools.Nodes.Add(detectNode);
        
        // 棋盘格标定工具节点
        var checkerboardNode = new TreeNode("棋盘格标定工具") { Tag = ("Checkerboard", _station.CheckerboardTool) };
        treeView_Tools.Nodes.Add(checkerboardNode);
        
        // 九点标定工具节点
        var npointNode = new TreeNode("九点标定工具") { Tag = ("NPoint", _station.NPointTool) };
        treeView_Tools.Nodes.Add(npointNode);
        
        treeView_Tools.ExpandAll();
        if (treeView_Tools.Nodes.Count > 0)
        {
            treeView_Tools.SelectedNode = treeView_Tools.Nodes[0];
        }
    }

    /// <summary>
    /// 初始化输出列表（只读，显示工具输出端子）
    /// </summary>
    private void InitializeOutputsGrid()
    {
        dgv_Outputs.Columns.Clear();
        dgv_Outputs.AutoGenerateColumns = false;
        
        dgv_Outputs.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "类型",
            HeaderText = "类型",
            Width = 120,
            ReadOnly = true
        });
        
        dgv_Outputs.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "名称",
            HeaderText = "名称",
            Width = 150,
            ReadOnly = true
        });
        
        dgv_Outputs.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "值",
            HeaderText = "当前值",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            ReadOnly = true
        });
    }

    /// <summary>
    /// 初始化映射列表
    /// </summary>
    private void InitializeMappingsGrid()
    {
        dgv_Mappings.Columns.Clear();
        dgv_Mappings.AutoGenerateColumns = false;
        
        // 工具列
        dgv_Mappings.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MappingRow.工具),
            HeaderText = "工具",
            Width = 100,
            ReadOnly = true
        });
        
        // 工具输出列
        dgv_Mappings.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MappingRow.工具输出),
            HeaderText = "工具输出",
            Width = 130,
            ReadOnly = true
        });
        
        // 输出类型列
        dgv_Mappings.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MappingRow.输出类型),
            HeaderText = "类型",
            Width = 80,
            ReadOnly = true
        });
        
        // 通讯输出列（下拉选择，动态根据行类型筛选）
        var commOutputCol = new DataGridViewComboBoxColumn
        {
            Name = "通讯输出",
            DataPropertyName = nameof(MappingRow.通讯输出),
            HeaderText = "通讯输出",
            Width = 160,
            DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        };
        dgv_Mappings.Columns.Add(commOutputCol);
        
        // 每次发送列（复选框）
        dgv_Mappings.Columns.Add(new DataGridViewCheckBoxColumn
        {
            Name = "每次发送",
            DataPropertyName = nameof(MappingRow.每次发送),
            HeaderText = "每次发送",
            Width = 70,
            ToolTipText = "勾选：每次拍照都发送\n不勾选：仅最后一次拍照时发送"
        });
        
        // 备注列
        dgv_Mappings.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MappingRow.备注),
            HeaderText = "备注",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });
        
        dgv_Mappings.DataSource = _mappings;
        
        // 监听单元格编辑开始事件，动态填充下拉选项
        dgv_Mappings.CellBeginEdit += DgvMappings_CellBeginEdit;
        dgv_Mappings.DataError += DgvMappings_DataError;
    }

    /// <summary>
    /// 处理数据绑定错误（避免ComboBox值不在列表中的错误）
    /// </summary>
    private void DgvMappings_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
        // 忽略数据绑定错误（通讯输出可能暂时不在列表中）
        e.Cancel = true;
    }

    /// <summary>
    /// 单元格开始编辑时动态填充通讯输出下拉选项（根据行类型筛选）
    /// </summary>
    private void DgvMappings_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
        if (dgv_Mappings.Columns[e.ColumnIndex].Name != "通讯输出")
            return;
        
        var row = _mappings[e.RowIndex];
        var outputType = row.输出类型;
        
        var cell = dgv_Mappings.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
        if (cell == null) return;
        
        cell.Items.Clear();
        cell.Items.Add(""); // 空选项
        
        if (!string.IsNullOrWhiteSpace(_station.CommDeviceName))
        {
            var device = CommunicationFactory.Instance.GetDevice(_station.CommDeviceName);
            if (device?.Table?.Outputs != null)
            {
                foreach (var output in device.Table.Outputs)
                {
                    // 类型匹配检查
                    if (IsTypeCompatible(outputType, output.ValueType.ToString()))
                    {
                        cell.Items.Add(output.Name);
                    }
                }
            }
        }
        
        if (cell.Items.Count <= 1)
        {
            cell.Items.Add($"<无匹配类型({outputType})的输出>");
        }
        
        // 确保当前值在列表中
        if (!string.IsNullOrEmpty(row.通讯输出) && !cell.Items.Contains(row.通讯输出))
        {
            cell.Items.Add(row.通讯输出);
        }
    }

    /// <summary>
    /// 判断工具输出类型与通讯输出类型是否兼容
    /// 支持数组类型匹配（如 Double[] -> FloatArray）
    /// </summary>
    private bool IsTypeCompatible(string toolOutputType, string commOutputType)
    {
        if (string.IsNullOrWhiteSpace(toolOutputType) || string.IsNullOrWhiteSpace(commOutputType))
            return true; // 类型未知时允许选择

        // 检查是否都是数组类型
        bool toolIsArray = toolOutputType.Contains("[]") || toolOutputType.EndsWith("Array", StringComparison.OrdinalIgnoreCase);
        bool commIsArray = commOutputType.EndsWith("Array", StringComparison.OrdinalIgnoreCase);
        
        // 数组和非数组不能混合匹配
        if (toolIsArray != commIsArray)
            return false;

        // 标准化类型名称（会移除数组标记）
        var normalizedToolType = NormalizeTypeName(toolOutputType);
        var normalizedCommType = NormalizeTypeName(commOutputType);

        // 数组类型匹配：基本类型必须兼容
        if (toolIsArray && commIsArray)
        {
            // double[] 可以发送到 FloatArray（会自动转换为float[]）
            if (normalizedToolType == "double" && normalizedCommType == "float")
                return true;
            if (normalizedToolType == "double" && normalizedCommType == "double")
                return true;
            if (normalizedToolType == "float" && normalizedCommType == "float")
                return true;
            // int[] -> IntArray, short[] -> ShortArray 等
            return string.Equals(normalizedToolType, normalizedCommType, StringComparison.OrdinalIgnoreCase);
        }

        // 非数组类型的原有匹配逻辑
        // double 只能和 double 匹配
        if (normalizedToolType == "double" && normalizedCommType == "double")
            return true;
        if (normalizedToolType == "double" || normalizedCommType == "double")
            return false;

        // float 只能和 float 匹配
        if (normalizedToolType == "float" && normalizedCommType == "float")
            return true;
        if (normalizedToolType == "float" || normalizedCommType == "float")
            return false;

        // 其它数值类型只能精确匹配
        if (string.Equals(normalizedToolType, normalizedCommType, StringComparison.OrdinalIgnoreCase))
            return true;

        // Boolean 类型
        var boolTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "boolean", "bool" };
        if (boolTypes.Contains(normalizedToolType) && boolTypes.Contains(normalizedCommType))
            return true;

        // 字符串类型
        var stringTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "string" };
        if (stringTypes.Contains(normalizedToolType) && stringTypes.Contains(normalizedCommType))
            return true;

        return false;
    }

    /// <summary>
    /// 标准化类型名称
    /// </summary>
    private string NormalizeTypeName(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return typeName;
        
        // 移除数组后缀进行基本类型比较
        typeName = typeName.Replace("[]", "").Replace("Array", "").Trim();
        
        return typeName.ToLowerInvariant() switch
        {
            "int16" or "short" => "short",
            "int32" or "int" => "int",
            "int64" or "long" => "long",
            "uint16" or "ushort" => "ushort",
            "uint32" or "uint" => "uint",
            "uint64" or "ulong" => "ulong",
            "single" or "float" => "float",
            "double" => "double",
            "decimal" => "decimal",
            "boolean" or "bool" => "bool",
            "string" => "string",
            "byte" => "byte",
            "sbyte" => "sbyte",
            _ => typeName.ToLowerInvariant()
        };
    }

    /// <summary>
    /// 加载已有映射
    /// </summary>
    private void LoadMappings()
    {
        _mappings.Clear();
        
        if (_station.OutputMappings != null)
        {
            foreach (var mapping in _station.OutputMappings)
            {
                // 获取输出类型
                var outputType = GetToolOutputType(mapping.ToolType, mapping.ToolOutputName);
                
                _mappings.Add(new MappingRow
                {
                    工具 = mapping.ToolType ?? "检测工具",
                    工具输出 = mapping.ToolOutputName,
                    输出类型 = outputType,
                    通讯输出 = mapping.CommOutputName,
                    每次发送 = mapping.SendEveryTime,
                    备注 = mapping.Description
                });
            }
        }
    }

    /// <summary>
    /// 为已有映射的ComboBox单元格预填充下拉项（确保已选值能正确显示）
    /// </summary>
    private void InitializeComboBoxCells()
    {
        var commOutputColIndex = dgv_Mappings.Columns["通讯输出"]?.Index ?? -1;
        if (commOutputColIndex < 0) return;
        
        // 获取所有可用的通讯输出
        var allOutputs = new List<HardwareCommNet.CommTable.CommCell>();
        if (!string.IsNullOrWhiteSpace(_station.CommDeviceName))
        {
            var device = CommunicationFactory.Instance.GetDevice(_station.CommDeviceName);
            if (device?.Table?.Outputs != null)
            {
                allOutputs = device.Table.Outputs.ToList();
            }
        }
        
        for (int i = 0; i < dgv_Mappings.Rows.Count && i < _mappings.Count; i++)
        {
            var cell = dgv_Mappings.Rows[i].Cells[commOutputColIndex] as DataGridViewComboBoxCell;
            if (cell == null) continue;
            
            var row = _mappings[i];
            var outputType = row.输出类型;
            
            cell.Items.Clear();
            cell.Items.Add(""); // 空选项
            
            // 填充匹配类型的通讯输出
            foreach (var output in allOutputs)
            {
                if (IsTypeCompatible(outputType, output.ValueType.ToString()))
                {
                    cell.Items.Add(output.Name);
                }
            }
            
            // 确保当前值在列表中（即使类型不匹配也要显示）
            if (!string.IsNullOrEmpty(row.通讯输出) && !cell.Items.Contains(row.通讯输出))
            {
                cell.Items.Add(row.通讯输出);
            }
            
            // 如果没有可选项，添加提示
            if (cell.Items.Count <= 1)
            {
                cell.Items.Add($"<无匹配类型({outputType})的输出>");
            }
            
            // 设置当前值
            if (!string.IsNullOrEmpty(row.通讯输出) && cell.Items.Contains(row.通讯输出))
            {
                cell.Value = row.通讯输出;
            }
        }
        
        // 刷新显示
        dgv_Mappings.Refresh();
    }

    /// <summary>
    /// 获取指定工具的输出端子类型
    /// </summary>
    private string GetToolOutputType(string toolType, string outputName)
    {
        CogToolBlock toolBlock = null;
        
        switch (toolType)
        {
            case "检测工具":
            case "Detection":
                toolBlock = _station.DetectionTool?.ToolBlock;
                break;
            case "棋盘格标定工具":
            case "Checkerboard":
                toolBlock = _station.CheckerboardTool?.ToolBlock;
                break;
            case "九点标定工具":
            case "NPoint":
                toolBlock = _station.NPointTool?.ToolBlock;
                break;
        }
        
        if (toolBlock == null || string.IsNullOrWhiteSpace(outputName))
            return "Object";
        
        try
        {
            if (toolBlock.Outputs.Contains(outputName))
            {
                var term = toolBlock.Outputs[outputName];
                Type t = null;
                try { t = term.Value?.GetType(); } catch { }
                if (t == null)
                {
                    try { t = term.GetType().GetProperty("ValueType")?.GetValue(term) as Type; } catch { }
                    if (t == null) { try { t = term.GetType().GetProperty("Type")?.GetValue(term) as Type; } catch { } }
                }
                return t?.Name ?? "Object";
            }
        }
        catch { }
        
        return "Object";
    }

    /// <summary>
    /// 保存映射
    /// </summary>
    private void SaveMappings()
    {
        _station.OutputMappings ??= new List<StationConfig.OutputMapping>();
        _station.OutputMappings.Clear();
        
        foreach (var row in _mappings)
        {
            if (string.IsNullOrWhiteSpace(row.工具输出) || 
                string.IsNullOrWhiteSpace(row.通讯输出) ||
                row.通讯输出.StartsWith("<"))
            {
                continue;
            }
            
            _station.OutputMappings.Add(new StationConfig.OutputMapping
            {
                ToolType = row.工具,
                ToolOutputName = row.工具输出,
                CommOutputName = row.通讯输出,
                SendEveryTime = row.每次发送,
                Description = row.备注
            });
        }
    }

    /// <summary>
    /// 绑定事件
    /// </summary>
    private void WireEvents()
    {
        // 树节点选择变化
        treeView_Tools.AfterSelect += TreeView_AfterSelect;
        
        // 双击输出添加映射
        dgv_Outputs.CellDoubleClick += DgvOutputs_CellDoubleClick;
        
        // 按钮事件
        btn_Add.Click += Btn_Add_Click;
        btn_Remove.Click += Btn_Remove_Click;
        btn_Save.Click += Btn_Save_Click;
        btn_Cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
    }

    /// <summary>
    /// 树节点选择变化 - 显示对应工具的输出
    /// </summary>
    private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
        dgv_Outputs.Rows.Clear();
        
        if (e.Node?.Tag is not ValueTuple<string, StationConfig.Tools> tag)
            return;
        
        _selectedToolType = tag.Item1;
        _selectedTool = tag.Item2;
        
        var toolBlock = _selectedTool?.ToolBlock;
        if (toolBlock == null)
        {
            dgv_Outputs.Rows.Add("", "<工具未配置>", "");
            return;
        }
        
        // 显示工具的所有输出端子
        for (int i = 0; i < toolBlock.Outputs.Count; i++)
        {
            var term = toolBlock.Outputs[i];
            
            // 获取类型
            Type t = null;
            try { t = term.Value?.GetType(); } catch { }
            if (t == null)
            {
                try { t = term.GetType().GetProperty("ValueType")?.GetValue(term) as Type; } catch { }
                if (t == null) { try { t = term.GetType().GetProperty("Type")?.GetValue(term) as Type; } catch { } }
            }
            if (t == null) t = typeof(object);
            
            // 获取值
            var valStr = "<null>";
            try
            {
                var v = term.Value;
                valStr = v == null ? "<null>" : v.ToString();
            }
            catch { }
            
            dgv_Outputs.Rows.Add(t.Name, term.Name, valStr);
        }
        
        // 更新标签
        lbl_Outputs.Text = $"{e.Node.Text} 输出 (双击添加映射)";
    }

    /// <summary>
    /// 双击输出添加映射
    /// </summary>
    private void DgvOutputs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        
        var outputName = dgv_Outputs.Rows[e.RowIndex].Cells["名称"].Value?.ToString();
        var outputType = dgv_Outputs.Rows[e.RowIndex].Cells["类型"].Value?.ToString();
        if (string.IsNullOrWhiteSpace(outputName) || outputName.StartsWith("<"))
            return;
        
        AddMapping(outputName, outputType);
    }

    /// <summary>
    /// 添加映射按钮
    /// </summary>
    private void Btn_Add_Click(object sender, EventArgs e)
    {
        if (dgv_Outputs.CurrentRow == null)
        {
            MessageBox.Show("请先在上方选择一个工具输出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        var outputName = dgv_Outputs.CurrentRow.Cells["名称"].Value?.ToString();
        var outputType = dgv_Outputs.CurrentRow.Cells["类型"].Value?.ToString();
        if (string.IsNullOrWhiteSpace(outputName) || outputName.StartsWith("<"))
        {
            MessageBox.Show("请选择有效的工具输出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        AddMapping(outputName, outputType);
    }

    /// <summary>
    /// 添加映射
    /// </summary>
    private void AddMapping(string outputName, string outputType = null)
    {
        // 获取工具显示名称
        var toolDisplayName = _selectedToolType switch
        {
            "Detection" => "检测工具",
            "Checkerboard" => "棋盘格标定工具",
            "NPoint" => "九点标定工具",
            _ => "未知工具"
        };
        
        // 如果没有传入类型，从当前选中行获取
        if (string.IsNullOrWhiteSpace(outputType) && dgv_Outputs.CurrentRow != null)
        {
            outputType = dgv_Outputs.CurrentRow.Cells["类型"].Value?.ToString();
        }
        outputType ??= "Object";
        
        // 检查是否已存在相同映射
        var exists = _mappings.Any(m => 
            m.工具 == toolDisplayName && 
            m.工具输出 == outputName);
        
        if (exists)
        {
            MessageBox.Show($"映射 [{toolDisplayName}].{outputName} 已存在", "提示", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        _mappings.Add(new MappingRow
        {
            工具 = toolDisplayName,
            工具输出 = outputName,
            输出类型 = outputType,
            通讯输出 = "",
            备注 = ""
        });
        
        // 为新添加的行初始化 ComboBox
        InitializeComboBoxCellForRow(_mappings.Count - 1, outputType);
    }

    /// <summary>
    /// 为指定行初始化 ComboBox 单元格
    /// </summary>
    private void InitializeComboBoxCellForRow(int rowIndex, string outputType)
    {
        var commOutputColIndex = dgv_Mappings.Columns["通讯输出"]?.Index ?? -1;
        if (commOutputColIndex < 0 || rowIndex < 0 || rowIndex >= dgv_Mappings.Rows.Count) return;
        
        var cell = dgv_Mappings.Rows[rowIndex].Cells[commOutputColIndex] as DataGridViewComboBoxCell;
        if (cell == null) return;
        
        cell.Items.Clear();
        cell.Items.Add(""); // 空选项
        
        // 填充匹配类型的通讯输出
        if (!string.IsNullOrWhiteSpace(_station.CommDeviceName))
        {
            var device = CommunicationFactory.Instance.GetDevice(_station.CommDeviceName);
            if (device?.Table?.Outputs != null)
            {
                foreach (var output in device.Table.Outputs)
                {
                    if (IsTypeCompatible(outputType, output.ValueType.ToString()))
                    {
                        cell.Items.Add(output.Name);
                    }
                }
            }
        }
        
        // 如果没有可选项，添加提示
        if (cell.Items.Count <= 1)
        {
            cell.Items.Add($"<无匹配类型({outputType})的输出>");
        }
    }

    /// <summary>
    /// 删除映射
    /// </summary>
    private void Btn_Remove_Click(object sender, EventArgs e)
    {
        if (dgv_Mappings.CurrentRow == null) return;
        
        int index = dgv_Mappings.CurrentRow.Index;
        if (index >= 0 && index < _mappings.Count)
        {
            _mappings.RemoveAt(index);
        }
    }

    /// <summary>
    /// 保存
    /// </summary>
    private void Btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            // 验证映射
            for (int i = 0; i < _mappings.Count; i++)
            {
                var row = _mappings[i];
                
                if (string.IsNullOrWhiteSpace(row.通讯输出) || row.通讯输出.StartsWith("<"))
                {
                    MessageBox.Show($"第{i + 1}行：请选择通讯输出变量", "验证失败", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            
            SaveMappings();
            LogHelper.Info($"[输出映射] 工位[{_station.Name}]已保存{_station.OutputMappings.Count}项输出映射");
            
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "[输出映射] 保存映射配置失败");
            MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
