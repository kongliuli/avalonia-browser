# avalonia-browser

A dedicated Avalonia browser for accessing and parsing autoglm.zhipuai.cn

## 项目概述

本项目是一个基于 Avalonia 11.1 的专用浏览器项目，旨在访问和解析 autoglm.zhipuai.cn 等网站。项目采用现代化的 MVVM 架构，提供跨平台的桌面应用体验。

## 项目结构

```
avalonia-browser/
├── src/                        # 源代码
│   └── SimpleWebBrowserDemo/   # 极简 Web Browser Demo
│       ├── Models/              # 数据模型
│       ├── ViewModels/          # 视图模型
│       ├── Views/               # 视图
│       ├── Services/            # 服务层
│       ├── Tests/               # 测试项目
│       ├── App.axaml
│       ├── App.axaml.cs
│       ├── Program.cs
│       └── SimpleWebBrowserDemo.csproj
├── docs/                        # 文档目录
│   ├── 20260119/           # 原始设计文档
│   ├── TodoList/             # 待办事项
│   ├── 项目完成总结.md        # 项目完成总结
│   ├── 使用说明.md            # 详细使用说明
│   └── 浏览器验证报告.md    # 验证报告
├── README.md                  # 本文件（项目说明）
├── AvaloniaBrowser.sln         # 解决方案文件
└── .gitignore                # Git 忽略文件
```

## 文档索引

### 📚 项目文档

- [项目完成总结](docs/项目完成总结.md) - 项目开发完成总结，包含已实现功能和技术栈
- [使用说明](docs/使用说明.md) - 详细的使用指南，包含快速开始、功能说明、常见问题等
- [浏览器验证报告](docs/浏览器验证报告.md) - 完整的验证报告，包含功能测试、性能评估、改进建议
- [CefGlue集成与SQLite存储设计方案](docs/CefGlue集成与SQLite存储设计方案.md) - CefGlue 集成与 SQLite 数据存储的完整设计方案
- [CefGlue集成与SQLite存储设计实施总结](docs/CefGlue集成与SQLite存储设计实施总结.md) - CefGlue 集成与 SQLite 存储的设计实施总结

### 📋 设计文档

- [技术方案与功能设计讨论](docs/20260119/Avalonia 专用浏览器项目技术方案与功能设计讨论.md) - 项目整体技术方案和功能设计
- [技术选型分析报告](docs/20260119/Avalonia专用浏览器项目技术选型分析报告.md) - 技术选型详细分析

### ✅ 待办事项

- [极简 Web Browser Demo TODO](docs/TodoList/01-极简WebBrowser-Demo-TODO.md) - 极简版本开发计划
- [集成功能模块 TODO](docs/TodoList/02-AvaloniaBrowser_集成功能模块_TODO.md) - 集成功能模块计划
- [组件化框架基座 TODO](docs/TodoList/03-组件化框架基座-TODO.md) - 组件化框架基座计划
- [组件基座完整项目 TODO](docs/TodoList/04-组件基座完整项目-TODO.md) - 完整项目计划

## 快速开始

### 环境要求

- .NET 8.0 SDK
- Visual Studio 2022 或 VS Code
- （可选）Avalonia Preview Tool

### 运行项目

```bash
cd src/SimpleWebBrowserDemo
dotnet run
```

### 发布应用

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained

# Linux
dotnet publish -c Release -r linux-x64 --self-contained

# macOS
dotnet publish -c Release -r osx-x64 --self-contained
```

## 项目版本

### 当前版本：极简 Web Browser Demo v1.0.0

**功能特性**：
- ✅ 现代化 UI 界面（Fluent 暗色主题）
- ✅ 完整的导航功能（前进、后退、刷新、主页）
- ✅ 智能地址栏（支持 URL 和搜索关键词）
- ✅ 网页加载和显示（基于 HttpClient）
- ✅ MVVM 架构（CommunityToolkit.Mvvm）
- ✅ 跨平台支持（Windows/Linux/macOS）

**技术栈**：
- Avalonia 11.1.0
- .NET 8.0
- CommunityToolkit.Mvvm 8.2.2
- System.Net.Http

**已知限制**：
- ⚠️ 使用 HttpClient 获取 HTML 内容（非真正的浏览器渲染）
- ⚠️ 仅支持基础 HTML 标签过滤
- ⚠️ 不支持 JavaScript 执行、CSS 样式渲染
- ⚠️ 不支持 Cookie 管理、本地存储、下载功能

## 开发路线图

### 阶段一：极简版本（已完成 ✅）

创建基础的浏览器应用，验证 Avalonia 框架的可行性。

**状态**：已完成
**交付物**：SimpleWebBrowserDemo

### 阶段二：集成功能模块（计划中 🔄）

集成所有功能模块，包括搜索、AI 辅助、数据存档等。

**状态**：计划中
**参考文档**：[02-AvaloniaBrowser_集成功能模块_TODO.md](docs/TodoList/02-AvaloniaBrowser_集成功能模块_TODO.md)

### 阶段三：组件化框架基座（计划中 📋）

构建可复用的组件库，为上层应用提供统一的 SDK 接口。

**状态**：计划中
**参考文档**：[03-组件化框架基座-TODO.md](docs/TodoList/03-组件化框架基座-TODO.md)

### 阶段四：完整项目（计划中 📋）

基于组件库构建完整的浏览器应用。

**状态**：计划中
**参考文档**：[04-组件基座完整项目-TODO.md](docs/TodoList/04-组件基座完整项目-TODO.md)

## 验证状态

### 编译状态

✅ **编译成功** - 项目可正常编译，无错误

### 功能测试

✅ **服务层测试通过** - URL 规范化、网页获取、HTML 格式化测试全部通过

✅ **UI 功能验证通过** - 导航、地址栏、页面显示功能正常

### 已知问题

⚠️ **渲染限制** - 当前使用简化渲染方式，仅支持基础 HTML 显示

## 贡献指南

欢迎贡献代码、报告问题或提出建议！

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 许可证

本项目采用 MIT 许可证 - 详见 LICENSE 文件

## 联系方式

- **项目主页**: avalonia-browser
- **问题反馈**: GitHub Issues
- **文档**: docs/ 目录
- **技术支持**: Avalonia Community Discord

## 致谢

- [Avalonia UI](https://avaloniaui.net/) - 跨平台 UI 框架
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) - MVVM 工具包
- [.NET](https://dotnet.microsoft.com/) - 开发平台

---

**最后更新**: 2026-01-20
**当前版本**: 1.0.0
**开发状态**: 极简版本已完成
