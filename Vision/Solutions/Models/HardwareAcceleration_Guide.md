# 深度学习硬件加速配置说明

## ?? 功能概述

在工位配置中新增**硬件加速**选项，支持在加载深度学习模型时选择不同的运行时：
- ? **GPU 运行时** (Runtime.GC) - 默认模式，兼容性好
- ? **TensorRT 硬件加速** (Runtime.TENSORRT) - 高性能模式，需要支持的硬件

---

## ?? 配置说明

### 1. 工位配置界面

在 PropertyGrid 的"深度学习"分类下，新增配置项：

```
深度学习
├── 是否加载模型    [?]
├── 是否使用硬件加速        [ ]  ← 新增
├── 模型文件夹路径          [D:\Models\MyModel]
└── 模型文件夹名称  <MyModel> (只读)
```

### 2. 配置选项说明

| 配置项 | 说明 | 默认值 |
|--------|------|--------|
| **是否加载模型** | 是否启用深度学习模型 | `false` |
| **是否使用硬件加速** | 勾选后使用 TensorRT，否则使用 GPU 运行时 | `false` |
| **模型文件夹路径** | 模型文件所在目录（包含 .hdl 和 .hdict 文件） | 空 |

---

## ?? 运行时选择逻辑

### 代码实现

```csharp
// 在 SolutionModels.cs 的 LoadVppFile 方法中
int runtimeValue = st.bUseHardwareAcceleration ? 2 : 0; // TENSORRT=2, GC=0

object[] parameters = [
  st.ModelPath,
    deviceTypeEnum != null ? Enum.ToObject(deviceTypeEnum, 0) : 0, // DeviceType.GPU
    runtimeEnum != null ? Enum.ToObject(runtimeEnum, runtimeValue) : runtimeValue
];
```

### 枚举值对照表

**DnnInferenceNet.BassClass.Runtime 枚举**:
```csharp
public enum Runtime
{
    GC = 0,     // GPU 运行时（默认）
    OPENVINO = 1,  // OpenVINO 加速
    TENSORRT = 2   // TensorRT 硬件加速
}
```

**DnnInferenceNet.BassClass.DeviceType 枚举**:
```csharp
public enum DeviceType
{
    GPU = 0,       // 使用 GPU
    CPU = 1        // 使用 CPU
}
```

---

## ?? 运行时对比

| 运行时 | 性能 | 兼容性 | 硬件要求 | 适用场景 |
|--------|------|--------|----------|----------|
| **GPU (GC)** | 中等 | ★★★★★ | 普通显卡 | 开发测试、通用场景 |
| **TensorRT** | 极高 | ★★★☆☆ | NVIDIA GPU | 生产环境、高性能需求 |

### TensorRT 硬件要求

1. **显卡**: NVIDIA GPU (支持 CUDA)
2. **驱动**: NVIDIA 显卡驱动 (最新版本)
3. **CUDA**: CUDA Toolkit 已安装
4. **TensorRT**: TensorRT 库已安装并配置

---

## ?? 使用步骤

### 步骤 1: 配置工位

1. 打开方案配置界面
2. 选择目标工位
3. 在 PropertyGrid 中配置：
   - ? 勾选"是否加载模型"
   - ? 勾选"是否使用硬件加速"（如需 TensorRT）
   - ?? 选择"模型文件夹路径"

### 步骤 2: 保存方案

点击保存按钮，配置会写入 `.uv` 文件。

### 步骤 3: 重新加载方案

关闭并重新打开方案，或重启程序，模型将按照配置的运行时加载。

### 步骤 4: 查看日志

查看日志窗口，确认模型加载成功：

```
工位[工位1]深度学习模型加载成功: MyModel (运行时: TensorRT硬件加速)
```

或

```
工位[工位1]深度学习模型加载成功: MyModel (运行时: GPU运行时)
```

---

## ?? 故障排查

### 1. TensorRT 加载失败

**错误信息**:
```
没有检测到可用设备
```

**解决方法**:
- 检查是否安装了 NVIDIA 显卡驱动
- 确认 CUDA 和 TensorRT 是否正确安装
- 尝试使用 GPU 运行时（取消勾选硬件加速）

### 2. 模型初始化失败

**错误信息**:
```
工位[工位1]模型初始化失败，返回码: -1
```

**解决方法**:
- 检查模型文件夹路径是否正确
- 确认模型文件（.hdl 和 .hdict）存在且匹配
- 查看详细日志了解具体错误原因

### 3. 性能未提升

**可能原因**:
- TensorRT 需要首次推理时进行优化（warmup）
- 模型未针对 TensorRT 优化
- 硬件配置不足

**建议**:
- 首次推理可能较慢，多次运行后性能会提升
- 使用专门针对 TensorRT 导出的模型
- 升级硬件配置

---

## ?? 性能优化建议

### 1. 选择合适的运行时

```
场景         推荐运行时
├── 开发调试             → GPU (GC)
├── 生产环境(高性能)     → TensorRT
├── 老旧硬件  → GPU (GC)
└── NVIDIA 显卡          → TensorRT
```

### 2. 模型优化

- 使用 TensorRT 导出的模型（.engine 格式）
- 优化模型结构，减少计算量
- 使用量化模型（INT8/FP16）

### 3. 批量推理

- 设置合适的 Batch Size
- 使用 `Batch_Size` 属性调整批处理大小

---

## ?? 测试验证

### 测试用例

```csharp
// 测试 GPU 运行时
var station1 = new ProcessStation();
station1.bLoadModel = true;
station1.bUseHardwareAcceleration = false;
station1.ModelPath = @"D:\Models\MyModel";
// 预期：使用 Runtime.GC (0)

// 测试 TensorRT 运行时
var station2 = new ProcessStation();
station2.bLoadModel = true;
station2.bUseHardwareAcceleration = true;
station2.ModelPath = @"D:\Models\MyModel";
// 预期：使用 Runtime.TENSORRT (2)
```

### 性能基准测试

建议记录以下指标对比：

| 指标 | GPU 运行时 | TensorRT |
|------|-----------|----------|
| 首次推理时间 | ___ ms | ___ ms |
| 平均推理时间 | ___ ms | ___ ms |
| 吞吐量 (FPS) | ___ | ___ |
| 显存占用 | ___ MB | ___ MB |

---

## ?? 相关代码文件

- **配置定义**: `Vision\Solutions\Models\ProcessStation.cs`
- **模型加载**: `Vision\Solutions\Models\SolutionModels.cs`
- **基础类**: `DLInferenceLib\DnnBase\DeepBase.cs`
- **语义分割**: `DLInferenceLib\DnnBase\DnnSemanticSegmetation.cs`

---

## ? 完成检查清单

- [ ] ProcessStation 添加了 `bUseHardwareAcceleration` 属性
- [ ] SolutionModels 修改了运行时选择逻辑
- [ ] 测试 GPU 运行时模型加载成功
- [ ] 测试 TensorRT 运行时模型加载成功
- [ ] 日志正确显示运行时类型
- [ ] 性能测试验证加速效果

---

**配置完成后，即可享受 TensorRT 带来的性能提升！** ??
