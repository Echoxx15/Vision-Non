using System.ComponentModel;
using HardwareCommNet;

namespace Vision.Frm.Modbus;

/// <summary>
/// 通讯设备数据模型
/// </summary>
public sealed class CommDeviceModel : INotifyPropertyChanged
{
    public string Name
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public string Type
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Type));
            }
        }
    }

    public string Status
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Status));
            }
        }
    }

    public IComm Device { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
