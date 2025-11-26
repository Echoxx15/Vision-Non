using System;
using System.Windows.Forms;
using HardwareCommNet.CommTable;

namespace HardwareCommNet.UI;

public partial class Frm_CommTable : Form
{
    private readonly IComm _device;
 
    public Frm_CommTable(IComm device)
    {
        InitializeComponent();
        _device = device ?? throw new ArgumentNullException(nameof(device));
        Text = $"通讯表配置 - {device.Name}";

        // 绑定输入输出表
        uInputOutputTable1.Bind(_device.Table, isInput: true);
        uOutputTable1.Bind(_device.Table); // ✅ 启用输出表
 
        btn_Cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
    }

    private void btn_Confirm_Click(object sender, EventArgs e)
    {
        try
        {
            Console.WriteLine($"[{_device.Name}] 通讯表保存 - 开始");
 
            // ✅ 第1步：将两个控件的更改写回设备的 CommTable
            Console.WriteLine($"[{_device.Name}] 正在保存输入表...");
            uInputOutputTable1.SaveToTable();
 
            Console.WriteLine($"[{_device.Name}] 正在保存输出表...");
            uOutputTable1.SaveToTable(); // ✅ 保存输出表
 
            // ✅ 验证：输出当前 Table 的状态
            var inputCount = _device.Table.Inputs?.Count ?? 0;
            var outputCount = _device.Table.Outputs?.Count ?? 0;
            Console.WriteLine($"[{_device.Name}] Table 当前状态: 输入{inputCount}项, 输出{outputCount}项");
 
            if (inputCount == 0 && outputCount == 0)
            {
                Console.WriteLine($"[{_device.Name}] ⚠️ 警告：Table 为空，可能保存失败或表格为空");
            }
 
            // ✅ 第2步：保存到设备配置并写入磁盘
            Console.WriteLine($"[{_device.Name}] 正在序列化并保存到磁盘...");
            CommunicationFactory.Instance.SaveConfigs();
 
            Console.WriteLine($"[{_device.Name}] ✅ 通讯表保存完成");
            MessageBox.Show("通讯表已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
 
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{_device.Name}] ❌ 保存失败: {ex.Message}");
            Console.WriteLine($"   堆栈: {ex.StackTrace}");
            MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}