using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HardwareCommNet.CommTable;

namespace HardwareCommNet.UI;

public partial class uOutputTable : UserControl
{
	private readonly BindingList<Row> _rows = [];
	private CommTable.CommTable _table;

	private class Row
	{
		public int 序号 { get; set; }
		public string 名称 { get; set; }
		public string 类型 { get; set; }
		public string 地址 { get; set; }
		public string 描述 { get; set; }
		public CommValueType RealType { get; set; }
	}

	public uOutputTable()
	{
		InitializeComponent();
		InitGrid();
		WireButtons();
		HookValidation();
	}

	public void Bind(CommTable.CommTable table)
	{
		_table = table ?? throw new ArgumentNullException(nameof(table));
		ReloadFromTable();
	}

	private void InitGrid()
	{
		dgv_Data.AutoGenerateColumns = false;
		dgv_Data.AllowUserToAddRows = false;
		dgv_Data.RowHeadersVisible = false;
		dgv_Data.Columns.Clear();
		
		// 只保留5列：序号、名称、类型、地址、描述
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.序号), HeaderText = "序号", Width = 60, ReadOnly = true });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.名称), HeaderText = "名称", Width = 200 });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.类型), HeaderText = "类型", Width = 100, ReadOnly = true });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.地址), HeaderText = "地址", Width = 150 });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.描述), HeaderText = "描述", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
		
		dgv_Data.DataSource = _rows;
	}

	private void WireButtons()
	{
		btn_Bool.Click += (_, _) => AddItem(CommValueType.Bool);
		btn_Short.Click += (_, _) => AddItem(CommValueType.Short);
		btn_Int.Click += (_, _) => AddItem(CommValueType.Int);
		btn_Float.Click += (_, _) => AddItem(CommValueType.Float);
		btn_Double.Click += (_, _) => AddItem(CommValueType.Double);
		btn_String.Click += (_, _) => AddItem(CommValueType.String);
		btn_BoolArray.Click += (_, _) => AddItem(CommValueType.BoolArray);
		btn_ShortArray.Click += (_, _) => AddItem(CommValueType.ShortArray);
		btn_IntArray.Click += (_, _) => AddItem(CommValueType.IntArray);
		btn_FLoatArray.Click += (_, _) => AddItem(CommValueType.FloatArray);
		btn_DoubleArray.Click += (_, _) => AddItem(CommValueType.DoubleArray);
		btn_StringArray.Click += (_, _) => AddItem(CommValueType.StringArray);
		btn_MoveUp.Click += (_, _) => MoveSelected(-1);
		btn_MoveDown.Click += (_, _) => MoveSelected(1);
		btn_Remove.Click += (_, _) => RemoveSelected();
	}

	private void HookValidation()
	{
		dgv_Data.CellEndEdit += (_, e) =>
		{
			if (e.RowIndex < 0 || e.RowIndex >= _rows.Count) return;
			var row = _rows[e.RowIndex];
			var colName = dgv_Data.Columns[e.ColumnIndex].DataPropertyName;
			
			if (colName == nameof(Row.名称))
			{
				if (!IsUniqueName(row.名称, e.RowIndex))
				{
					MessageBox.Show("名称已存在", "校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					row.名称 = MakeUnique(row.名称);
					dgv_Data.Refresh();
				}
			}
			else if (colName == nameof(Row.地址))
			{
				if (string.IsNullOrWhiteSpace(row.地址))
				{
					MessageBox.Show("地址不能为空", "校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		};
	}

	public void SaveToTable()
	{
		if (_table == null)
		{
			Console.WriteLine("❌ SaveToTable: _table 为 null");
			return;
		}
		
		Console.WriteLine($"SaveToTable: 开始保存输出表，行数={_rows.Count}");
		
		// 校验：名称唯一、地址必填
		for (int i = 0; i < _rows.Count; i++)
		{
			var r = _rows[i];
			if (!IsUniqueName(r.名称, i))
			{
				Console.WriteLine($"❌ 第{i+1}行校验失败：名称重复");
				MessageBox.Show($"第{i+1}行名称重复", "校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			if (string.IsNullOrWhiteSpace(r.地址))
			{
				Console.WriteLine($"❌ 第{i+1}行校验失败：地址为空");
				MessageBox.Show($"第{i+1}行地址不能为空", "校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
		}
		
		Console.WriteLine($"SaveToTable: 校验通过，开始写回输出表");
		
		// 清空输出表
		_table.ClearOutputs();
		
		// 写回输出表
		int savedCount = 0;
		foreach (var r in _rows)
		{
			var cell = new CommCell
			{
				Name = r.名称,
				ValueType = r.RealType,
				Address = r.地址,
				Description = r.描述,
				// 输出表不需要这些字段，设置为默认值
				StartByte = 0,
				Length = 1,
				TriggerValues = new List<string>(),
				IsTrigger = false
			};
			
			_table.AddOrUpdateOutput(cell);
			savedCount++;
			Console.WriteLine($"  已添加输出: {cell.Name} ({cell.ValueType}) @ {cell.Address}");
		}
		
		Console.WriteLine($"✅ SaveToTable 完成: 共保存 {savedCount} 项到输出表");
	}

	private bool IsUniqueName(string name, int currentIndex)
	{ return !_rows.Where((_, idx) => idx != currentIndex).Any(r => string.Equals(r.名称?.Trim(), name?.Trim(), System.StringComparison.OrdinalIgnoreCase)); }
	private string MakeUnique(string baseName)
	{ string n = (baseName ?? "Var").Trim(); int i =1; while (_rows.Any(r => string.Equals(r.名称, n, System.StringComparison.OrdinalIgnoreCase))) n = baseName + i++; return n; }

	private static bool TryParseByteSpec(string spec, out int size, out string reason)
	{
		size =1; reason = null; if (string.IsNullOrWhiteSpace(spec)) { size =1; return true; }
		var s = spec.Trim();
		if (Regex.IsMatch(s, "^\\d+$")) { if (int.TryParse(s, out var v) && v >=1) { size = v; return true; } reason = "字节需>=1"; return false; }
		var m = Regex.Match(s, "^(\\d+)[-~](\\d+)$");
		if (m.Success)
		{
			var a = int.Parse(m.Groups[1].Value); var b = int.Parse(m.Groups[2].Value);
			if (a >=1 && b >= a) { size = b - a +1; return true; }
			reason = "范围应为 起>=1 且终>=起"; return false;
		}
		reason = "格式应为 N 或 N-M"; return false;
	}

	private void ReloadFromTable()
	{
		_rows.Clear();
		if (_table == null) return;
		
		var list = _table.Outputs;
		foreach (var c in list)
		{
			_rows.Add(new Row
			{
				序号 = c.Index,
				名称 = c.Name,
				类型 = c.ValueType.ToString(),
				地址 = c.Address,
				描述 = c.Description,
				RealType = c.ValueType
			});
		}
	}

	private void AddItem(CommValueType type)
	{
		string baseName = "Output";
		int suffix = 0;
		string name;
		do { name = baseName + suffix++; } while (_rows.Any(r => string.Equals(r.名称, name, StringComparison.OrdinalIgnoreCase)));
		
		_rows.Add(new Row
		{
			序号 = _rows.Count + 1,
			名称 = name,
			类型 = type.ToString(),
			地址 = string.Empty,
			描述 = string.Empty,
			RealType = type
		});
	}

	private void MoveSelected(int offset)
	{
		if (dgv_Data.CurrentRow == null) return; int idx = dgv_Data.CurrentRow.Index;
		int newIdx = idx + offset; if (newIdx <0 || newIdx >= _rows.Count) return;
		var item = _rows[idx]; _rows.RemoveAt(idx); _rows.Insert(newIdx, item); Renumber();
		dgv_Data.CurrentCell = dgv_Data.Rows[newIdx].Cells[0];
	}

	private void RemoveSelected()
	{
		if (dgv_Data.CurrentRow == null) return; int idx = dgv_Data.CurrentRow.Index; _rows.RemoveAt(idx); Renumber();
	}

	private void Renumber() { for (int i =0; i < _rows.Count; i++) _rows[i].序号 = i +1; }
}