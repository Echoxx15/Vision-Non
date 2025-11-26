using HardwareCommNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Vision.Manager.CameraManager;

namespace Vision.Frm.FrmHardwareState;

public partial class Frm_HardwareState : UserControl
{
    private readonly Dictionary<string, StateItem> CameraDeviceStateItems = new();
    private readonly Dictionary<string, StateItem> CommDeviceStateItems = new();

    public Frm_HardwareState()
    {
        InitializeComponent();

        // 相机设备管理
        foreach (var kv in CameraManager.Instance.GetAllDeviceStates())
        {
            AddOrUpdateCamera(kv.Key, kv.Value.expain, kv.Value.isConnected);
        }

        CameraManager.Instance.DeviceStateChanged += AddOrUpdateCamera;
        CameraManager.Instance.DeviceRemoved += RemoveCamera;

        // 订阅 CommunicationFactory 的设备事件（统一管理所有通讯设备）
        CommunicationFactory.Instance.DeviceCreated += OnCommDeviceCreated;
        CommunicationFactory.Instance.DeviceRemoved += OnCommDeviceRemoved;
        CommunicationFactory.Instance.DeviceRenamed += OnCommDeviceRenamed;

        // 加载已存在的通讯设备
        LoadExistingCommDevices();

        // 响应面板大小变化
        flowlayoutPanel_HardCamera.SizeChanged += FlowlayoutPanel_HardCamera_SizeChanged;
        flowlayoutPanel_HardComm.SizeChanged += FlowlayoutPanel_HardComm_SizeChanged;
    }

    #region 相机设备管理

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
            catch
            {
            }
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

    #endregion

    #region 通讯设备管理（统一通过 CommunicationFactory）

    /// <summary>
    /// 加载已存在的通讯设备（在订阅事件之后调用）
    /// </summary>
    private void LoadExistingCommDevices()
    {
        try
        {
            var devices = CommunicationFactory.Instance.GetAllDevices().ToList();
            foreach (var device in devices)
            {
                try
                {
                    var deviceType = GetDeviceType(device);
                    OnCommDeviceCreated(device.Name, deviceType, device);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载已存在的通讯设备失败 [{device?.Name}]: {ex.Message}");
                }
            }

            if (devices.Count > 0)
            {
                Console.WriteLine($"已加载 {devices.Count} 个已存在的通讯设备到状态面板");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载已存在的通讯设备失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取设备的类型名称
    /// </summary>
    private string GetDeviceType(IComm device)
    {
        if (device == null) return string.Empty;

        var type = device.GetType();
        var attr = type.GetCustomAttribute<CommManufacturerAttribute>();
        return attr?.ManufacturerName ?? type.Name;
    }

    /// <summary>
    /// 通讯设备创建事件处理
    /// </summary>
    private void OnCommDeviceCreated(string name, string type, IComm device)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, string, IComm>(OnCommDeviceCreated), name, type, device);
            return;
        }

        try
        {
            var key = $"COMM:{name}";
            var text = $"{type} - {name}";

            // 检查是否已存在（避免重复添加）
            if (CommDeviceStateItems.ContainsKey(key))
            {
                Console.WriteLine($"通讯设备状态项已存在，跳过添加：{key}");
                return;
            }

            var item = new StateItem(key, text, device.IsConnected, device)
            {
                Height = 50,
                Width = flowlayoutPanel_HardComm.ClientSize.Width
            };

            CommDeviceStateItems[key] = item;
            flowlayoutPanel_HardComm.Controls.Add(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"添加通讯设备状态项失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 通讯设备移除事件处理
    /// </summary>
    private void OnCommDeviceRemoved(string name, string type)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, string>(OnCommDeviceRemoved), name, type);
            return;
        }

        try
        {
            var key = $"COMM:{name}";
            if (CommDeviceStateItems.TryGetValue(key, out var item))
            {
                flowlayoutPanel_HardComm.Controls.Remove(item);
                item.Dispose();
                CommDeviceStateItems.Remove(key);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"移除通讯设备状态项失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 通讯设备重命名事件处理
    /// </summary>
    private void OnCommDeviceRenamed(string oldName, string newName, string type)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, string, string>(OnCommDeviceRenamed), oldName, newName, type);
            return;
        }

        try
        {
            var oldKey = $"COMM:{oldName}";
            var newKey = $"COMM:{newName}";
            var newText = $"{type} - {newName}";

            if (CommDeviceStateItems.TryGetValue(oldKey, out var item))
            {
                // 更新文本
                item.SetState(newText, item.Connected);

                // 更新字典键
                CommDeviceStateItems.Remove(oldKey);
                CommDeviceStateItems[newKey] = item;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"重命名通讯设备状态项失败：{ex.Message}");
        }
    }

    private void FlowlayoutPanel_HardComm_SizeChanged(object sender, EventArgs e)
    {
        foreach (Control ctrl in flowlayoutPanel_HardComm.Controls)
        {
            ctrl.Width = flowlayoutPanel_HardComm.ClientSize.Width;
        }
    }

    #endregion
}