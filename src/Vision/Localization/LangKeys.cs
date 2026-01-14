namespace Vision.Localization;

/// <summary>
/// 语言键常量 - 集中管理所有翻译键
/// 使用方式: LanguageService.Instance.Get(LangKeys.Common_OK)
/// </summary>
public static class LangKeys
{
    #region 通用

    public const string Common_OK = "Common.OK";
    public const string Common_Cancel = "Common.Cancel";
    public const string Common_Save = "Common.Save";
    public const string Common_Delete = "Common.Delete";
    public const string Common_Edit = "Common.Edit";
    public const string Common_Add = "Common.Add";
    public const string Common_Close = "Common.Close";
    public const string Common_Confirm = "Common.Confirm";
    public const string Common_Yes = "Common.Yes";
    public const string Common_No = "Common.No";
    public const string Common_Warning = "Common.Warning";
    public const string Common_Error = "Common.Error";
    public const string Common_Info = "Common.Info";
    public const string Common_Success = "Common.Success";
    public const string Common_Failed = "Common.Failed";
    public const string Common_Loading = "Common.Loading";
    public const string Common_Refresh = "Common.Refresh";
    public const string Common_Search = "Common.Search";
    public const string Common_Reset = "Common.Reset";
    public const string Common_Export = "Common.Export";
    public const string Common_Import = "Common.Import";
    public const string Common_Settings = "Common.Settings";
    public const string Common_Help = "Common.Help";
    public const string Common_About = "Common.About";
    public const string Common_Exit = "Common.Exit";
    public const string Common_Start = "Common.Start";
    public const string Common_Stop = "Common.Stop";
    public const string Common_Run = "Common.Run";
    public const string Common_Pause = "Common.Pause";
    public const string Common_Rename = "Common.Rename";
    public const string Common_Select = "Common.Select";
    public const string Common_Unit = "Common.Unit";
    public const string Common_Day = "Common.Day";
    public const string Common_Open = "Common.Open";

    #endregion

    #region 主界面

    public const string Main_Title = "Main.Title";
    public const string Main_File = "Main.File";
    public const string Main_View = "Main.View";
    public const string Main_Tools = "Main.Tools";
    public const string Main_Language = "Main.Language";
    public const string Main_Solution = "Main.Solution";
    public const string Main_NewSolution = "Main.NewSolution";
    public const string Main_OpenSolution = "Main.OpenSolution";
    public const string Main_SaveSolution = "Main.SaveSolution";
    public const string Main_CloseSolution = "Main.CloseSolution";
    
    // 菜单项
    public const string Main_User = "Main.User";
    public const string Main_System = "Main.System";
    public const string Main_SystemParams = "Main.SystemParams";
    public const string Main_FileParams = "Main.FileParams";
    public const string Main_SystemOnline = "Main.SystemOnline";
    public const string Main_SystemOffline = "Main.SystemOffline";
    public const string Main_Chinese = "Main.Chinese";
    public const string Main_English = "Main.English";
    
    // 工具栏
    public const string Main_SolutionList = "Main.SolutionList";
    public const string Main_GlobalVariables = "Main.GlobalVariables";
    public const string Main_StationConfig = "Main.StationConfig";
    public const string Main_CameraHardware = "Main.CameraHardware";
    public const string Main_CommHardware = "Main.CommHardware";
    public const string Main_LightConfig = "Main.LightConfig";
    public const string Main_DeepLearning = "Main.DeepLearning";
    public const string Main_DisplaySettings = "Main.DisplaySettings";
    public const string Main_StrobeTest = "Main.StrobeTest";
    
    // 状态栏
    public const string Main_StatusOnline = "Main.StatusOnline";
    public const string Main_StatusOffline = "Main.StatusOffline";
    public const string Main_NoUser = "Main.NoUser";
    public const string Main_CurrentSolution = "Main.CurrentSolution";
    public const string Main_RunTime = "Main.RunTime";
    public const string Main_MemoryUsage = "Main.MemoryUsage";
    public const string Main_CPUUsage = "Main.CPUUsage";
    public const string Main_DiskRemaining = "Main.DiskRemaining";
    
    // 分组框
    public const string Main_HardwareStatus = "Main.HardwareStatus";
    public const string Main_LogPanel = "Main.LogPanel";

    #endregion

    #region 工位

    public const string Station_Title = "Station.Title";
    public const string Station_Name = "Station.Name";
    public const string Station_Camera = "Station.Camera";
    public const string Station_Light = "Station.Light";
    public const string Station_Detection = "Station.Detection";
    public const string Station_Output = "Station.Output";
    public const string Station_Trigger = "Station.Trigger";
    public const string Station_Add = "Station.Add";
    public const string Station_Delete = "Station.Delete";
    public const string Station_Config = "Station.Config";
    public const string Station_RunDetection_LoadImage = "Station.RunDetection_LoadImage";
    public const string Station_RunDetection_TriggerCamera = "Station.RunDetection_TriggerCamera";
    public const string Station_SimulateFlyCapture = "Station.SimulateFlyCapture";
    public const string Station_OpenConfig = "Station.OpenConfig";

    #endregion

    #region 相机

    public const string Camera_Title = "Camera.Title";
    public const string Camera_SN = "Camera.SN";
    public const string Camera_Manufacturer = "Camera.Manufacturer";
    public const string Camera_Connect = "Camera.Connect";
    public const string Camera_Disconnect = "Camera.Disconnect";
    public const string Camera_Grab = "Camera.Grab";
    public const string Camera_Exposure = "Camera.Exposure";
    public const string Camera_Gain = "Camera.Gain";
    public const string Camera_TriggerMode = "Camera.TriggerMode";
    public const string Camera_SoftTrigger = "Camera.SoftTrigger";
    public const string Camera_HardTrigger = "Camera.HardTrigger";

    #endregion

    #region 光源

    public const string Light_Title = "Light.Title";
    public const string Light_Channel = "Light.Channel";
    public const string Light_Brightness = "Light.Brightness";
    public const string Light_TurnOn = "Light.TurnOn";
    public const string Light_TurnOff = "Light.TurnOff";

    #endregion

    #region 通讯

    public const string Comm_Title = "Comm.Title";
    public const string Comm_Connect = "Comm.Connect";
    public const string Comm_Disconnect = "Comm.Disconnect";
    public const string Comm_Send = "Comm.Send";
    public const string Comm_Receive = "Comm.Receive";
    public const string Comm_IPAddress = "Comm.IPAddress";
    public const string Comm_Port = "Comm.Port";

    #endregion

    #region 用户

    public const string User_Title = "User.Title";
    public const string User_Login = "User.Login";
    public const string User_Logout = "User.Logout";
    public const string User_Register = "User.Register";
    public const string User_Username = "User.Username";
    public const string User_Password = "User.Password";
    public const string User_Permission = "User.Permission";
    public const string User_ChangePassword = "User.ChangePassword";

    #endregion

    #region 日志

    public const string Log_Title = "Log.Title";
    public const string Log_Time = "Log.Time";
    public const string Log_Level = "Log.Level";
    public const string Log_Message = "Log.Message";
    public const string Log_Clear = "Log.Clear";

    #endregion

    #region 文件参数

    public const string File_Title = "File.Title";
    public const string File_SavePath = "File.SavePath";
    public const string File_StorageSettings = "File.StorageSettings";
    public const string File_DiskAlarmSettings = "File.DiskAlarmSettings";
    public const string File_SaveRawImage = "File.SaveRawImage";
    public const string File_SaveResultImage = "File.SaveResultImage";
    public const string File_DeleteImages = "File.DeleteImages";
    public const string File_SeparateOKNG = "File.SeparateOKNG";
    public const string File_RawImageRetention = "File.RawImageRetention";
    public const string File_ResultImageRetention = "File.ResultImageRetention";
    public const string File_RawImageType = "File.RawImageType";
    public const string File_ResultImageType = "File.ResultImageType";
    public const string File_DiskMonitorEnabled = "File.DiskMonitorEnabled";
    public const string File_AlarmThreshold = "File.AlarmThreshold";
    public const string File_CheckTime1 = "File.CheckTime1";
    public const string File_CheckTime2 = "File.CheckTime2";
    public const string File_ThresholdNote = "File.ThresholdNote";

    #endregion

    #region 消息

    public const string Msg_ConfirmDelete = "Msg.ConfirmDelete";
    public const string Msg_SaveSuccess = "Msg.SaveSuccess";
    public const string Msg_SaveFailed = "Msg.SaveFailed";
    public const string Msg_OperationSuccess = "Msg.OperationSuccess";
    public const string Msg_OperationFailed = "Msg.OperationFailed";
    public const string Msg_InvalidInput = "Msg.InvalidInput";
    public const string Msg_ConnectionFailed = "Msg.ConnectionFailed";
    public const string Msg_ConnectionSuccess = "Msg.ConnectionSuccess";
    public const string Msg_PleaseSelectItem = "Msg.PleaseSelectItem";
    public const string Msg_UnsavedChanges = "Msg.UnsavedChanges";
    public const string Msg_ConfirmLogout = "Msg.ConfirmLogout";

    #endregion
}
