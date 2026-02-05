using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DnnInterfaceNet;
using HalconDotNet;

namespace DnnSemanticNet;

/// <summary>
/// 语义分割模型插件 - 实现 IDnnModel 接口
/// </summary>
[DnnModelType("语义分割", "基于深度学习的语义分割模型，支持像素级分类")]
public class DnnSemanticSegmentation : IDnnModel, IRenameableDnnModel, IConfigurableDnnModel, IOptimizableDnnModel, IBatchInferableDnnModel
{
    #region 私有字段

    private string _name;
    private readonly object _lock = new();

    // Halcon 句柄
    private HTuple _modelHandle;
    private HTuple _preprocessParam;
    private HTuple _deviceHandle;
    private HTuple _deviceHandles;
    private HTuple _classNames;
    private HTuple _classIDs;
    private HTuple _datasetInfo;

    // 配置参数
    private DnnDeviceType _deviceType = DnnDeviceType.GPU;
    private DnnRuntime _runtime = DnnRuntime.GC;
    private bool _loadOnStartup;

    /// <summary>
    /// 调试模式：保存中间结果图像的路径，为空则不保存
    /// </summary>
    public string DebugSavePath { get; set; }

    #endregion

    #region 属性

    public string Name => _name;
    public bool IsLoaded { get; private set; }
    public string ModelPath { get; private set; }
    
    /// <summary>
    /// 当前加载的模型文件完整路径
    /// </summary>
    public string LoadedModelFile { get; private set; }
    
    /// <summary>
    /// 是否为优化后的模型
    /// </summary>
    public bool IsOptimizedModel { get; private set; }
    
    /// <summary>
    /// 当前模型的运行时类型（实际加载的）
    /// </summary>
    public string ActualRuntime { get; private set; }
    
    /// <summary>
    /// 是否在启动时自动加载
    /// </summary>
    public bool LoadOnStartup
    {
        get => _loadOnStartup;
        set => _loadOnStartup = value;
    }

    /// <summary>
    /// 类别名称数组
    /// </summary>
    public HTuple ClassNames => _classNames;

    /// <summary>
    /// 类别ID数组
    /// </summary>
    public HTuple ClassIDs => _classIDs;

    /// <summary>
    /// 数据集信息
    /// </summary>
    public HTuple DatasetInfo => _datasetInfo;

    /// <summary>
    /// 模型句柄（供外部高级使用）
    /// </summary>
    public HTuple ModelHandle => _modelHandle;

    /// <summary>
    /// 预处理参数
    /// </summary>
    public HTuple PreprocessParam => _preprocessParam;

    #endregion

    #region 构造函数

    public DnnSemanticSegmentation(string name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    #endregion

    #region IDnnModel 实现

    public UserControl GetConfigControl()
    {
        // 返回配置界面控件
        return new SemanticConfigControl(this);
    }

    public bool Load(string modelPath, DnnDeviceType deviceType, DnnRuntime runtime)
    {
        lock (_lock)
        {
            try
            {
                Unload();

                ModelPath = modelPath;
                _deviceType = deviceType;
                _runtime = runtime;

                // 查询可用设备
                if (!QueryAvailableDevices(deviceType, runtime))
                {
                    Console.WriteLine($"[{_name}] 查询设备失败");
                    return false;
                }

                // 扫描并读取模型文件
                if (!TryFindAndReadModel(modelPath))
                {
                    Console.WriteLine($"[{_name}] 读取模型文件失败");
                    return false;
                }

                // 设置设备
                SelectDevice(_deviceHandles, deviceType.ToString());

                IsLoaded = true;
                Console.WriteLine($"[{_name}] 模型加载成功: {Path.GetFileName(modelPath)}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_name}] 加载模型失败: {ex.Message}");
                Unload();
                return false;
            }
        }
    }

    public void Unload()
    {
        lock (_lock)
        {
            try
            {
                _modelHandle?.Dispose();
                _preprocessParam?.Dispose();
                _deviceHandle?.Dispose();
                _deviceHandles?.Dispose();
                _classNames?.Dispose();
                _classIDs?.Dispose();
                _datasetInfo?.Dispose();
            }
            catch { }
            finally
            {
                _modelHandle = new HTuple();
                _preprocessParam = new HTuple();
                _deviceHandle = new HTuple();
                _deviceHandles = new HTuple();
                _classNames = new HTuple();
                _classIDs = new HTuple();
                _datasetInfo = new HTuple();
                IsLoaded = false;
            }
        }
    }

    public DnnResult Infer(object imageObj)
    {
        lock (_lock)
        {
            var result = new DnnResult();

            if (!IsLoaded)
            {
                result.Success = false;
                result.ErrorMessage = "模型未加载";
                return result;
            }

            HalconOperate.Operate.ImageConvertVisionPro2HObject(imageObj, out var hoImage);
            if (!(hoImage is HObject image))
            {
                result.Success = false;
                result.ErrorMessage = "输入图像必须是 HalconDotNet.HObject 类型";
                return result;
            }

            HTuple dlSampleBatch = null;
            HTuple dlResultBatch = null;

            try
            {
                // 获取图像尺寸
                HOperatorSet.GetImageSize(image, out var width, out var height);

                // 生成样本
                GenDlSamplesFromImages(image, out dlSampleBatch);
                PreprocessDlSamples(dlSampleBatch, _preprocessParam);

                // 推理
                HOperatorSet.ApplyDlModel(_modelHandle, dlSampleBatch, new HTuple(), out dlResultBatch);

                // 处理结果
                var segImage = dlResultBatch.TupleSelect(0).TupleGetDictObject("segmentation_image");
                
                // 转换为映射图像
                HOperatorSet.GetImageSize(segImage, out var segWidth, out var segHeight);
                HOperatorSet.GenImageConst(out var mapImage, "byte", segWidth, segHeight);

                for (int j = 0; j < _classIDs.Length; j++)
                {
                    HOperatorSet.Threshold(segImage, out var classRegion, _classIDs[j], _classIDs[j]);
                    HOperatorSet.PaintRegion(classRegion, mapImage, out mapImage, _classIDs[j], "fill");
                    classRegion?.Dispose();
                }

                // 缩放到原始尺寸
                HOperatorSet.ZoomImageSize(mapImage, out var resultImage, width, height, "constant");

                result.Success = true;
                result.ResultImage = resultImage;

                // 收集类别信息
                result.ClassIDs = new int[_classIDs.Length];
                result.ClassNames = new string[_classNames.Length];
                for (int i = 0; i < _classIDs.Length; i++)
                {
                    result.ClassIDs[i] = _classIDs[i].I;
                    result.ClassNames[i] = _classNames[i].S;
                }

                segImage?.Dispose();
                mapImage?.Dispose();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                dlSampleBatch?.Dispose();
                dlResultBatch?.Dispose();
            }

            return result;
        }
    }

    public void Dispose()
    {
        Unload();
    }

    #endregion

    #region 批量推理方法

    /// <summary>
    /// 批量推理（分块拼接模式）- 实现 IBatchInferableDnnModel 接口
    /// </summary>
    public DnnResult InferBatch(object imageObj, int[] coordArray1, int[] coordArray2, 
        int cropWidth, int cropHeight, 
        CoordinateFormat format = CoordinateFormat.XY,
        List<object> maskRegions = null)
    {
        if (!(imageObj is HObject image))
        {
            return new DnnResult { Success = false, ErrorMessage = "输入图像必须是 HalconDotNet.HObject 类型" };
        }

        // 根据坐标格式转换为Halcon的行列格式（Row, Column）
        int[] rows, cols;
        if (format == CoordinateFormat.XY)
        {
            // XY格式：coordArray1=X(列), coordArray2=Y(行)
            // 转换为Halcon格式：rows=Y, cols=X
            cols = coordArray1;
            rows = coordArray2;
        }
        else
        {
            // RowColumn格式：coordArray1=Row(行), coordArray2=Column(列)
            // 已经是Halcon格式
            rows = coordArray1;
            cols = coordArray2;
        }

        // 转换遮罩区域
        List<HObject> halconMasks = null;
        if (maskRegions != null && maskRegions.Count > 0)
        {
            halconMasks = new List<HObject>();
            foreach (var mask in maskRegions)
            {
                if (mask is HObject hMask)
                    halconMasks.Add(hMask);
            }
        }

        return InferBatchInternal(image, cols, rows, cropWidth, cropHeight, halconMasks);
    }

    /// <summary>
    /// 使用网格自动分块推理大图 - 实现 IBatchInferableDnnModel 接口
    /// </summary>
    public DnnResult InferWithTiling(object imageObj, int tileWidth, int tileHeight, 
        int overlapX = 0, int overlapY = 0, List<object> maskRegions = null)
    {
        if (!(imageObj is HObject image))
        {
            return new DnnResult { Success = false, ErrorMessage = "输入图像必须是 HalconDotNet.HObject 类型" };
        }

        // 转换遮罩区域
        List<HObject> halconMasks = null;
        if (maskRegions != null && maskRegions.Count > 0)
        {
            halconMasks = new List<HObject>();
            foreach (var mask in maskRegions)
            {
                if (mask is HObject hMask)
                    halconMasks.Add(hMask);
            }
        }

        return InferWithTilingInternal(image, tileWidth, tileHeight, overlapX, overlapY, halconMasks);
    }

    /// <summary>
    /// 批量推理内部实现（使用Halcon的列、行格式）
    /// </summary>
    /// <param name="image">图像</param>
    /// <param name="colCoords">列坐标数组（X）</param>
    /// <param name="rowCoords">行坐标数组（Y）</param>
    /// <param name="cropWidth">裁剪宽度</param>
    /// <param name="cropHeight">裁剪高度</param>
    /// <param name="maskRegions">遮罩区域</param>
    private DnnResult InferBatchInternal(HObject image, int[] colCoords, int[] rowCoords, 
        int cropWidth, int cropHeight, List<HObject> maskRegions = null)
    {
        lock (_lock)
        {
            var result = new DnnResult();
            if (!IsLoaded) { result.Success = false; result.ErrorMessage = "模型未加载"; return result; }
            if (image == null) { result.Success = false; result.ErrorMessage = "输入图像不能为空"; return result; }
            if (colCoords == null || rowCoords == null || colCoords.Length == 0 || colCoords.Length != rowCoords.Length)
            { result.Success = false; result.ErrorMessage = "坐标数组无效"; return result; }

            HObject croppedImages = null;
            HTuple dlSampleBatch = null;
            HObject[] inferredImages = null;
            try
            {
                HOperatorSet.GetImageSize(image, out var origWidth, out var origHeight);
                
                // 裁剪所有子图像
                croppedImages = CropImagesFromSource(image, colCoords, rowCoords, cropWidth, cropHeight);
                
                // 调试模式：保存裁剪的小图（路径为空时使用默认路径）
                SaveDebugCroppedImages(croppedImages, colCoords, rowCoords);
                
                // 为每张图像创建样本字典（关键修复！）
                GenDlSampleBatchFromImages(croppedImages, out dlSampleBatch);
                
                // 预处理所有样本
                PreprocessDlSampleBatch(dlSampleBatch, _preprocessParam);
                
                // 批量推理（传入坐标信息用于调试保存）
                if (!InferBatchCore(dlSampleBatch, cropWidth, cropHeight, out inferredImages, maskRegions, colCoords, rowCoords))
                { 
                    result.Success = false; 
                    result.ErrorMessage = "批量推理失败"; 
                    return result; 
                }
                
                // 拼接结果
                result.ResultImage = TileImagesToResult(inferredImages, colCoords, rowCoords, origWidth.I, origHeight.I);
                result.Success = true;
                result.ClassIDs = new int[_classIDs.Length];
                result.ClassNames = new string[_classNames.Length];
                for (int i = 0; i < _classIDs.Length; i++) 
                { 
                    result.ClassIDs[i] = _classIDs[i].I; 
                    result.ClassNames[i] = _classNames[i].S; 
                }
                return result;
            }
            catch (Exception ex) 
            { 
                result.Success = false; 
                result.ErrorMessage = $"批量推理异常: {ex.Message}"; 
                Console.WriteLine($"[{_name}] 批量推理异常: {ex.Message}\n{ex.StackTrace}");
                return result; 
            }
            finally 
            { 
                croppedImages?.Dispose(); 
                DisposeSampleBatch(dlSampleBatch);
                DisposeImageArray(inferredImages); 
            }
        }
    }

    /// <summary>
    /// 自动分块推理内部实现
    /// </summary>
    private DnnResult InferWithTilingInternal(HObject image, int tileWidth, int tileHeight, 
        int overlapX = 0, int overlapY = 0, List<HObject> maskRegions = null)
    {
        lock (_lock)
        {
            var result = new DnnResult();
            if (!IsLoaded) { result.Success = false; result.ErrorMessage = "模型未加载"; return result; }
            if (image == null) { result.Success = false; result.ErrorMessage = "输入图像不能为空"; return result; }
            try
            {
                HOperatorSet.GetImageSize(image, out var origWidth, out var origHeight);
                int imgWidth = origWidth.I, imgHeight = origHeight.I;
                if (imgWidth <= tileWidth && imgHeight <= tileHeight) return Infer(image);
                int stepX = Math.Max(1, tileWidth - overlapX), stepY = Math.Max(1, tileHeight - overlapY);
                var colCoords = new List<int>(); var rowCoords = new List<int>();
                for (int y = 0; y <= imgHeight - tileHeight; y += stepY)
                {
                    for (int x = 0; x <= imgWidth - tileWidth; x += stepX) { colCoords.Add(x); rowCoords.Add(y); }
                    if (imgWidth > tileWidth && (colCoords.Count == 0 || colCoords[colCoords.Count - 1] != imgWidth - tileWidth))
                    { colCoords.Add(imgWidth - tileWidth); rowCoords.Add(y); }
                }
                if (imgHeight > tileHeight && (rowCoords.Count == 0 || rowCoords[rowCoords.Count - 1] != imgHeight - tileHeight))
                {
                    int y = imgHeight - tileHeight;
                    for (int x = 0; x <= imgWidth - tileWidth; x += stepX) { colCoords.Add(x); rowCoords.Add(y); }
                    if (imgWidth > tileWidth) { colCoords.Add(imgWidth - tileWidth); rowCoords.Add(y); }
                }
                if (colCoords.Count == 0) return Infer(image);
                Console.WriteLine($"[{_name}] 分块推理: {imgWidth}x{imgHeight}, 块{tileWidth}x{tileHeight}, 共{colCoords.Count}块");
                return InferBatchInternal(image, colCoords.ToArray(), rowCoords.ToArray(), tileWidth, tileHeight, maskRegions);
            }
            catch (Exception ex) { result.Success = false; result.ErrorMessage = $"分块推理异常: {ex.Message}"; return result; }
        }
    }

    #endregion

    #region 批量推理辅助方法

    /// <summary>
    /// 从原图裁剪多个子图像
    /// </summary>
    /// <param name="src">源图像</param>
    /// <param name="colCoords">列坐标数组（X）</param>
    /// <param name="rowCoords">行坐标数组（Y）</param>
    /// <param name="w">宽度</param>
    /// <param name="h">高度</param>
    private HObject CropImagesFromSource(HObject src, int[] colCoords, int[] rowCoords, int w, int h)
    {
        HObject result = new HObject(); result.GenEmptyObj();
        for (int i = 0; i < colCoords.Length; i++)
        {
            HObject rect = null, reduced = null, cropped = null;
            try
            {
                // Halcon GenRectangle1 参数顺序: Row1, Column1, Row2, Column2
                int row1 = rowCoords[i];
                int col1 = colCoords[i];
                int row2 = row1 + h - 1;
                int col2 = col1 + w - 1;
                HOperatorSet.GenRectangle1(out rect, row1, col1, row2, col2);
                HOperatorSet.ReduceDomain(src, rect, out reduced);
                HOperatorSet.CropDomain(reduced, out cropped);
                var temp = result.ConcatObj(cropped); result.Dispose(); result = temp;
            }
            finally { rect?.Dispose(); reduced?.Dispose(); }
        }
        return result;
    }

    /// <summary>
    /// 从多张图像生成DL样本批次（每张图像一个样本字典）
    /// </summary>
    /// <param name="images">包含多张图像的HObject（通过ConcatObj连接）</param>
    /// <param name="dlSampleBatch">输出的样本批次（HTuple数组，每个元素是一个字典句柄）</param>
    private void GenDlSampleBatchFromImages(HObject images, out HTuple dlSampleBatch)
    {
        // 获取图像数量
        HOperatorSet.CountObj(images, out var imageCount);
        int count = imageCount.I;

        // 创建样本数组
        HTuple samples = new HTuple();

        for (int i = 1; i <= count; i++)  // Halcon索引从1开始
        {
            HOperatorSet.SelectObj(images, out var singleImage, i);
            HOperatorSet.CreateDict(out var dlSample);
            HOperatorSet.SetDictObject(singleImage, dlSample, "image");
            samples = samples.TupleConcat(dlSample);
            // 注意：不要释放 singleImage，因为它被添加到字典中了
        }

        dlSampleBatch = samples;
    }

    /// <summary>
    /// 预处理多个DL样本
    /// </summary>
    private void PreprocessDlSampleBatch(HTuple dlSampleBatch, HTuple preprocessParam)
    {
        // 获取预处理参数
        HOperatorSet.GetDictTuple(preprocessParam, "image_width", out var imageWidth);
        HOperatorSet.GetDictTuple(preprocessParam, "image_height", out var imageHeight);
        HOperatorSet.GetDictTuple(preprocessParam, "image_num_channels", out var numChannels);
        HOperatorSet.GetDictTuple(preprocessParam, "image_range_min", out var rangeMin);
        HOperatorSet.GetDictTuple(preprocessParam, "image_range_max", out var rangeMax);

        // 对每个样本进行预处理
        for (int i = 0; i < dlSampleBatch.Length; i++)
        {
            HTuple dlSample = dlSampleBatch[i];

            // 获取原始图像
            var origImage = dlSample.TupleGetDictObject("image");

            // 转换通道数
            HOperatorSet.CountChannels(origImage, out var channels);
            HObject convertedImage;
            if (channels.I == 1 && numChannels.I == 3)
            {
                HOperatorSet.Compose3(origImage, origImage, origImage, out convertedImage);
            }
            else if (channels.I == 3 && numChannels.I == 1)
            {
                HOperatorSet.Rgb1ToGray(origImage, out convertedImage);
            }
            else
            {
                convertedImage = origImage.Clone();
            }

            // 缩放到目标尺寸
            HOperatorSet.ZoomImageSize(convertedImage, out var zoomedImage, imageWidth, imageHeight, "constant");

            // 转换为实数图像并归一化
            HOperatorSet.ConvertImageType(zoomedImage, out var realImage, "real");
            HOperatorSet.ScaleImage(realImage, out var scaledImage,
                (rangeMax.D - rangeMin.D) / 255.0, rangeMin.D);

            // 更新样本中的图像
            HOperatorSet.SetDictObject(scaledImage, dlSample, "image");

            // 释放临时图像
            origImage?.Dispose();
            convertedImage?.Dispose();
            zoomedImage?.Dispose();
            realImage?.Dispose();
        }
    }

    private bool InferBatchCore(HTuple batch, int tw, int th, out HObject[] results, List<HObject> masks = null, int[] colCoords = null, int[] rowCoords = null)
    {
        results = null; HTuple dlResult = null;
        try
        {
            HOperatorSet.ApplyDlModel(_modelHandle, batch, new HTuple(), out dlResult);
            int cnt = batch.Length; results = new HObject[cnt];
            for (int i = 0; i < cnt; i++)
            {
                HObject seg = null, map = null;
                try
                {
                    seg = dlResult.TupleSelect(i).TupleGetDictObject("segmentation_image");
                    HOperatorSet.GetImageSize(seg, out var sw, out var sh);
                    HOperatorSet.GenImageConst(out map, "byte", sw, sh);
                    for (int j = 0; j < _classIDs.Length; j++)
                    {
                        HObject cr = null;
                        try { HOperatorSet.Threshold(seg, out cr, _classIDs[j], _classIDs[j]); HOperatorSet.PaintRegion(cr, map, out map, _classIDs[j], "fill"); }
                        finally { cr?.Dispose(); }
                    }
                    if (masks != null) foreach (var m in masks) if (m != null) HOperatorSet.OverpaintRegion(map, m, 0, "fill");
                    HOperatorSet.ZoomImageSize(map, out results[i], tw, th, "constant");
                    
                    // 调试模式：保存每个小图的推理结果（路径为空时使用默认路径）
                    if (colCoords != null && rowCoords != null && i < colCoords.Length)
                    {
                        SaveDebugResultImage(results[i], i, colCoords[i], rowCoords[i]);
                    }
                }
                finally { seg?.Dispose(); map?.Dispose(); }
            }
            return true;
        }
        catch (Exception ex) { Console.WriteLine($"[{_name}] 批量推理失败: {ex.Message}"); DisposeImageArray(results); results = null; return false; }
        finally { dlResult?.Dispose(); }
    }

    /// <summary>
    /// 将多张图像拼接为完整图像
    /// </summary>
    /// <param name="imgs">图像数组</param>
    /// <param name="colCoords">列坐标数组（X）</param>
    /// <param name="rowCoords">行坐标数组（Y）</param>
    /// <param name="totalWidth">总宽度</param>
    /// <param name="totalHeight">总高度</param>
    private HObject TileImagesToResult(HObject[] imgs, int[] colCoords, int[] rowCoords, int totalWidth, int totalHeight)
    {
        if (imgs == null || imgs.Length == 0) return null;
        HObject concat = null, tiled = null;
        try
        {
            concat = imgs[0].Clone();
            for (int i = 1; i < imgs.Length; i++) { var t = concat.ConcatObj(imgs[i]); concat.Dispose(); concat = t; }
            int[] rr = Enumerable.Repeat(-1, imgs.Length).ToArray();
            // TileImagesOffset 参数顺序: Image, TiledImage, OffsetRow, OffsetCol, ...
            HOperatorSet.TileImagesOffset(concat, out tiled, 
                new HTuple(rowCoords), new HTuple(colCoords), 
                new HTuple(rr), new HTuple(rr), new HTuple(rr), new HTuple(rr), 
                totalWidth, totalHeight);
            return tiled;
        }
        catch (Exception ex) { Console.WriteLine($"[{_name}] 拼接失败: {ex.Message}"); tiled?.Dispose(); return null; }
        finally { concat?.Dispose(); }
    }

    private void DisposeImageArray(HObject[] imgs) { if (imgs == null) return; foreach (var i in imgs) try { i?.Dispose(); } catch { } }

    /// <summary>
    /// 释放样本批次
    /// </summary>
    private void DisposeSampleBatch(HTuple batch)
    {
        if (batch == null || batch.Length == 0) return;
        try
        {
            batch.Dispose();
        }
        catch { }
    }

    // 调试用：当前批次的调试文件夹
    private string _currentDebugDir;

    /// <summary>
    /// 保存调试用的裁剪图像
    /// </summary>
    private void SaveDebugCroppedImages(HObject croppedImages, int[] colCoords, int[] rowCoords)
    {
        try
        {
            // 获取保存路径（如果为空则使用默认路径）
            var basePath = string.IsNullOrEmpty(DebugSavePath) 
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugOutput")
                : DebugSavePath;
            
            // 创建新的调试目录（每次批量推理一个目录）
            _currentDebugDir = Path.Combine(basePath, $"debug_{DateTime.Now:yyyyMMdd_HHmmss}");
            if (!Directory.Exists(_currentDebugDir))
                Directory.CreateDirectory(_currentDebugDir);

            HOperatorSet.CountObj(croppedImages, out var count);
            for (int i = 1; i <= count.I; i++)
            {
                HOperatorSet.SelectObj(croppedImages, out var img, i);
                var fileName = Path.Combine(_currentDebugDir, $"crop_{i - 1:D3}_x{colCoords[i - 1]}_y{rowCoords[i - 1]}.png");
                HOperatorSet.WriteImage(img, "png", 0, fileName.Replace("\\", "/"));
                img?.Dispose();
            }
            Console.WriteLine($"[{_name}] 已保存 {count.I} 张裁剪图像到: {_currentDebugDir}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{_name}] 保存裁剪图像失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 保存调试用的推理结果图像
    /// </summary>
    private void SaveDebugResultImage(HObject resultImage, int index, int col, int row)
    {
        try
        {
            if (string.IsNullOrEmpty(_currentDebugDir) || !Directory.Exists(_currentDebugDir))
            {
                // 获取保存路径（如果为空则使用默认路径）
                var basePath = string.IsNullOrEmpty(DebugSavePath)
                    ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebugOutput")
                    : DebugSavePath;
                
                _currentDebugDir = Path.Combine(basePath, $"debug_{DateTime.Now:yyyyMMdd_HHmmss}");
                if (!Directory.Exists(_currentDebugDir))
                    Directory.CreateDirectory(_currentDebugDir);
            }

            var fileName = Path.Combine(_currentDebugDir, $"result_{index:D3}_x{col}_y{row}.png");
            HOperatorSet.WriteImage(resultImage, "png", 0, fileName.Replace("\\", "/"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{_name}] 保存结果图像失败: {ex.Message}");
        }
    }

    #endregion

    #region IRenameableDnnModel 实现

    public void SetName(string name)
    {
        _name = name;
    }

    #endregion

    #region IConfigurableDnnModel 实现

    public DnnModelConfig GetConfig()
    {
        return new DnnModelConfig
        {
            Name = _name,
            Type = "语义分割",
            ModelPath = ModelPath,
            DeviceType = _deviceType,
            Runtime = _runtime,
            LoadOnStartup = _loadOnStartup
        };
    }

    public void ApplyConfig(DnnModelConfig config)
    {
        if (config == null) return;

        _deviceType = config.DeviceType;
        _runtime = config.Runtime;
        _loadOnStartup = config.LoadOnStartup;

        // 如果有模型路径且尚未加载，可选择加载
        if (!string.IsNullOrEmpty(config.ModelPath))
        {
            ModelPath = config.ModelPath;
        }
    }

    #endregion

    #region IOptimizableDnnModel 实现

    /// <summary>
    /// 获取可用的优化设备列表
    /// </summary>
    public List<string> GetAvailableOptimizeDevices()
    {
        var devices = new List<string>();

        try
        {
            // 查询 TensorRT 设备
            try
            {
                HOperatorSet.QueryAvailableDlDevices("ai_accelerator_interface", "tensorrt", out var tensorRTHandles);
                for (int i = 0; i < tensorRTHandles.Length; i++)
                {
                    HOperatorSet.GetDlDeviceParam(tensorRTHandles[i], "name", out var name);
                    devices.Add($"{name.S} (TensorRT)");
                }
                tensorRTHandles?.Dispose();
            }
            catch { }

            // 查询 OpenVINO 设备
            try
            {
                HOperatorSet.QueryAvailableDlDevices("ai_accelerator_interface", "openvino", out var openVINOHandles);
                for (int i = 0; i < openVINOHandles.Length; i++)
                {
                    HOperatorSet.GetDlDeviceParam(openVINOHandles[i], "name", out var name);
                    devices.Add($"{name.S} (OpenVINO)");
                }
                openVINOHandles?.Dispose();
            }
            catch { }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{_name}] 获取优化设备列表失败: {ex.Message}");
        }

        return devices;
    }

    /// <summary>
    /// 优化并导出模型
    /// </summary>
    public bool OptimizeAndExport(string deviceName, DnnOptimizePrecision precision, int batchSize, Action<int, string> progress = null)
    {
        if (!IsLoaded)
        {
            progress?.Invoke(0, "错误: 模型未加载");
            Console.WriteLine($"[{_name}] 优化失败: 模型未加载");
            return false;
        }

        if (string.IsNullOrEmpty(ModelPath))
        {
            progress?.Invoke(0, "错误: 模型路径为空");
            Console.WriteLine($"[{_name}] 优化失败: 模型路径为空");
            return false;
        }

        if (_modelHandle == null || _modelHandle.Length == 0)
        {
            progress?.Invoke(0, "错误: 模型句柄无效");
            Console.WriteLine($"[{_name}] 优化失败: 模型句柄无效");
            return false;
        }

        try
        {
            progress?.Invoke(5, "正在查询设备...");
            Console.WriteLine($"[{_name}] 开始优化，设备: {deviceName}, 精度: {precision}, 批次大小: {batchSize}");

            // 解析设备类型
            bool isTensorRT = deviceName.Contains("TensorRT");
            bool isOpenVINO = deviceName.Contains("OpenVINO");

            if (!isTensorRT && !isOpenVINO)
            {
                progress?.Invoke(0, "不支持的设备类型");
                Console.WriteLine($"[{_name}] 优化失败: 不支持的设备类型 - {deviceName}");
                return false;
            }

            // 查询对应设备
            HTuple deviceHandles;
            string accelerator = isTensorRT ? "tensorrt" : "openvino";

            try
            {
                HOperatorSet.QueryAvailableDlDevices("ai_accelerator_interface", accelerator, out deviceHandles);
            }
            catch (Exception ex)
            {
                progress?.Invoke(0, $"查询设备失败: {ex.Message}");
                Console.WriteLine($"[{_name}] 查询设备失败: {ex.Message}");
                return false;
            }

            if (deviceHandles == null || deviceHandles.Length == 0)
            {
                progress?.Invoke(0, $"未找到可用的 {accelerator} 设备");
                Console.WriteLine($"[{_name}] 优化失败: 未找到可用的 {accelerator} 设备");
                return false;
            }

            // 选择匹配的设备
            HTuple selectedDevice = null;
            for (int i = 0; i < deviceHandles.Length; i++)
            {
                HOperatorSet.GetDlDeviceParam(deviceHandles[i], "name", out var name);
                if (deviceName.Contains(name.S))
                {
                    selectedDevice = deviceHandles[i];
                    Console.WriteLine($"[{_name}] 选择设备: {name.S}");
                    break;
                }
            }

            if (selectedDevice == null)
            {
                selectedDevice = deviceHandles[0];
                HOperatorSet.GetDlDeviceParam(selectedDevice, "name", out var fallbackName);
                Console.WriteLine($"[{_name}] 使用默认设备: {fallbackName.S}");
            }

            progress?.Invoke(10, "正在准备优化...");

            // 设置批次大小
            try
            {
                HOperatorSet.SetDlModelParam(_modelHandle, "batch_size", batchSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_name}] 设置批次大小失败（可忽略）: {ex.Message}");
            }

            progress?.Invoke(25, "正在优化模型（这可能需要几分钟）...");

            // 转换精度字符串
            string precisionStr = precision switch
            {
                DnnOptimizePrecision.FP16 => "float16",
                DnnOptimizePrecision.INT8 => "int8",
                _ => "float32"
            };

            Console.WriteLine($"[{_name}] 开始调用 OptimizeDlModelForInference，精度: {precisionStr}");

            // 优化模型 - Halcon 24.11 正确调用方式
            // 首先获取优化参数
            HTuple optimizeForInferenceParams;
            try
            {
                HOperatorSet.GetDlDeviceParam(selectedDevice, "optimize_for_inference_params", out optimizeForInferenceParams);
                Console.WriteLine($"[{_name}] 获取优化参数成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_name}] 获取优化参数失败: {ex.Message}，使用默认参数");
                optimizeForInferenceParams = new HTuple();
            }
            
            HTuple optimizedModelHandle;
            HTuple conversionReport;
            
            try
            {
                Console.WriteLine($"[{_name}] 调用 OptimizeDlModelForInference (Halcon 24.11)");
                Console.WriteLine($"[{_name}] DLModelHandle: {_modelHandle}");
                Console.WriteLine($"[{_name}] DLDevice: {selectedDevice}");
                Console.WriteLine($"[{_name}] Precision: {precisionStr}");
                
                // Halcon 24.11 签名:
                // OptimizeDlModelForInference(DLModelHandle, DLDevice, Precision, GenParamName, OptimizeForInferenceParams, 
                //                            out DLModelHandleOptimized, out ConversionReport)
                HOperatorSet.OptimizeDlModelForInference(
                    _modelHandle,               // DLModelHandle - 原始模型
                    selectedDevice,             // DLDevice - 设备句柄
                    precisionStr,               // Precision - 精度 ("float32", "float16", "int8")
                    new HTuple(),               // GenParamName - 空的额外参数
                    optimizeForInferenceParams, // OptimizeForInferenceParams - 优化参数
                    out optimizedModelHandle,   // DLModelHandleOptimized - 优化后的模型
                    out conversionReport);      // ConversionReport - 转换报告
                    
                Console.WriteLine($"[{_name}] 优化成功！");
            }
            catch (Exception ex)
            {
                progress?.Invoke(0, $"Halcon优化失败: {ex.Message}");
                Console.WriteLine($"[{_name}] OptimizeDlModelForInference 失败: {ex.Message}");
                Console.WriteLine($"[{_name}] 异常详情: {ex}");
                return false;
            }

            progress?.Invoke(80, "正在保存优化模型...");

            // 生成输出文件名 - 查找原始模型文件
            var hdlFiles = Directory.GetFiles(ModelPath, "*.hdl", SearchOption.TopDirectoryOnly);
            if (hdlFiles.Length == 0)
            {
                progress?.Invoke(0, "未找到原始模型文件");
                Console.WriteLine($"[{_name}] 优化失败: 在 {ModelPath} 中未找到 .hdl 文件");
                return false;
            }

            string originalName = Path.GetFileNameWithoutExtension(hdlFiles[0]);
            string suffix = isTensorRT ? "_tensorrt" : "_openvino";
            string optimizedPath = Path.Combine(ModelPath, $"{originalName}{suffix}.hdl").Replace("\\", "/");

            Console.WriteLine($"[{_name}] 保存优化模型到: {optimizedPath}");

            try
            {
                HOperatorSet.WriteDlModel(optimizedModelHandle, optimizedPath);
            }
            catch (Exception ex)
            {
                progress?.Invoke(0, $"保存模型失败: {ex.Message}");
                Console.WriteLine($"[{_name}] 保存模型失败: {ex.Message}");
                return false;
            }

            progress?.Invoke(100, "优化完成");

            // 清理
            optimizedModelHandle?.Dispose();

            Console.WriteLine($"[{_name}] 模型优化导出成功: {optimizedPath}");
            return true;
        }
        catch (Exception ex)
        {
            progress?.Invoke(0, $"优化失败: {ex.Message}");
            Console.WriteLine($"[{_name}] 模型优化失败: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }

    /// <summary>
    /// 检查优化模型是否存在
    /// </summary>
    public bool HasOptimizedModel(DnnRuntime runtime)
    {
        if (string.IsNullOrEmpty(ModelPath) || !Directory.Exists(ModelPath))
            return false;

        string suffix = runtime switch
        {
            DnnRuntime.TensorRT => "_tensorrt",
            DnnRuntime.OpenVINO => "_openvino",
            _ => null
        };

        if (suffix == null)
            return false;

        // 检查是否存在优化后的模型文件
        var optimizedFiles = Directory.GetFiles(ModelPath, $"*{suffix}.hdl", SearchOption.TopDirectoryOnly);
        return optimizedFiles.Length > 0;
    }

    #endregion

    #region IInferenceDeviceProvider 实现

    /// <summary>
    /// 枚举当前机器可用的推理设备及其运行时
    /// </summary>
    public List<DnnInferenceDeviceInfo> GetAvailableInferenceDevices()
    {
        var devices = new List<DnnInferenceDeviceInfo>();

        // 通用 GC 运行时
        TryCollectDevices(
            (new HTuple("runtime")).TupleConcat("runtime"),
            (new HTuple("gpu")).TupleConcat("cpu"),
            DnnRuntime.GC,
            devices);

        // OpenVINO 加速
        TryCollectDevices(
            new HTuple("ai_accelerator_interface"),
            new HTuple("openvino"),
            DnnRuntime.OpenVINO,
            devices);

        // TensorRT 加速
        TryCollectDevices(
            new HTuple("ai_accelerator_interface"),
            new HTuple("tensorrt"),
            DnnRuntime.TensorRT,
            devices);

        return devices;
    }

    private void TryCollectDevices(HTuple interfaceName, HTuple deviceName, DnnRuntime runtime, List<DnnInferenceDeviceInfo> output)
    {
        try
        {
            HOperatorSet.QueryAvailableDlDevices(interfaceName, deviceName, out var handles);
            AppendDeviceInfo(handles, runtime, output);
            handles?.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{_name}] 查询{runtime}设备失败: {ex.Message}");
        }
    }

    private void AppendDeviceInfo(HTuple handles, DnnRuntime runtime, List<DnnInferenceDeviceInfo> output)
    {
        if (handles == null || handles.Length == 0) return;

        for (int i = 0; i < handles.Length; i++)
        {
            try
            {
                HOperatorSet.GetDlDeviceParam(handles[i], "name", out var name);
                HOperatorSet.GetDlDeviceParam(handles[i], "type", out var type);

                var deviceType = string.Equals(type.S, "cpu", StringComparison.OrdinalIgnoreCase)
                    ? DnnDeviceType.CPU
                    : DnnDeviceType.GPU;

                output.Add(new DnnInferenceDeviceInfo
                {
                    Name = name.S,
                    Runtime = runtime,
                    DeviceType = deviceType
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_name}] 读取{runtime}设备信息失败: {ex.Message}");
            }
        }
    }

    #endregion

    #region 私有辅助方法

    private bool QueryAvailableDevices(DnnDeviceType deviceType, DnnRuntime runtime)
    {
        try
        {
            switch (runtime)
            {
                case DnnRuntime.GC:
                    HOperatorSet.QueryAvailableDlDevices(
                        (new HTuple("runtime")).TupleConcat("runtime"),
                        (new HTuple("gpu")).TupleConcat("cpu"),
                        out _deviceHandles);
                    break;

                case DnnRuntime.OpenVINO:
                    HOperatorSet.QueryAvailableDlDevices(
                        "ai_accelerator_interface",
                        "openvino",
                        out _deviceHandles);
                    break;

                case DnnRuntime.TensorRT:
                    HOperatorSet.QueryAvailableDlDevices(
                        "ai_accelerator_interface",
                        "tensorrt",
                        out _deviceHandles);
                    break;
            }

            if (_deviceHandles.Length == 0)
            {
                Console.WriteLine("没有检测到可用设备");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"查询设备失败: {ex.Message}");
            return false;
        }
    }

    private bool TryFindAndReadModel(string directory)
    {
        try
        {
            if (!Directory.Exists(directory))
                return false;

            // 查找 .hdl 文件
            var hdlFiles = Directory.GetFiles(directory, "*.hdl", SearchOption.TopDirectoryOnly);
            if (hdlFiles.Length == 0)
                return false;

            // 根据运行时选择对应的模型文件
            string hdlFile = null;
            string suffix = _runtime switch
            {
                DnnRuntime.TensorRT => "_tensorrt",
                DnnRuntime.OpenVINO => "_openvino",
                _ => null
            };

            // 优先查找优化后的模型
            if (!string.IsNullOrEmpty(suffix))
            {
                hdlFile = hdlFiles.FirstOrDefault(f => 
                    Path.GetFileNameWithoutExtension(f).EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
                
                if (hdlFile != null)
                {
                    IsOptimizedModel = true;
                    ActualRuntime = _runtime.ToString();
                    Console.WriteLine($"[{_name}] 找到优化模型: {Path.GetFileName(hdlFile)}");
                }
            }

            // 如果没有找到优化模型，使用原始模型
            if (hdlFile == null)
            {
                // 排除优化模型文件，找原始模型
                hdlFile = hdlFiles.FirstOrDefault(f =>
                {
                    var name = Path.GetFileNameWithoutExtension(f);
                    return !name.EndsWith("_tensorrt", StringComparison.OrdinalIgnoreCase) &&
                           !name.EndsWith("_openvino", StringComparison.OrdinalIgnoreCase);
                });

                // 如果全是优化模型，就用第一个
                if (hdlFile == null)
                    hdlFile = hdlFiles[0];

                IsOptimizedModel = false;
                ActualRuntime = "GC";
                Console.WriteLine($"[{_name}] 使用原始模型: {Path.GetFileName(hdlFile)}");
            }

            LoadedModelFile = hdlFile;
            string modelFilePath = hdlFile.Replace("\\", "/");
            HOperatorSet.ReadDlModel(modelFilePath, out _modelHandle);

            // 尝试从模型参数中获取运行时信息
            try
            {
                HOperatorSet.GetDlModelParam(_modelHandle, "runtime", out var runtimeParam);
                ActualRuntime = runtimeParam.S;
                Console.WriteLine($"[{_name}] 模型运行时: {ActualRuntime}");
            }
            catch
            {
                // 某些模型可能没有这个参数
            }

            // 尝试获取设备信息
            try
            {
                HOperatorSet.GetDlModelParam(_modelHandle, "device", out var deviceParam);
                Console.WriteLine($"[{_name}] 模型设备: {deviceParam}");
            }
            catch { }

            // 查找对应的预处理参数文件
            string hdlPrefix = Path.GetFileNameWithoutExtension(hdlFile);
            // 如果是优化模型，尝试找原始模型的预处理参数
            if (IsOptimizedModel && !string.IsNullOrEmpty(suffix))
            {
                hdlPrefix = hdlPrefix.Replace(suffix, "");
            }
            
            var hdictFiles = Directory.GetFiles(directory, $"*_dl_preprocess_params.hdict", SearchOption.TopDirectoryOnly);
            if (hdictFiles.Length > 0)
            {
                string hdictFile = hdictFiles[0].Replace("\\", "/");
                HOperatorSet.ReadDict(hdictFile, new HTuple(), new HTuple(), out _preprocessParam);
            }

            // 获取模型参数（语义分割模型）
            try
            {
                HOperatorSet.GetDlModelParam(_modelHandle, "class_names", out _classNames);
                HOperatorSet.GetDlModelParam(_modelHandle, "class_ids", out _classIDs);

                HOperatorSet.CreateDict(out _datasetInfo);
                HOperatorSet.SetDictTuple(_datasetInfo, "class_ids", _classIDs);
                HOperatorSet.SetDictTuple(_datasetInfo, "class_names", _classNames);

                Console.WriteLine($"[{_name}] 模型类别数量: {_classIDs.Length}");
            }
            catch
            {
                Console.WriteLine($"[{_name}] 警告: 无法获取类别信息（可能不是语义分割模型）");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{_name}] 读取模型文件失败: {ex.Message}");
            return false;
        }
    }

    private void SelectDevice(HTuple handles, string type)
    {
        for (int i = 0; i < handles.Length; i++)
        {
            HOperatorSet.GetDlDeviceParam(handles[i], "type", out var devType);
            if (devType.S == type.ToLower())
            {
                _deviceHandle = handles[i];
                HOperatorSet.SetDlModelParam(_modelHandle, "device", _deviceHandle);
                Console.WriteLine($"使用设备: {type}");
                break;
            }
        }
    }

    private void GenDlSamplesFromImages(HObject image, out HTuple dlSamples)
    {
        HOperatorSet.CreateDict(out var dlSample);
        HOperatorSet.SetDictObject(image, dlSample, "image");
        dlSamples = dlSample;
    }

    private void PreprocessDlSamples(HTuple dlSamples, HTuple preprocessParam)
    {
        // 获取预处理参数
        HOperatorSet.GetDictTuple(preprocessParam, "image_width", out var imageWidth);
        HOperatorSet.GetDictTuple(preprocessParam, "image_height", out var imageHeight);
        HOperatorSet.GetDictTuple(preprocessParam, "image_num_channels", out var numChannels);
        HOperatorSet.GetDictTuple(preprocessParam, "image_range_min", out var rangeMin);
        HOperatorSet.GetDictTuple(preprocessParam, "image_range_max", out var rangeMax);

        // 获取原始图像
        var origImage = dlSamples.TupleGetDictObject("image");

        // 转换通道数
        HOperatorSet.CountChannels(origImage, out var channels);
        HObject convertedImage;
        if (channels.I == 1 && numChannels.I == 3)
        {
            HOperatorSet.Compose3(origImage, origImage, origImage, out convertedImage);
        }
        else if (channels.I == 3 && numChannels.I == 1)
        {
            HOperatorSet.Rgb1ToGray(origImage, out convertedImage);
        }
        else
        {
            convertedImage = origImage.Clone();
        }

        // 缩放到目标尺寸
        HOperatorSet.ZoomImageSize(convertedImage, out var zoomedImage, imageWidth, imageHeight, "constant");

        // 转换为实数图像并归一化
        HOperatorSet.ConvertImageType(zoomedImage, out var realImage, "real");
        HOperatorSet.ScaleImage(realImage, out var scaledImage, 
            (rangeMax.D - rangeMin.D) / 255.0, rangeMin.D);

        // 更新样本
        HOperatorSet.SetDictObject(scaledImage, dlSamples, "image");

        origImage?.Dispose();
        convertedImage?.Dispose();
        zoomedImage?.Dispose();
        realImage?.Dispose();
    }

    #endregion


    ///// <summary>
    ///// 单张图片推理
    ///// </summary>
    ///// <param name="image">VisionPro原图</param>
    ///// <param name="hv_reImage">推理结果图,返回映射的图像</param>
    ///// <returns>1-成功, -1-失败</returns>
    //public int ApplyModel(ICogImage image, out HObject hv_reImage)
    //{
    //    lock (_lock)
    //    {
    //        hv_reImage = null;
    //        HObject hv_Image = null;
    //        HTuple hv_DLSampleBatch = null;

    //        try
    //        {
    //            if (!IsInitialSuccese)
    //            {
    //                Console.WriteLine("模型未初始化");
    //                return -1;
    //            }

    //            ICogImage img = image.CopyBase(CogImageCopyModeConstants.CopyPixels);
    //            int Width = img.Width;
    //            int Height = img.Height;

    //            ImageProcess.ImageConvertVisionPro2HObject(img, out hv_Image);
    //            developExport.gen_dl_samples_from_images(hv_Image, out hv_DLSampleBatch);
    //            developExport.preprocess_dl_samples(hv_DLSampleBatch, hv_DLPreprocessParam);

    //            if (Inference(hv_DLSampleBatch, Width, Height, out var hv_reImages))
    //            {
    //                hv_reImage = hv_reImages[0];
    //                return 1;
    //            }
    //            else
    //            {
    //                return -1;
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine($"ApplyModel异常: {e.Message}");
    //            return -1;
    //        }
    //        finally
    //        {
    //            // 释放临时资源
    //            hv_Image?.Dispose();
    //            hv_DLSampleBatch?.Dispose();
    //        }
    //    }
    //}


    ///// <summary>
    ///// 批量推理（拼接模式）
    ///// </summary>
    ///// <param name="image">原始图像</param>
    ///// <param name="PixelXArray">图像X像素坐标数组</param>
    ///// <param name="PixelYArray">图像Y像素坐标数组</param>
    ///// <param name="catWidth">裁剪宽度</param>
    ///// <param name="catHeight">裁剪高度</param>
    ///// <param name="hv_reImage">推理结果图</param>
    ///// <param name="maskRegions">遮罩区域</param>
    ///// <returns>1-成功, -1-失败</returns>
    //public int ApplyModel(ICogImage image, int[] PixelXArray, int[] PixelYArray, int catWidth, int catHeight,
    //  out HObject hv_reImage, List<HObject> maskRegions = null)
    //{
    //    lock (_lock)
    //    {
    //        hv_reImage = null;
    //        HObject hv_Image = null;
    //        HObject h_ImageArray = null;
    //        HTuple hv_DLSampleBatch = null;

    //        try
    //        {
    //            if (!IsInitialSuccese)
    //            {
    //                Console.WriteLine("模型未初始化");
    //                return -1;
    //            }

    //            ImageProcess.ImageConvertVisionPro2HObject(image, out hv_Image);

    //            int[] Rows = PixelYArray;
    //            int[] Cols = PixelXArray;
    //            int cnt = Rows.Length;
    //            int[] hv_rCat = new int[cnt];

    //            h_ImageArray = new HObject();
    //            h_ImageArray.GenEmptyObj();

    //            for (int i = 0; i < Rows.Length; i++)
    //            {
    //                HObject rectangle = null, imageReduced = null, imagePart = null;
    //                try
    //                {
    //                    hv_rCat[i] = -1;
    //                    HOperatorSet.GenRectangle1(out rectangle, Rows[i], Cols[i], Rows[i] + catHeight - 1,
    //                      Cols[i] + catWidth - 1);
    //                    HOperatorSet.ReduceDomain(hv_Image, rectangle, out imageReduced);
    //                    HOperatorSet.CropDomain(imageReduced, out imagePart);
    //                    h_ImageArray = h_ImageArray.ConcatObj(imagePart);
    //                }
    //                finally
    //                {
    //                    rectangle?.Dispose();
    //                    imageReduced?.Dispose();
    //                    imagePart?.Dispose();
    //                }
    //            }

    //            developExport.gen_dl_samples_from_images(h_ImageArray, out hv_DLSampleBatch);
    //            developExport.preprocess_dl_samples(hv_DLSampleBatch, hv_DLPreprocessParam);

    //            if (!Inference(hv_DLSampleBatch, catWidth, catHeight, out var hResultImages, maskRegions))
    //            {
    //                return -1;
    //            }

    //            HObject h_rsImgArray = null;
    //            try
    //            {
    //                h_rsImgArray = hResultImages[0];
    //                for (int i = 1; i < hResultImages.Length; i++)
    //                {
    //                    var temp = h_rsImgArray.ConcatObj(hResultImages[i]);
    //                    h_rsImgArray.Dispose();
    //                    h_rsImgArray = temp;
    //                }

    //                HOperatorSet.TileImagesOffset(h_rsImgArray, out hv_reImage, Rows, Cols, hv_rCat, hv_rCat, hv_rCat,
    //                  hv_rCat, image.Width, image.Height);
    //                return 1;
    //            }
    //            finally
    //            {
    //                h_rsImgArray?.Dispose();
    //                if (hResultImages != null)
    //                {
    //                    foreach (var img in hResultImages)
    //                    {
    //                        img?.Dispose();
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine($"批量推理异常: {e.Message}");
    //            return -1;
    //        }
    //        finally
    //        {
    //            hv_Image?.Dispose();
    //            h_ImageArray?.Dispose();
    //            hv_DLSampleBatch?.Dispose();
    //        }
    //    }
    //}

    //private bool Inference(HTuple hv_DLSampleBatch, int rawWidth, int rawHeight, out HObject[] hv_reImages,
    //  List<HObject> maskRegions = null)
    //{
    //    hv_reImages = null;
    //    HTuple hv_DLResultBatch = null;

    //    try
    //    {
    //        HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleBatch, new HTuple(),
    //          out hv_DLResultBatch);

    //        hv_reImages = new HObject[hv_DLSampleBatch.Length];

    //        for (int i = 0; i < hv_DLSampleBatch.Length; i++)
    //        {
    //            HObject ho_SegmentationImage = null;
    //            HObject hv_MapImage = null;
    //            HTuple width = null, height = null;

    //            try
    //            {
    //                ho_SegmentationImage = hv_DLResultBatch.TupleSelect(i).TupleGetDictObject("segmentation_image");
    //                HOperatorSet.GetImageSize(ho_SegmentationImage, out width, out height);
    //                HOperatorSet.GenImageConst(out hv_MapImage, "byte", width, height);

    //                // 将分割标签转换为灰度图
    //                for (int j = 0; j < hv_ClassIDs.Length; j++)
    //                {
    //                    HObject ho_ClassRegion = null;
    //                    try
    //                    {
    //                        HOperatorSet.Threshold(ho_SegmentationImage, out ho_ClassRegion, hv_ClassIDs[j],
    //                          hv_ClassIDs[j]);
    //                        HOperatorSet.PaintRegion(ho_ClassRegion, hv_MapImage, out hv_MapImage, hv_ClassIDs[j],
    //                          "fill");
    //                    }
    //                    finally
    //                    {
    //                        ho_ClassRegion?.Dispose();
    //                    }
    //                }

    //                // 应用遮罩区域
    //                if (maskRegions != null)
    //                {
    //                    foreach (var t in maskRegions)
    //                    {
    //                        HOperatorSet.OverpaintRegion(hv_MapImage, t, 0, "fill");
    //                    }
    //                }

    //                // 缩放到原始尺寸
    //                HOperatorSet.ZoomImageSize(hv_MapImage, out hv_reImages[i], rawWidth, rawHeight, "constant");
    //            }
    //            finally
    //            {
    //                ho_SegmentationImage?.Dispose();
    //                hv_MapImage?.Dispose();
    //                width?.Dispose();
    //                height?.Dispose();
    //            }
    //        }

    //        return true;
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine($"推理异常: {e.Message}");
    //        if (hv_reImages != null)
    //        {
    //            foreach (var img in hv_reImages)
    //            {
    //                img?.Dispose();
    //            }
    //        }

    //        hv_reImages = null;
    //        return false;
    //    }
    //    finally
    //    {
    //        hv_DLResultBatch?.Dispose();
    //    }
    //}
}
