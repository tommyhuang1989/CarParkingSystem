using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Unities
{
    public class SqliteTypeConverter
    {
        private static readonly Dictionary<string, Type> _typeMappings = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
    {
        // 整数类型
        { "INTEGER", typeof(int) },    // SQLite 的 INTEGER 对应 C# 的 Int64（long）//也返回 int
        { "INT", typeof(int) },         // 例如：SQLite 的 INT 对应 Int32（int）
        { "BIGINT", typeof(long) },
        { "SMALLINT", typeof(short) },
        { "TINYINT", typeof(byte) },
        
        // 浮点数类型
        { "REAL", typeof(double) },     // SQLite 的 REAL 默认映射为 C# 的 double
        { "DOUBLE", typeof(double) },
        { "FLOAT", typeof(float) },     // 若需高精度，可用 decimal
        { "DECIMAL", typeof(decimal) },
        { "NUMERIC", typeof(decimal) },
        
        // 字符串与二进制
        { "TEXT", typeof(string) },     // TEXT、VARCHAR 均视为字符串
        { "VARCHAR", typeof(string) },
        { "CHAR", typeof(string) },
        { "CLOB", typeof(string) },
        { "BLOB", typeof(byte[]) },     // 二进制数据对应 byte 数组
        
        // 布尔类型
        { "BOOLEAN", typeof(bool) },    // SQLite 无原生布尔，通常用 0/1 或字符串表示
        { "BOOL", typeof(bool) },
        
        // 时间类型
        { "DATETIME", typeof(DateTime) }, // 日期时间需显式处理字符串转换
        { "DATE", typeof(DateTime) },
        { "TIME", typeof(DateTime) }
    };

        private static readonly Dictionary<string, string> _normalTypeMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // 普通 sql 转换到 sqlite 类型
        { "int", "INTEGER" },    
        { "datetime", "TEXT" },         
        { "varchar", "TEXT" },
        { "nvarchar", "TEXT" },
        { "ntext", "TEXT" },
        { "bit", "INTEGER" },
        { "bigint", "INTEGER" },
        { "money", "DECIMAL(19,4)" },//DECIMAL(19,4)
        { "decimal", "DECIMAL(19,4)" },//DECIMAL(19,4)
        
    };

        /// <summary>
        /// 根据 SQLite 类型名返回 C# 类型
        /// </summary>
        public static Type GetCSharpType(string sqliteTypeName)
        {
            // 去除类型名中的长度修饰（如 VARCHAR(255) → VARCHAR）
            string normalizedType = sqliteTypeName.Split('(')[0].Trim().ToUpper();

            if (_typeMappings.TryGetValue(normalizedType, out Type csharpType))
            {
                return csharpType;
            }

            // 默认返回字符串类型（兼容 SQLite 的动态类型特性）
            return typeof(string);
        }

        public static string GetSqliteType(string sqliteTypeName)
        {
            // 去除类型名中的长度修饰（如 VARCHAR(255) → VARCHAR）
            string normalizedType = sqliteTypeName.Split('(')[0].Trim().ToUpper();

            if (_normalTypeMappings.TryGetValue(normalizedType, out string csharpType))
            {
                return csharpType;
            }

            // 默认返回字符串类型（兼容 SQLite 的动态类型特性）
            return "TEXT";
        }

        public static bool IsDateTime(string sqliteTypeName, string defaultValue)
        {
            // 去除类型名中的长度修饰（如 VARCHAR(255) → VARCHAR）
            string normalizedType = sqliteTypeName.Split('(')[0].Trim().ToUpper();

            if (defaultValue != null && sqliteTypeName.ToUpper().Equals("TEXT") && defaultValue.ToLower().Equals("(datetime('now','localtime'))"))
            {
                return true;
            }

            return false;
        }

        public static List<string> GetSqliteTypes()
        {
            List<string> types = new List<string>();

            if (_typeMappings != null)
            {
                types.AddRange(_typeMappings.Keys);
            }
            return types;
        }
    }
}
