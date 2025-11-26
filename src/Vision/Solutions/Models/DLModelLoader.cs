using Logger;
using Vision.Solutions.Models;

namespace Vision.Solutions.Models;

public static class DLModelLoader
{
  public static void TryLoad(StationConfig station)
  {
    // 深度学习加载已在当前方案中停用，保留空实现以兼容旧调用
    LogHelper.Info($"[DLModelLoader] 跳过工位[{station?.Name}]深度学习加载（已停用）");
  }

  public static void TryUnload(StationConfig station)
  {
    LogHelper.Info($"[DLModelLoader] 跳过工位[{station?.Name}]深度学习释放（已停用）");
  }

  // 兼容调用
  public static bool LoadModel(StationConfig station)
  {
    TryLoad(station);
    return true; // 视为成功
  }

  public static void UnloadModel(StationConfig station)
  {
    TryUnload(station);
  }
}