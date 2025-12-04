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
		public bool 触发信号 { get; set; } // 是否为触发信号
		
		// TCP专用字段
		public int 字段索引 { get; set; } = -1; // -1表示使用整个消息
		public string 分隔符 { get; set; } = ",";
		public string 触发模式 { get; set; } = "值匹配"; // 值匹配、值变化、任意非空
		
		public CommValueType RealType { get; set; }
		public List<string> TriggerValuesList { get; set; } = new List<string>();
		public int TriggerModeValue { get; set; } = 0; // 0=值匹配, 1=值变化, 2=任意非空
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
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.名称), HeaderText = "名称", Width = 140 });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.类型), HeaderText = "类型", Width = 70, ReadOnly = true });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.起始字节), HeaderText = "起始字节", Width = 65 });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.长度), HeaderText = "长度", Width = 50 });
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.地址), HeaderText = "地址", Width = 100 });
		
		// TCP专用列：字段索引（-1表示使用整个消息）
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn 
		{ 
			DataPropertyName = nameof(Row.字段索引), 
			HeaderText = "字段索引", 
			Width = 65,
			ToolTipText = "TCP消息按分隔符拆分后的字段索引，-1表示使用整个消息"
		});
		
		// TCP专用列：分隔符
		dgv_Data.Columns.Add(new DataGridViewTextBoxColumn 
		{ 
			DataPropertyName = nameof(Row.分隔符), 
			HeaderText = "分隔符", 
			Width = 60,
			ToolTipText = "TCP消息的字段分隔符，如逗号(,)、分号(;)等"
		});
		
		// ✅ 触发信号勾选列
		dgv_Data.Columns.Add(new DataGridViewCheckBoxColumn 
		{ 
			DataPropertyName = nameof(Row.触发信号), 
			HeaderText = "触发信号", 
			Width = 70,
			TrueValue = true,
			FalseValue = false
		});
		
		// TCP专用列：触发模式下拉框
		var triggerModeCol = new DataGridViewComboBoxColumn
		{
			DataPropertyName = nameof(Row.触发模式),
			HeaderText = "触发模式",
			Width = 85,
			FlatStyle = FlatStyle.Flat,
			ToolTipText = "值匹配：仅当值在触发值列表中时触发\n值变化：值发生变化时触发\n任意非空：收到非空值就触发"
		};
		triggerModeCol.Items.AddRange("值匹配", "值变化", "任意非空");
		dgv_Data.Columns.Add(triggerModeCol);
		
		// 触发值按钮列
		var btnCol = new DataGridViewButtonColumn
		{
			DataPropertyName = nameof(Row.编辑值),
			HeaderText = "触发值",
			Width = 120,
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
			// 地址允许为空，运行时检查
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
			else if (colName == nameof(Row.字段索引))
			{
				// 字段索引可以为-1（使用整个消息）或>=0（使用指定字段）
				if (row.字段索引 < -1) row.字段索引 = -1;
				dgv_Data.Refresh();
			}
			else if (colName == nameof(Row.触发模式))
			{
				// 将触发模式文本转换为数值
				row.TriggerModeValue = TextToTriggerMode(row.触发模式);
			}
		};
	}

	/// <summary>
	/// 保存到通讯表
	/// </summary>
	/// <returns>保存成功返回true，校验失败返回false</returns>
	public bool SaveToTable()
	{
		if (_table == null)
		{
			Console.WriteLine("❌ SaveToTable: _table 为 null");
			return false;
		}
		
		Console.WriteLine($"SaveToTable: 开始保存，isInput={_isInput}, 行数={_rows.Count}");
		
		// 校验：名称唯一、长度>=1、起始字节>=0（地址允许为空，运行时检查）
		for (int i = 0; i < _rows.Count; i++)
		{
			var r = _rows[i];
			if (!IsUniqueName(r.名称, i))
			{
				Console.WriteLine($"❌ 第{i+1}行校验失败：名称重复");
				MessageBox.Show($"第{i+1}行名称重复", "校验失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			if (r.长度 < 1)
			{
				Console.WriteLine($"❌ 第{i+1}行校验失败：长度必须>=1");
				MessageBox.Show($"第{i+1}行长度必须>=1", "校验失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			if (r.起始字节 < 0)
			{
				Console.WriteLine($"❌ 第{i+1}行校验失败：起始字节不能为负");
				MessageBox.Show($"第{i+1}行起始字节不能为负", "校验失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
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
				IsTrigger = r.触发信号,
				// TCP专用字段
				FieldIndex = r.字段索引,
				Delimiter = r.分隔符 ?? ",",
				TriggerMode = r.TriggerModeValue
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
			Console.WriteLine($"  已添加: {cell.Name} ({cell.ValueType}) @ {cell.Address}, 起始={cell.StartByte}, 长度={cell.Length}, 触发信号={cell.IsTrigger}, 字段索引={cell.FieldIndex}, 分隔符={cell.Delimiter}, 触发模式={cell.TriggerMode}");
		}
		
		Console.WriteLine($"✅ SaveToTable 完成: 共保存 {savedCount} 项到 {(_isInput ? "输入" : "输出")}表");
		return true;
	}

	private bool IsUniqueName(string name, int currentIndex)
	{ return !_rows.Where((_, idx) => idx != currentIndex).Any(r => string.Equals(r.名称?.Trim(), name?.Trim(), System.StringComparison.OrdinalIgnoreCase)); }
	private string MakeUnique(string baseName)
	{ string n = (baseName ?? "Var").Trim(); int i =1; while (_rows.Any(r => string.Equals(r.名称, n, System.StringComparison.OrdinalIgnoreCase))) n = baseName + i++; return n; }

	// TCP专用：触发模式文本与数值转换
	private static string TriggerModeToText(int mode)
	{
		return mode switch
		{
			0 => "值匹配",
			1 => "值变化",
			2 => "任意非空",
			_ => "值匹配"
		};
	}
	
	private static int TextToTriggerMode(string text)
	{
		return text switch
		{
			"值匹配" => 0,
			"值变化" => 1,
			"任意非空" => 2,
			_ => 0
		};
	}

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
				触发信号 = c.IsTrigger,
				// TCP专用字段
				字段索引 = c.FieldIndex,
				分隔符 = c.Delimiter ?? ",",
				触发模式 = TriggerModeToText(c.TriggerMode),
				TriggerModeValue = c.TriggerMode,
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
			触发信号 = false,
			// TCP专用字段默认值
			字段索引 = -1,       // -1表示使用整个消息
			分隔符 = ",",        // 默认逗号分隔
			触发模式 = "值匹配", // 默认值匹配模式
			TriggerModeValue = 0,
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