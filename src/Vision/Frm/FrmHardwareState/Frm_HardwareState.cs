using HardwareCommNet;
using HardwareCameraNet;
using LightControlNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Vision.Frm.FrmHardwareState;

/// <summary>
/// 硬件状态显示界面
/// 统一显示相机、通讯设备、光源控制器的状态
/// </summary>
public partial class Frm_HardwareState : UserControl
{
    // 所有设备状态项统一管理
    private readonly Dictionary<string, StateItem> _deviceStateItems = new();

    // 设备类型前缀
    private const string CAMERA_PREFIX = "CAMERA:";
    private const string COMM_PREFIX = "COMM:";
    private const string LIGHT_PREFIX = "LIGHT:";

    public Frm_HardwareState()
    {
        InitializeComponent();

        // 初始化已有设备
        LoadExistingDevices();

        // 订阅相机事件
        CameraFactory.Instance.DeviceStateChanged += OnCameraStateChanged;
        CameraFactory.Instance.DeviceRemoved += OnCameraRemoved;

        // 订阅通讯设备事件
        CommunicationFactory.Instance.DeviceCreated += OnCommDeviceCreated;
        CommunicationFactory.Instance.DeviceRemoved += OnCommDeviceRemoved;
        CommunicationFactory.Instance.DeviceRenamed += OnCommDeviceRenamed;

        // 订阅光源控制器事件
        LightFactory.Instance.DeviceCreated += OnLightDeviceCreated;
        LightFactory.Instance.DeviceRemoved += OnLightDeviceRemoved;
        LightFactory.Instance.DeviceRenamed += OnLightDeviceRenamed;

        // 响应面板大小变化
        flowLayoutPanel_Devices.SizeChanged += FlowLayoutPanel_Devices_SizeChanged;
    }

    #region 初始化加载

    /// <summary>
    /// 加载所有已存在的设备
    /// </summary>
    private void LoadExistingDevices()
    {
        // 1. 加载相机设备
        foreach (var kv in CameraFactory.Instance.GetAllDeviceStates())
        {
            AddOrUpdateDevice(CAMERA_PREFIX + kv.Key, "相机", kv.Value.expain, kv.Value.isConnected, null);
        }

        // 2. 加载通讯设备
        try
        {
            var commDevices = CommunicationFactory.Instance.GetAllDevices().ToList();
            foreach (var device in commDevices)
            {
                try
                {
                    var deviceType = GetCommDeviceType(device);
                    AddOrUpdateDevice(COMM_PREFIX + device.Name, deviceType, device.Name, device.IsConnected, device);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载通讯设备失败 [{device?.Name}]: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载通讯设备列表失败: {ex.Message}");
        }

        // 3. 加载光源控制器
        try
        {
            var lightControllers = LightFactory.Instance.GetAllControllers().ToList();
            foreach (var controller in lightControllers)
            {
                try
                {
                    var type = controller.Type.ToString();
                    AddOrUpdateDevice(LIGHT_PREFIX + controller.Name, type, controller.Name, controller.IsConnected, null, controller);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载光源控制器失败 [{controller?.Name}]: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载光源控制器列表失败: {ex.Message}");
        }
    }

    #endregion

    #region 统一设备管理

    /// <summary>
    /// 添加或更新设备状态项
    /// </summary>
    /// <param name="key">唯一键（包含前缀）</param>
    /// <param name="deviceType">设备类型名</param>
    /// <param name="deviceName">设备名称</param>
    /// <param name="isConnected">是否已连接</param>
    /// <param name="commDevice">通讯设备实例（用于订阅状态事件）</param>
    /// <param name="lightController">光源控制器实例（用于订阅状态事件）</param>
    private void AddOrUpdateDevice(string key, string deviceType, string deviceName, bool isConnected, 
        IComm commDevice = null, ILightController lightController = null)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, string, string, bool, IComm, ILightController>(
                AddOrUpdateDevice), key, deviceType, deviceName, isConnected, commDevice, lightController);
            return;
        }

        var displayText = $"[{deviceType}] {deviceName}";

        if (_deviceStateItems.TryGetValue(key, out var existingItem))
        {
            // 已存在，更新状态
            existingItem.SetState(displayText, isConnected);
        }
        else
        {
            // 新建状态项
            StateItem item;
            if (commDevice != null)
            {
                item = new StateItem(key, displayText, isConnected, commDevice);
            }
            else if (lightController != null)
            {
                item = new StateItem(key, displayText, isConnected, lightController);
            }
            else
            {
                item = new StateItem(key, displayText, isConnected);
            }

            item.Height = 50;
            item.Width = flowLayoutPanel_Devices.ClientSize.Width - 20; // 预留滚动条空间

            _deviceStateItems[key] = item;
            flowLayoutPanel_Devices.Controls.Add(item);
        }
    }

    /// <summary>
    /// 移除设备状态项
    /// </summary>
    private void RemoveDevice(string key)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string>(RemoveDevice), key);
            return;
        }

        if (_deviceStateItems.TryGetValue(key, out var item))
        {
            try
            {
                flowLayoutPanel_Devices.Controls.Remove(item);
                item.Dispose();
            }
            catch { }
            finally
            {
                _deviceStateItems.Remove(key);
            }
        }
    }

    /// <summary>
    /// 重命名设备
    /// </summary>
    private void RenameDevice(string oldKey, string newKey, string deviceType, string newName, bool isConnected)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, string, string, string, bool>(RenameDevice), 
                oldKey, newKey, deviceType, newName, isConnected);
            return;
        }

        if (_deviceStateItems.TryGetValue(oldKey, out var item))
        {
            var displayText = $"[{deviceType}] {newName}";
            item.SetState(displayText, isConnected);

            // 更新字典键
            _deviceStateItems.Remove(oldKey);
            _deviceStateItems[newKey] = item;
        }
    }

    private void FlowLayoutPanel_Devices_SizeChanged(object sender, EventArgs e)
    {
        foreach (Control ctrl in flowLayoutPanel_Devices.Controls)
        {
            ctrl.Width = flowLayoutPanel_Devices.ClientSize.Width - 20; // 预留滚动条空间
        }
    }

    #endregion

    #region 相机设备事件处理

    private void OnCameraStateChanged(string sn, string expain, bool isConnected)
    {
        var key = CAMERA_PREFIX + sn;
        var displayName = string.IsNullOrEmpty(expain) ? sn : expain;
        AddOrUpdateDevice(key, "相机", displayName, isConnected, null);
    }

    private void OnCameraRemoved(string sn, string manufacturer)
    {
        RemoveDevice(CAMERA_PREFIX + sn);
    }

    #endregion

    #region 通讯设备事件处理

    private string GetCommDeviceType(IComm device)
    {
        if (device == null) return "通讯";
        var type = device.GetType();
        var attr = type.GetCustomAttribute<CommManufacturerAttribute>();
        return attr?.ManufacturerName ?? "通讯";
    }

    private void OnCommDeviceCreated(string name, string type, IComm device)
    {
        AddOrUpdateDevice(COMM_PREFIX + name, type, name, device?.IsConnected ?? false, device);
    }

    private void OnCommDeviceRemoved(string name, string type)
    {
        RemoveDevice(COMM_PREFIX + name);
    }

    private void OnCommDeviceRenamed(string oldName, string newName, string type)
    {
        var device = CommunicationFactory.Instance.GetDevice(newName);
        RenameDevice(COMM_PREFIX + oldName, COMM_PREFIX + newName, type, newName, device?.IsConnected ?? false);
    }

    #endregion

    #region 光源控制器事件处理

    private void OnLightDeviceCreated(string name, LightControllerType type, ILightController controller)
    {
        AddOrUpdateDevice(LIGHT_PREFIX + name, type.ToString(), name, controller?.IsConnected ?? false, null, controller);
    }

    private void OnLightDeviceRemoved(string name, LightControllerType type)
    {
        RemoveDevice(LIGHT_PREFIX + name);
    }

    private void OnLightDeviceRenamed(string oldName, string newName, LightControllerType type)
    {
        var controller = LightFactory.Instance.GetController(newName);
        RenameDevice(LIGHT_PREFIX + oldName, LIGHT_PREFIX + newName, type.ToString(), newName, controller?.IsConnected ?? false);
    }

    #endregion
}
