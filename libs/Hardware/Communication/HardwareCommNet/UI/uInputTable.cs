using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HardwareCommNet.CommTable;

namespace HardwareCommNet.UI;

public partial class uInputTable : UserControl
{
	private BindingList<Row> _rows = new();
	private bool _isInput = true;
	private CommTable.CommTable _table;

	private class Row
	{
		public int 序号 { get; set; }
		public string 名称 { get; set; }
		public string 类型 { get; set; }
		public int 起始字节 { get; set; }
		public int 长度 { get; set; }
		public string 地址 { get; set; }
		public string 编辑值 { get; set; }
		public string 描述 { get; set; }
		public bool 触发信号 { get; set; } // ✅ 新增：是否为触发信号
		public CommValueType RealType { get; set; }
		public List<string> TriggerValuesList { get; set; } = new List<string>();
	}

	public uInputTable()
	{
		InitializeComponent();
		InitGrid();
		WireButtons();
		HookValidation();
	}

	public void Bind(CommTable.CommTable table, bool isInput)
	{
		_table = table ?? throw new ArgumentNullException(nameof(table));
		_isInput = isInput;
		ReloadFromTable();
	}

	private void InitGrid()
	{
		dgv_Data.AutoGenerateColumns = false;
		dgv_Data.AllowUserToAddRows = false;
		dgv_Data.RowHeadersVisible = false;
		dgv_Data.Columns.Clear();
		
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.序号), HeaderText = "序号", Width = 60, ReadOnly = true });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.名称), HeaderText = "名称", Width = 160 });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.类型), HeaderText = "类型", Width = 80, ReadOnly = true });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.起始字节), HeaderText = "起始字节", Width = 80 });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.长度), HeaderText = "长度", Width = 60 });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.地址), HeaderText = "地址", Width = 120 });
		
		// ✅ 新增：触发信号勾选列
		dgv_Data.Columns.Add(new DataGridViewCheckBoxColumn 
		{ 
			DataPropertyName = nameof(Row.触发信号), 
			HeaderText = "触发信号", 
			Width = 80,
			TrueValue = true,
			FalseValue = false
		});
		
		// 触发值按钮列
		var btnCol = new DataGridViewButtonColumn
		{
			DataPropertyName = nameof(Row.编辑值),
			HeaderText = "触发值",
			Width = 200,
			Text = "编辑...",
			UseColumnTextForButtonValue = false
		};
		dgv_Data.Columns.Add(btnCol);
		dgv_Data.CellContentClick += Dgv_Data_CellContentClick;
		
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.描述), HeaderText = "描述", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
		dgv_Data.DataSource = _rows;
	}

	private void Dgv_Data_CellContentClick(object sender, DataGridViewCellEventArgs e)
	{
		if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
		var col = dgv_Data.Columns[e.ColumnIndex];
		if (col.HeaderText == "触发值")
		{
			var row = _rows[e.RowIndex];
			using var frm = new Frm_TriggerValuesEditor(row.TriggerValuesList);
			if (frm.ShowDialog(this) == DialogResult.OK)
			{
				row.TriggerValuesList = frm.TriggerValues;
				row.编辑值 = $"共 {row.TriggerValuesList.Count} 项";
				dgv_Data.Refresh();
			}
		}
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
			else if (colName == nameof(Row.起始字节))
			{
				if (row.起始字节 < 0) row.起始字节 = 0;
				dgv_Data.Refresh();
			}
			else if (colName == nameof(Row.长度))
			{
				if (row.长度 < 1) row.长度 = 1;
				dgv_Data.Refresh();
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
		
		Console.WriteLine($"SaveToTable: 开始保存，isInput={_isInput}, 行数={_rows.Count}");
		
		// 校验：名称唯一、地址必填、长度>=1
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
			if (r.长度 < 1)
			{
				Console.WriteLine($"❌ 第{i+1}行校验失败：长度必须>=1");
				MessageBox.Show($"第{i+1}行长度必须>=1", "校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			if (r.起始字节 < 0)
			{
				Console.WriteLine($"❌ 第{i+1}行校验失败：起始字节不能为负");
				MessageBox.Show($"第{i+1}行起始字节不能为负", "校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
		}
		
		Console.WriteLine($"SaveToTable: 校验通过，开始写回 Table");
		
		// 写回
		if (_isInput)
		{
			Console.WriteLine("清空输入表...");
			_table.ClearInputs();
		}
		else
		{
			Console.WriteLine("清空输出表...");
			_table.ClearOutputs();
		}
		
		int savedCount = 0;
		foreach (var r in _rows)
		{
			var cell = new CommCell
			{
				Name = r.名称,
				ValueType = r.RealType,
				StartByte = r.起始字节,
				Length = r.长度,
				Address = r.地址,
				TriggerValues = new List<string>(r.TriggerValuesList),
				Description = r.描述,
				IsTrigger = r.触发信号 // ✅ 保存触发信号标志
			};
			
			if (_isInput)
			{
				_table.AddOrUpdateInput(cell);
			}
			else
			{
				_table.AddOrUpdateOutput(cell);
			}
			savedCount++;
			Console.WriteLine($"  已添加: {cell.Name} ({cell.ValueType}) @ {cell.Address}, 起始={cell.StartByte}, 长度={cell.Length}, 触发信号={cell.IsTrigger}");
		}
		
		Console.WriteLine($"✅ SaveToTable 完成: 共保存 {savedCount} 项到 {(_isInput ? "输入" : "输出")}表");
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
		
		var list = _isInput ? _table.Inputs : _table.Outputs;
		foreach (var c in list)
		{
			var triggerList = new List<string>(c.TriggerValues ?? new List<string>());
			_rows.Add(new Row
			{
				序号 = c.Index,
				名称 = c.Name,
				类型 = c.ValueType.ToString(),
				起始字节 = c.StartByte,
				长度 = c.Length > 0 ? c.Length : 1,
				地址 = c.Address,
				编辑值 = triggerList.Count > 0 ? $"共 {triggerList.Count} 项" : "无",
				描述 = c.Description,
				触发信号 = c.IsTrigger, // ✅ 加载触发信号标志
				RealType = c.ValueType,
				TriggerValuesList = triggerList
			});
		}
	}

	private void AddItem(CommValueType type)
	{
		string baseName = "Var";
		int suffix = 0;
		string name;
		do { name = baseName + suffix++; } while (_rows.Any(r => string.Equals(r.名称, name, StringComparison.OrdinalIgnoreCase)));
		
		_rows.Add(new Row
		{
			序号 = _rows.Count + 1,
			名称 = name,
			类型 = type.ToString(),
			起始字节 = 0,
			长度 = 1,
			地址 = string.Empty,
			编辑值 = "无",
			描述 = string.Empty,
			RealType = type,
			TriggerValuesList = new List<string>()
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