# 深度学习运行时配置完整指南

## ?? 功能概述

在工位配置中提供**三种深度学习运行时**选择，每种运行时针对不同的硬件和场景优化：

| 运行时 | 适用硬件 | 性能 | 兼容性 | 推荐场景 |
|--------|---------|------|--------|---------|
| **GPU 运行时 (GC)** | 通用 GPU | ★★★☆☆ | ★★★★★ | 开发测试、通用部署 |
| **OpenVINO 加速** | Intel CPU/GPU/VPU | ★★★★☆ | ★★★☆☆ | Intel 硬件平台 |
| **TensorRT 加速** | NVIDIA GPU | ★★★★★ | ★★★☆☆ | 高性能生产环境 |

---

## ?? 配置界面

### PropertyGrid 配置项

```
深度学习
├── 是否加载模型  [?]
├── 运行时类型             [GPU 运行时 ▼]  ← 新增下拉选择
│   ├── GPU 运行时
│   ├── OpenVINO 加速
│   └── TensorRT 加速
├── 模型文件夹路径         [D:\Models\MyModel]
└── 模型文件夹名称   <MyModel> (只读)
```

### 配置属性说明

```csharp
// ProcessStation.cs 中的配置属性

[Category("深度学习"), DisplayName("运行时类型")]
[Description("选择深度学习运行时：GPU(兼容性好) / OpenVINO(Intel优化) / TensorRT(NVIDIA高性能)")]
public DLRuntime RuntimeType { get; set; } = DLRuntime.GC;
```

---

## ?? 运行时详细说明

### 1. GPU 运行时 (GC) - 默认推荐

**枚举值**: `DLRuntime.GC = 0`

**对应 Halcon**: `Runtime.GC`

**硬件要求**:
- 支持 CUDA 的 NVIDIA GPU（推荐）
- 或 CPU 模式（性能较低）

**优点**:
- ? 兼容性最好，几乎所有硬件都支持
- ? 配置简单，无需额外安装
- ? 稳定性高，适合开发调试

**缺点**:
- ? 性能不如专用加速器
- ? 未针对特定硬件优化

**推荐场景**:
- 开发和测试阶段
- 没有特殊硬件要求的项目
- 需要快速部署的场景

---

### 2. OpenVINO 加速

**枚举值**: `DLRuntime.OpenVINO = 1`

**对应 Halcon**: `Runtime.OPENVINO`

**硬件要求**:
- Intel CPU (第6代及以上)
- Intel 集成显卡
- Intel Movidius VPU
- Intel FPGA

**优点**:
- ? Intel 硬件优化，性能提升显著
- ? 支持多种 Intel 硬件加速器
- ? 功耗相对较低

**缺点**:
- ? 仅支持 Intel 硬件
- ? 需要安装 OpenVINO toolkit
- ? 模型转换可能需要额外工作

**推荐场景**:
- 使用 Intel 硬件平台
- 边缘计算设备（低功耗要求）
- 嵌入式系统

**安装要求**:
```bash
# 需要安装 Intel OpenVINO Toolkit
# https://www.intel.com/content/www/us/en/developer/tools/openvino-toolkit/overview.html
```

---

### 3. TensorRT 加速

**枚举值**: `DLRuntime.TensorRT = 2`

**对应 Halcon**: `Runtime.TENSORRT`

**硬件要求**:
- NVIDIA GPU (支持 CUDA)
  - 推荐: RTX 系列 (Turing/Ampere/Ada 架构)
  - 最低: GTX 10 系列

**优点**:
- ? 性能最强，推理速度最快
- ? 针对 NVIDIA GPU 深度优化
- ? 支持 INT8/FP16 量化加速
- ? 动态优化，自动调整批处理

**缺点**:
- ? 仅支持 NVIDIA GPU
- ? 需要安装 CUDA 和 TensorRT
- ? 首次推理较慢（优化阶段）

**推荐场景**:
- 生产环境高性能需求
- 实时检测应用
- 大批量图像处理
- NVIDIA 硬件平台

**安装要求**:
```bash
# 1. NVIDIA 显卡驱动（最新版本）
# 2. CUDA Toolkit 11.x 或 12.x
# 3. cuDNN 8.x
# 4. TensorRT 8.x
```

---

## ?? 配置步骤

### 步骤 1: 选择运行时

1. 打开方案配置界面
2. 选择目标工位
3. 在 PropertyGrid 的"深度学习"分类下：
   - ? 勾选"是否加载模型"
   - ?? 选择"运行时类型"下拉菜单
   - ?? 选择合适的运行时

### 步骤 2: 配置模型路径

选择包含以下文件的模型文件夹：
- `model.hdl` - 模型文件
- `model_dl_preprocess_params.hdict` - 预处理参数

### 步骤 3: 保存并重新加载

1. 保存方案
2. 重新打开方案或重启程序
3. 查看日志确认加载成功

---

## ?? 性能对比

### 基准测试（示例）

测试环境：
- CPU: Intel Core i7-10700K
- GPU: NVIDIA RTX 3070
- 模型: 语义分割模型 (512x512)

| 运行时 | 首次推理 | 平均推理 | 吞吐量 | 显存占用 |
|--------|---------|---------|--------|---------|
| GPU (GC) | 150ms | 45ms | 22 FPS | 2.5GB |
| OpenVINO | 200ms | 35ms | 28 FPS | 1.8GB |
| TensorRT | 800ms | 15ms | 66 FPS | 3.2GB |

**注意**: 
- TensorRT 首次推理较慢是因为需要优化计算图
- 实际性能因模型复杂度和硬件配置而异

---

## ?? 代码实现

### 枚举定义

```csharp
// Vision\Solutions\Models\DLRuntime.cs

public enum DLRuntime
{
    /// <summary>
    /// GPU 运行时（默认）- 兼容性最好
    /// </summary>
    [Description("GPU 运行时")]
    GC = 0,

 /// <summary>
    /// OpenVINO 加速 - Intel 硬件优化
    /// </summary>
    [Description("OpenVINO 加速")]
    OpenVINO = 1,

    /// <summary>
    /// TensorRT 硬件加速 - NVIDIA GPU 高性能
    /// </summary>
    [Description("TensorRT 加速")]
    TensorRT = 2
}
```

### 配置属性

```csharp
// ProcessStation.cs

[Category("深度学习"), DisplayName("运行时类型")]
[Description("选择深度学习运行时：GPU(兼容性好) / OpenVINO(Intel优化) / TensorRT(NVIDIA高性能)")]
public DLRuntime RuntimeType { get; set; } = DLRuntime.GC;
```

### 模型加载逻辑

```csharp
// SolutionModels.cs

// 将 DLRuntime 枚举值转换为对应的整数
int runtimeValue = (int)st.RuntimeType; // GC=0, OpenVINO=1, TensorRT=2

object[] parameters = [
    st.ModelPath,
    deviceTypeEnum != null ? Enum.ToObject(deviceTypeEnum, 0) : 0,
  runtimeEnum != null ? Enum.ToObject(runtimeEnum, runtimeValue) : runtimeValue
];

var result = initMethod.Invoke(modelInstance, parameters);

// 显示运行时类型
string runtimeInfo = st.RuntimeType switch
{
    DLRuntime.GC => "GPU 运行时",
    DLRuntime.OpenVINO => "OpenVINO 加速",
    DLRuntime.TensorRT => "TensorRT 加速",
    _ => st.RuntimeType.ToString()
};

LogHelper.Info($"工位[{st.Name}]深度学习模型加载成功: {st.ModelFolderName} (运行时: {runtimeInfo})");
```

---

## ?? 故障排查

### 1. GPU 运行时加载失败

**错误信息**:
```
没有检测到可用设备
```

**解决方法**:
- 检查 NVIDIA 显卡驱动是否安装
- 确认 CUDA 是否正确配置
- 尝试使用 CPU 模式（性能较低）

---

### 2. OpenVINO 加载失败

**错误信息**:
```
没有检测到可用设备
```

**解决方法**:
- 确认 OpenVINO Toolkit 已安装
- 检查环境变量配置
- 验证 Intel 硬件支持
- 查看 OpenVINO 版本兼容性

**环境变量检查**:
```batch
# Windows
echo %INTEL_OPENVINO_DIR%

# 应该输出类似
# C:\Program Files (x86)\Intel\openvino_2022
```

---

### 3. TensorRT 加载失败

**错误信息**:
```
没有检测到可用设备
```

**解决方法**:
- 检查 NVIDIA 显卡是否支持 CUDA
- 确认 CUDA Toolkit 已安装
- 验证 TensorRT 库已正确配置
- 检查 cuDNN 版本兼容性

**版本兼容性检查**:
| 组件 | 推荐版本 |
|------|---------|
| CUDA | 11.8 / 12.0 |
| cuDNN | 8.6 / 8.9 |
| TensorRT | 8.5 / 8.6 |

**验证 TensorRT**:
```bash
# 检查 TensorRT 安装
where tensorrt

# 或检查环境变量
echo %TENSORRT_HOME%
```

---

### 4. 模型初始化失败

**常见原因**:
- 模型文件损坏或不完整
- 模型格式与运行时不兼容
- 内存不足

**解决方法**:
1. 检查模型文件完整性
2. 确认模型是否支持所选运行时
3. 增加系统内存或显存
4. 尝试使用其他运行时

---

## ?? 性能优化建议

### 1. 运行时选择策略

```
场景分类   推荐运行时
├── 开发调试环境          → GPU (GC)
├── Intel 硬件平台    → OpenVINO
├── NVIDIA GPU 平台    → TensorRT
├── 实时性要求高     → TensorRT
├── 功耗敏感应用     → OpenVINO
└── 通用部署             → GPU (GC)
```

### 2. TensorRT 优化技巧

**预热推理**:
```csharp
// 首次加载后执行几次推理，让 TensorRT 优化计算图
for (int i = 0; i < 3; i++)
{
    model.ApplyModel(dummyImage, out _);
}
```

**批处理优化**:
```csharp
// 设置合适的批处理大小
model.Batch_Size = 4; // 根据显存调整
```

**精度权衡**:
- FP32: 精度最高，速度最慢
- FP16: 精度略降，速度提升 2-3x
- INT8: 精度下降明显，速度提升 3-5x

---

### 3. OpenVINO 优化技巧

**模型转换**:
```bash
# 使用 OpenVINO Model Optimizer 转换模型
mo --input_model model.onnx \
   --output_dir ./openvino \
   --data_type FP16
```

**设备选择**:
```csharp
// 优先使用 GPU，如不可用则使用 CPU
// OpenVINO 会自动选择最佳设备
```

---

## ?? 测试验证

### 单元测试示例

```csharp
[Test]
public void TestRuntimeSelection()
{
    var station1 = new ProcessStation 
    { 
        RuntimeType = DLRuntime.GC,
   ModelPath = @"D:\Models\Test"
    };
    // 预期：使用 Runtime.GC (0)

    var station2 = new ProcessStation 
    { 
        RuntimeType = DLRuntime.OpenVINO,
      ModelPath = @"D:\Models\Test"
    };
 // 预期：使用 Runtime.OPENVINO (1)

    var station3 = new ProcessStation 
    { 
        RuntimeType = DLRuntime.TensorRT,
        ModelPath = @"D:\Models\Test"
    };
    // 预期：使用 Runtime.TENSORRT (2)
}
```

### 性能基准测试

```csharp
public void BenchmarkRuntime(DLRuntime runtime)
{
    var station = new ProcessStation
    {
   bLoadModel = true,
        RuntimeType = runtime,
        ModelPath = @"D:\Models\BenchmarkModel"
    };

    // 加载模型
    SolutionManager.Instance.LoadModel(station);

  // 预热
    for (int i = 0; i < 10; i++)
    {
  model.ApplyModel(testImage, out _);
    }

    // 测试
    var sw = Stopwatch.StartNew();
 for (int i = 0; i < 100; i++)
    {
        model.ApplyModel(testImage, out _);
    }
    sw.Stop();

    Console.WriteLine($"{runtime}: {sw.ElapsedMilliseconds / 100.0}ms 平均");
}
```

---

## ?? 参考资源

### 官方文档

1. **Halcon Deep Learning**
   - https://www.mvtec.com/products/halcon/deep-learning

2. **Intel OpenVINO**
   - https://docs.openvino.ai/

3. **NVIDIA TensorRT**
   - https://developer.nvidia.com/tensorrt

### 常见问题

**Q: 如何选择合适的运行时？**
A: 根据硬件平台和性能需求选择。Intel 平台推荐 OpenVINO，NVIDIA 平台推荐 TensorRT，通用场景使用 GPU (GC)。

**Q: 可以在运行时动态切换吗？**
A: 需要重新加载模型。建议在配置阶段确定运行时类型。

**Q: TensorRT 首次推理为什么很慢？**
A: TensorRT 需要优化计算图，建议进行预热推理。

**Q: OpenVINO 和 TensorRT 可以共存吗？**
A: 可以，但同一个工位同一时间只能使用一种运行时。

---

## ? 完成检查清单

- [ ] DLRuntime.cs 枚举文件已创建
- [ ] ProcessStation 添加了 RuntimeType 属性
- [ ] SolutionModels 修改了运行时选择逻辑
- [ ] 测试 GPU 运行时加载成功
- [ ] 测试 OpenVINO 运行时加载（如有硬件）
- [ ] 测试 TensorRT 运行时加载（如有硬件）
- [ ] 日志正确显示运行时类型
- [ ] 配置可以正确保存和加载

---

**现在你可以根据硬件平台和性能需求灵活选择最适合的深度学习运行时！** ??
