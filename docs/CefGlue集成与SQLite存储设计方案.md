# CefGlue 集成与 SQLite 数据存储设计方案

## 一、技术选型

### 1.1 CefGlue 介绍

**CefGlue** (Chromium Embedded Framework Glue) 是一个基于 Chromium 的 .NET 绑定库，提供完整的浏览器功能。

**优势**：
- ✅ 完整的 HTML/CSS/JavaScript 支持
- ✅ 现代浏览器特性（WebGL, WebSocket, WebRTC 等）
- ✅ 跨平台支持（Windows, Linux, macOS）
- ✅ 与 Avalonia 良好集成
- ✅ 活跃的社区支持
- ✅ 性能优异

**劣势**：
- ⚠️ 需要额外的 CEF 运行时文件
- ⚠️ 首次启动较慢（需要加载 CEF）
- ⚠️ 应用体积较大（包含 CEF 运行时）

### 1.2 SQLite 介绍

**SQLite** 是一个轻量级的嵌入式关系数据库管理系统。

**优势**：
- ✅ 无需独立服务器
- ✅ 零配置
- ✅ 单文件存储
- ✅ 跨平台支持
- ✅ 事务支持
- ✅ 优秀的性能

**适用场景**：
- 浏览历史存储
- 书签管理
- 用户设置持久化
- 缓存数据

### 1.3 技术栈

```
┌─────────────────────────────────────────────────┐
│          Avalonia UI 层                  │
├─────────────────────────────────────────────────┤
│         MVVM 层（ViewModels）        │
├─────────────────────────────────────────────────┤
│         服务层（Services）               │
├─────────────────────────────────────────────────┤
│         数据层（Data）                   │
│  ┌──────────────────────────────────────┐  │
│  │  SQLite 数据库                    │  │
│  │  (浏览历史、书签、设置）      │  │
│  └──────────────────────────────────────┘  │
├─────────────────────────────────────────────────┤
│         CefGlue WebView 层              │
│  ┌──────────────────────────────────────┐  │
│  │  CEF 运行时                      │  │
│  │  (Chromium 引擎）                │  │
│  └──────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
```

---

## 二、数据库设计

### 2.1 数据库架构

**数据库名称**: `browser.db`

**表结构**：

#### 表 1: 浏览历史（browsing_history）

```sql
CREATE TABLE browsing_history (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    url TEXT NOT NULL,
    title TEXT,
    visit_time INTEGER NOT NULL,
    visit_count INTEGER DEFAULT 1,
    favicon_url TEXT,
    description TEXT
);

CREATE INDEX idx_history_time ON browsing_history(visit_time DESC);
CREATE INDEX idx_history_url ON browsing_history(url);
```

**字段说明**：
- `id`: 主键，自增
- `url`: 访问的 URL
- `title`: 页面标题
- `visit_time`: 访问时间（Unix 时间戳）
- `visit_count`: 访问次数
- `favicon_url`: 网站图标 URL
- `description`: 页面描述

#### 表 2: 书签（bookmarks）

```sql
CREATE TABLE bookmarks (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    url TEXT NOT NULL,
    title TEXT NOT NULL,
    folder_id INTEGER,
    created_at INTEGER NOT NULL,
    favicon_url TEXT,
    description TEXT,
    order_index INTEGER DEFAULT 0
);

CREATE INDEX idx_bookmarks_folder ON bookmarks(folder_id);
CREATE INDEX idx_bookmarks_order ON bookmarks(folder_id, order_index);
```

**字段说明**：
- `id`: 主键，自增
- `url`: 书签 URL
- `title`: 书签标题
- `folder_id`: 所属文件夹 ID（外键）
- `created_at`: 创建时间（Unix 时间戳）
- `favicon_url`: 网站图标 URL
- `description`: 书签描述
- `order_index`: 排序索引

#### 表 3: 书签文件夹（bookmark_folders）

```sql
CREATE TABLE bookmark_folders (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    parent_id INTEGER,
    created_at INTEGER NOT NULL,
    order_index INTEGER DEFAULT 0
);

CREATE INDEX idx_folders_parent ON bookmark_folders(parent_id);
```

**字段说明**：
- `id`: 主键，自增
- `name`: 文件夹名称
- `parent_id`: 父文件夹 ID（外键）
- `created_at`: 创建时间（Unix 时间戳）
- `order_index`: 排序索引

#### 表 4: 用户设置（user_settings）

```sql
CREATE TABLE user_settings (
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL,
    updated_at INTEGER NOT NULL
);
```

**字段说明**：
- `key`: 设置键
- `value`: 设置值
- `updated_at`: 更新时间（Unix 时间戳）

**常用设置键**：
- `home_page`: 主页 URL
- `search_engine`: 搜索引擎 URL
- `theme`: 主题设置（light/dark）
- `default_zoom`: 默认缩放级别
- `enable_javascript`: 是否启用 JavaScript
- `enable_images`: 是否加载图片
- `clear_browsing_data_on_exit`: 退出时清除数据

#### 表 5: 下载历史（downloads）

```sql
CREATE TABLE downloads (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    url TEXT NOT NULL,
    file_name TEXT NOT NULL,
    file_path TEXT NOT NULL,
    file_size INTEGER,
    mime_type TEXT,
    created_at INTEGER NOT NULL,
    completed INTEGER DEFAULT 0
);

CREATE INDEX idx_downloads_time ON downloads(created_at DESC);
```

**字段说明**：
- `id`: 主键，自增
- `url`: 下载源 URL
- `file_name`: 文件名
- `file_path`: 本地文件路径
- `file_size`: 文件大小（字节）
- `mime_type`: MIME 类型
- `created_at`: 创建时间（Unix 时间戳）
- `completed`: 是否完成（0/1）

### 2.2 数据库初始化脚本

```sql
-- 创建所有表
CREATE TABLE IF NOT EXISTS browsing_history (...);
CREATE TABLE IF NOT EXISTS bookmarks (...);
CREATE TABLE IF NOT EXISTS bookmark_folders (...);
CREATE TABLE IF NOT EXISTS user_settings (...);
CREATE TABLE IF NOT EXISTS downloads (...);

-- 创建默认书签文件夹
INSERT INTO bookmark_folders (name, parent_id, created_at, order_index)
VALUES ('未分类', NULL, strftime('%s', 'now'), 0);

-- 创建默认设置
INSERT INTO user_settings (key, value, updated_at)
VALUES 
    ('home_page', 'https://www.bing.com', strftime('%s', 'now')),
    ('search_engine', 'https://www.bing.com/search?q=', strftime('%s', 'now')),
    ('theme', 'dark', strftime('%s', 'now')),
    ('default_zoom', '100', strftime('%s', 'now'));
```

---

## 三、数据模型设计

### 3.1 C# 数据模型

#### BrowsingHistory 模型

```csharp
public class BrowsingHistory
{
    public long Id { get; set; }
    
    public string Url { get; set; } = string.Empty;
    
    public string? Title { get; set; }
    
    public long VisitTime { get; set; }
    
    public int VisitCount { get; set; } = 1;
    
    public string? FaviconUrl { get; set; }
    
    public string? Description { get; set; }
}
```

#### Bookmark 模型

```csharp
public class Bookmark
{
    public long Id { get; set; }
    
    public string Url { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public long? FolderId { get; set; }
    
    public long CreatedAt { get; set; }
    
    public string? FaviconUrl { get; set; }
    
    public string? Description { get; set; }
    
    public int OrderIndex { get; set; } = 0;
}
```

#### BookmarkFolder 模型

```csharp
public class BookmarkFolder
{
    public long Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public long? ParentId { get; set; }
    
    public long CreatedAt { get; set; }
    
    public int OrderIndex { get; set; } = 0;
}
```

#### UserSetting 模型

```csharp
public class UserSetting
{
    public string Key { get; set; } = string.Empty;
    
    public string Value { get; set; } = string.Empty;
    
    public long UpdatedAt { get; set; }
}
```

#### DownloadItem 模型

```csharp
public class DownloadItem
{
    public long Id { get; set; }
    
    public string Url { get; set; } = string.Empty;
    
    public string FileName { get; set; } = string.Empty;
    
    public string FilePath { get; set; } = string.Empty;
    
    public long? FileSize { get; set; }
    
    public string? MimeType { get; set; }
    
    public long CreatedAt { get; set; }
    
    public bool Completed { get; set; }
}
```

### 3.2 仓储接口设计

#### IBrowsingHistoryRepository

```csharp
public interface IBrowsingHistoryRepository
{
    Task<IEnumerable<BrowsingHistory>> GetAllAsync();
    Task<BrowsingHistory?> GetByIdAsync(long id);
    Task<IEnumerable<BrowsingHistory>> GetRecentAsync(int limit = 20);
    Task AddAsync(BrowsingHistory history);
    Task UpdateAsync(BrowsingHistory history);
    Task DeleteAsync(long id);
    Task DeleteAllAsync();
    Task<IEnumerable<BrowsingHistory>> SearchAsync(string query);
}
```

#### IBookmarkRepository

```csharp
public interface IBookmarkRepository
{
    Task<IEnumerable<Bookmark>> GetAllAsync();
    Task<Bookmark?> GetByIdAsync(long id);
    Task<IEnumerable<Bookmark>> GetByFolderAsync(long folderId);
    Task AddAsync(Bookmark bookmark);
    Task UpdateAsync(Bookmark bookmark);
    Task DeleteAsync(long id);
    Task DeleteAllAsync();
    Task MoveToFolderAsync(long bookmarkId, long folderId);
}
```

#### IBookmarkFolderRepository

```csharp
public interface IBookmarkFolderRepository
{
    Task<IEnumerable<BookmarkFolder>> GetAllAsync();
    Task<BookmarkFolder?> GetByIdAsync(long id);
    Task<BookmarkFolder?> GetRootFolderAsync();
    Task AddAsync(BookmarkFolder folder);
    Task UpdateAsync(BookmarkFolder folder);
    Task DeleteAsync(long id);
    Task<IEnumerable<BookmarkFolder>> GetChildrenAsync(long parentId);
}
```

#### IUserSettingsRepository

```csharp
public interface IUserSettingsRepository
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    Task<IEnumerable<UserSetting>> GetAllAsync();
    Task DeleteAsync(string key);
}
```

---

## 四、CefGlue 集成方案

### 4.1 NuGet 包引用

```xml
<PackageReference Include="CefGlue.Avalonia" Version="*" />
<PackageReference Include="CefGlue.Common" Version="*" />
<PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.0" />
<PackageReference Include="Dapper" Version="2.1.28" />
```

### 4.2 CefGlue 控件封装

```csharp
public class CefWebView : UserControl
{
    private CefGlueBrowser? _browser;
    
    public static readonly StyledProperty<string> UrlProperty =
        AvaloniaProperty.Register<CefWebView, string>(
            nameof(Url),
            defaultValue: "about:blank");
    
    public string Url
    {
        get => GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }
    
    public static readonly StyledProperty<bool> CanGoBackProperty =
        AvaloniaProperty.Register<CefWebView, bool>(
            nameof(CanGoBack),
            defaultValue: false);
    
    public bool CanGoBack
    {
        get => GetValue(CanGoBackProperty);
        private set => SetValue(CanGoBackProperty, value);
    }
    
    public static readonly StyledProperty<bool> CanGoForwardProperty =
        AvaloniaProperty.Register<CefWebView, bool>(
            nameof(CanGoForward),
            defaultValue: false);
    
    public bool CanGoForward
    {
        get => GetValue(CanGoForwardProperty);
        private set => SetValue(CanGoForwardProperty, value);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        _browser = new CefGlueBrowser
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        
        _browser.Navigated += OnNavigated;
        _browser.NavigationStateChanged += OnNavigationStateChanged;
        
        Content = _browser;
    }
    
    private void OnNavigated(object? sender, EventArgs e)
    {
        if (_browser != null)
        {
            CanGoBack = _browser.CanGoBack;
            CanGoForward = _browser.CanGoForward;
        }
    }
    
    private void OnNavigationStateChanged(object? sender, EventArgs e)
    {
    }
    
    public void Navigate(string url)
    {
        _browser?.Load(url);
    }
    
    public void GoBack()
    {
        _browser?.GoBack();
    }
    
    public void GoForward()
    {
        _browser?.GoForward();
    }
    
    public void Refresh()
    {
        _browser?.Reload();
    }
}
```

### 4.3 CEF 配置

```csharp
public class CefSettings
{
    public static CefSettings GetDefault()
    {
        return new CefSettings
        {
            CachePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CefCache"),
            UserDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CefUserData"),
            LogFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CefLog.txt"),
            LogSeverity = LogSeverity.Warning,
            PersistSessionCookies = true,
            PersistUserPreferences = true,
            RemoteDebuggingPort = 8088,
            MultiThreadedMessageLoop = true
        };
    }
}
```

---

## 五、服务层设计

### 5.1 数据库服务

```csharp
public class DatabaseService : IDisposable
{
    private readonly string _dbPath;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    
    public DatabaseService()
    {
        var appDataFolder = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);
        _dbPath = Path.Combine(appDataFolder, "browser.db");
    }
    
    public async Task InitializeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();
            
            var initScript = File.ReadAllText("Assets/init.sql");
            await connection.ExecuteAsync(initScript);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task<SqliteConnection> GetConnectionAsync()
    {
        await _semaphore.WaitAsync();
        var connection = new SqliteConnection($"Data Source={_dbPath}");
        await connection.OpenAsync();
        return connection;
    }
    
    public void ReleaseConnection()
    {
        _semaphore.Release();
    }
    
    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}
```

### 5.2 浏览历史服务

```csharp
public class BrowsingHistoryService
{
    private readonly IBrowsingHistoryRepository _repository;
    
    public BrowsingHistoryService(IBrowsingHistoryRepository repository)
    {
        _repository = repository;
    }
    
    public async Task AddVisitAsync(string url, string? title)
    {
        var history = new BrowsingHistory
        {
            Url = url,
            Title = title,
            VisitTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            VisitCount = 1
        };
        
        await _repository.AddAsync(history);
    }
    
    public async Task<IEnumerable<BrowsingHistory>> GetRecentAsync(int limit = 20)
    {
        return await _repository.GetRecentAsync(limit);
    }
    
    public async Task ClearAsync()
    {
        await _repository.DeleteAllAsync();
    }
}
```

---

## 六、UI 更新方案

### 6.1 新增功能面板

#### 历史记录面板

```xml
<Grid>
    <ListBox ItemsSource="{Binding BrowsingHistory}"
              SelectedItem="{Binding SelectedHistoryItem}"
              DoubleTapped="OnHistoryItemDoubleTapped">
        <ListBox.ItemTemplate>
            <DataTemplate>
                <StackPanel Orientation="Horizontal" Spacing="8">
                    <TextBlock Text="{Binding Title}" 
                               FontWeight="SemiBold"/>
                    <TextBlock Text="{Binding VisitTime, Converter={Static DateTimeConverter}}"
                               Opacity="0.7"/>
                </StackPanel>
            </DataTemplate>
        </ListBox.ItemTemplate>
    </ListBox>
</Grid>
```

#### 书签管理面板

```xml
<TabControl>
    <TabItem Header="书签">
        <TreeView ItemsSource="{Binding BookmarkFolders}"
                  SelectedItem="{Binding SelectedFolder}">
            <TreeView.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal" Spacing="8">
                        <TextBlock Text="{Binding Name}"/>
                        <Button Click="OnAddBookmarkClick" 
                                Content="+"/>
                    </StackPanel>
                </DataTemplate>
            </TreeView.ItemTemplate>
        </TreeView>
    </TabItem>
    <TabItem Header="设置">
        <ScrollViewer>
            <StackPanel Spacing="16">
                <CheckBox Content="启用 JavaScript" 
                          IsChecked="{Binding EnableJavaScript}"/>
                <CheckBox Content="启用图片" 
                          IsChecked="{Binding EnableImages}"/>
                <ComboBox Header="主题"
                          Items="{Binding Themes}"
                          SelectedItem="{Binding SelectedTheme}"/>
            </StackPanel>
        </ScrollViewer>
    </TabItem>
</TabControl>
```

### 6.2 ViewModel 更新

```csharp
public partial class MainViewModel : ObservableObject
{
    // 现有属性...
    
    // 新增属性
    [ObservableProperty]
    private ObservableCollection<BrowsingHistory> _browsingHistory = new();
    
    [ObservableProperty]
    private ObservableCollection<Bookmark> _bookmarks = new();
    
    [ObservableProperty]
    private ObservableCollection<BookmarkFolder> _bookmarkFolders = new();
    
    [ObservableProperty]
    private bool _showHistoryPanel;
    
    [ObservableProperty]
    private bool _showBookmarksPanel;
    
    // 新增命令
    [RelayCommand]
    private void ShowHistory()
    {
        ShowHistoryPanel = true;
        ShowBookmarksPanel = false;
    }
    
    [RelayCommand]
    private void ShowBookmarks()
    {
        ShowHistoryPanel = false;
        ShowBookmarksPanel = true;
    }
    
    [RelayCommand]
    private void AddBookmark()
    {
        // 添加当前页面到书签
    }
    
    [RelayCommand]
    private void ClearHistory()
    {
        // 清除历史记录
    }
}
```

---

## 七、实施步骤

### 阶段一：基础设施（1-2 天）

- [ ] 1.1 添加 NuGet 包引用
  - CefGlue.Avalonia
  - CefGlue.Common
  - Microsoft.Data.Sqlite
  - Dapper

- [ ] 1.2 创建数据库初始化脚本
  - Assets/init.sql
  - 创建所有表
  - 插入默认数据

- [ ] 1.3 实现数据模型
  - BrowsingHistory
  - Bookmark
  - BookmarkFolder
  - UserSetting
  - DownloadItem

- [ ] 1.4 实现仓储接口和实现
  - IBrowsingHistoryRepository
  - IBookmarkRepository
  - IBookmarkFolderRepository
  - IUserSettingsRepository

### 阶段二：数据层（2-3 天）

- [ ] 2.1 实现 DatabaseService
  - 数据库连接管理
  - 初始化逻辑
  - 连接池管理

- [ ] 2.2 实现 SQLite 仓储
  - 使用 Dapper 进行数据访问
  - 实现所有 CRUD 操作
  - 添加事务支持

- [ ] 2.3 实现业务服务
  - BrowsingHistoryService
  - BookmarkService
  - UserSettingsService

### 阶段三：CefGlue 集成（3-4 天）

- [ ] 3.1 创建 CefWebView 控件
  - 封装 CefGlueBrowser
  - 实现导航功能
  - 实现事件处理

- [ ] 3.2 集成到主窗口
  - 替换现有的 TextBlock
  - 连接数据绑定
  - 实现页面加载

- [ ] 3.3 实现 CEF 配置
  - CefSettings 类
  - 缓存路径配置
  - 日志配置

### 阶段四：UI 更新（4-5 天）

- [ ] 4.1 更新主窗口
  - 添加侧边栏
  - 添加历史记录面板
  - 添加书签面板

- [ ] 4.2 实现设置界面
  - 用户设置面板
  - 隐私设置
  - 安全设置

- [ ] 4.3 优化用户体验
  - 加载动画
  - 错误提示
  - 快捷键支持

### 阶段五：测试和优化（5-6 天）

- [ ] 5.1 功能测试
  - 浏览历史测试
  - 书签管理测试
  - 设置持久化测试

- [ ] 5.2 性能优化
  - 数据库查询优化
  - 内存管理优化
  - CEF 性能调优

- [ ] 5.3 文档更新
  - 更新 README
  - 更新使用说明
  - 创建集成文档

---

## 八、风险和挑战

### 8.1 技术风险

**CefGlue 集成风险**：
- ⚠️ CEF 运行时文件管理复杂
- ⚠️ 首次启动时间较长
- ⚠️ 应用体积增大
- ⚠️ 需要处理 CEF 进程生命周期

**SQLite 数据库风险**：
- ⚠️ 并发访问需要同步机制
- ⚠️ 大量数据可能影响性能
- ⚠️ 需要定期清理历史数据

### 8.2 性能挑战

**内存管理**：
- CEF 运行时占用大量内存
- 需要实现内存监控
- 可能需要限制历史记录数量

**数据库性能**：
- 历史记录表可能快速增长
- 需要定期清理旧数据
- 需要添加适当的索引

### 8.3 用户体验挑战

**启动时间**：
- CEF 首次启动需要 3-5 秒
- 需要显示加载动画
- 需要友好的错误提示

**兼容性**：
- 不同平台的 CEF 运行时可能不同
- 需要测试多平台
- 需要处理平台特定配置

---

## 九、成功标准

### 9.1 功能完整度

- ✅ 完整的 HTML/CSS/JavaScript 支持
- ✅ 浏览历史持久化
- ✅ 书签管理功能
- ✅ 用户设置持久化
- ✅ 下载管理功能
- ✅ 隐私和安全设置

### 9.2 性能指标

- ✅ 页面加载时间 < 2 秒
- ✅ 数据库查询 < 100ms
- ✅ 内存占用 < 500MB
- ✅ 启动时间 < 5 秒

### 9.3 用户体验

- ✅ 流畅的页面导航
- ✅ 友好的错误提示
- ✅ 直观的设置界面
- ✅ 快速的书签访问

---

**设计版本**: 1.0.0  
**设计日期**: 2026-01-20  
**设计人员**: AI Assistant
