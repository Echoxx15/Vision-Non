using System.Windows.Forms;

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

        public static Form ConfigForm => LightFactory.Instance.GetConfigForm;
    }
}
