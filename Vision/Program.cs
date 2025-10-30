using Logger;
using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Vision.Frm.MainForm;
using Vision.Localization;
using Vision.Infrastructure;

namespace Vision;

static class Program
{
  // 导入Windows API以启用DPI感知
  [DllImport("shcore.dll", SetLastError = true)]
  private static extern int SetProcessDpiAwareness(ProcessDpiAwareness awareness);

  // DPI感知级别枚举
  private enum ProcessDpiAwareness
  {
    Unaware = 0, // 不感知DPI
    SystemAware = 1, // 系统DPI感知
    PerMonitorAware = 2 // 每个显示器DPI感知
  }

  /// <summary>
  /// 应用程序的主入口点。
  /// </summary>
  [STAThread]
  static void Main()
  {
    // 在任何UI创建前启用DPI感知（解决DevExpress DpiAwareness异常）
    try
    {
      SetProcessDpiAwareness(ProcessDpiAwareness.PerMonitorAware);
    }
    catch
    {
      // 如果SetProcessDpiAwareness不可用（旧版Windows或COM异常），跳过
      // .NET Framework 4.8.1中无SetHighDpiMode，使用DllImport是最好的方式
    }

    bool createdNew;
    Mutex instance = new Mutex(initiallyOwned: true, "MutexName", out createdNew);
    string AllowCreatedMore = ConfigurationManager.AppSettings["AllowCreatedMore"];
    if ((string.IsNullOrWhiteSpace(AllowCreatedMore) || AllowCreatedMore.ToLower() == "false") && !createdNew)
    {
      MessageBox.Show("已经启动了一个程序，请先退出！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      Application.Exit();
    }
    else
    {
      // Initialize localization before any UI is created
      LocalizationManager.Initialize();

      // Register additional dll probing paths: Debug\Libraly and Debug\Runtime (and subfolders)
      // Note: Folder name kept as provided: "Libraly"
      AssemblyResolver.RegisterProbingFolders("Libraly", "Runtime");

      // 设置应用程序异常处理
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      // 处理UI线程异常
      Application.ThreadException += Application_ThreadException;
      // 处理非UI线程异常
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      LogHelper.Info("程序启动");
      Application.Run(new Frm_Main());
    }
  }

  /// <summary>
  /// 处理UI线程异常
  /// </summary>
  private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
  {
    try
    {
      // 记录异常日志
      LogHelper.Error(e.Exception, "UI线程异常");

      // 向用户显示友好错误消息
      MessageBox.Show(
        "程序遇到了一个问题，已记录异常信息。\n\n" +
        "错误信息: " + e.Exception.Message,
        "应用程序错误",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
    }
    catch (Exception ex)
    {
      try
      {
        LogHelper.Fatal(ex, "处理UI线程异常时发生错误");
      }
      catch
      {
        // 如果日志记录也失败，使用消息框作为最后手段
        MessageBox.Show("无法记录异常信息: " + ex.Message, "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }

  /// <summary>
  /// 处理非UI线程异常
  /// </summary>
  private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
  {
    try
    {
      Exception ex = e.ExceptionObject as Exception;

      // 记录异常日志
      if (ex != null)
      {
        LogHelper.Fatal(ex, "非UI线程异常");
      }
      else
      {
        LogHelper.Fatal(new Exception("未知异常类型"),
          "发生未知类型的非UI线程异常: " + e.ExceptionObject.ToString());
      }

      // 如果异常导致应用程序终止，记录这一信息
      if (e.IsTerminating)
      {
        LogHelper.Fatal(new Exception("应用程序即将终止"), "由于未处理的异常，应用程序即将关闭");

        MessageBox.Show(
          "程序遇到了一个严重问题，必须关闭。\n请联系技术支持获取帮助。",
          "应用程序即将关闭",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
      }
    }
    catch (Exception ex)
    {
      try
      {
        LogHelper.Fatal(ex, "处理非UI线程异常时发生错误");
      }
      catch
      {
        // 如果日志记录也失败，使用消息框作为最后手段
        MessageBox.Show("无法记录异常信息: " + ex.Message, "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}