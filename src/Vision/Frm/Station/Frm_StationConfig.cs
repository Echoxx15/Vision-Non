using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Cognex.VisionPro;
using Logger;
using HardwareCameraNet;
using Vision.Localization;
using Vision.Solutions.Models;
using Vision.Solutions.TaskFlow;

namespace Vision.Frm.Station;

public partial class Frm_StationConfig : Form, ILocalizable
{
 private BindingList<StationConfig> _models;
 private CancellationTokenSource _flyCaptureCts;

 public Frm_StationConfig()
 {
  InitializeComponent();
  Load += Frm_StationConfig_Load;
  tree_Station.AfterSelect += Tree_Station_AfterSelect;
  tree_Station.DrawMode = TreeViewDrawMode.OwnerDrawAll;
  tree_Station.DrawNode += Tree_Station_DrawNode;
  tsm_OpenForm.Click += Tsm_OpenForm_Click;
  tsm_LoadImageRun.Click += Tsm_LoadImageRun_Click;
  tsm_TriggerCameraRun.Click += Tsm_TriggerCameraRun_Click;
  tsm_SimulateFlyCapture.Click += Tsm_SimulateFlyCapture_Click;
  safePropertyGrid1.PropertyValueChanged += safePropertyGrid1_PropertyValueChanged;
  contextMenuStrip2.Opening += ContextMenuStrip2_Opening;
  
  // 订阅语言变更事件
  LanguageService.Instance.LanguageChanged += OnLanguageChanged;
  this.FormClosed += (_, _) => LanguageService.Instance.LanguageChanged -= OnLanguageChanged;
 }
 
 private void OnLanguageChanged(object sender, string languageCode)
 {
  if (IsDisposed) return;
  if (InvokeRequired)
   BeginInvoke(new Action(ApplyLanguage));
  else
   ApplyLanguage();
 }
 
 /// <summary>
 /// 应用当前语言到界面控件
 /// </summary>
 public void ApplyLanguage()
 {
  var lang = LanguageService.Instance;
  
  // 窗体标题
  this.Text = lang.Get("Title");
  
  // 右键菜单项
  tsb_Add.Text = lang.Get("tsb_Add");
  tsm_Remove.Text = lang.Get("tsm_Remove");
  tsm_ReName.Text = lang.Get("tsm_ReName");
  tsm_LoadImageRun.Text = lang.Get("tsm_LoadImageRun");
  tsm_TriggerCameraRun.Text = lang.Get("tsm_TriggerCameraRun");
  tsm_SimulateFlyCapture.Text = lang.Get("tsm_SimulateFlyCapture");
  tsm_OpenForm.Text = lang.Get("tsm_OpenForm");
 }

 private void Tree_Station_DrawNode(object sender, DrawTreeNodeEventArgs e)
 {
  Color selectedBackColor = Color.FromArgb(0, 122, 204);
  Color normalBackColor = Color.FromArgb(45, 45, 48);
  Color textColor = Color.FromArgb(220, 220, 220);
  Color selectedTextColor = Color.White;

  bool isSelected = (e.State & TreeNodeStates.Selected) != 0;
  
  Rectangle bounds = e.Bounds;
  if (bounds is { Width: > 0, Height: > 0 })
  {
   using (var brush = new SolidBrush(isSelected ? selectedBackColor : normalBackColor))
   {
    e.Graphics.FillRectangle(brush, bounds);
   }
   
   var textBounds = new Rectangle(bounds.X + 8, bounds.Y + (bounds.Height - e.Node.TreeView.Font.Height) / 2, 
                                  bounds.Width - 8, bounds.Height);
   TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.TreeView.Font, textBounds, 
                         isSelected ? selectedTextColor : textColor, 
                         TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
  }
 }

 private void Frm_StationConfig_Load(object sender, EventArgs e)
 {
  var sol = SolutionManager.Instance.Current;
  if (sol == null) return;
  _models = new BindingList<StationConfig>(sol.Stations);
  RefreshTree();
  if (tree_Station.Nodes.Count > 0) tree_Station.SelectedNode = tree_Station.Nodes[0];
  
  // 应用当前语言
  ApplyLanguage();
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
  safePropertyGrid1.SelectedObject = obj;
 }

 private void ContextMenuStrip2_Opening(object sender, CancelEventArgs e)
 {
  var gi = safePropertyGrid1.SelectedGridItem;
  if (gi == null || tree_Station.SelectedNode?.Tag is not StationConfig)
  {
   tsm_OpenForm.Enabled = false;
   tsm_OpenForm.Text = "打开配置";
   tsm_OpenForm.Tag = null;
   return;
  }

  var label = gi.Label?.Trim();
  if (string.Equals(label, "检测工具启用", StringComparison.OrdinalIgnoreCase))
  {
   tsm_OpenForm.Enabled = true;
   tsm_OpenForm.Text = "检测工具配置";
   tsm_OpenForm.Tag = "Detect";
  }
  else if (string.Equals(label, "棋盘格标定启用", StringComparison.OrdinalIgnoreCase))
  {
   tsm_OpenForm.Enabled = true;
   tsm_OpenForm.Text = "棋盘格标定工具配置";
   tsm_OpenForm.Tag = "Checkerboard";
  }
  else if (string.Equals(label, "九点标定工具启用", StringComparison.OrdinalIgnoreCase))
  {
   tsm_OpenForm.Enabled = true;
   tsm_OpenForm.Text = "九点标定工具配置";
   tsm_OpenForm.Tag = "NPoint";
  }
  else if (string.Equals(label, "通讯输出配置", StringComparison.OrdinalIgnoreCase))
  {
   tsm_OpenForm.Enabled = true;
   tsm_OpenForm.Text = "通讯输出配置";
   tsm_OpenForm.Tag = "OutputMapping";
  }
  else
  {
   tsm_OpenForm.Enabled = false;
   tsm_OpenForm.Text = "打开配置";
   tsm_OpenForm.Tag = null;
  }
 }

 private void safePropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
 {
  if (tree_Station.SelectedNode?.Tag is not StationConfig st) return;

  var label = e.ChangedItem.Label;

  if (label == "Name" || label == "工位名称")
  {
   tree_Station.SelectedNode.Text = st.Name;
  }
  else if (label == "SN" || label == "相机序列号")
  {
   var cam = CameraFactory.Instance.GetAllCameras().Find(c => c.SN == st.SN);
   st.CameraType = cam == null ? string.Empty : cam.Type.ToString();

   if (!string.IsNullOrWhiteSpace(st.SN))
   {
    st.CameraParams ??= new StationCameraParams();
    if (cam is { IsConnected: true })
    {
     st.CameraParams.Exposure = cam.Parameters.ExposureTime;
     st.CameraParams.Gain = cam.Parameters.Gain;
     st.CameraParams.TimeoutMs = 3000;
     st.CameraParams.TriggerMode = TriggerMode.软触发;
     st.CameraParams.TriggerCount = 1;
     st.CameraParams.Width = cam.Parameters.Width;
     st.CameraParams.Height = cam.Parameters.Height;
    }
   }
   else
   {
    st.CameraParams = null;
   }

   safePropertyGrid1.Refresh();
  }
  else if (label == "CommDeviceName" || label == "通讯设备" ||
           label == "TriggerVariableName" || label == "触发变量" ||
           label == "TriggerValue" || label == "触发值")
  {
   try
   {
    TaskFlowManager.Instance.ReloadStation(st.Name);
    LogHelper.Info($"[工位配置] 工位[{st.Name}]通讯配置已更新，已重新绑定触发");
   }
   catch (Exception ex)
   {
    LogHelper.Error(ex, $"[工位配置] 重新绑定工位[{st.Name}]通讯触发失败");
   }
  }
  else if (label == "Brightness1" || label == "亮度1" || label == "Brightness2" || label == "亮度2" || 
           label == "Channel1" || label == "通道1" || label == "Channel2" || label == "通道2" || 
           label == "LightConfigName" || label == "光源配置")
  {
   try
   {
    var lc = st.LightControl;
    if (lc != null && lc.EnableLightControl && !string.IsNullOrWhiteSpace(lc.LightConfigName))
    {
     LightSource.LightSourceManager.Instance.SetBrightness(lc.LightConfigName, lc.Channel1, lc.Brightness1);
     if (lc.IsMultiChannel)
      LightSource.LightSourceManager.Instance.SetBrightness(lc.LightConfigName, lc.Channel2, lc.Brightness2);
     LogHelper.Info($"[工位配置] 工位[{st.Name}]亮度已写入: {lc.LightConfigName}");
    }
   }
   catch (Exception ex)
   {
    LogHelper.Error(ex, $"[工位配置] 工位[{st.Name}]写入亮度失败");
   }
  }
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
   catch { }
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

 private void Tsm_SimulateFlyCapture_Click(object sender, EventArgs e)
 {
  if (tree_Station.SelectedNode?.Tag is not StationConfig st)
  {
   MessageBox.Show("请先选择工位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
   return;
  }

  using var ofd = new OpenFileDialog
  {
   Filter = "图片路径列表|*.txt",
   Title = "选择包含图片路径的TXT文件"
  };
  
  if (ofd.ShowDialog() != DialogResult.OK) return;
  
  try
  {
   var lines = File.ReadAllLines(ofd.FileName);
   var imagePaths = new List<string>();
   
   foreach (var line in lines)
   {
    var path = line.Trim();
    if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
    {
     imagePaths.Add(path);
    }
   }
   
   if (imagePaths.Count == 0)
   {
    MessageBox.Show("TXT文件中没有找到有效的图片路径", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    return;
   }
   
   _flyCaptureCts?.Cancel();
   _flyCaptureCts = new CancellationTokenSource();
   var token = _flyCaptureCts.Token;
   
   LogHelper.Info($"[模拟飞拍] 工位[{st.Name}]开始模拟飞拍，共{imagePaths.Count}张图片");
   
   Task.Run(async () =>
   {
    int imageIndex = 1;
    foreach (var imagePath in imagePaths)
    {
     if (token.IsCancellationRequested) break;
     
     try
     {
      if (TaskFlowManager.Instance.TryGetTaskFlow(st.Name, out var flow))
      {
       flow.StartFromImage(imagePath, imageIndex);
       LogHelper.Info($"[模拟飞拍] 工位[{st.Name}]执行第{imageIndex}张图片: {Path.GetFileName(imagePath)}");
      }
      
      imageIndex++;
      await Task.Delay(100, token);
     }
     catch (OperationCanceledException) { break; }
     catch (Exception ex)
     {
      LogHelper.Error(ex, $"[模拟飞拍] 工位[{st.Name}]执行图片失败: {imagePath}");
     }
    }
    
    LogHelper.Info($"[模拟飞拍] 工位[{st.Name}]飞拍流程完成");
   }, token);
   
   MessageBox.Show($"已启动模拟飞拍流程，共{imagePaths.Count}张图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
  }
  catch (Exception ex)
  {
   LogHelper.Error(ex, $"[模拟飞拍] 工位[{st.Name}]启动失败");
   MessageBox.Show($"启动模拟飞拍失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
     if(MessageBox.Show("是否移除工位?","",MessageBoxButtons.YesNo) != DialogResult.Yes)
         return;
     if (tree_Station.SelectedNode?.Tag is StationConfig st)
     {
         string folder = null;
         try { var sol = SolutionManager.Instance.Current; folder = SolutionManager.GetStationFolder(sol, st); } catch { }

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
  safePropertyGrid1.Refresh();

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
        try { if (File.Exists(target)) File.Delete(target); File.Move(file, target); } catch { }
       }
       foreach (var dir in Directory.GetDirectories(oldDir))
       {
        var target = Path.Combine(newDir, Path.GetFileName(dir));
        try { if (Directory.Exists(target)) Directory.Delete(target, true); Directory.Move(dir, target); } catch { }
       }
       try { Directory.Delete(oldDir, true); } catch { }
      }
     }
     else
     {
      Directory.CreateDirectory(newDir);
     }
    }
   }
  }
  catch { }
 }

 private void Tsm_OpenForm_Click(object sender, EventArgs e)
 {
  var gi = safePropertyGrid1.SelectedGridItem;
  if (gi == null || tree_Station.SelectedNode?.Tag is not StationConfig) return;
  if (tree_Station.SelectedNode?.Tag is not StationConfig st) return;

  var tag = tsm_OpenForm.Tag as string;
  switch (tag)
  {
   case "Detect":
   {
    using var frm = new Frm_Tool();
    frm.Text = "检测工具配置";
    frm.LoadDetection(st.DetectionTool, st);
    frm.ShowDialog(this);
    break;
   }
   case "Checkerboard":
   {
    using var frm = new Frm_Tool();
    frm.Text = "棋盘格标定工具配置";
    frm.LoadDetection(st.CheckerboardTool, st);
    frm.ShowDialog(this);
    break;
   }
   case "NPoint":
   {
    using var frm = new Frm_Tool();
    frm.Text = "九点标定工具配置";
    frm.LoadDetection(st.NPointTool, st);
    frm.ShowDialog(this);
    break;
   }
   case "OutputMapping":
   {
    try
    {
     using var frm = new Frm_OutputMapping(st);
     if (frm.ShowDialog(this) == DialogResult.OK)
     {
      safePropertyGrid1.Refresh();
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
  }
 }

 private static string SafeName(string name)
 {
  if (string.IsNullOrEmpty(name)) return name;
  var invalid = Path.GetInvalidFileNameChars();
  foreach (var c in invalid) name = name.Replace(c, '_');
  return name;
 }

 private void SaveToolVpp(StationConfig st, StationConfig.Tools tool)
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
     try { if (File.Exists(varsPath)) File.Delete(varsPath); } catch { }
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
  catch { }
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
