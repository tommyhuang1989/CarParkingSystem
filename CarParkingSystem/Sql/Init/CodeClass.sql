CREATE TABLE IF NOT EXISTS CodeClass (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  project_name TEXT NOT NULL,      			 -- 项目名称
  table_name TEXT NOT NULL,             	 -- 表名
  class_name TEXT NOT NULL		 -- 类名  
);

