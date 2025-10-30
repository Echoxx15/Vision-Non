using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Vision.Manager.CameraManager;

namespace Vision.Frm.FrmHardwareState;

public partial class Frm_HardwareState : UserControl
{
    private readonly Dictionary<string, StateItem> CameraDeviceStateItems = new();

    public Frm_HardwareState()
    {
        InitializeComponent();

        // 先同步所有设备状态（相机）
        foreach (var kv in CameraManager.Instance.GetAllDeviceStates())
        {
            AddOrUpdateCamera(kv.Key, kv.Value.expain, kv.Value.isConnected);
        }
        CameraManager.Instance.DeviceStateChanged += AddOrUpdateCamera;
        CameraManager.Instance.DeviceRemoved += RemoveCamera;

        flowlayoutPanel_HardCamera.SizeChanged += FlowlayoutPanel_HardCamera_SizeChanged;
        flowlayoutPanel_HardComm.SizeChanged += FlowlayoutPanel_HardComm_SizeChanged;
    }

    private void AddOrUpdateCamera(string sn, string expain, bool isConnected)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, string, bool>(AddOrUpdateCamera), sn, expain, isConnected);
            return;
        }

        if (CameraDeviceStateItems.TryGetValue(sn, out var stateItem))
        {
            if (!string.IsNullOrEmpty(expain)) stateItem.SetState(expain, isConnected);
            else stateItem.SetState(stateItem.Text, isConnected);
        }
        else
        {
            var item = new StateItem(sn, expain, isConnected);
            CameraDeviceStateItems.Add(sn, item);
            item.Height = 50;
            item.Width = flowlayoutPanel_HardCamera.ClientSize.Width;
            flowlayoutPanel_HardCamera.Controls.Add(item);
        }
    }

    private void RemoveCamera(string sn)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string>(RemoveCamera), sn);
            return;
        }
        if (CameraDeviceStateItems.TryGetValue(sn, out var item))
        {
            try
            {
                flowlayoutPanel_HardCamera.Controls.Remove(item);
                item.Dispose();
            }
            catch { }
            finally
            {
                CameraDeviceStateItems.Remove(sn);
            }
        }
    }

    private void FlowlayoutPanel_HardCamera_SizeChanged(object sender, System.EventArgs e)
    {
        foreach (Control ctrl in flowlayoutPanel_HardCamera.Controls)
        {
            ctrl.Width = flowlayoutPanel_HardCamera.ClientSize.Width;
        }
    }

    private void FlowlayoutPanel_HardComm_SizeChanged(object sender, EventArgs e)
    {
        foreach (Control ctrl in flowlayoutPanel_HardComm.Controls)
        {
            ctrl.Width = flowlayoutPanel_HardComm.ClientSize.Width;
        }
    }
}