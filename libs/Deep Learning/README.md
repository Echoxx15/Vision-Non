# 深度学习模型插件系统

## 架构概述

本系统采用插件化架构，类似于通讯设备管理模块。通过接口和反射机制实现模型的动态加载和管理。

## 项目结构

```
Deep Learning/
├── DnnInterfaceNet/          # 接口定义库
│   ├── IDnnModel.cs          # 核心模型接口
│   ├── DnnModelTypeAttribute.cs  # 模型类型特性
│   ├── DnnModelConfig.cs     # 配置序列化类
│   ├── DnnPluginServer.cs    # 插件发现和加载
│   └── DnnModelFactory.cs    # 模型工厂（创建/管理模型实例）
│
├── DnnSemanticNet/           # 语义分割插件
│   ├── DnnSemanticSegmentation.cs  # 语义分割模型实现
│   └── SemanticConfigControl.cs    # 配置界面控件
│
├── DnnOCRNet/                # 深度OCR插件
│   ├── DnnDeepOCR.cs         # OCR模型实现
│   └── OCRConfigControl.cs   # 配置界面控件
│
└── DnnInferenceNet/          # 旧版推理库（保留兼容）
```

## 核心接口

### IDnnModel

所有深度学习模型插件必须实现此接口：

```csharp
public interface IDnnModel : IDisposable
{
    string Name { get; }           // 模型实例名称
    bool IsLoaded { get; }         // 是否已加载
    string ModelPath { get; }      // 模型文件路径
    
    UserControl GetConfigControl(); // 获取配置界面
    bool Load(string modelPath, DnnDeviceType deviceType, DnnRuntime runtime);
    void Unload();
    DnnResult Infer(object image); // 推理（图像类型由实现决定）
}
```

### DnnModelTypeAttribute

用于标记模型类型信息：

```csharp
[DnnModelType("语义分割", "基于深度学习的语义分割模型")]
public class DnnSemanticSegmentation : IDnnModel { }
```

## 使用方式

### 1. 加载插件

```csharp
// 在程序启动时初始化插件服务器
DnnPluginServer.Instance.LoadPlugins();
```

### 2. 创建模型实例

```csharp
// 获取可用的模型类型
var types = DnnPluginServer.Instance.GetLoadedPluginTypes();

// 创建模型实例
var model = DnnModelFactory.Instance.CreateModel("语义分割", "我的模型1");
```

### 3. 加载和推理

```csharp
// 加载模型
model.Load(@"C:\Models\MyModel", DnnDeviceType.GPU, DnnRuntime.GC);

// 执行推理
var result = model.Infer(halconImage);
if (result.Success)
{
    // 处理结果
    var segmentationImage = result.ResultImage;
}
```

### 4. 保存和加载配置

```csharp
// 保存配置
DnnModelFactory.Instance.SaveConfigs();

// 程序启动时加载配置
DnnModelFactory.Instance.LoadConfigs();
```

## 插件输出路径

插件项目编译后会自动输出到：
```
Vision\bin\Debug\Plugins\Dnn\
```

## 创建新插件

1. 创建新的类库项目
2. 引用 `DnnInterfaceNet` 项目
3. 实现 `IDnnModel` 接口
4. 添加 `[DnnModelType]` 特性
5. 创建配置界面控件（可选）
6. 将输出路径设置为 Plugins\Dnn 目录

示例：
```csharp
[DnnModelType("目标检测", "YOLO目标检测模型")]
public class DnnObjectDetection : IDnnModel, IConfigurableDnnModel
{
    // 实现接口方法...
}
```

## 配置文件

模型配置保存在 `Configs/DnnModelConfigs.xml`：

```xml
<DnnModelConfigCollection>
  <Models>
    <DnnModelConfig>
      <Name>语义分割模型1</Name>
      <Type>语义分割</Type>
      <ModelPath>D:\Models\Semantic1</ModelPath>
      <DeviceType>GPU</DeviceType>
      <Runtime>GC</Runtime>
      <LoadOnStartup>true</LoadOnStartup>
    </DnnModelConfig>
  </Models>
</DnnModelConfigCollection>
```

## 界面集成

在主程序菜单中添加"深度学习配置"入口，打开 `Frm_DLModelConfig` 窗体进行模型管理。
