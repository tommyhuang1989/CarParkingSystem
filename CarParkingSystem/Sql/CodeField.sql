
CREATE TABLE IF NOT EXISTS CodeField (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  cid INTEGER NOT NULL,                                   --属于那个类
  field_name TEXT NOT NULL,      			 -- 字段名称
  field_type TEXT NOT NULL,             	 -- 字段类型
  field_length INTEGER DEFAULT 0,		 -- 字段长度
  field_remark TEXT NOT NULL,             	 -- 备注  
  is_main_key INTEGER NOT NULL,             	 -- 是否为主键    
  is_allow_null INTEGER,            	 -- 是否可空    
  is_auto_increment INTEGER,             	 -- 是否为自增长
  is_unique INTEGER,             	 -- 是否唯一
  default_value TEXT             	 -- 默认值    
);