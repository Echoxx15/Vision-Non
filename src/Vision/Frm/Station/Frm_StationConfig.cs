using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Cognex.VisionPro;
using Vision.Manager.CameraManager;
using Vision.Solutions.Models;
using Logger;
using Vision.Frm.Solution;
using Vision.Solutions.WorkFlow;
using System.Collections.Generic;
using System.Xml.Serialization;
using Vision.Solutions.TaskFlow;
using Vision.Frm.Station; // ✅ 添加输出映射窗体的命名空间

namespace Vision.Frm.Process;

public partial class Frm_StationConfig : Form
{
 private BindingList<StationConfig> _models; // 存储模型列表

 public Frm_StationConfig()
 {
  InitializeComponent();
  Load += Frm_StationConfig_Load;
  tree_Station.AfterSelect += Tree_Station_AfterSelect;
  tsm_OpenForm.Click += Tsm_OpenForm_Click;
  tsm_LoadImageRun.Click += Tsm_LoadImageRun_Click;
  tsm_TriggerCameraRun.Click += Tsm_TriggerCameraRun_Click;
  propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
  contextMenuStrip2.Opening += ContextMenuStrip2_Opening;
 }

 private void Frm_StationConfig_Load(object sender, EventArgs e)
 {
  var sol = SolutionManager.Instance.Current;
  if (sol == null) return;
  _models = new BindingList<StationConfig>(sol.Stations);
  RefreshTree();
  if (tree_Station.Nodes.Count > 0) tree_Station.SelectedNode = tree_Station.Nodes[0];
 }

 private void RefreshTree()
 {
  tree_Station.Nodes.Clear();
  foreach (var s in _models)
  {
   var node = new TreeNode(s.Name) { Tag = s };
   tree_Station.Nodes.Add(node);
  }
 }

 private void Tree_Station_AfterSelect(object sender, TreeViewEventArgs e)
 {
  var obj = e.Node?.Tag as StationConfig;
  propertyGrid1.SelectedObject = obj;
 }

 private void ContextMenuStrip2_Opening(object sender, CancelEventArgs e)
 {
  var gi = propertyGrid1.SelectedGridItem;
  if (gi == null || tree_Station.SelectedNode?.Tag is not StationConfig)
  {
   tsm_OpenForm.Enabled = false;
   tsm_OpenForm.Text = "打开配置";
   tsm_OpenForm.Tag = null;
   return;
  }

  var label = gi.Label?.Trim();
  if (string.Equals(label, "检测工具", StringComparison.OrdinalIgnoreCase))
  {
   tsm_OpenForm.Enabled = true;
   tsm_OpenForm.Text = "检测工具配置";
   tsm_OpenForm.Tag = "Detect";
  }
  else if (string.Equals(label, "棋盘格标定工具配置", StringComparison.OrdinalIgnoreCase))
  {
   tsm_OpenForm.Enabled = true;
   tsm_OpenForm.Text = "棋盘格标定工具配置";
   tsm_OpenForm.Tag = "Checkerboard";
  }
  else if (string.Equals(label, "九点标定工具配置", StringComparison.OrdinalIgnoreCase))
  {
   tsm_OpenForm.Enabled = true;
   tsm_OpenForm.Text = "九点标定工具配置";
   tsm_OpenForm.Tag = "NPoint";
  }
  else if (string.Equals(label, "输出映射配置", StringComparison.OrdinalIgnoreCase))
  {
   tsm_OpenForm.Enabled = true;
   tsm_OpenForm.Text = "输出映射配置";
   tsm_OpenForm.Tag = "OutputMapping";
  }
  else
  {
   tsm_OpenForm.Enabled = false;
   tsm_OpenForm.Text = "打开配置";
   tsm_OpenForm.Tag = null;
  }
 }

 private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
 {
  if (tree_Station.SelectedNode?.Tag is not StationConfig st) return;

  var label = e.ChangedItem.Label;

  if (label == "Name" || label == "工位名称")
  {
   tree_Station.SelectedNode.Text = st.Name;
  }
  else if (label == "SN" || label == "相机序列号")
  {
   var cam = CameraManager.Instance.GetAllCameras().Find(c => c.SN == st.SN);
   st.CameraType = cam == null ? string.Empty : cam.Type.ToString();

   if (!string.IsNullOrWhiteSpace(st.SN))
   {
    st.CameraParams ??= new StationCameraParams();
    if (cam is { IsConnected: true })
    {
     st.CameraParams.Exposure = cam.Parameters.ExposureTime;
     st.CameraParams.Gain = cam.Parameters.Gain;
     st.CameraParams.TimeoutMs =3000;
     st.CameraParams.TriggerMode = TriggerMode.软触发;
     st.CameraParams.TriggerCount =1;
     st.CameraParams.Width = cam.Parameters.Width;
     st.CameraParams.Height = cam.Parameters.Height;
    }
   }
   else
   {
    st.CameraParams = null;
   }

   propertyGrid1.Refresh();
  }
  else if (label == "ModelPath" || label == "模型文件夹路径" ||
           label == "RuntimeType" || label == "运行时类型" ||
           label == "bLoadModel" || label == "是否加载模型")
  {
   HandleModelConfigChanged(st, label);
  }
  // ✅ 监听通讯配置变化，自动重新绑定触发
  else if (label == "CommDeviceName" || label == "通讯设备" ||
           label == "TriggerVariableName" || label == "触发变量" ||
           label == "TriggerValue" || label == "触发值")
  {
   try
   {
    // 重新绑定该工位的通讯触发
    TaskFlowManager.Instance.ReloadStation(st.Name);
    LogHelper.Info($"[工位配置] 工位[{st.Name}]通讯配置已更新，已重新绑定触发");
   }
   catch (Exception ex)
   {
    LogHelper.Error(ex, $"[工位配置] 重新绑定工位[{st.Name}]通讯触发失败");
   }
  }
 }

 private void HandleModelConfigChanged(StationConfig st, string changedProperty)
 {
  if (!st.bLoadModel)
  {
   if (st.DLModel != null)
   {
    DLModelLoader.UnloadModel(st);
    MessageBox.Show($"工位[{st.Name}] 模型已卸载", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
   }

   return;
  }

  if (string.IsNullOrWhiteSpace(st.ModelPath))
  {
   MessageBox.Show("请先选择模型文件夹路径", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
   return;
  }

  if (!Directory.Exists(st.ModelPath))
  {
   MessageBox.Show($"模型路径不存在:\n{st.ModelPath}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
   return;
  }

  var result = MessageBox.Show(
   $"检测到模型配置变更：{changedProperty}\n\n" +
   $"工位: {st.Name}\n" +
   $"模型: {st.ModelFolderName}\n" +
   $"运行时: {GetRuntimeDisplayName(st.RuntimeType)}\n\n" +
   "是否立即加载模型？\n\n" +
   "（加载过程可能需要几秒钟，TensorRT首次加载可能较慢）",
   "确认加载模型",
   MessageBoxButtons.YesNo,
   MessageBoxIcon.Question);

  if (result != DialogResult.Yes) return;

  UseWaitCursor = true;
  Application.DoEvents();

  try
  {
   var success = DLModelLoader.LoadModel(st);

   if (success)
   {
    MessageBox.Show(
     $"✓ 模型加载成功！\n\n" +
     $"工位: {st.Name}\n" +
     $"模型: {st.ModelFolderName}\n" +
     $"运行时: {GetRuntimeDisplayName(st.RuntimeType)}",
     "成功",
     MessageBoxButtons.OK,
     MessageBoxIcon.Information);
   }
   else
   {
    MessageBox.Show(
     $"✗ 模型加载失败\n\n" +
     "请查看日志窗口了解详细错误信息。\n\n" +
     "常见问题：\n" +
     "• 模型文件不完整\n" +
     "•运行时与硬件不匹配\n" +
     "• 缺少必要的驱动或库",
     "失败",
     MessageBoxButtons.OK,
     MessageBoxIcon.Warning);
   }
  }
  finally
  {
   UseWaitCursor = false;
  }

  propertyGrid1.Refresh();
 }

 private static string GetRuntimeDisplayName(DLRuntime runtime)
 {
  return runtime switch
  {
   DLRuntime.GC => "GPU运行时",
   DLRuntime.OpenVINO => "OpenVINO 加速",
   DLRuntime.TensorRT => "TensorRT 加速",
   _ => runtime.ToString()
  };
 }

 private void Tsm_LoadImageRun_Click(object sender, EventArgs e)
 {
  if (tree_Station.SelectedNode?.Tag is not StationConfig st)
  {
   MessageBox.Show("请先选择工位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
   return;
  }

  using var ofd = new OpenFileDialog
  {
   Filter = "图片文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff",
   Title = "选择图片",
   Multiselect = true
  };
  if (ofd.ShowDialog() != DialogResult.OK) return;
  foreach (var f in ofd.FileNames)
  {
   try
   {
    if (TaskFlowManager.Instance.TryGetTaskFlow(st.Name, out var flow))
     flow.StartFromImage(f);
   }
   catch
   {
   }
  }
 }

 private void Tsm_TriggerCameraRun_Click(object sender, EventArgs e)
 {
  if (tree_Station.SelectedNode?.Tag is not StationConfig st)
  {
   MessageBox.Show("请先选择工位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
   return;
  }

  if (string.IsNullOrWhiteSpace(st.SN))
  {
   MessageBox.Show($"工位[{st.Name}]未配置相机序列号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
   return;
  }

  if (st.DetectionTool?.ToolBlock == null)
  {
   MessageBox.Show($"工位[{st.Name}]检测工具未配置", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
   return;
  }

  try
  {
   if (TaskFlowManager.Instance.TryGetTaskFlow(st.Name, out var flow))
   {
    flow.Start();
    LogHelper.Info($"手动触发工位[{st.Name}]相机拍照");
    MessageBox.Show($"已触发工位[{st.Name}]相机拍照", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
   }
   else
   {
    MessageBox.Show($"未找到工位[{st.Name}]流程", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
   }
  }
  catch (Exception ex)
  {
   MessageBox.Show($"触发失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
   LogHelper.Error(ex, $"工位[{st.Name}]手动触发相机失败");
  }
 }

 private void tsb_Add_Click(object sender, EventArgs e)
 {
  var idx = _models.Count + 1;
  var name = $"工位{idx}";
  var m = new StationConfig
  {
   Name = name,
   Enable = true,
   SN = string.Empty,
   CameraType = string.Empty,
   bShow = false,
   DisplayName = string.Empty,
   RecoredIndex = 0
  };
  _models.Add(m);
  RefreshTree();

  try
  {
   var sol = SolutionManager.Instance.Current;
   var folder = SolutionManager.GetStationFolder(sol, m);
   if (!string.IsNullOrEmpty(folder)) Directory.CreateDirectory(folder);
   
   // ✅ 为新工位创建TaskFlow（即使没有配置通讯设备）
   TaskFlowManager.Instance.ReloadStation(m.Name);
   LogHelper.Info($"[工位配置] 已为新工位[{m.Name}]创建任务流");
  }
  catch (Exception ex)
  {
   LogHelper.Error(ex, $"[工位配置] 创建工位[{name}]任务流失败");
  }
 }

 private void tsm_Remove_Click(object sender, EventArgs e)
 {
  if (tree_Station.SelectedNode?.Tag is StationConfig st)
  {
   string folder = null;
   try { var sol = SolutionManager.Instance.Current; folder = SolutionManager.GetStationFolder(sol, st); } catch { }

   // ✅ 先从TaskFlowManager中移除
   try
   {
    TaskFlowManager.Instance.RemoveStation(st.Name);
    LogHelper.Info($"[工位配置] 已从TaskFlowManager移除工位[{st.Name}]");
   }
   catch (Exception ex)
   {
    LogHelper.Error(ex, $"[工位配置] 移除工位[{st.Name}]任务流失败");
   }

   _models.Remove(st);
   RefreshTree();

   try { if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder)) Directory.Delete(folder, true); } catch { }
  }
 }

 private void tsm_ReName_Click(object sender, EventArgs e)
 {
  if (tree_Station.SelectedNode?.Tag is not StationConfig st) return;

  using var dlg = new InputBoxForm("重命名", "请输入新名称：", st.Name);
  if (dlg.ShowDialog(this) != DialogResult.OK) return;

  var input = dlg.Value?.Trim();
  if (string.IsNullOrWhiteSpace(input)) return;

  var oldName = st.Name;
  st.Name = input;
  tree_Station.SelectedNode.Text = st.Name;
  propertyGrid1.Refresh();

  try
  {
   var sol = SolutionManager.Instance.Current;
   var root = SolutionManager.GetSolutionFolder(sol);
   if (!string.IsNullOrEmpty(root))
   {
    var oldDir = Path.Combine(root, SafeName(oldName));
    var newDir = Path.Combine(root, SafeName(st.Name));
    if (!string.Equals(oldDir, newDir, StringComparison.OrdinalIgnoreCase))
    {
     if (Directory.Exists(oldDir))
     {
      if (!Directory.Exists(newDir)) Directory.Move(oldDir, newDir);
      else
      {
       foreach (var file in Directory.GetFiles(oldDir))
       {
        var target = Path.Combine(newDir, Path.GetFileName(file));
        try
        {
         if (File.Exists(target)) File.Delete(target);
         File.Move(file, target);
        }
        catch
        {
        }
       }

       foreach (var dir in Directory.GetDirectories(oldDir))
       {
        var target = Path.Combine(newDir, Path.GetFileName(dir));
        try
        {
         if (Directory.Exists(target)) Directory.Delete(target, true);
         Directory.Move(dir, target);
        }
        catch
        {
        }
       }

       try
       {
        Directory.Delete(oldDir, true);
       }
       catch
       {
       }
      }
     }
     else
     {
      Directory.CreateDirectory(newDir);
     }
    }
   }
  }
  catch
  {
  }
 }

 private void Tsm_OpenForm_Click(object sender, EventArgs e)
 {
  var gi = propertyGrid1.SelectedGridItem;
  if (gi == null || tree_Station.SelectedNode?.Tag is not StationConfig) return;
  if (tree_Station.SelectedNode?.Tag is not StationConfig st) return;

  var tag = tsm_OpenForm.Tag as string;
  switch (tag)
  {
   case "Detect":
   {
    using var frm = new Frm_Tool();
    frm.Text = "检测工具配置";
    frm.ToolBlock = st.DetectionTool?.ToolBlock;
    frm.LoadDetection(st.DetectionTool);
    frm.ShowDialog(this);
    try
    {
     SaveToolVpp(st, st.DetectionTool);
    }
    catch
    {
    }

    break;
   }
   case "Checkerboard":
   {
    using var frm = new Frm_Tool();
    frm.Text = "棋盘格标定工具配置";
    frm.ToolBlock = st.CheckerboardTool.ToolBlock;
    frm.ShowDialog(this);
    break;
   }
   case "NPoint":
   {
    using var frm = new Frm_Tool();
    frm.Text = "九点标定工具配置";
    frm.ToolBlock = st.NPointTool.ToolBlock;
    frm.ShowDialog(this);
    break;
   }
   case "OutputMapping":
   {
    // ✅ 打开输出映射配置窗体
    try
    {
     using var frm = new Frm_OutputMapping(st);
     if (frm.ShowDialog(this) == DialogResult.OK)
     {
      // 刷新PropertyGrid显示
      propertyGrid1.Refresh();
      LogHelper.Info($"[工位配置] 工位[{st.Name}]输出映射已更新");
     }
    }
    catch (Exception ex)
    {
     LogHelper.Error(ex, $"[工位配置] 打开工位[{st.Name}]输出映射配置失败");
     MessageBox.Show($"打开输出映射配置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    break;
   }
   default:
    break;
  }
 }

 private static string SafeName(string name)
 {
  if (string.IsNullOrEmpty(name)) return name;
  var invalid = Path.GetInvalidFileNameChars();
  foreach (var c in invalid) name = name.Replace(c, '_');
  return name;
 }

 private void SaveToolVpp(StationConfig st, StationConfig.ToolBase tool)
 {
  try
  {
   var sol = SolutionManager.Instance.Current;
   if (sol == null || tool?.ToolBlock == null) return;
   var path = SolutionManager.GetToolVppPath(sol, st, tool);
   if (string.IsNullOrEmpty(path)) return;
   var dir = Path.GetDirectoryName(path);
   if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
   CogSerializer.SaveObjectToFile(tool.ToolBlock, path);

   var varsPath = SolutionManager.GetToolVarsPath(sol, st, tool);
   if (!string.IsNullOrEmpty(varsPath))
   {
    var list = tool.Vars ?? new List<StationConfig.DetectVarDef>();
    if (list.Count == 0)
    {
     try
     {
      if (File.Exists(varsPath)) File.Delete(varsPath);
     }
     catch
     {
     }
    }
    else
    {
     var vdir = Path.GetDirectoryName(varsPath);
     if (!string.IsNullOrEmpty(vdir)) Directory.CreateDirectory(vdir);
     var ser = new XmlSerializer(typeof(List<StationConfig.DetectVarDef>));
     using var fs = File.Create(varsPath);
     ser.Serialize(fs, list);
    }
   }
  }
  catch
  {
  }
 }

}

internal class InputBoxForm : Form
{
 private readonly TextBox _text;
 public string Value => _text.Text;

 public InputBoxForm(string title, string prompt, string defaultValue = "")
 {
  Text = title;
  Width = 360;
  Height = 150;
  FormBorderStyle = FormBorderStyle.FixedDialog;
  MaximizeBox = false;
  MinimizeBox = false;
  StartPosition = FormStartPosition.CenterParent;
  var lbl = new Label { Left = 12, Top = 12, Width = 320, Text = prompt };
  _text = new TextBox { Left = 12, Top = 36, Width = 320, Text = defaultValue };
  var ok = new Button { Text = "确定", Left = 166, Width = 75, Top = 70, DialogResult = DialogResult.OK };
  var cancel = new Button { Text = "取消", Left = 257, Width = 75, Top = 70, DialogResult = DialogResult.Cancel };
  AcceptButton = ok;
  CancelButton = cancel;
  Controls.AddRange([lbl, _text, ok, cancel]);
 }
}