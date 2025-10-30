using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Cognex.VisionPro;
using Vision.Manager.CameraManager;
using Vision.Solutions.Models;
using Logger;
using Vision.Frm.Solution;
using Vision.Solutions.WorkFlow; // 添加Frm_Tool和Frm_Camera的命名空间

namespace Vision.Frm.Process;

public partial class Frm_StationConfig : Form
{
  private BindingList<ProcessStation> _models; // 存储模型列表

  public Frm_StationConfig()
  {
    InitializeComponent();
    Load += Frm_StationConfig_Load;
    tree_Station.AfterSelect += Tree_Station_AfterSelect;
    tsm_OpenForm.Click += Tsm_OpenForm_Click;
    tsm_LoadImageRun.Click += Tsm_LoadImageRun_Click;
    tsm_TriggerCameraRun.Click += Tsm_TriggerCameraRun_Click;  // 新增
    propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
    contextMenuStrip2.Opening += ContextMenuStrip2_Opening;
    
    // 禁用添加/删除/重命名按钮
    try
    {
      if (tsb_Add != null) tsb_Add.Enabled = false;
      if (tsm_Remove != null) tsm_Remove.Enabled = false;
      if (tsm_ReName != null) tsm_ReName.Enabled = false;
    }
    catch { }
  }

  private void Tsm_LoadImageRun_Click(object sender, EventArgs e)
  {
    if (tree_Station.SelectedNode?.Tag is not ProcessStation st)
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
    var files = ofd.FileNames;
    foreach (var f in files)
    {
      try { WorkFlow.Instance.ManualRun(f, st.Name); }
      catch { }
  }
  }

  /// <summary>
  /// 运行检测-触发相机
  /// </summary>
  private void Tsm_TriggerCameraRun_Click(object sender, EventArgs e)
  {
    if (tree_Station.SelectedNode?.Tag is not ProcessStation st)
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
      // 调用 WorkFlow 的手动触发相机方法
      WorkFlow.Instance.ManualRun(st.Name);
      LogHelper.Info($"手动触发工位[{st.Name}]相机拍照");
      MessageBox.Show($"已触发工位[{st.Name}]相机拍照", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
      MessageBox.Show($"触发失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
   LogHelper.Error(ex, $"工位[{st.Name}]手动触发相机失败");
    }
  }

  private void Frm_StationConfig_Load(object sender, EventArgs e)
  {
    var sol = SolutionManager.Instance.Current;
    if (sol == null) return;
    _models = new BindingList<ProcessStation>(sol.Stations);
    RefreshTree();
    if (tree_Station.Nodes.Count > 0) tree_Station.SelectedNode = tree_Station.Nodes[0];
  }

  private void ContextMenuStrip2_Opening(object sender, CancelEventArgs e)
  {
    var gi = propertyGrid1.SelectedGridItem;
    if (gi == null || tree_Station.SelectedNode?.Tag is not ProcessStation)
    {
      tsm_OpenForm.Enabled = false;
      tsm_OpenForm.Text = "打开配置";
      tsm_OpenForm.Tag = null;
      return;
    }
    var label = gi.Label?.Trim();
    if (string.Equals(label, "检测工具", StringComparison.OrdinalIgnoreCase))
    {
      tsm_OpenForm.Enabled = true; tsm_OpenForm.Text = "检测工具配置"; tsm_OpenForm.Tag = "Detect";
    }
    else if (string.Equals(label, "棋盘格标定工具", StringComparison.OrdinalIgnoreCase))
    {
      tsm_OpenForm.Enabled = true; tsm_OpenForm.Text = "棋盘格标定工具配置"; tsm_OpenForm.Tag = "Checkerboard";
    }
    else if (string.Equals(label, "九点标定工具", StringComparison.OrdinalIgnoreCase))
    {
      tsm_OpenForm.Enabled = true; tsm_OpenForm.Text = "九点标定工具配置"; tsm_OpenForm.Tag = "NPoint";
    }
    else if (label == "相机类型")
    {
      tsm_OpenForm.Enabled = true; tsm_OpenForm.Text = "打开相机参数"; tsm_OpenForm.Tag = "CameraParams";
    }
    else { tsm_OpenForm.Enabled = false; tsm_OpenForm.Text = "打开配置"; tsm_OpenForm.Tag = null; }
  }

  private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
  {
    if (tree_Station.SelectedNode?.Tag is not ProcessStation st) return;
      
    var label = e.ChangedItem.Label;
  
    if (label == "Name" || label == "工位名称")
    {
      tree_Station.SelectedNode.Text = st.Name;
    }
    else if (label == "SN" || label == "相机序列号")
    {
      // 更新相机类型
      var cam = CameraManager.Instance.GetAllCameras().Find(c => c.SN == st.SN);
      st.CameraType = cam == null ? string.Empty : cam.Type.ToString();

      // 选择了有效SN时，创建并初始化CameraParams；未选择则不实例化
      if (!string.IsNullOrWhiteSpace(st.SN))
      {
        st.CameraParams ??= new StationCameraParams();
        if (cam is { IsConnected: true })
        {
          st.CameraParams.Exposure = cam.Parameters.ExposureTime;
          st.CameraParams.Gain = cam.Parameters.Gain;
          st.CameraParams.TimeoutMs = 3000;
          st.CameraParams.TriggerMode = TriggerMode.Software;
          st.CameraParams.TriggerCount = 1;
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
      // 模型相关配置变更，立即加载模型
      HandleModelConfigChanged(st, label);
    }
  }

  /// <summary>
  /// 处理模型配置变更
  /// </summary>
  private void HandleModelConfigChanged(ProcessStation st, string changedProperty)
  {
    // 如果禁用了模型加载，卸载当前模型
    if (!st.bLoadModel)
    {
      if (st.DLModel != null)
      {
        DLModelLoader.UnloadModel(st);
        MessageBox.Show($"工位[{st.Name}] 模型已卸载", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      return;
    }

    // 检查模型路径是否有效
    if (string.IsNullOrWhiteSpace(st.ModelPath))
    {
      MessageBox.Show("请先选择模型文件夹路径", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    if (!System.IO.Directory.Exists(st.ModelPath))
    {
      MessageBox.Show($"模型路径不存在:\n{st.ModelPath}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
      return;
    }

    // 询问用户是否立即加载模型
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

    // 显示加载提示
    UseWaitCursor = true;
    Application.DoEvents();

    try
    {
      // 调用模型加载器
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
          "• 运行时与硬件不匹配\n" +
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

  /// <summary>
  /// 获取运行时的显示名称
  /// </summary>
  private static string GetRuntimeDisplayName(DLRuntime runtime)
  {
    return runtime switch
    {
      DLRuntime.GC => "GPU 运行时",
      DLRuntime.OpenVINO => "OpenVINO 加速",
      DLRuntime.TensorRT => "TensorRT 加速",
      _ => runtime.ToString()
    };
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
    propertyGrid1.SelectedObject = e.Node?.Tag;
  }

  private void Tsb_Add_Click(object sender, EventArgs e)
  {
    var idx = _models.Count + 1;
    var name = $"工位{idx}";
    var m = new ProcessStation
    {
      Name = name,
      Enable = true,
      SN = string.Empty,
      CameraType = string.Empty,
      // CameraParams 在选择SN后再创建
      bShow = false,
      DisplayName = string.Empty,
      RecoredIndex = 0
    };
    _models.Add(m);
    RefreshTree();

    // 确保创建对应的工位文件夹
    try
    {
      var sol = SolutionManager.Instance.Current;
      var folder = SolutionManager.GetStationFolder(sol, m);
      if (!string.IsNullOrEmpty(folder)) Directory.CreateDirectory(folder);
    }
    catch { }
  }

  private void Tsm_Remove_Click(object sender, EventArgs e)
  {
    if (tree_Station.SelectedNode?.Tag is ProcessStation st)
    {
      // 删除前记录路径
      string folder = null;
      try { var sol = SolutionManager.Instance.Current; folder = SolutionManager.GetStationFolder(sol, st); } catch { }

      _models.Remove(st);
      RefreshTree();

      // 删除对应的工位文件夹（可选）
      try { if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder)) Directory.Delete(folder, true); } catch { }
    }
  }

  private void Tsm_ReName_Click(object sender, EventArgs e)
  {
    if (tree_Station.SelectedNode?.Tag is ProcessStation st)
    {
      using var dlg = new InputBoxForm("重命名", "请输入新名称：", st.Name);
      if (dlg.ShowDialog(this) == DialogResult.OK)
      {
        var input = dlg.Value?.Trim();
        if (!string.IsNullOrWhiteSpace(input))
        {
          var oldName = st.Name;
          st.Name = input;
          tree_Station.SelectedNode.Text = st.Name;
          propertyGrid1.Refresh();

          // 重命名对应的工位文件夹
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
                  // 若新目录已存在，则合并（将旧目录内容移动到新目录）
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
                  // 旧目录不存在则直接确保新目录存在
                  Directory.CreateDirectory(newDir);
                }
              }
            }
          }
          catch { }
        }
      }
    }
  }

  private void Tsm_OpenForm_Click(object sender, EventArgs e)
  {
    var gi = propertyGrid1.SelectedGridItem;
    if (gi == null || tree_Station.SelectedNode?.Tag is not ProcessStation) return;
    if (tree_Station.SelectedNode?.Tag is not ProcessStation st) return;

    var tag = tsm_OpenForm.Tag as string;
    switch (tag)
    {
      case "Detect":
      {
        using var frm = new Frm_Tool(false);
        frm.Text = "检测工具配置";
        frm.ToolBlock = st.DetectionTool?.ToolBlock;
        frm.LoadDetection(st.DetectionTool);
        frm.ShowDialog(this);
        // 窗口关闭时已自动同步 ToolBlock 到内存，无需立即保存到磁盘
        break;
      }
      case "Checkerboard":
      {
        using var frm = new Frm_Tool();
        frm.Text = "棋盘格标定工具配置";
        frm.ToolBlock = st.CheckerboardTool.ToolBlock;
        frm.ShowDialog(this);
        // 窗口关闭时已自动同步 ToolBlock 到内存，无需立即保存到磁盘
        break;
      }
      case "NPoint":
      {
        using var frm = new Frm_Tool();
        frm.Text = "九点标定工具配置";
        frm.ToolBlock = st.NPointTool.ToolBlock;
        frm.ShowDialog(this);
        // 窗口关闭时已自动同步 ToolBlock 到内存，无需立即保存到磁盘
        break;
      }
      case "CameraParams":
        // 未选择SN时禁止打开相机参数，避免实例化
        if (string.IsNullOrWhiteSpace(st.SN))
        {
          MessageBox.Show("请先选择相机序列号");
          return;
        }

        var cam = CameraManager.Instance.GetAllCameras().Find(c => c.SN == st.SN);

        // 在打开参数前，若还未实例化则根据配置创建默认参数
        st.CameraParams ??= new StationCameraParams
        {
          Exposure = cam != null ? cam.Parameters.ExposureTime : 0,
          Gain = cam != null ? cam.Parameters.Gain : 0,
          TimeoutMs = 3000,
          TriggerMode = TriggerMode.Software,
          TriggerCount = 1,
          Width = cam != null ? cam.Parameters.Width : 0,
          Height = cam != null ? cam.Parameters.Height : 0
        };

        using (var frm = new Frm_CameraConfig())
        {
          frm.Init(st.CameraParams, cam);
          if (frm.ShowDialog(this) == DialogResult.OK)
          {
            // 用户点击确定后，立即应用参数到相机
            try
            {
              WorkFlow.Instance.ApplyCameraParameters(st.Name);
              LogHelper.Info($"工位[{st.Name}]相机参数已应用");
            }
            catch (Exception ex)
            {
              LogHelper.Error(ex, $"应用工位[{st.Name}]相机参数失败");
              MessageBox.Show($"应用相机参数失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          }
        }

        // 刷新属性显示
        propertyGrid1.Refresh();
        break;
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

  private void SaveToolVpp(ProcessStation st, ProcessStation.ToolBase tool)
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