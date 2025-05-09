PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS user (
  id INTEGER PRIMARY KEY AUTOINCREMENT,        -- 用户ID（主键，自增）
  username VARCHAR(32) NOT NULL UNIQUE,        -- 用户名（唯一登录标识）
  password_hash CHAR(64) NOT NULL,             -- 密码哈希值（SHA256长度固定为64）
  salt CHAR(16) NOT NULL,                      -- 加密盐值（16位随机字符串）
  email VARCHAR(100) CHECK(email LIKE '%@%.%'),-- 邮箱（格式校验）
  phone CHAR(11) CHECK(
        LENGTH(phone) = 11 
        AND phone GLOB '[0-9]*'
    ),                                         -- 手机（11位，都是数字）
  status INTEGER DEFAULT 1 NOT NULL,           -- 状态（0=禁用，1=正常，2=锁定）
  created_at TEXT DEFAULT (datetime('now')),   -- 创建时间（UTC时间）
  updated_at TEXT,                             -- 修改时间（UTC时间）
  last_login_time TEXT                         -- 最后登录时间
);

/* 索引说明 */
CREATE INDEX IF NOT EXISTS idx_user_status ON user(status);
CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_email ON user(email);


INSERT OR IGNORE INTO user (username,  password_hash, salt, email, phone, status, created_at, updated_at, last_login_time) values ('admin', '3cKYNgPEmjVqF5paUUrwOkDeUGYshLL8cXGYhWj911k=', 'peugn5F4ewYWQVJsawkMOA==', 'admin@qq.com', '13389542658', '1', '2025-03-05 14:25:26', '2025-03-05 14:25:26', '0000-00-00 00:00:00');

COMMIT;
PRAGMA foreign_keys = ON;