using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Vision.Controls;

/// <summary>
/// 安全的 PropertyGrid 控件 - 禁用滚轮和双击切换下拉项
/// 用户只能通过点击下拉箭头或手动输入来修改值，防止误操作
/// </summary>
public class SafePropertyGrid : PropertyGrid, IMessageFilter
{
    /// <summary>
    /// 是否阻止滚轮切换下拉项（默认 true）
    /// </summary>
    public bool BlockMouseWheelOnDropDown { get; set; } = true;

    /// <summary>
    /// 是否阻止双击切换下拉项（默认 true）
    /// </summary>
    public bool BlockDoubleClickToggle { get; set; } = true;

    private Control _gridView;
    private const int WM_LBUTTONDBLCLK = 0x0203;
    private const int WM_MOUSEWHEEL = 0x020A;

    [DllImport("user32.dll")]
    private static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

    public SafePropertyGrid()
    {
        DrawFlatToolbar = true;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        
        // 获取内部 GridView 引用
        try
        {
            var gridViewField = typeof(PropertyGrid).GetField("gridView",
                BindingFlags.NonPublic | BindingFlags.Instance);
            _gridView = gridViewField?.GetValue(this) as Control;
        }
        catch { }

        // 注册消息过滤器
        Application.AddMessageFilter(this);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        Application.RemoveMessageFilter(this);
        base.OnHandleDestroyed(e);
    }

    /// <summary>
    /// 全局消息过滤 - 在消息分发到控件之前拦截
    /// </summary>
    public bool PreFilterMessage(ref Message m)
    {
        if (!IsHandleCreated) return false;
        
        // 只处理双击和滚轮消息
        if (m.Msg != WM_LBUTTONDBLCLK && m.Msg != WM_MOUSEWHEEL)
        {
            return false;
        }
        
        IntPtr targetHandle = m.HWnd;
        
        // 检查是否是我们的子控件
        if (targetHandle != Handle && !IsChild(Handle, targetHandle))
        {
            return false;
        }

        // 处理左键双击 - 对于有下拉编辑器的属性，完全阻止双击
        if (m.Msg == WM_LBUTTONDBLCLK && BlockDoubleClickToggle)
        {
            if (ShouldBlockDoubleClick())
            {
                return true; // 完全吃掉双击消息
            }
        }

        // 处理滚轮
        if (m.Msg == WM_MOUSEWHEEL && BlockMouseWheelOnDropDown)
        {
            if (ShouldBlockWheel())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 判断是否应该阻止双击
    /// 对于有下拉选项的属性（bool、enum、有StandardValues的类型），阻止双击切换
    /// </summary>
    private bool ShouldBlockDoubleClick()
    {
        try
        {
            var gridItem = SelectedGridItem;
            if (gridItem == null) return false;
            
            // 只要是有下拉编辑器的属性，就阻止双击（无论点击位置）
            return HasDropDownEditor(gridItem);
        }
        catch
        {
            return false;
        }
    }

    private bool ShouldBlockWheel()
    {
        try
        {
            var gridItem = SelectedGridItem;
            return gridItem != null && HasDropDownEditor(gridItem);
        }
        catch
        {
            return false;
        }
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (BlockMouseWheelOnDropDown && SelectedGridItem != null)
        {
            if (HasDropDownEditor(SelectedGridItem))
            {
                ((HandledMouseEventArgs)e).Handled = true;
                return;
            }
        }
        base.OnMouseWheel(e);
    }

    /// <summary>
    /// 检查属性是否有下拉编辑器（bool、enum、或有 StandardValues 的类型）
    /// </summary>
    private bool HasDropDownEditor(GridItem gridItem)
    {
        if (gridItem?.PropertyDescriptor == null) return false;

        var converter = gridItem.PropertyDescriptor.Converter;
        if (converter != null && converter.GetStandardValuesSupported())
        {
            return true;
        }

        var propType = gridItem.PropertyDescriptor.PropertyType;
        if (propType != null && propType.IsEnum)
        {
            return true;
        }

        if (propType == typeof(bool))
        {
            return true;
        }

        return false;
    }
}
