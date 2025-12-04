using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Vision.Controls;

/// <summary>
/// 安全的 PropertyGrid 控件 - 禁用滚轮和双击切换下拉项
/// 用户只能通过点击下拉箭头并选择来修改值，防止误操作
/// </summary>
public class SafePropertyGrid : PropertyGrid
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
    private NativeWindow _gridViewHook;

    public SafePropertyGrid()
    {
        // 启用更好的视觉效果
        DrawFlatToolbar = true;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        HookGridView();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        UnhookGridView();
        base.OnHandleDestroyed(e);
    }

    /// <summary>
    /// 钩住内部的 GridView 控件来拦截消息
    /// </summary>
    private void HookGridView()
    {
        try
        {
            var gridViewField = typeof(PropertyGrid).GetField("gridView",
                BindingFlags.NonPublic | BindingFlags.Instance);
            _gridView = gridViewField?.GetValue(this) as Control;

            if (_gridView != null && _gridView.IsHandleCreated)
            {
                _gridViewHook = new GridViewMessageFilter(this, _gridView);
                _gridViewHook.AssignHandle(_gridView.Handle);
            }
            else if (_gridView != null)
            {
                _gridView.HandleCreated += (s, e) =>
                {
                    _gridViewHook = new GridViewMessageFilter(this, _gridView);
                    _gridViewHook.AssignHandle(_gridView.Handle);
                };
            }
        }
        catch { }
    }

    private void UnhookGridView()
    {
        _gridViewHook?.ReleaseHandle();
        _gridViewHook = null;
    }

    /// <summary>
    /// 内部类：拦截 GridView 的消息
    /// </summary>
    private class GridViewMessageFilter : NativeWindow
    {
        private readonly SafePropertyGrid _owner;
        private readonly Control _gridView;

        public GridViewMessageFilter(SafePropertyGrid owner, Control gridView)
        {
            _owner = owner;
            _gridView = gridView;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_LBUTTONDBLCLK = 0x0203;
            const int WM_MOUSEWHEEL = 0x020A;

            // 阻止双击切换值
            if (m.Msg == WM_LBUTTONDBLCLK && _owner.BlockDoubleClickToggle)
            {
                var gridItem = _owner.SelectedGridItem;
                if (gridItem != null && _owner.HasDropDownEditor(gridItem))
                {
                    // 获取点击位置
                    int x = (short)(m.LParam.ToInt32() & 0xFFFF);
                    int y = (short)((m.LParam.ToInt32() >> 16) & 0xFFFF);
                    var pt = new Point(x, y);

                    if (_owner.IsMouseInValueAreaInternal(_gridView, pt))
                    {
                        // 完全忽略双击，不做任何处理
                        // 这样就不会触发值的切换
                        return;
                    }
                }
            }

            // 阻止滚轮切换值
            if (m.Msg == WM_MOUSEWHEEL && _owner.BlockMouseWheelOnDropDown)
            {
                var gridItem = _owner.SelectedGridItem;
                if (gridItem != null && _owner.HasDropDownEditor(gridItem))
                {
                    // 阻止滚轮消息
                    return;
                }
            }

            base.WndProc(ref m);
        }
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (BlockMouseWheelOnDropDown && SelectedGridItem != null)
        {
            if (HasDropDownEditor(SelectedGridItem))
            {
                if (IsMouseInValueArea(e.Location))
                {
                    ((HandledMouseEventArgs)e).Handled = true;
                    return;
                }
            }
        }

        base.OnMouseWheel(e);
    }

    /// <summary>
    /// 检查鼠标是否在值编辑区域（内部使用，坐标相对于 GridView）
    /// </summary>
    internal bool IsMouseInValueAreaInternal(Control gridView, Point ptInGridView)
    {
        try
        {
            var internalLabelWidthField = gridView.GetType().GetField("labelWidth",
                BindingFlags.NonPublic | BindingFlags.Instance);

            int labelWidth = 100;
            if (internalLabelWidthField != null)
            {
                labelWidth = (int)internalLabelWidthField.GetValue(gridView);
            }

            return ptInGridView.X > labelWidth;
        }
        catch { }

        return ptInGridView.X > (gridView.Width * 0.4);
    }

    /// <summary>
    /// 检查鼠标是否在值编辑区域
    /// </summary>
    private bool IsMouseInValueArea(Point clientPoint)
    {
        if (_gridView != null)
        {
            var ptInGridView = _gridView.PointToClient(PointToScreen(clientPoint));
            return IsMouseInValueAreaInternal(_gridView, ptInGridView);
        }

        return clientPoint.X > (Width * 0.4);
    }

    /// <summary>
    /// 检查属性是否有下拉编辑器
    /// </summary>
    internal bool HasDropDownEditor(GridItem gridItem)
    {
        if (gridItem?.PropertyDescriptor == null) return false;

        // 检查是否有标准值列表
        var converter = gridItem.PropertyDescriptor.Converter;
        if (converter != null && converter.GetStandardValuesSupported())
        {
            return true;
        }

        // 检查是否是枚举类型
        var propType = gridItem.PropertyDescriptor.PropertyType;
        if (propType != null && propType.IsEnum)
        {
            return true;
        }

        // 检查是否是布尔类型
        if (propType == typeof(bool))
        {
            return true;
        }

        return false;
    }
}
