using System.Drawing;

namespace Vision.Frm.FrmHardwareState;

public sealed partial class StateItem : AntdUI.Button
{
    private string _sn { get; }
    public string SN => _sn;

    /// <summary>
    /// 创建状态Item
    /// </summary>
    /// <param name="sn"></param>
    /// <param name="text"></param>设备名
    /// <param name="state"></param>设备状态
    public StateItem(string sn,string text,bool state)
    {
        InitializeComponent();
        _sn = sn;
        OriginalBackColor = Color.FromArgb(64, 64, 64);
        SetState(text, state);    
    }
    /// <summary>
    /// 更新设备状态和备注
    /// </summary>
    public void SetState(string text, bool state)
    {
        Text = text;
        DefaultBack = state ? Color.Lime : Color.Red;
    }

}