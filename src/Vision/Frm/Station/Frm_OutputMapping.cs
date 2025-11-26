using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Vision.Solutions.Models;
using Logger;
using HardwareCommNet;

namespace Vision.Frm.Station;

/// <summary>
/// 工具输出与通讯输出映射配置窗体
/// 
/// 功能：
/// - 配置检测工具的输出端子与通讯设备输出变量的映射关系
/// - 支持添加、删除、上移、下移映射项
/// - 实时验证工具输出和通讯输出是否存在
/// </summary>
public partial class Frm_OutputMapping : Form
{
    private readonly StationConfig _station;
    private readonly BindingList<MappingRow> _mappings = new();

    private class MappingRow
    {
        public int 序号 { get; set; }
        public string 工具输出 { get; set; }
        public string 通讯输出 { get; set; }
        public string 描述 { get; set; }
    }

    public Frm_OutputMapping(StationConfig station)
    {
        InitializeComponent();
        _station = station ?? throw new ArgumentNullException(nameof(station));
        
        InitializeGrid();
        LoadMappings();
        WireEvents();
    }

    private void InitializeGrid()
    {
        dgv_Mappings.AutoGenerateColumns = false;
        dgv_Mappings.AllowUserToAddRows = false;
        dgv_Mappings.RowHeadersVisible = false;
        dgv_Mappings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv_Mappings.MultiSelect = false;
        dgv_Mappings.Columns.Clear();

        // 序号列
        dgv_Mappings.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MappingRow.序号),
            HeaderText = "序号",
            Width = 60,
            ReadOnly = true
        });

        // 工具输出列（下拉选择）
        var toolOutputCol = new DataGridViewComboBoxColumn
        {
            DataPropertyName = nameof(MappingRow.工具输出),
            HeaderText = "工具输出端子",
            Width = 200,
            DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        };
        RefreshToolOutputItems(toolOutputCol);
        dgv_Mappings.Columns.Add(toolOutputCol);

        // 通讯输出列（下拉选择）
        var commOutputCol = new DataGridViewComboBoxColumn
        {
            DataPropertyName = nameof(MappingRow.通讯输出),
            HeaderText = "通讯输出变量",
            Width = 200,
            DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        };
        RefreshCommOutputItems(commOutputCol);
        dgv_Mappings.Columns.Add(commOutputCol);

        // 描述列
        dgv_Mappings.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MappingRow.描述),
            HeaderText = "描述",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        dgv_Mappings.DataSource = _mappings;
    }

    private void RefreshToolOutputItems(DataGridViewComboBoxColumn col)
    {
        col.Items.Clear();

        if (_station.DetectionTool?.ToolBlock != null)
        {
            var outputs = _station.DetectionTool.ToolBlock.Outputs;
            for (int i = 0; i < outputs.Count; i++)
            {
                col.Items.Add(outputs[i].Name);
            }
        }

        if (col.Items.Count == 0)
        {
            col.Items.Add("<无可用输出>");
        }
    }

    private void RefreshCommOutputItems(DataGridViewComboBoxColumn col)
    {
        col.Items.Clear();

        if (!string.IsNullOrWhiteSpace(_station.CommDeviceName))
        {
            var device = CommunicationFactory.Instance.GetDevice(_station.CommDeviceName);
            if (device != null)
            {
                var outputs = device.Table.Outputs;
                foreach (var output in outputs)
                {
                    col.Items.Add(output.Name);
                }
            }
        }

        if (col.Items.Count == 0)
        {
            col.Items.Add("<无可用输出>");
        }
    }

    private void LoadMappings()
    {
        _mappings.Clear();

        if (_station.OutputMappings != null)
        {
            int index = 1;
            foreach (var mapping in _station.OutputMappings)
            {
                _mappings.Add(new MappingRow
                {
                    序号 = index++,
                    工具输出 = mapping.ToolOutputName,
                    通讯输出 = mapping.CommOutputName,
                    描述 = mapping.Description
                });
            }
        }
    }

    private void SaveMappings()
    {
        _station.OutputMappings.Clear();

        foreach (var row in _mappings)
        {
            if (string.IsNullOrWhiteSpace(row.工具输出) || 
                string.IsNullOrWhiteSpace(row.通讯输出) ||
                row.工具输出.StartsWith("<") || 
                row.通讯输出.StartsWith("<"))
            {
                continue; // 跳过无效映射
            }

            _station.OutputMappings.Add(new StationConfig.OutputMapping
            {
                ToolOutputName = row.工具输出,
                CommOutputName = row.通讯输出,
                Description = row.描述
            });
        }
    }

    private void WireEvents()
    {
        btn_Add.Click += Btn_Add_Click;
        btn_Remove.Click += Btn_Remove_Click;
        btn_MoveUp.Click += Btn_MoveUp_Click;
        btn_MoveDown.Click += Btn_MoveDown_Click;
        btn_Refresh.Click += Btn_Refresh_Click;
        btn_Save.Click += Btn_Save_Click;
        btn_Cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
    }

    private void Btn_Add_Click(object sender, EventArgs e)
    {
        _mappings.Add(new MappingRow
        {
            序号 = _mappings.Count + 1,
            工具输出 = string.Empty,
            通讯输出 = string.Empty,
            描述 = string.Empty
        });
    }

    private void Btn_Remove_Click(object sender, EventArgs e)
    {
        if (dgv_Mappings.CurrentRow == null) return;

        int index = dgv_Mappings.CurrentRow.Index;
        _mappings.RemoveAt(index);
        Renumber();
    }

    private void Btn_MoveUp_Click(object sender, EventArgs e)
    {
        if (dgv_Mappings.CurrentRow == null) return;

        int index = dgv_Mappings.CurrentRow.Index;
        if (index <= 0) return;

        var item = _mappings[index];
        _mappings.RemoveAt(index);
        _mappings.Insert(index - 1, item);
        Renumber();

        dgv_Mappings.CurrentCell = dgv_Mappings.Rows[index - 1].Cells[0];
    }

    private void Btn_MoveDown_Click(object sender, EventArgs e)
    {
        if (dgv_Mappings.CurrentRow == null) return;

        int index = dgv_Mappings.CurrentRow.Index;
        if (index >= _mappings.Count - 1) return;

        var item = _mappings[index];
        _mappings.RemoveAt(index);
        _mappings.Insert(index + 1, item);
        Renumber();

        dgv_Mappings.CurrentCell = dgv_Mappings.Rows[index + 1].Cells[0];
    }

    private void Btn_Refresh_Click(object sender, EventArgs e)
    {
        // 刷新下拉列表项
        var toolOutputCol = dgv_Mappings.Columns[1] as DataGridViewComboBoxColumn;
        var commOutputCol = dgv_Mappings.Columns[2] as DataGridViewComboBoxColumn;

        if (toolOutputCol != null) RefreshToolOutputItems(toolOutputCol);
        if (commOutputCol != null) RefreshCommOutputItems(commOutputCol);

        dgv_Mappings.Refresh();
        MessageBox.Show("已刷新可用的输出端子和输出变量列表", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void Btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            // 验证映射
            for (int i = 0; i < _mappings.Count; i++)
            {
                var row = _mappings[i];

                if (string.IsNullOrWhiteSpace(row.工具输出) || row.工具输出.StartsWith("<"))
                {
                    MessageBox.Show($"第{i + 1}行：工具输出端子未选择", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(row.通讯输出) || row.通讯输出.StartsWith("<"))
                {
                    MessageBox.Show($"第{i + 1}行：通讯输出变量未选择", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            SaveMappings();
            LogHelper.Info($"[输出映射] 工位[{_station.Name}]保存了{_station.OutputMappings.Count}个输出映射");

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "[输出映射] 保存映射配置失败");
            MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Renumber()
    {
        for (int i = 0; i < _mappings.Count; i++)
        {
            _mappings[i].序号 = i + 1;
        }
    }
}
