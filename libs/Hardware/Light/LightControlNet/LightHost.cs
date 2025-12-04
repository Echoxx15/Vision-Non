using System.Windows.Forms;
using LightControlNet.UI;

namespace LightControlNet
{
    public static class LightHost
    {
        public static void Initialize()
        {
            LightPluginServer.Instance.LoadPlugins();
            LightManager.Instance.BuildMap();
            LightFactory.Instance.ApplyConfigs();
        }

        /// <summary>
        /// 获取光源配置窗体（用于 ShowDialog）
        /// </summary>
        public static Form ConfigForm => new Frm_LightConfig();

        /// <summary>
        /// 获取光源配置控件（用于嵌入其他界面）
        /// </summary>
        public static UserControl ConfigControl => LightFactory.Instance.GetConfigControl();
    }
}
