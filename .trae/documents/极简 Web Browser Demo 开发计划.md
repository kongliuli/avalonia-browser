# 极简 Web Browser Demo 开发计划

## 项目结构
```
avalonia-browser/
├── src/
│   └── SimpleWebBrowserDemo/
│       ├── ViewModels/
│       │   └── MainViewModel.cs
│       ├── Views/
│       │   └── MainWindow.axaml
│       ├── Services/
│       │   └── NavigationService.cs
│       ├── Models/
│       │   └── BrowserState.cs
│       ├── App.axaml
│       ├── App.axaml.cs
│       └── SimpleWebBrowserDemo.csproj
├── README.md
└── .gitignore
```

## 核心功能实现

### 1. 项目配置
- 创建 Avalonia 11.1 项目，目标框架 .NET 8.0
- 引入必要 NuGet 包：
  - Avalonia (11.1.x)
  - Avalonia.Desktop (11.1.x)
  - Avalonia.Themes.Fluent (11.1.x)
  - CommunityToolkit.Mvvm (8.2.x)
  - Avalonia.HtmlRenderer（用于轻量级 HTML 渲染）

### 2. MVVM 架构
- **MainViewModel**：实现浏览器核心逻辑
  - 地址栏属性（CurrentUrl）
  - 导航命令（GoCommand, BackCommand, ForwardCommand, RefreshCommand）
  - 浏览器状态（CanGoBack, CanGoForward, IsLoading）

### 3. UI 界面
- **MainWindow.axaml**：
  - 顶部工具栏：前进、后退、刷新按钮 + 地址栏输入框
  - 主内容区：HTML 渲染控件容器
  - 应用 Fluent 主题（暗色/亮色）

### 4. 核心服务
- **NavigationService**：
  - URL 验证和规范化
  - 历史记录管理
  - 页面加载状态跟踪

### 5. 数据模型
- **BrowserState**：
  - 当前 URL
  - 历史记录栈
  - 加载状态

## 技术要点
- 使用 CommunityToolkit.Mvvm 的 `[ObservableProperty]` 和 `[RelayCommand]`
- 异步命令处理页面加载
- 错误处理和用户友好提示
- Fluent 主题集成

## 验证标准
✅ 能够成功启动并显示主界面
✅ 能够输入并加载有效的网页 URL
✅ 前进/后退导航功能正常
✅ 页面刷新功能正常
✅ 能够显示页面加载状态
✅ 错误页面能够友好提示

## 交付物
- 完整的 Avalonia 项目源代码
- 可执行的 .exe 文件
- README.md 项目说明文档
- 基础功能测试报告