
CREATE TABLE IF NOT EXISTS role (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  role_code TEXT NOT NULL UNIQUE,      -- 角色编码（如ADMIN/USER）
  role_name TEXT NOT NULL,             -- 角色显示名称（如"系统管理员"）
	pid INTEGER DEFAULT 0,      -- 父角色ID（实现角色继承层级）
  inherit_parent BOOLEAN DEFAULT 0,    -- 是否继承父角色权限
  description TEXT,
  created_at TEXT DEFAULT (datetime('now','localtime'))
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_role_code ON role(role_code);

