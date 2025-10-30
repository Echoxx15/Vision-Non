# Frm_StationConfig.Designer.cs 手动修改指南

## 需要修改的位置

### 1. 在 InitializeComponent 方法中，找到这一行：

```csharp
this.tsm_LoadImageRun = new System.Windows.Forms.ToolStripMenuItem();
```

### 在其后添加：

```csharp
this.tsm_TriggerCameraRun = new System.Windows.Forms.ToolStripMenuItem();
```

---

### 2. 找到 contextMenuStrip1.Items.AddRange 这一行：

```csharp
this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
    this.tsb_Add,
    this.tsm_Remove,
    this.tsm_ReName,
    this.tsm_LoadImageRun});
```

### 修改为：

```csharp
this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
    this.tsb_Add,
    this.tsm_Remove,
    this.tsm_ReName,
    this.tsm_LoadImageRun,
    this.tsm_TriggerCameraRun});  // 新增这一行
```

---

### 3. 找到 contextMenuStrip1 的 Size 设置：

```csharp
this.contextMenuStrip1.Size = new System.Drawing.Size(233, 124);
```

### 修改为（增加30像素高度）：

```csharp
this.contextMenuStrip1.Size = new System.Drawing.Size(233, 154);
```

---

### 4. 找到 tsm_LoadImageRun 的配置部分：

```csharp
// 
// tsm_LoadImageRun
// 
this.tsm_LoadImageRun.Name = "tsm_LoadImageRun";
this.tsm_LoadImageRun.Size = new System.Drawing.Size(232, 30);
this.tsm_LoadImageRun.Text = "运行检测-加载图片";
```

### 在其后添加：

```csharp
// 
// tsm_TriggerCameraRun
// 
this.tsm_TriggerCameraRun.Name = "tsm_TriggerCameraRun";
this.tsm_TriggerCameraRun.Size = new System.Drawing.Size(232, 30);
this.tsm_TriggerCameraRun.Text = "运行检测-触发相机";
```

---

### 5. 找到文件末尾的字段声明区域：

```csharp
private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
private System.Windows.Forms.ToolStripMenuItem tsm_OpenForm;
private System.Windows.Forms.ToolStripMenuItem tsm_LoadImageRun;
```

### 在其后添加：

```csharp
private System.Windows.Forms.ToolStripMenuItem tsm_TriggerCameraRun;
```

---

## 完整的修改后效果

修改完成后，右键菜单应该显示：
- 添加
- 删除
- 重命名
- 运行检测-加载图片
- 运行检测-触发相机  ← 新增

