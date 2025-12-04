using System.Windows.Forms;
using HardwareCameraNet.UI;

namespace HardwareCameraNet
{
    /// <summary>
    /// 相机模块宿主：供主程序访问相机配置功能
    /// </summary>
    public static class CameraHost
    {
        /// <summary>
        /// 初始化相机模块
        /// </summary>
        public static void Initialize()
        {
            CameraPluginServer.Instance.LoadPlugins();
            CameraFactory.Instance.Initialize();
        }

        /// <summary>
        /// 获取相机配置窗体（用于 ShowDialog 或作为子窗体）
        /// </summary>
        public static Form ConfigForm => new Frm_CameraConfig();

        /// <summary>
        /// 释放相机模块资源
        /// </summary>
        public static void Dispose()
        {
            CameraFactory.Instance.UnInitialize();
        }
    }
}
