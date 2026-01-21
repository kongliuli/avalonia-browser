-- 初始化浏览器数据库
-- 创建时间: 2026-01-20

-- 创建浏览历史表
CREATE TABLE IF NOT EXISTS browsing_history (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    url TEXT NOT NULL,
    title TEXT,
    visit_time INTEGER NOT NULL,
    visit_count INTEGER DEFAULT 1,
    favicon_url TEXT,
    description TEXT
);

-- 创建浏览历史索引
CREATE INDEX IF NOT EXISTS idx_history_time ON browsing_history(visit_time DESC);
CREATE INDEX IF NOT EXISTS idx_history_url ON browsing_history(url);

-- 创建书签表
CREATE TABLE IF NOT EXISTS bookmarks (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    url TEXT NOT NULL,
    title TEXT NOT NULL,
    folder_id INTEGER,
    created_at INTEGER NOT NULL,
    favicon_url TEXT,
    description TEXT,
    order_index INTEGER DEFAULT 0
);

-- 创建书签索引
CREATE INDEX IF NOT EXISTS idx_bookmarks_folder ON bookmarks(folder_id);
CREATE INDEX IF NOT EXISTS idx_bookmarks_order ON bookmarks(folder_id, order_index);

-- 创建书签文件夹表
CREATE TABLE IF NOT EXISTS bookmark_folders (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    parent_id INTEGER,
    created_at INTEGER NOT NULL,
    order_index INTEGER DEFAULT 0
);

-- 创建文件夹索引
CREATE INDEX IF NOT EXISTS idx_folders_parent ON bookmark_folders(parent_id);

-- 创建用户设置表
CREATE TABLE IF NOT EXISTS user_settings (
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL,
    updated_at INTEGER NOT NULL
);

-- 创建下载历史表
CREATE TABLE IF NOT EXISTS downloads (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    url TEXT NOT NULL,
    file_name TEXT NOT NULL,
    file_path TEXT NOT NULL,
    file_size INTEGER,
    mime_type TEXT,
    created_at INTEGER NOT NULL,
    completed INTEGER DEFAULT 0
);

-- 创建下载历史索引
CREATE INDEX IF NOT EXISTS idx_downloads_time ON downloads(created_at DESC);

-- 插入默认书签文件夹
INSERT INTO bookmark_folders (name, parent_id, created_at, order_index)
VALUES ('未分类', NULL, strftime('%s', 'now'), 0);

-- 插入默认用户设置
INSERT INTO user_settings (key, value, updated_at)
VALUES 
    ('home_page', 'https://www.bing.com', strftime('%s', 'now')),
    ('search_engine', 'https://www.bing.com/search?q=', strftime('%s', 'now')),
    ('theme', 'dark', strftime('%s', 'now')),
    ('default_zoom', '100', strftime('%s', 'now')),
    ('enable_javascript', 'true', strftime('%s', 'now')),
    ('enable_images', 'true', strftime('%s', 'now')),
    ('clear_browsing_data_on_exit', 'false', strftime('%s', 'now')),
    ('max_history_items', '100', strftime('%s', 'now')),
    ('show_bookmarks_bar', 'true', strftime('%s', 'now'));
