using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Cognex.VisionPro;
using DnnInterfaceNet;
using DnnSemanticNet;
using HalconDotNet;

namespace SemanticSegmentationDemo
{
    /// <summary>
    /// 语义分割测试Demo - 使用Halcon显示原图和分割结果
    /// </summary>
    public partial class MainForm : Form
    {
        #region 私有字段

        private DnnSemanticSegmentation _model;
        private Bitmap _currentImage;
        private HObject _resultImage;
        private string _currentImagePath;
        
        // Halcon窗口句柄
        private HWindow _hWndOriginal;
        private HWindow _hWndResult;

        #endregion

        #region 构造函数

        public MainForm()
        {
            InitializeComponent();
            InitializeHalconWindows();
            
            // 初始化模型实例
            _model = new DnnSemanticSegmentation("测试模型");
        }

        #endregion

        #region 界面初始化

        private void InitializeComponent()
        {
            this.Text = "语义分割测试Demo";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1200, 600);
            this.FormClosing += MainForm_FormClosing;

            // 顶部工具栏面板
            var toolPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 115,
                BackColor = Color.FromArgb(45, 45, 48),
                Padding = new Padding(10)
            };
            this.Controls.Add(toolPanel);

            // 模型路径
            var lblModel = new Label
            {
                Text = "模型路径:",
                ForeColor = Color.White,
                Location = new Point(10, 15),
                AutoSize = true
            };
            toolPanel.Controls.Add(lblModel);

            txtModelPath = new TextBox
            {
                Location = new Point(80, 12),
                Width = 400,
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            toolPanel.Controls.Add(txtModelPath);

            var btnSelectModel = new Button
            {
                Text = "...",
                Location = new Point(490, 10),
                Width = 40,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnSelectModel.Click += BtnSelectModel_Click;
            toolPanel.Controls.Add(btnSelectModel);

            var btnLoadModel = new Button
            {
                Text = "加载模型",
                Location = new Point(540, 10),
                Width = 80,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnLoadModel.Click += BtnLoadModel_Click;
            toolPanel.Controls.Add(btnLoadModel);

            // 设备类型
            var lblDevice = new Label
            {
                Text = "设备:",
                ForeColor = Color.White,
                Location = new Point(640, 15),
                AutoSize = true
            };
            toolPanel.Controls.Add(lblDevice);

            cmbDeviceType = new ComboBox
            {
                Location = new Point(680, 12),
                Width = 80,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White
            };
            cmbDeviceType.Items.AddRange(new object[] { "GPU", "CPU" });
            cmbDeviceType.SelectedIndex = 0;
            toolPanel.Controls.Add(cmbDeviceType);

            // 运行时
            var lblRuntime = new Label
            {
                Text = "运行时:",
                ForeColor = Color.White,
                Location = new Point(780, 15),
                AutoSize = true
            };
            toolPanel.Controls.Add(lblRuntime);

            cmbRuntime = new ComboBox
            {
                Location = new Point(840, 12),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White
            };
            cmbRuntime.Items.AddRange(new object[] { "GC", "OpenVINO", "TensorRT" });
            cmbRuntime.SelectedIndex = 0;
            toolPanel.Controls.Add(cmbRuntime);

            // 第二行 - 图像操作
            var lblImage = new Label
            {
                Text = "图像路径:",
                ForeColor = Color.White,
                Location = new Point(10, 50),
                AutoSize = true
            };
            toolPanel.Controls.Add(lblImage);

            txtImagePath = new TextBox
            {
                Location = new Point(80, 47),
                Width = 400,
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            toolPanel.Controls.Add(txtImagePath);

            var btnSelectImage = new Button
            {
                Text = "...",
                Location = new Point(490, 45),
                Width = 40,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnSelectImage.Click += BtnSelectImage_Click;
            toolPanel.Controls.Add(btnSelectImage);

            var btnLoadImage = new Button
            {
                Text = "加载图像",
                Location = new Point(540, 45),
                Width = 80,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnLoadImage.Click += BtnLoadImage_Click;
            toolPanel.Controls.Add(btnLoadImage);

            btnInfer = new Button
            {
                Text = "执行推理",
                Location = new Point(640, 45),
                Width = 80,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Enabled = false
            };
            btnInfer.Click += BtnInfer_Click;
            toolPanel.Controls.Add(btnInfer);

            // 模型状态标签
            lblStatus = new Label
            {
                Text = "模型状态: 未加载",
                ForeColor = Color.Orange,
                Location = new Point(740, 50),
                AutoSize = true
            };
            toolPanel.Controls.Add(lblStatus);

            // 推理时间标签
            lblInferTime = new Label
            {
                Text = "",
                ForeColor = Color.LightGreen,
                Location = new Point(900, 50),
                AutoSize = true
            };
            toolPanel.Controls.Add(lblInferTime);

            // 第三行 - 调试选项和批量推理
            var lblDebugPath = new Label
            {
                Text = "调试保存:",
                ForeColor = Color.White,
                Location = new Point(10, 85),
                AutoSize = true
            };
            toolPanel.Controls.Add(lblDebugPath);

            txtDebugPath = new TextBox
            {
                Location = new Point(80, 82),
                Width = 300,
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugOutput")
            };
            toolPanel.Controls.Add(txtDebugPath);

            var btnSelectDebugPath = new Button
            {
                Text = "...",
                Location = new Point(390, 80),
                Width = 40,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
            btnSelectDebugPath.Click += (s, e) =>
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "选择调试图像保存目录";
                    if (fbd.ShowDialog() == DialogResult.OK)
                        txtDebugPath.Text = fbd.SelectedPath;
                }
            };
            toolPanel.Controls.Add(btnSelectDebugPath);

            chkEnableDebug = new CheckBox
            {
                Text = "启用调试保存",
                ForeColor = Color.White,
                Location = new Point(440, 84),
                AutoSize = true
            };
            toolPanel.Controls.Add(chkEnableDebug);

            // 批量推理参数
            var lblTileSize = new Label
            {
                Text = "分块尺寸:",
                ForeColor = Color.White,
                Location = new Point(570, 85),
                AutoSize = true
            };
            toolPanel.Controls.Add(lblTileSize);

            nudTileWidth = new NumericUpDown
            {
                Location = new Point(640, 82),
                Width = 70,
                Minimum = 64,
                Maximum = 4096,
                Value = 512,
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White
            };
            toolPanel.Controls.Add(nudTileWidth);

            var lblX = new Label
            {
                Text = "x",
                ForeColor = Color.White,
                Location = new Point(715, 85),
                AutoSize = true
            };
            toolPanel.Controls.Add(lblX);

            nudTileHeight = new NumericUpDown
            {
                Location = new Point(730, 82),
                Width = 70,
                Minimum = 64,
                Maximum = 4096,
                Value = 512,
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White
            };
            toolPanel.Controls.Add(nudTileHeight);

            btnBatchInfer = new Button
            {
                Text = "批量推理",
                Location = new Point(820, 80),
                Width = 80,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                Enabled = false
            };
            btnBatchInfer.Click += BtnBatchInfer_Click;
            toolPanel.Controls.Add(btnBatchInfer);

            // 底部状态栏
            var statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(0, 122, 204)
            };
            lblClassInfo = new ToolStripStatusLabel
            {
                Text = "类别信息: 无",
                ForeColor = Color.White
            };
            statusStrip.Items.Add(lblClassInfo);
            this.Controls.Add(statusStrip);

            // 图像显示区域
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 650,
                BackColor = Color.FromArgb(30, 30, 30)
            };
            this.Controls.Add(splitContainer);

            // 左侧 - 原图
            var panelOriginal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30)
            };
            splitContainer.Panel1.Controls.Add(panelOriginal);

            var lblOriginal = new Label
            {
                Text = "原始图像",
                Dock = DockStyle.Top,
                Height = 25,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(45, 45, 48),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelOriginal.Controls.Add(lblOriginal);

            hWindowOriginal = new HWindowControl
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            panelOriginal.Controls.Add(hWindowOriginal);
            hWindowOriginal.BringToFront();

            // 右侧 - 结果图
            var panelResult = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30)
            };
            splitContainer.Panel2.Controls.Add(panelResult);

            var lblResult = new Label
            {
                Text = "分割结果",
                Dock = DockStyle.Top,
                Height = 25,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(45, 45, 48),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelResult.Controls.Add(lblResult);

            hWindowResult = new HWindowControl
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            panelResult.Controls.Add(hWindowResult);
            hWindowResult.BringToFront();

            splitContainer.BringToFront();
        }

        private void InitializeHalconWindows()
        {
            this.Load += (s, e) =>
            {
                try
                {
                    _hWndOriginal = hWindowOriginal.HalconWindow;
                    _hWndResult = hWindowResult.HalconWindow;

                    // 设置默认显示属性
                    _hWndOriginal.SetColor("green");
                    _hWndOriginal.SetLineWidth(2);
                    
                    _hWndResult.SetColor("green");
                    _hWndResult.SetLineWidth(2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"初始化Halcon窗口失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        #endregion

        #region 控件声明

        private TextBox txtModelPath;
        private TextBox txtImagePath;
        private TextBox txtDebugPath;
        private ComboBox cmbDeviceType;
        private ComboBox cmbRuntime;
        private Button btnInfer;
        private Button btnBatchInfer;
        private CheckBox chkEnableDebug;
        private NumericUpDown nudTileWidth;
        private NumericUpDown nudTileHeight;
        private Label lblStatus;
        private Label lblInferTime;
        private ToolStripStatusLabel lblClassInfo;
        private HWindowControl hWindowOriginal;
        private HWindowControl hWindowResult;

        #endregion

        #region 事件处理

        private void BtnSelectModel_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "选择模型文件夹（包含.hdl或优化后的模型文件）";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtModelPath.Text = fbd.SelectedPath;
                }
            }
        }

        private void BtnLoadModel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModelPath.Text))
            {
                MessageBox.Show("请先选择模型路径", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(txtModelPath.Text))
            {
                MessageBox.Show("模型路径不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                lblStatus.Text = "模型状态: 加载中...";
                lblStatus.ForeColor = Color.Yellow;
                Application.DoEvents();

                var deviceType = cmbDeviceType.SelectedIndex == 0 ? DnnDeviceType.GPU : DnnDeviceType.CPU;
                var runtime = GetSelectedRuntime();

                var success = _model.Load(txtModelPath.Text, deviceType, runtime);

                if (success)
                {
                    lblStatus.Text = $"模型状态: 已加载 ({_model.ActualRuntime})";
                    lblStatus.ForeColor = Color.LightGreen;
                    btnInfer.Enabled = _currentImage != null;
                    btnBatchInfer.Enabled = _currentImage != null;
                    UpdateClassInfo();
                    MessageBox.Show($"模型加载成功!\n运行时: {_model.ActualRuntime}\n优化模型: {_model.IsOptimizedModel}", 
                        "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblStatus.Text = "模型状态: 加载失败";
                    lblStatus.ForeColor = Color.Red;
                    btnInfer.Enabled = false;
                    btnBatchInfer.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "模型状态: 加载异常";
                lblStatus.ForeColor = Color.Red;
                btnInfer.Enabled = false;
                MessageBox.Show($"加载模型失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSelectImage_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "图像文件|*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff|所有文件|*.*";
                ofd.Title = "选择测试图像";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtImagePath.Text = ofd.FileName;
                }
            }
        }

        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtImagePath.Text))
            {
                MessageBox.Show("请先选择图像文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(txtImagePath.Text))
            {
                MessageBox.Show("图像文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 释放旧图像
                _currentImage?.Dispose();

                // 读取图像
                //HOperatorSet.ReadImage(out _currentImage, txtImagePath.Text);
                _currentImage = new Bitmap(txtImagePath.Text);
                _currentImagePath = txtImagePath.Text;

                //// 显示图像
                //DisplayImage(_currentImage, _hWndOriginal);

                //// 清空结果
                //_resultImage?.Dispose();
                //_resultImage = null;
                //_hWndResult.ClearWindow();

                btnInfer.Enabled = _model.IsLoaded;
                btnBatchInfer.Enabled = _model.IsLoaded;
                lblInferTime.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnInfer_Click(object sender, EventArgs e)
        {
            if (_currentImage == null || !_model.IsLoaded)
            {
                MessageBox.Show("请先加载模型和图像", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnInfer.Enabled = false;
                lblInferTime.Text = "推理中...";
                Application.DoEvents();

                var sw = System.Diagnostics.Stopwatch.StartNew();
                ICogImage inputImage = new CogImage8Grey(_currentImage);
                // 执行推理
                var result = _model.Infer(inputImage);

                sw.Stop();
                lblInferTime.Text = $"推理耗时: {sw.ElapsedMilliseconds} ms";

                if (result.Success)
                {
                    // 释放旧结果
                    _resultImage?.Dispose();
                    _resultImage = result.ResultImage as HObject;

                    // 显示结果 - 使用伪彩色
                    DisplaySegmentationResult(_resultImage, _hWndResult);
                }
                else
                {
                    MessageBox.Show($"推理失败: {result.ErrorMessage}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"推理异常: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnInfer.Enabled = true;
            }
        }

        private void BtnBatchInfer_Click(object sender, EventArgs e)
        {
            if (_currentImage == null || !_model.IsLoaded)
            {
                MessageBox.Show("请先加载模型和图像", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnBatchInfer.Enabled = false;
                btnInfer.Enabled = false;
                lblInferTime.Text = "批量推理中...";
                Application.DoEvents();

                // 设置调试保存路径
                if (chkEnableDebug.Checked && !string.IsNullOrWhiteSpace(txtDebugPath.Text))
                {
                    _model.DebugSavePath = txtDebugPath.Text;
                    if (!Directory.Exists(txtDebugPath.Text))
                        Directory.CreateDirectory(txtDebugPath.Text);
                }
                else
                {
                    _model.DebugSavePath = null;
                }

                var sw = System.Diagnostics.Stopwatch.StartNew();
                
                // 读取Halcon图像
                HOperatorSet.ReadImage(out var hImage, _currentImagePath.Replace("\\", "/"));
                
                // 执行批量推理
                int tileW = (int)nudTileWidth.Value;
                int tileH = (int)nudTileHeight.Value;
                var result = _model.InferWithTiling(hImage, tileW, tileH, 0, 0);

                sw.Stop();
                lblInferTime.Text = $"批量推理耗时: {sw.ElapsedMilliseconds} ms";

                if (result.Success)
                {
                    // 释放旧结果
                    _resultImage?.Dispose();
                    _resultImage = result.ResultImage as HObject;

                    // 显示结果 - 使用伪彩色
                    DisplaySegmentationResult(_resultImage, _hWndResult);
                    
                    if (chkEnableDebug.Checked)
                    {
                        MessageBox.Show($"批量推理完成！\n调试图像已保存到: {_model.DebugSavePath}", 
                            "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"批量推理失败: {result.ErrorMessage}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                hImage?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"批量推理异常: {ex.Message}\n{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBatchInfer.Enabled = true;
                btnInfer.Enabled = true;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _model?.Dispose();
                _currentImage?.Dispose();
                _resultImage?.Dispose();
            }
            catch { }
        }

        #endregion

        #region 辅助方法

        private DnnRuntime GetSelectedRuntime()
        {
            switch (cmbRuntime.SelectedIndex)
            {
                case 0: return DnnRuntime.GC;
                case 1: return DnnRuntime.OpenVINO;
                case 2: return DnnRuntime.TensorRT;
                default: return DnnRuntime.GC;
            }
        }

        private void UpdateClassInfo()
        {
            if (_model.ClassNames != null && _model.ClassNames.Length > 0)
            {
                var names = new string[_model.ClassNames.Length];
                for (int i = 0; i < _model.ClassNames.Length; i++)
                {
                    names[i] = _model.ClassNames[i].S;
                }
                lblClassInfo.Text = $"类别信息: {string.Join(", ", names)}";
            }
            else
            {
                lblClassInfo.Text = "类别信息: 无";
            }
        }

        private void DisplayImage(HObject image, HWindow hWnd)
        {
            if (image == null || hWnd == null) return;

            try
            {
                HOperatorSet.GetImageSize(image, out var width, out var height);
                HOperatorSet.SetPart(hWnd, 0, 0, height - 1, width - 1);
                HOperatorSet.DispObj(image, hWnd);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"显示图像失败: {ex.Message}");
            }
        }

        private void DisplaySegmentationResult(HObject segImage, HWindow hWnd)
        {
            if (segImage == null || hWnd == null) return;

            try
            {
                HOperatorSet.GetImageSize(segImage, out var width, out var height);
                HOperatorSet.SetPart(hWnd, 0, 0, height - 1, width - 1);

                // 将分割结果转换为伪彩色显示
                // 获取图像的最小最大值
                HOperatorSet.MinMaxGray(segImage, segImage, 0, out var min, out var max, out var _);
                
                int numClasses = (int)max.D + 1;
                var colors = GenerateClassColors(numClasses);
                
                // 创建三通道彩色图像
                HOperatorSet.GenImageConst(out var redChannel, "byte", width, height);
                HOperatorSet.GenImageConst(out var greenChannel, "byte", width, height);
                HOperatorSet.GenImageConst(out var blueChannel, "byte", width, height);

                // 对每个类别着色
                for (int i = 0; i < numClasses && i < colors.GetLength(0); i++)
                {
                    HOperatorSet.Threshold(segImage, out var region, i, i);
                    if (region != null)
                    {
                        HOperatorSet.PaintRegion(region, redChannel, out redChannel, colors[i, 2], "fill");
                        HOperatorSet.PaintRegion(region, greenChannel, out greenChannel, colors[i, 1], "fill");
                        HOperatorSet.PaintRegion(region, blueChannel, out blueChannel, colors[i, 0], "fill");
                        region.Dispose();
                    }
                }

                // 合成RGB图像
                HOperatorSet.Compose3(redChannel, greenChannel, blueChannel, out var colorImage);
                HOperatorSet.DispObj(colorImage, hWnd);

                // 清理
                redChannel?.Dispose();
                greenChannel?.Dispose();
                blueChannel?.Dispose();
                colorImage?.Dispose();
            }
            catch (Exception ex)
            {
                // 如果伪彩色失败，直接显示灰度图
                Console.WriteLine($"显示分割结果异常: {ex.Message}");
                HOperatorSet.DispObj(segImage, hWnd);
            }
        }

        private int[,] GenerateClassColors(int numClasses)
        {
            // 生成类别颜色 (BGR格式)
            var baseColors = new int[,]
            {
                {0, 0, 0},       // 背景 - 黑色
                {255, 0, 0},     // 类别1 - 蓝色
                {0, 255, 0},     // 类别2 - 绿色
                {0, 0, 255},     // 类别3 - 红色
                {255, 255, 0},   // 类别4 - 青色
                {255, 0, 255},   // 类别5 - 紫色
                {0, 255, 255},   // 类别6 - 黄色
                {255, 128, 0},   // 类别7 - 
                {128, 0, 255},   // 类别8 - 
                {0, 128, 255},   // 类别9 -
                {128, 255, 0},   // 类别10 -
                {255, 0, 128}    // 类别11 -
            };

            // 如果需要更多颜色，复制基础颜色
            var result = new int[Math.Max(numClasses, baseColors.GetLength(0)), 3];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                var idx = i % baseColors.GetLength(0);
                result[i, 0] = baseColors[idx, 0];
                result[i, 1] = baseColors[idx, 1];
                result[i, 2] = baseColors[idx, 2];
            }

            return result;
        }

        #endregion
    }
}
