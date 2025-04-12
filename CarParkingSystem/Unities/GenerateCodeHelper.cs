using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Styling;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Common;
using CarParkingSystem.Dao;
using CarParkingSystem.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using SukiUI.ColorTheme;
using SukiUI.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Resources.Extensions;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CarParkingSystem.Unities
{
    public class GenerateCodeHelper
    {
        public static Dictionary<string, string> _needTranslateKey = new Dictionary<string, string>();
        private static readonly int SEARCH_COLUMN_NUMBER = 8;//搜索栏每行显示 4组，8列
        //public static string TableNameStr = string.Empty;//因为 EF 的 DbSet 是复数，如 Users，所以这里改一下//20250403,不需要手动加 s 了
        ///" + obj.ProjectName + "
        public static bool Run(CodeClass obj)
        {
            //TableNameStr = obj.TableName + "s";
            _needTranslateKey.Clear();//每次使用前先清空

            CreateFile(obj.ProjectName + "\\Sql\\Generate\\" + obj.TableName + ".sql", CreateGenerateSql(obj));
            CreateFile(obj.ProjectName + "\\Sql\\Init\\" + obj.TableName + ".sql", CreateInitSql(obj));

            CreateFile(obj.ProjectName + "\\Messages\\Selected" + obj.ClassName + "Message.cs", CreateMessages(obj));
            CreateFile(obj.ProjectName + "\\Models\\" + obj.ClassName + ".cs", CreateCs(obj));
            CreateFile(obj.ProjectName + "\\Dao\\" + obj.ClassName + "Dao.cs", CreateDao(obj));

            CreateFile(obj.ProjectName + "\\ViewModels\\" + obj.ClassName + "ActionWindowViewModel.cs", CreateActionWindowViewModel(obj));
            CreateFile(obj.ProjectName + "\\Views\\" + obj.ClassName + "ActionWindow.axaml.cs", CreateActionWindowCs(obj));
            CreateFile(obj.ProjectName + "\\Views\\" + obj.ClassName + "ActionWindow.axaml", CreateActionWindow(obj));

            CreateFile(obj.ProjectName + "\\ViewModels\\" + obj.ClassName + "ViewModel.cs", CreateViewModel(obj));
            CreateFile(obj.ProjectName + "\\Views\\" + obj.ClassName + "View.axaml.cs", CreateViewCs(obj));
            CreateFile(obj.ProjectName + "\\Views\\" + obj.ClassName + "View.axaml", CreateView(obj));

            AddToastRefreshToken(obj.ProjectName + "\\Unities\\TokenManage.cs", obj.ClassName);
            AddToastCloseToken(obj.ProjectName + "\\Unities\\TokenManage.cs", obj.ClassName);

            AddServices(obj.ProjectName + "\\App.axaml.cs", obj.ClassName + "ActionWindow", obj.ClassName + "ActionWindowViewModel");
            AddServices(obj.ProjectName + "\\App.axaml.cs", obj.ClassName + "View", obj.ClassName + "ViewModel");//View、View Model 注册
            AddServicesWithDao(obj.ProjectName + "\\App.axaml.cs", obj.ClassName + "Dao", obj.ClassName + "Dao");//Dao 注册

            AddDbSet(obj.ProjectName + "\\Dao\\AppDbContext.cs", obj.ClassName);
            AddBuildTable(obj.ProjectName + "\\Dao\\AppDbContext.cs", obj.ClassName);//20250403, add
            AddBuildTableMapping(obj.ProjectName + "\\Dao\\AppDbContext.cs", CreateBuildTableMapping(obj));
            AddTranslates(obj.ProjectName + "\\I18n\\Resource.resx");

            return true;
        }
        public static bool RunBK(CodeClass obj)
        {
            //TableNameStr = obj.TableName + "s";
            CreateFile("../Sql/" + obj.TableName + ".sql", CreateGenerateSql(obj));
            CreateFile("../Messages/Selected" + obj.ClassName + "Message.cs", CreateMessages(obj));//创建 model 选中的 Messages，如 SelectedUserMessages
            //CreateFile("../Messages/Create" + obj.ClassName + "TableMessage.cs", CreateTableMessages(obj));//创建通知 MainVM 创建数据库的 Messages，如 CreateUserTableMessages
            CreateFile("../Models/" + obj.ClassName + ".cs", CreateCs(obj));//创建 model，如 User
            AddToastRefreshToken(obj.ProjectName + "\\Unities\\TokenManage.cs", obj.ClassName);

            CreateFile("../Dao/" + obj.ClassName + "Dao.cs", CreateDao(obj));//创建 modelDao，如 UserDao
            CreateFile("../Views/" + obj.ClassName + "ActionWindow.axaml", CreateActionWindow(obj));//创建 model 新增、更改的界面，如 UserActionWindow.axaml
            CreateFile("../Views/" + obj.ClassName + "ActionWindow.axaml.cs", CreateActionWindowCs(obj));//创建 model 新增、更改的界面，如 UserActionWindow.axaml.cs
            AddToastCloseToken(obj.ProjectName + "\\Unities\\TokenManage.cs", obj.ClassName);

            CreateFile("../ViewModels/" + obj.ClassName + "ActionWindowViewModel.cs", CreateActionWindowViewModel(obj));
            CreateFile("../Views/" + obj.ClassName + "View.axaml", CreateView(obj));//创建 model 新增、更改的界面，如 UserActionWindow.axaml
            CreateFile("../Views/" + obj.ClassName + "View.axaml.cs", CreateViewCs(obj));//创建 model 新增、更改的界面，如 UserActionWindow.axaml.cs
            CreateFile("../ViewModels/" + obj.ClassName + "ViewModel.cs", CreateViewModel(obj));
            AddServices(obj.ProjectName + "\\App.axaml.cs", obj.ClassName + "ActionWindow", obj.ClassName + "ActionWindowViewModel");//添加 IoC 注册
            AddServices(obj.ProjectName + "\\App.axaml.cs", obj.ClassName + "View", obj.ClassName + "ViewModel");//添加 IoC 注册
            AddDbSet(obj.ProjectName + "\\Dao.AppDbContext.cs", obj.ClassName);//添加 DbSet 注册

            return true;
        }

        /// <summary>
        /// 创建一级菜单的
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Run(CodeClassFirstFloor obj)
        {
            _needTranslateKey.Clear();//每次使用前先清空
            CreateFile(obj.ProjectName + "\\ViewModels\\" + obj.ClassName + "ViewModel.cs", CreateViewModel(obj));

            CreateFile(obj.ProjectName + "\\Views\\" + obj.ClassName + "View.axaml.cs", CreateViewCs(obj));//创建 model 新增、更改的界面，如 UserActionWindow.axaml.cs
            CreateFile(obj.ProjectName + "\\Views\\" + obj.ClassName + "View.axaml", CreateView(obj));//创建 model 新增、更改的界面，如 UserActionWindow.axaml

            AddServices(obj.ProjectName + "\\App.axaml.cs", obj.ClassName+"View", obj.ClassName+"ViewModel");//添加 IoC 注册
            AddTranslateKeys(obj.ClassName.ToString(), obj.ClassName.ToString());
            AddTranslates(obj.ProjectName + "\\I18n\\Resource.resx");
            return true;
        }
        public static void EnsureDirectoryExists(string filePath)
        {
            // 提取文件路径中的目录部分（自动处理路径分隔符）
            string directoryPath = System.IO.Path.GetDirectoryName(filePath);

            // 当目录路径非空时创建目录
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath); // 自动递归创建所有缺失的目录
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="contentstr"></param>
        private static void CreateFile(string filename, string contentstr)
        {
            //AppContext.BaseDirectory 获取的是 exe 运行的路径，所以需要往前 4级才能找到解决方案的目录
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            //string basePath = @"E:\CodeGenerator\";
            string filePath = System.IO.Path.Combine(basePath, filename);
            //var path = HttpContext.Current.Server.MapPath("../TempFiles");
            //var pp = Directory.GetParent(filePath).Parent.FullName;
            EnsureDirectoryExists(filePath);
            //if (!Directory.Exists(pp))
            //{
            //    Directory.CreateDirectory(pp);
            //}
            //var sql_path = path + "/Sql";
            //if (!Directory.Exists(sql_path))
            //{
            //    Directory.CreateDirectory(sql_path);
            //}
            //var cs_path = path + "/Components";
            //if (!Directory.Exists(cs_path))
            //{
            //    Directory.CreateDirectory(cs_path);
            //}

            //filename = HttpContext.Current.Server.MapPath(filename);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            FileStream mystream = new FileStream(filePath, FileMode.Create);
            StreamWriter mywriter = new StreamWriter(mystream, Encoding.Unicode);
            try
            {
                //写入数据
                mywriter.WriteLine(contentstr);
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                mywriter.Flush();
                mywriter.Close();
            }

            //using (FileStream mystream = new FileStream(
            //filePath,
            //FileMode.Create,
            //FileAccess.ReadWrite,
            //FileShare.ReadWrite)) // 允许其他进程读写
            //{
            //    StreamWriter mywriter = new StreamWriter(mystream, Encoding.Unicode);
            //}
        }
        private static void CreateDbTable(string filename, string contentstr)
        {
            //AppContext.BaseDirectory 获取的是 exe 运行的路径，所以需要往前 4级才能找到解决方案的目录
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            //string basePath = @"E:\CodeGenerator\";
            string filePath = System.IO.Path.Combine(basePath, filename);

        }

        #region 辅助方法
            // 主操作方法
        public static void InsertCodeAtMarker(string filePath,
                                      string marker,
                                      string newCode,
                                      bool createBackup = true)
        {
            try
            {
                // 读取文件内容
                var content = new StringBuilder(File.ReadAllText(filePath, Encoding.UTF8));

                // 先判断是否已存在相同代码
                int newCodeIndex = IndexOf(content, newCode);
                if (newCodeIndex > -1)
                {
                    return;
                }
                    // 查找标记位置
                    int insertIndex = IndexOf(content, marker);
                if (insertIndex == -1)
                {
                    //MessageBox.Show("标记未找到");
                    return;
                }

                // 计算实际插入位置（标记行末尾）
                //insertIndex += marker.Length + Environment.NewLine.Length;
                insertIndex += marker.Length;

                // 插入新代码
                content.Insert(insertIndex, $"\n{newCode}\n");

                // 创建备份
                if (createBackup)
                    File.Copy(filePath, $"{filePath}.bak", true);

                // 保存修改
                File.WriteAllText(filePath, content.ToString(), Encoding.UTF8);

                //MessageBox.Show("代码插入成功");
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"操作失败：{ex.Message}");
            }
        }

        public static void InsertCodeAtMarkerBefore(string filePath,
                                      string marker,
                                      string newCode,
                                      bool createBackup = true)
        {
            try
            {
                // 读取文件内容
                var content = new StringBuilder(File.ReadAllText(filePath, Encoding.UTF8));

                // 先判断是否已存在相同代码
                int newCodeIndex = IndexOf(content, newCode);
                if (newCodeIndex > -1)
                {
                    return;
                }
                // 查找标记位置
                int insertIndex = IndexOf(content, marker);
                if (insertIndex == -1)
                {
                    //MessageBox.Show("标记未找到");
                    return;
                }

                // 计算实际插入位置（标记行末尾）
                //insertIndex += marker.Length + Environment.NewLine.Length;
                
                //insertIndex += marker.Length;//20250403, 直接插入，不换行？

                // 插入新代码
                content.Insert(insertIndex, $"\n{newCode}\n");

                // 创建备份
                if (createBackup)
                    File.Copy(filePath, $"{filePath}.bak", true);

                // 保存修改
                File.WriteAllText(filePath, content.ToString(), Encoding.UTF8);

                //MessageBox.Show("代码插入成功");
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"操作失败：{ex.Message}");
            }
        }

        // 扩展方法：查找字符串索引 (this StringBuilder sb, string value)
        private static int IndexOf(StringBuilder sb, string value)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                if (i + value.Length > sb.Length) break;

                bool match = true;
                for (int j = 0; j < value.Length; j++)
                {
                    if (sb[i + j] != value[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return i;
            }
            return -1;
        }
        #endregion

        /// <summary>
        /// 创建当在表格中勾选时发送的消息类型
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="className"></param>
        private static void AddToastRefreshToken(string filename, string className)
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            string path = System.IO.Path.Combine(basePath, filename);

            string marker = "public static readonly string REFRESH_SUMMARY_SELECTED_USER_TOKEN = \"RefreshSummarySelectedUserToken\";";
            string newCode = $"public static readonly string REFRESH_SUMMARY_SELECTED_{className.ToUpper()}_TOKEN = \"RefreshSummarySelected{className}Token\";";
            InsertCodeAtMarker(path, marker, newCode, createBackup: false);
        }
        private static void AddToastCloseToken(string filename, string className)
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            string path = System.IO.Path.Combine(basePath, filename);

            string marker = "public static readonly string USER_ACTION_WINDOW_CLOSE_TOKEN = \"UserActionWindowCloseToken\";";
            string newCode = $"public static readonly string {className.ToUpper()}_ACTION_WINDOW_CLOSE_TOKEN = \"{className}ActionWindowCloseToken\";";
            InsertCodeAtMarker(path, marker, newCode, createBackup: false);
        }

        /// <summary>
        /// 在 AppDbContext.cs 文件中添加 DbSet 的内容
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="className"></param>
        private static void AddDbSet(string filename, string className)
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            string path = System.IO.Path.Combine(basePath, filename);

            string marker = "public DbSet<User> Users { get; set; }";
            string newCode = $"public DbSet<{className}> {className}s {{ get; set; }}";
            InsertCodeAtMarker(path, marker, newCode, createBackup: false);
        }
        /// <summary>
        /// 在 AppDbContext.cs 文件中添加 Build{0}Table 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="className"></param>
        private static void AddBuildTable(string filename, string className)
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            string path = System.IO.Path.Combine(basePath, filename);

            string marker = "BuildUserTable(modelBuilder);";
            string newCode = $"Build{className}Table(modelBuilder);";
            InsertCodeAtMarker(path, marker, newCode, createBackup: false);
        }

        /// <summary>
        /// 在 AppDbContext.cs 文件中添加 Build{0}Table 的内容,进行表的字段映射
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="className"></param>
        private static void AddBuildTableMapping(string filename, string content)
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            string path = System.IO.Path.Combine(basePath, filename);

            string marker = "BuildCodeFieldTable(modelBuilder);\r\n        }";

            InsertCodeAtMarker(path, marker, content, createBackup: false);
        }

        /// <summary>
        /// 添加 IoC 注册，注册 View 和 ViewModel
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="viewName"></param>
        /// <param name="viewModelName"></param>
        private static void AddServices(string filename, string viewName, string viewModelName)
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            string path = System.IO.Path.Combine(basePath, filename);

            string marker = ".AddView<UserView, UserViewModel>(services)";
            string newCode = $".AddView<{viewName}, {viewModelName}>(services)";
            InsertCodeAtMarker(path, marker, newCode, createBackup: false);
        }
        private static void AddServicesWithDao(string filename, string daoType, string dao)
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            string path = System.IO.Path.Combine(basePath, filename);

            string marker = "services.TryAddSingleton<UserDao, UserDao>();";
            string newCode = $"services.TryAddSingleton<{daoType}, {dao}>();";
            InsertCodeAtMarker(path, marker, newCode, createBackup: false);
        }

        

        public static void AddTranslateKeys(string key, string value)
        {
            var valueStr = Functions.SplitWords(value);
            if (!_needTranslateKey.ContainsKey(key))
            {
                _needTranslateKey.Add(key, valueStr);
            }
        }

        /// <summary>
        /// 添加需要翻译的自动到 ResX 资源文件，后续需要人工操作进行自动翻译
        /// </summary>
        /// <param name="filename"></param>
        private static void AddTranslates(string filename)
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            string path = System.IO.Path.Combine(basePath, filename);
            AddDataNodeIfNotExists(path, _needTranslateKey);
        }

        public static void AddDataNodeIfNotExists(string filePath, Dictionary<string, string> needTranslateKeys)
        {
            if (needTranslateKeys == null || needTranslateKeys.Count == 0) return;
            /*
            string parentDir = System.IO.Path.GetDirectoryName(filePath);
            List<string> files = new List<string>();
            var otherFiles = Directory.GetFiles(parentDir, "Resource.*.resx");
            files.AddRange(otherFiles);
            files.Add(filePath);//需要保证最后修改的是英语
            foreach (string file in files)
            {
            */
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true; // 保留原有格式

                try
                {
                    
                    // 尝试加载现有 XML 文件
                    xmlDoc.Load(filePath);
                }
                catch (System.IO.FileNotFoundException)
                {
                    // 文件不存在时创建基础结构
                    XmlDeclaration decl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                    xmlDoc.AppendChild(decl);
                    XmlElement root = xmlDoc.CreateElement("root");
                    xmlDoc.AppendChild(root);
                }

                // 获取根节点
                XmlElement rootNode = xmlDoc.DocumentElement;
                if (rootNode == null)
                {
                    throw new InvalidOperationException("XML 文件缺少根节点 <root>");
                }


                foreach (var item in needTranslateKeys)
                {
                    var name = item.Key;
                    var value = item.Value;
                    // 检查 name 是否已存在
                    string xpath = $"/root/data[@name='{name}']";
                    XmlNode existingNode = xmlDoc.SelectSingleNode(xpath);
                    if (existingNode != null)
                    {
                        Console.WriteLine($"名称 '{name}' 已存在，无需添加");
                        continue;//return;
                    }

                    // 创建新 data 节点
                    XmlElement dataNode = xmlDoc.CreateElement("data");
                    dataNode.SetAttribute("name", name);
                    dataNode.SetAttribute("xml:space", "preserve"); // 保留空白字符

                    // 创建 value 子节点
                    XmlElement valueNode = xmlDoc.CreateElement("value");
                    valueNode.InnerText = value;

                    //var fileName = System.IO.Path.GetFileName(file);
                    //if (fileName.Equals("Resource.resx"))//只给英语的赋值，其他语言保留空值，后续使用自动翻译补充
                    //{
                    //    valueNode.InnerText = value;
                    //}else
                    //{
                    //    valueNode.InnerText = value;// string.Empty;
                    //}
                    dataNode.AppendChild(valueNode);

                    // 将新节点追加到根节点末尾
                    rootNode.AppendChild(dataNode);
                }
               


                // 保存文件
                xmlDoc.Save(filePath);
                //Console.WriteLine($"节点 '{name}' 添加成功");
            //}
        }

        public static void UpdateResourceFile(string filePath, string key, object value)
        {
            // 如果文件不存在，创建空文件
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            // 读取现有资源到字典
            var existingKeys = new Dictionary<string, object>();
            using (var reader = new DeserializingResourceReader(filePath))
            {
                foreach (DictionaryEntry entry in reader)
                {
                    existingKeys[entry.Key.ToString()] = entry.Value;
                }
            }

            // 判断键是否存在，若不存在则添加
            if (!existingKeys.ContainsKey(key))
            {
                existingKeys.Add(key, value);

                // 写入更新后的资源
                using (var writer = new PreserializedResourceWriter(filePath))
                {
                    foreach (var item in existingKeys)
                    {
                        writer.AddResource(item.Key, item.Value);
                    }
                }
            }
        }

        private static void AppendFile(string filename, string contentstr)
        {
            //AppContext.BaseDirectory 获取的是 exe 运行的路径，所以需要往前 4级才能找到解决方案的目录
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            string path = System.IO.Path.Combine(basePath, filename);
            //var path = HttpContext.Current.Server.MapPath("../TempFiles");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //var sql_path = path + "/Sql";
            //if (!Directory.Exists(sql_path))
            //{
            //    Directory.CreateDirectory(sql_path);
            //}
            //var cs_path = path + "/Components";
            //if (!Directory.Exists(cs_path))
            //{
            //    Directory.CreateDirectory(cs_path);
            //}

            //filename = HttpContext.Current.Server.MapPath(filename);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            FileStream mystream = new FileStream(filename, FileMode.Create);
            StreamWriter mywriter = new StreamWriter(mystream, Encoding.Unicode);
            //写入数据
            mywriter.WriteLine(contentstr);
            mywriter.Flush();
            mywriter.Close();
        }

        #region 创建一级菜单的
        /// <summary>
        /// 根据类名创建 View 类，(UserControl, 点击左侧菜单时，在右侧显示的内容)
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateView(CodeClassFirstFloor enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("<UserControl xmlns=\"https://github.com/avaloniaui\"\r\n");
            str.Append("             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n");
            str.Append("             xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"\r\n");
            str.Append("             xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"\r\n");
            str.Append("             mc:Ignorable=\"d\" d:DesignWidth=\"800\" d:DesignHeight=\"450\"\r\n");
            str.Append(string.Format("             x:Class=\"{0}.Views.{1}View\">\r\n", enu.ProjectName, enu.ClassName));
            str.Append(string.Format("    {0}View\r\n", enu.ClassName));
            str.Append("</UserControl>\r\n");

            return str.ToString();
        }

        /// <summary>
        /// 根据类名创建 ModelView 类的后台代码
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateViewCs(CodeClassFirstFloor enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("using Avalonia;\r\n");
            str.Append("using Avalonia.Controls;\r\n");
            str.Append("using Avalonia.Markup.Xaml;\r\n");
            str.Append("\r\n");
            str.Append("namespace "+ enu.ProjectName +".Views;\r\n");
            str.Append("\r\n");
            str.Append(string.Format("public partial class {0}View : UserControl\r\n", enu.ClassName));
            str.Append("{\r\n");
            str.Append(string.Format("    public {0}View()\r\n", enu.ClassName));
            str.Append("    {\r\n");
            str.Append("        InitializeComponent();\r\n");
            str.Append("    }\r\n");
            str.Append("}\r\n");

            return str.ToString();
        }

        private static string CreateViewModel(CodeClassFirstFloor enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("using Avalonia.Controls;\r\n");
            str.Append("using Avalonia.Controls.ApplicationLifetimes;\r\n");
            str.Append("using CarParkingSystem.Controls;\r\n");
            str.Append("using CarParkingSystem.Dao;\r\n");
            str.Append("using CarParkingSystem.Models;\r\n");
            str.Append("using CarParkingSystem.Unities;\r\n");
            str.Append("using CarParkingSystem.Views;\r\n");
            str.Append("using CommunityToolkit.Mvvm.ComponentModel;\r\n");
            str.Append("using CommunityToolkit.Mvvm.Input;\r\n");
            str.Append("using CommunityToolkit.Mvvm.Messaging;\r\n");
            str.Append("using LinqKit;\r\n");
            str.Append("using Material.Icons;\r\n");
            str.Append("using Microsoft.Extensions.DependencyInjection;\r\n");
            str.Append("using SukiUI.Dialogs;\r\n");
            str.Append("using System;\r\n");
            str.Append("using System.Collections.Generic;\r\n");
            str.Append("using System.Collections.ObjectModel;\r\n");
            str.Append("using System.Data;\r\n");
            str.Append("using System.Linq;\r\n");
            str.Append("using System.Linq.Expressions;\r\n");
            str.Append("using System.Text;\r\n");
            str.Append("using System.Threading.Tasks;\r\n");
            str.Append("using static CarParkingSystem.ViewModels.MainWindowViewModel;\r\n");
            str.Append("\r\n");
            str.Append("namespace " + enu.ProjectName + ".ViewModels{\r\n");
            str.Append("    /// <summary>\r\n");
            str.Append("    /// 一级菜单\r\n");
            str.Append("    /// </summary>\r\n");
            str.Append(string.Format("    public partial class {0}ViewModel : DemoPageBase\r\n", enu.ClassName));
            str.Append("    {\r\n");
            str.Append(string.Format("        public {0}ViewModel() : base(\"{0}\", MaterialIconKind.{1}, pid: {2}, id: {3}, index: {4})\r\n", enu.ClassName, enu.Icon, enu.Pid, enu.Id, enu.Index));
            str.Append("        {\r\n");
            str.Append("        }\r\n");
            str.Append("    }\r\n");
            str.Append("}\r\n");

            return str.ToString();
        }
        #endregion

        #region 创建二级菜单的
        /*
        private static string CreateRequest(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("using System;\r\n");
            str.Append("using System.Collections;\r\n");
            str.Append("using System.Collections.Generic;\r\n");
            str.Append("using System.Text;\r\n");
            str.Append("using System.Web;\r\n");
            str.Append("using System.Data;\r\n");
            str.Append("using Core;\r\n");
            str.Append("namespace " + enu.g_ProjectId + "\r\n");
            str.Append("{\r\n");
            str.Append(string.Format("public class {0}Request:BaseRequest\r\n", enu.ClassName));
            str.Append("{\r\n");

            string fieldtype = ";";
            List<CodeField> al = CodeFields.GetDatas(enu.AysnId);
            foreach (CodeField field in al)
            {
                if (fieldtype.IndexOf(";" + Formatter.GetTypestr(field.FieldType) + ";") == -1)
                {
                    fieldtype = fieldtype + Formatter.GetTypestr(field.FieldType) + ";";
                }
                if (field.IsMainKey == false)
                {
                    str.Append("public  " + Formatter.GetTypestr(field.FieldType) + " " + field.FieldName + " {get;set;} \r\n");
                }
            }

            str.Append("}\r\n");
            str.Append("}\r\n");

            return str.ToString();
        }
        */

        /// <summary>
        /// 根据类名创建 Messages类
        /// 创建 model 选中的 Messages，如 SelectedUserMessages
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateMessages(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("using Avalonia.Controls.Notifications;\r\n");
            str.Append("using System;\r\n");
            str.Append("using System.Collections.Generic;\r\n");
            str.Append("using System.Linq;\r\n");
            str.Append("using System.Text;\r\n");
            str.Append("using System.Threading.Tasks;\r\n");
            str.Append("\r\n");
            str.Append("namespace "+ enu.ProjectName +".Messages\r\n");
            str.Append("{\r\n");
            str.Append("    /// <summary>\r\n");
            str.Append("    /// 当在表格中勾选时，发送的消息类型\r\n");
            str.Append("    /// </summary>\r\n");
            str.Append("    public record Selected"+enu.ClassName+"Message\r\n");
            str.Append("    {\r\n");
            str.Append("        public string Title { get; set; }\r\n");
            str.Append("        public string Content { get; set; }\r\n");
            str.Append("\r\n");
            str.Append("    }\r\n");
            str.Append("}\r\n");


            return str.ToString();
        }

        /// <summary>
        /// 根据类名创建 Messages类
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateTableMessages(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("using Avalonia.Controls.Notifications;\r\n");
            str.Append("using System;\r\n");
            str.Append("using System.Collections.Generic;\r\n");
            str.Append("using System.Linq;\r\n");
            str.Append("using System.Text;\r\n");
            str.Append("using System.Threading.Tasks;\r\n");
            str.Append("\r\n");
            str.Append("namespace " + enu.ProjectName + ".Messages\r\n");
            str.Append("{\r\n");
            str.Append(string.Format("    public record Create{0}TableMessage\r\n", enu.ClassName));
            str.Append("    {\r\n");
            str.Append("        public string Title { get; set; }\r\n");
            str.Append("        public string Content { get; set; }\r\n");
            str.Append("        public string Path { get; set; }\r\n");
            str.Append("\r\n");
            str.Append("    }\r\n");
            str.Append("}\r\n");

            return str.ToString();
        }

        /// <summary>
        /// 根据类名创建 实体类
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateCs(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("using CarParkingSystem.Common;\r\n");
            str.Append("using CarParkingSystem.Messages;\r\n");
            str.Append("using CarParkingSystem.Unities;\r\n");
            str.Append("using CarParkingSystem.ViewModels;\r\n");
            str.Append("using CommunityToolkit.Mvvm.ComponentModel;\r\n");
            str.Append("using CommunityToolkit.Mvvm.Input;\r\n");
            str.Append("using CommunityToolkit.Mvvm.Messaging;\r\n");
            str.Append("using System.ComponentModel.DataAnnotations.Schema;\r\n"); 
            str.Append("using System;\r\n");
            str.Append("using System.Collections.Generic;\r\n");
            str.Append("using System.Linq;\r\n");
            str.Append("using System.Text;\r\n");
            str.Append("using System.Threading.Tasks;\r\n");
            str.Append("\r\n");
            str.Append("namespace " + enu.ProjectName + ".Models\r\n");
            str.Append("{\r\n");
            str.Append(string.Format("    public partial class {0} : BaseEntity\r\n", enu.ClassName));
            str.Append("    {\r\n");

            var al = enu.CodeFields;
            #region 建立属性
            foreach (CodeField field in al)
            {
                if (!field.FieldName.Equals("Id") 
                    && !field.FieldName.Equals("aysn_id")) //Id 和 IsSelected 都在基类 BaseEntity 中定义了
                {
                    str.Append("        /// <summary>\r\n");
                    str.Append("        /// " + field.FieldRemark + "\r\n");
                    str.Append("        /// </summary>\r\n");
                    str.Append(string.Format("        private {0} {1};\r\n", SqliteTypeConverter.GetCSharpType(field.FieldType), field.FieldNameCamelCase));
                    str.Append("\r\n");
                    str.Append("        [Column]\r\n");
                    str.Append(string.Format("        public {0} {1}\r\n", SqliteTypeConverter.GetCSharpType(field.FieldType), field.FieldNamePascalCase));
                    str.Append("        {\r\n");
                    str.Append("            get { return "+ field.FieldNameCamelCase + "; }\r\n");
                    str.Append("            set {\r\n");
                    str.Append(string.Format("                if ({0} != value)\r\n", field.FieldNameCamelCase));
                    str.Append("                {\r\n");
                    str.Append(string.Format("                    {0} = value;\r\n", field.FieldNameCamelCase));
                    str.Append("                    OnPropertyChanged();\r\n");
                    str.Append("                }\r\n");
                    str.Append("            }\r\n");
                    str.Append("        }\r\n");
                    str.Append("\r\n");
                }
            }
            #endregion

            #region SelectCommand
            str.Append("        #region 命令\r\n");
            str.Append("\r\n");
            str.Append("        [NoExport]\r\n");
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private void Selected()\r\n");
            str.Append("        {\r\n");
            str.Append(string.Format("            WeakReferenceMessenger.Default.Send<Selected{0}Message, string>(TokenManage.REFRESH_SUMMARY_SELECTED_{1}_TOKEN);\r\n", enu.ClassName, enu.ClassName.ToUpper()));
            str.Append("        }\r\n");
            str.Append("        #endregion \r\n");
            #endregion
            str.Append("    }\r\n");
            str.Append("}\r\n");

            return str.ToString();
        }

        /// <summary>
        /// 根据类名创建 实体类 Dao，如：UserDao
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateDao(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("using CarParkingSystem.Models;\r\n");
            str.Append("using Microsoft.EntityFrameworkCore;\r\n");
            str.Append("using System.Collections.Generic;\r\n");
            str.Append("using System.Linq;\r\n");
            str.Append("using System.Threading.Tasks;\r\n");
            str.Append("namespace " + enu.ProjectName + ".Dao\r\n");
            str.Append("{\r\n");
            str.Append(string.Format("    public class {0}Dao : BaseDao<{0}>\r\n", enu.ClassName));
            str.Append("    {\r\n");
            str.Append("\r\n");
            str.Append(string.Format("        public {0}Dao(AppDbContext context) : base(context)\r\n", enu.ClassName));
            str.Append("        {\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");

            var al = enu.CodeFields;
            #region 建立查询 Unique 属性的方法
            foreach (CodeField field in al)
            {
                if (field.IsUnique)
                {
                    str.Append(string.Format("        public {0} GetBy{1}(string {2})\r\n", enu.ClassName, field.FieldName, Functions.FirstCharToLower(field.FieldName)));
                    str.Append("        {\r\n");
                    str.Append(string.Format("            return _context.{0}s?.FirstOrDefault(u => u.{1}.Equals({2}))!;\r\n", enu.ClassName, field.FieldName, Functions.FirstCharToLower(field.FieldName)));
                    str.Append("        }\r\n");
                }
            }
            #endregion

            str.Append("    }\r\n");
            str.Append("}\r\n");

            return str.ToString();
        }
        /// <summary>
        /// 根据类名创建 ActionWindow 类，新增、更改共用，即创建 model 新增、更改的界面，如 UserActionWindow.axaml
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateActionWindow(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            #region 头部标签
            str.Append("<suki:SukiWindow xmlns=\"https://github.com/avaloniaui\"\r\n");
            str.Append("        xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n");
            str.Append("        xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"\r\n");
            str.Append("        xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"\r\n");
            str.Append("		xmlns:suki=\"https://github.com/kikipoulet/SukiUI\"\r\n");
            str.Append("		xmlns:i18n=\"https://codewf.com\"\r\n");
            str.Append("		xmlns:lang=\"clr-namespace:CarParkingSystem.I18n;assembly=CarParkingSystem\"\r\n");
            str.Append("		xmlns:vm=\"using:CarParkingSystem.ViewModels\"\r\n");
            str.Append("		xmlns:cv=\"using:CarParkingSystem.Converters\"\r\n");
            str.Append("		xmlns:ap=\"using:CarParkingSystem.AttachedProperty\"\r\n");
            str.Append("        mc:Ignorable=\"d\" d:DesignWidth=\"580\" d:DesignHeight=\"480\"\r\n");
            str.Append("		Width=\"580\" SizeToContent=\"Height\"\r\n");
            str.Append(string.Format("        x:Class=\"{0}.{1}ActionWindow\"\r\n", enu.ProjectName, enu.ClassName));
            str.Append("        Title=\"{Binding Title}\"\r\n");
            str.Append("		IsTitleBarVisible=\"False\"\r\n");
            str.Append("		WindowStartupLocation=\"CenterOwner\"\r\n");
            str.Append("				 CanMaximize=\"False\"\r\n");
            str.Append("				 CanResize=\"False\"\r\n");
            str.Append(string.Format("		x:DataType=\"vm:{0}ActionWindowViewModel\">\r\n", enu.ClassName));
            #endregion
            #region Hosts
            str.Append("	\r\n");
            str.Append("	<suki:SukiWindow.Hosts>\r\n");
            str.Append("		<Panel HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\">\r\n");
            str.Append("			<suki:SukiToastHost Manager=\"{ Binding ToastManager}\" />\r\n");
            str.Append("		</Panel>\r\n");
            str.Append("		<suki:SukiDialogHost Manager=\"{ Binding DialogManager}\"/>\r\n");
            str.Append("	</suki:SukiWindow.Hosts>\r\n");
            #endregion
            str.Append("	<Panel>\r\n");
            str.Append("		<StackPanel Spacing=\"5\" Margin=\"20, 30, 20, 30\">\r\n");
            #region 标题
            str.Append("			<TextBlock Text=\"{ Binding Title}\" HorizontalAlignment=\"Center\" FontSize=\"18\" FontWeight=\"Bold\" Margin=\"10,20\"/>\r\n");
            #endregion

            var al = enu.CodeFields;
            #region 建立属性
            foreach (CodeField field in al)
            {
                if (field.FieldName.Equals("Id") || field.FieldName.Equals("aysn_id"))
                {
                    continue;
                }

                //20250331，日期格式的先不显示 
                if (!SqliteTypeConverter.IsDateTime(field.FieldType, field.DefaultValue))
                {
                    str.Append("			<Grid ColumnDefinitions=\"3*,5*,3*\" ShowGridLines=\"False\">\r\n");
                    str.Append("				<TextBlock Grid.Row=\"1\" Grid.Column=\"0\" VerticalAlignment=\"Center\" HorizontalAlignment=\"Right\" Margin=\"0,0,10,0\">\r\n");
                    if (field.IsRequired)
                    {
                        str.Append("					<Run Text=\"*\" Foreground=\"Red\"/>\r\n");
                    }
                    str.Append("					<Run Text=\"{i18n:I18n {x:Static lang:Language." + field.FieldNamePascalCase + "}}\"/>\r\n");
                    AddTranslateKeys(field.FieldNamePascalCase, field.FieldNamePascalCase);
                    str.Append("				</TextBlock>\r\n");
                    str.Append("				<TextBox Grid.Row=\"1\" Grid.Column=\"1\" Height=\"50\" VerticalAlignment=\"Center\" Text=\"{Binding " + field.FieldNamePascalCase + "}\" Watermark=\"{i18n:I18n {x:Static lang:Language.PleaseEnter" + field.FieldNamePascalCase + "}}\" suki:TextBoxExtensions.AddDeleteButton=\"True\" x:CompileBindings=\"False\" />\r\n");
                    AddTranslateKeys("PleaseEnter" + field.FieldNamePascalCase, "PleaseEnter" + field.FieldNamePascalCase);
                    str.Append("				<TextBlock Grid.Row=\"1\" Grid.Column=\"2\" Text=\"{Binding " + field.FieldNamePascalCase + "ValidationMessage}\" IsVisible=\"{Binding " + field.FieldNamePascalCase + "ValidationMessage, Converter = {x:Static StringConverters.IsNotNullOrEmpty}}\" TextWrapping=\"Wrap\" Foreground=\"Red\" VerticalAlignment=\"Center\"  Margin=\"10,0,0,0\"/>\r\n");
                    str.Append("			</Grid>\r\n");
                    str.Append("\r\n");
                }
            }
            #endregion

            #region 增加、更新按钮
            str.Append("\r\n");
            str.Append("			<!--Add or Update Button-->\r\n");
            str.Append("			<Grid ColumnDefinitions=\"3*, 5*, 3*\" ShowGridLines=\"False\">\r\n");
            str.Append("				<StackPanel Grid.Row=\"5\" Grid.Column=\"1\" Grid.ColumnSpan=\"2\" HorizontalAlignment=\"Left\">\r\n");
            str.Append("					<TextBlock Text=\"{Binding UpdateInfo}\" Foreground=\"Red\" HorizontalAlignment=\"Left\" Margin=\"0,10\"/>\r\n");
            str.Append("					<Button Content=\"{i18n:I18n {x:Static lang:Language.Add}}\" Command=\"{Binding AddCommand}\" Classes=\"Flat\" Margin=\"5,0,0,30\" Height=\"50\" MinWidth=\"150\"\r\n");
            str.Append("							IsVisible=\"{Binding IsAdd"+ enu.ClassName + "}\" HotKey=\"Enter\"/>\r\n");
            str.Append("					<Button Content=\"{i18n:I18n {x:Static lang:Language.Update}}\" Command=\"{Binding UpdateCommand}\" Classes=\"Flat\" Margin=\"5,0,0,30\" Height=\"50\" MinWidth=\"150\" IsVisible=\"{Binding !IsAdd"+ enu.ClassName + "}\" HotKey=\"Enter\"/>\r\n");
            str.Append("				</StackPanel>\r\n");
            str.Append("			</Grid>\r\n");
            str.Append("		</StackPanel>\r\n");
            #endregion

            #region 右上角的关闭按钮
            str.Append("\r\n");
            str.Append("		<!--右上角关闭按钮-->\r\n");
            str.Append("		<Button Classes=\"Rounded PathIconButton\"\r\n");
            str.Append("				Width=\"24\"\r\n");
            str.Append("				Height=\"24\"\r\n");
            str.Append("			Margin=\"0, 10, 10, 0\" Padding=\"0\"\r\n");
            str.Append("		  HorizontalAlignment=\"Right\"\r\n");
            str.Append("		  VerticalAlignment=\"Top\"\r\n");
            str.Append("				HorizontalContentAlignment=\"Center\"\r\n");
            str.Append("				FontSize=\"12\"\r\n");
            str.Append("				FontWeight=\"DemiBold\" Command=\"{ Binding CloseCommand}\">\r\n");
            str.Append("			<PathIcon Data=\"{x:Static suki:Icons.CircleOutlineClose}\"  Classes=\"Manual\">\r\n");
            str.Append("			</PathIcon>\r\n");
            str.Append("		</Button>\r\n");
            str.Append("	</Panel>\r\n");
            #endregion
            str.Append("</suki:SukiWindow>\r\n");

            return str.ToString();
        }

        /// <summary>
        /// 根据类名创建 ActionWindow 类的后台代码，即创建 model 新增、更改的界面，如 UserActionWindow.axaml.cs
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateActionWindowCs(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("using Avalonia;\r\n");
            str.Append("using Avalonia.Controls;\r\n");
            str.Append("using Avalonia.Markup.Xaml;\r\n");
            str.Append("using CarParkingSystem.Models;\r\n");
            str.Append("using CarParkingSystem.Unities;\r\n");
            str.Append("using CarParkingSystem.Messages;\r\n");
            str.Append("using CommunityToolkit.Mvvm.Messaging;\r\n");
            str.Append("using SukiUI.Controls;\r\n");
            str.Append("using System;\r\n");
            str.Append("\r\n");
            str.Append("namespace " + enu.ProjectName + "\r\n");
            str.Append("{\r\n");

            str.Append(string.Format("public partial class {0}ActionWindow : SukiWindow\r\n", enu.ClassName));
            str.Append("{\r\n");
            str.Append(string.Format("    public {0}ActionWindow()\r\n", enu.ClassName));
            str.Append("    {\r\n");
            str.Append("        InitializeComponent(); \r\n");
            str.Append("        \r\n");
            str.Append(string.Format("        WeakReferenceMessenger.Default.Register<string, string>(this, TokenManage.{0}_ACTION_WINDOW_CLOSE_TOKEN, Recieve);\r\n", enu.ClassName.ToUpper()));
            str.Append("    }\r\n");



            str.Append("\r\n");
            str.Append("    private void Recieve(object recipient, string message)\r\n");
            str.Append("    {\r\n");
            str.Append("        WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(\r\n");
            str.Append("                new MaskLayerMessage\r\n");
            str.Append("                {\r\n");
            str.Append("                    IsNeedShow = false,\r\n");
            str.Append("                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);\r\n");
            str.Append("\r\n");
            str.Append("        this.Close();\r\n");
            str.Append("    }\r\n");
            str.Append("}\r\n");
            str.Append("}\r\n");

            return str.ToString();
        }

        /// <summary>
        /// 创建操作窗口对应的 ViewModel，如：UserActionWindowViewModel
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateActionWindowViewModel(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("using Avalonia.Threading;\r\n");
            str.Append("using AvaloniaExtensions.Axaml.Markup;\r\n");
            str.Append("using CarParkingSystem.Dao;\r\n");
            str.Append("using CarParkingSystem.I18n;\r\n");
            str.Append("using CarParkingSystem.Models;\r\n");
            str.Append("using CarParkingSystem.Unities;\r\n");
            str.Append("using CarParkingSystem.Messages;\r\n");
            str.Append("using CommunityToolkit.Mvvm.ComponentModel;\r\n");
            str.Append("using CommunityToolkit.Mvvm.Input;\r\n");
            str.Append("using CommunityToolkit.Mvvm.Messaging;\r\n");
            str.Append("using SukiUI.Dialogs;\r\n");
            str.Append("using SukiUI.Toasts;\r\n");
            str.Append("using System;\r\n");
            str.Append("using System.Linq;\r\n");
            str.Append("using System.Threading.Tasks;\r\n");
            str.Append("using RegularExpressionAttribute = CarParkingSystem.I18n.RegularExpressionAttribute;\r\n");
            str.Append("using RequiredAttribute = CarParkingSystem.I18n.RequiredAttribute;\r\n");
            str.Append("\r\n");
            str.Append("namespace " + enu.ProjectName + ".ViewModels\r\n");
            str.Append("{\r\n");


            str.Append("    /// <summary>\r\n");
            str.Append("    /// 为新增、修改信息界面提供数据的 ViewModel\r\n");
            str.Append("    /// </summary>\r\n");
            str.Append(string.Format("    public partial class {0}ActionWindowViewModel : ViewModelValidatorBase\r\n", enu.ClassName));
            str.Append("    {\r\n");
            str.Append("        private AppDbContext _appDbContext;\r\n");
            str.Append(string.Format("        private {0}Dao _{1}Dao;\r\n", enu.ClassName, Functions.FirstCharToLower(enu.ClassName)));
            str.Append("        public ISukiToastManager ToastManager { get; }\r\n");
            str.Append("        public ISukiDialogManager DialogManager { get; }\r\n");
            str.Append("\r\n");
            str.Append("        [ObservableProperty] private string _title;//窗口标题\r\n");
            str.Append("        [ObservableProperty] private string _updateInfo;//提交后的错误信息汇总（如果有的话）\r\n");
            str.Append("\r\n");
            str.Append(string.Format("        [ObservableProperty] private bool? _isAdd{0};// true=Add; false=Update\r\n", enu.ClassName));
            str.Append("        [ObservableProperty] private bool _isBusy;\r\n");
            str.Append("        //[ObservableProperty] private bool _isEnabled = true;\r\n");
            str.Append(string.Format("        [ObservableProperty] private {0} _selected{0};\r\n", enu.ClassName));
            str.Append("\r\n");

            var al = enu.CodeFields;
            #region 建立属性
            foreach (CodeField field in al)
            {
                if (field.FieldName.Equals("Id")
                    || field.FieldName.Equals("aysn_id")) continue;//Id 不需要创建属性

                //if (!SqliteTypeConverter.IsDateTime(field.FieldType, field.DefaultValue))
                //{


                    //日期格式还是挺复杂的
                    if (field.IsRequired && !SqliteTypeConverter.IsDateTime(field.FieldType, field.DefaultValue))
                    {
                        str.Append(string.Format("        [Required(StringResourceKey.{0}Required)]\r\n", field.FieldNamePascalCase));
                        AddTranslateKeys(field.FieldNamePascalCase + "Required", field.FieldNamePascalCase + " Required");
                    }
                    str.Append("        [ObservableProperty]\r\n");
                    str.Append(string.Format("        private " + SqliteTypeConverter.GetCSharpType(field.FieldType) + " _{0};\r\n", field.FieldNameCamelCase));
                //}
            }
            #endregion
            str.Append("\r\n");
            str.Append("        public "+enu.ClassName+"ActionWindowViewModel(AppDbContext appDbContext, "+enu.ClassName+"Dao "+Functions.FirstCharToLower(enu.ClassName)+"Dao, ISukiToastManager toastManager, ISukiDialogManager dialogManager)\r\n");
            str.Append("        {\r\n");
            str.Append("            _appDbContext = appDbContext;\r\n");
            str.Append("            _"+ Functions.FirstCharToLower(enu.ClassName) + "Dao = "+ Functions.FirstCharToLower(enu.ClassName) + "Dao;\r\n");
            str.Append("            ToastManager = toastManager;\r\n");
            str.Append("            DialogManager = dialogManager;\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");

            #region 建立属性验证结果属性
            str.Append("\r\n");
            foreach (CodeField field in al)
            {
                if (field.FieldName.Equals("Id") || field.FieldName.Equals("aysn_id")) continue;//Id 不需要创建属性

                //if (!SqliteTypeConverter.IsDateTime(field.FieldType, field.DefaultValue))
                //{
                    if (field.IsRequired)
                    {
                        str.Append(string.Format("        [ObservableProperty] private string _{0}ValidationMessage;\r\n", field.FieldNameCamelCase));
                    }
                //}
            }
            str.Append("\r\n");
            #endregion

            str.Append("        public void ClearVertifyErrors()\r\n");
            str.Append("        {\r\n");
            str.Append("            ClearErrors();//清除系统验证错误（例如 TextBox 边框变红）\r\n");
            str.Append("            //清除验证错误信息\r\n");
            foreach (CodeField field in al)
            {
                if (field.FieldName.Equals("Id") || field.FieldName.Equals("aysn_id")) continue;//Id 不需要创建属性

                //if (!SqliteTypeConverter.IsDateTime(field.FieldType, field.DefaultValue))
                //{
                    if (field.IsRequired)
                    {
                        str.Append("            UpdateValidationMessage(nameof(" + field.FieldNamePascalCase + "));\r\n");
                    }
                //}
            }
            str.Append("            }\r\n");

            #region Update 命令
            str.Append("        #region 命令\r\n");
            str.Append("        /// <summary>\r\n");
            str.Append("        /// 更新时，先判断：\r\n");
            str.Append("        /// 1.该用户是否存在\r\n");
            str.Append("        /// 2.修改后的用户信息会不会跟已经存在的信息冲突\r\n");
            str.Append("        /// </summary>\r\n");
            str.Append("        /// <returns></returns>\r\n");
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private Task Update()\r\n");
            str.Append("        {\r\n");
            str.Append("            if (IsBusy)\r\n");
            str.Append("                return Task.CompletedTask;\r\n");
            str.Append("\r\n");
            str.Append("            return Task.Run(async () =>\r\n");
            str.Append("            {\r\n");
            str.Append("                IsBusy = true;\r\n");
            str.Append("\r\n");
            str.Append("                ValidateAllProperties();\r\n");
            str.Append("                //前端验证\r\n");
            str.Append("                if (HasErrors)\r\n");
            str.Append("                {\r\n");
            #region 建立属性验证结果属性
            foreach (CodeField field in al)
            {
                if (field.FieldName.Equals("Id") || field.FieldName.Equals("aysn_id")) continue;//Id 不需要创建属性

                if (!SqliteTypeConverter.IsDateTime(field.FieldType, field.DefaultValue))
                {
                    if (field.IsRequired)
                    {
                        str.Append(string.Format("                    UpdateValidationMessage(nameof({0}));\r\n", field.FieldNamePascalCase));
                    }
                }
            }
            #endregion
            str.Append("                    IsBusy = false;\r\n");
            str.Append("                    return;\r\n");
            str.Append("                }\r\n");
            str.Append("\r\n");
            str.Append("                //验证成功后，需要将之前的错误信息清空\r\n");
            str.Append("                ClearErrors();\r\n");
            str.Append("                UpdateInfo = string.Empty;\r\n");
            str.Append("\r\n");

            str.Append("                var hasSameRecord = false;\r\n");
            #region 判断是否有重复记录（有些字段是唯一的，不能相同）
            StringBuilder hasSameRecordStr = new StringBuilder();
            foreach (CodeField field in al)
            {
                if (field.IsUnique)
                {
                    hasSameRecordStr.Append(string.Format("                var hasSameRecord = _{0}Dao.HasSameRecord(Selected{1}.Id, {2});\r\n", Functions.FirstCharToLower(enu.ClassName), enu.ClassName, field.FieldNamePascalCase));
                }
            }
            str.Append("                if ((bool)hasSameRecord)\r\n");
            str.Append("                {\r\n");
            str.Append("                    Dispatcher.UIThread.Invoke(() =>\r\n");
            str.Append("                    {\r\n");
            str.Append("                        ToastManager.CreateToast()\r\n");
            str.Append(string.Format("                .WithTitle(I18nManager.GetString(\"Update{0}Prompt\"))\r\n", enu.ClassName));
            AddTranslateKeys($"Update{enu.ClassName}Prompt", $"Update{enu.ClassName}Prompt");
            str.Append(string.Format("                .WithContent(I18nManager.GetString(\"{0}Exists\"))\r\n", enu.ClassName));
            AddTranslateKeys($"{enu.ClassName}Exists", $"{enu.ClassName}Exists");
            str.Append("                //.OfType(Avalonia.Controls.Notifications.NotificationType.Error)//20250402,不要 icon\r\n");
            str.Append("                .Dismiss().After(TimeSpan.FromSeconds(3)).Dismiss().ByClicking().Queue();\r\n");
            str.Append("                                });\r\n");
            str.Append("                return;\r\n");
            str.Append("                }\r\n");
            str.Append(string.Format("                var temp{0} = _{1}Dao.GetById(Selected{2}.Id);\r\n", enu.ClassName, Functions.FirstCharToLower(enu.ClassName), enu.ClassName));
            str.Append(string.Format("                if (temp{0} != null)\r\n", enu.ClassName));
            str.Append("                {\r\n");
            str.Append("                    string tempDt = DateTime.Now.ToString(\"yyyy-MM-dd HH:mm:ss\");\r\n"); 
            str.Append("\r\n");
            #region 遍历给需要更新的字段赋值
            foreach (CodeField field in al)
            {
                if (field.FieldName.Equals("Id") || field.FieldName.Equals("aysn_id"))
                {
                    continue;
                }
                else if (field.FieldName.Contains("CreatedAt") || field.FieldName.Contains("created_at"))
                {
                    str.Append(string.Format("                    temp{0}.{1} = tempDt;\r\n", enu.ClassName, field.FieldNamePascalCase));
                }
                else if (field.FieldName.Contains("UpdatedAt") || field.FieldName.Contains("updated_at"))
                {
                    str.Append(string.Format("                    temp{0}.{1} = tempDt;\r\n", enu.ClassName, field.FieldNamePascalCase));
                }
                else if ((field.FieldName.EndsWith("Date") || field.FieldName.EndsWith("Time")
                    || field.FieldName.EndsWith("date") || field.FieldName.EndsWith("time"))
                    && SqliteTypeConverter.GetCSharpType(field.FieldType).FullName != "System.Int32")
                {
                    str.Append(string.Format("                    temp{0}.{1} = tempDt;\r\n", enu.ClassName, field.FieldNamePascalCase));
                }
                else
                {
                    str.Append(string.Format("                    temp{0}.{1} = {1};\r\n", enu.ClassName, field.FieldNamePascalCase, field.FieldNamePascalCase));
                }
            }
            #endregion
            str.Append(string.Format("                    int result = _{0}Dao.Update(temp{1});\r\n", Functions.FirstCharToLower(enu.ClassName), enu.ClassName));
            str.Append("                    if (result >= 0)\r\n");
            str.Append("                    {\r\n");

            str.Append("                        Dispatcher.UIThread.Invoke(() =>\r\n");
            str.Append("                        {\r\n");
            str.Append("                            WeakReferenceMessenger.Default.Send(\r\n");
            str.Append("                                 new ToastMessage\r\n");
            str.Append("                                 {\r\n");  
            str.Append("                                     CurrentModelType = typeof("+enu.ClassName+"),\r\n");
            str.Append(string.Format("                                     Title = I18nManager.GetString(\"Update{0}Prompt\"),\r\n", enu.ClassName));
            str.Append("                                     Content = I18nManager.GetString(\"UpdateSuccessfully\"),\r\n");
            str.Append("                                     NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,\r\n");
            str.Append("                                     NeedRefreshData = true\r\n");
            str.Append("                                 }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);\r\n");
            str.Append("\r\n");
            str.Append(string.Format("                            WeakReferenceMessenger.Default.Send(\"Close {0}ActionWindow\", TokenManage.{1}_ACTION_WINDOW_CLOSE_TOKEN);\r\n", enu.ClassName, enu.ClassName.ToUpper()));
            str.Append("                        });\r\n");
            str.Append("                    }\r\n");
            str.Append("                    else\r\n");
            str.Append("                    {\r\n");
            str.Append("                        var message = I18nManager.GetString(\"UpdateFailed\");\r\n");
            str.Append("                        UpdateInfo = message;\r\n");
            str.Append("                    }\r\n");
            str.Append("                    await Task.Delay(2000);\r\n");
            str.Append("\r\n");
            str.Append("                    IsBusy = false;\r\n");
            str.Append("                }\r\n");
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            #endregion
            #endregion

            #region Add 命令
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private Task Add()\r\n");
            str.Append("        {\r\n");
            str.Append("            if (IsBusy)\r\n");
            str.Append("                return Task.CompletedTask;\r\n");
            str.Append("\r\n");
            str.Append("            return Task.Run(async () =>\r\n");
            str.Append("            {\r\n");
            str.Append("                IsBusy = true;\r\n");
            str.Append("\r\n");
            str.Append("                ValidateAllProperties();\r\n");
            str.Append("                //前端验证\r\n");
            str.Append("                if (HasErrors)\r\n");
            str.Append("                {\r\n");
            #region 建立属性验证结果属性
            foreach (CodeField field in al)
            {
                if (field.FieldName.Equals("Id") || field.FieldName.Equals("aysn_id")) continue;//Id 不需要创建属性

                if (!SqliteTypeConverter.IsDateTime(field.FieldType, field.DefaultValue))
                {
                    if (field.IsRequired)
                    {
                        str.Append(string.Format("                    UpdateValidationMessage(nameof({0}));\r\n", field.FieldNamePascalCase));
                    }
                }
            }
            #endregion
            str.Append("                    IsBusy = false;\r\n");
            str.Append("                    return;\r\n");
            str.Append("                }\r\n");
            str.Append("\r\n");
            str.Append("                //验证成功后，需要将之前的错误信息清空\r\n");
            str.Append("                ClearErrors();\r\n");
            str.Append("                UpdateInfo = string.Empty;\r\n");
            str.Append("\r\n");
            str.Append("/*\r\n");
            str.Append("//对数据的唯一性进行验证，这里需要测试来修正\r\n");
            str.Append(string.Format("                var temp{0} = _{1}Dao.GetByUsername(Username);\r\n", enu.ClassName, Functions.FirstCharToLower(enu.ClassName)));
            str.Append("                if (tempUser != null)\r\n");
            str.Append("                {\r\n");
            str.Append(string.Format("                    UsernameValidationMessage = I18nManager.GetString(\"UsernameExists\");\r\n"));
            str.Append("                return;\r\n");
            str.Append("                }\r\n");
            str.Append("*/\r\n");
            str.Append("                string tempDt = DateTime.Now.ToString(\"yyyy-MM-dd HH:mm:ss\");\r\n");
            str.Append("                var result =_"+ Functions.FirstCharToLower(enu.ClassName) + "Dao.Add(new "+ enu.ClassName + "{\r\n");
            #region 遍历给需要更新的字段赋值
            foreach (CodeField field in al)
            {
                if (field.FieldName.Equals("Id") || field.FieldName.Equals("aysn_id"))
                {
                    continue;
                }
                else if (field.FieldName.Contains("CreatedAt") || field.FieldName.Contains("created_at"))
                {
                    str.Append(string.Format("                    {0} = tempDt,\r\n", field.FieldNamePascalCase));
                }
                else if (field.FieldName.Contains("UpdatedAt") || field.FieldName.Contains("updated_at"))
                {
                    str.Append(string.Format("                    {0} = tempDt,\r\n", field.FieldNamePascalCase));
                }
                else if ((field.FieldName.EndsWith("Date") || field.FieldName.EndsWith("Time")
                    || field.FieldName.EndsWith("date") || field.FieldName.EndsWith("time"))
                    && SqliteTypeConverter.GetCSharpType(field.FieldType).FullName != "System.Int32")
                {
                    str.Append(string.Format("                    {0} = tempDt,\r\n", field.FieldNamePascalCase));
                }
                else
                {
                    str.Append(string.Format("                    {0} = {0},\r\n", field.FieldNamePascalCase));
                }
            }
            str.Append("                });\r\n");
            str.Append("                if (result > 0)\r\n");
            str.Append("                {\r\n");
            str.Append("                    Dispatcher.UIThread.Invoke(() =>\r\n");
            str.Append("                    {\r\n");
            str.Append("                        WeakReferenceMessenger.Default.Send(\r\n");
            str.Append("                            new ToastMessage { \r\n");
            str.Append("                                     CurrentModelType = typeof(" + enu.ClassName + "),\r\n");
            str.Append(string.Format("                            Title = I18nManager.GetString(\"Create{0}Prompt\"),\r\n", enu.ClassName));
            AddTranslateKeys($"Create{enu.ClassName}Prompt", $"Create{enu.ClassName}Prompt");
            str.Append("                            Content = I18nManager.GetString(\"CreateSuccessfully\"),\r\n");
            str.Append("                            NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,\r\n");
            str.Append("                            NeedRefreshData = true\r\n");
            str.Append("                            }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);\r\n");
            str.Append(string.Format("                        WeakReferenceMessenger.Default.Send(\"Close {0}ActionWindow\", TokenManage.{0}_ACTION_WINDOW_CLOSE_TOKEN);\r\n", enu.ClassName.ToUpper()));
            str.Append("                    });\r\n");
            str.Append("                }\r\n");
            str.Append("                else\r\n");
            str.Append("                {\r\n");
            str.Append("                    UpdateInfo = I18nManager.GetString(\"CreateFailed\");\r\n");
            str.Append("                }\r\n");
            str.Append("                await Task.Delay(2000);\r\n");
            str.Append("\r\n");
            str.Append("                IsBusy = false;\r\n");
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            #endregion
            #endregion
            #region Close 命令
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private void Close()\r\n");
            str.Append("        {\r\n");
            str.Append(string.Format("            WeakReferenceMessenger.Default.Send<string, string>(\"Close\", TokenManage.{1}_ACTION_WINDOW_CLOSE_TOKEN);\r\n", enu.ClassName, enu.ClassName.ToUpper()));
            str.Append("        }\r\n");
            str.Append("\r\n");
            #endregion
            str.Append("        #endregion\r\n");
            str.Append("    }\r\n");
            str.Append("}\r\n");
            return str.ToString();
        }

        /// <summary>
        /// 根据类名创建 View 类，(UserControl, 点击左侧菜单时，在右侧显示的内容)
        /// 即创建 model 新增、更改的界面，如 UserActionWindow.axaml
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateView(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            #region 头部标签
            str.Append("<UserControl xmlns=\"https://github.com/avaloniaui\"\r\n");
            str.Append("             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n");
            str.Append("             xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"\r\n");
            str.Append("             xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"\r\n");
            str.Append("			 xmlns:suki=\"https://github.com/kikipoulet/SukiUI\"\r\n");
            str.Append("			 xmlns:i18n=\"https://codewf.com\"\r\n");
            str.Append("			 xmlns:lang=\"clr-namespace:CarParkingSystem.I18n;assembly=CarParkingSystem\"\r\n");
            str.Append("			 xmlns:vm=\"using:CarParkingSystem.ViewModels\"\r\n");
            str.Append("			 xmlns:control=\"using:CarParkingSystem.Controls\"\r\n");
            str.Append("			 xmlns:model=\"using:CarParkingSystem.Models\"\r\n");
            str.Append("             mc:Ignorable=\"d\" d:DesignWidth=\"1000\" d:DesignHeight=\"860\"\r\n");
            str.Append(string.Format("             x:Class=\"{0}.{1}View\"\r\n", enu.ProjectName, enu.ClassName));
            str.Append(string.Format("			 x:DataType=\"vm:{0}ViewModel\">\r\n", enu.ClassName));
            str.Append("\r\n");
            #endregion
            str.Append("	<Panel>\r\n");
            str.Append("		<StackPanel Classes=\"HeaderContent\">\r\n");
            #region 标题
            str.Append("			<DockPanel Margin=\"20, 20, 20, 0\">\r\n");
            str.Append("				<StackPanel>\r\n");
            str.Append("					<TextBlock DockPanel.Dock=\"Left\" Text=\"{i18n:I18n {x:Static lang:Language."+ enu.ClassName + "Management}}\" FontSize=\"32\" FontWeight=\"Bold\" />\r\n");
            AddTranslateKeys(enu.ClassName + "Management", enu.ClassName + "Management");
            str.Append("					<TextBlock DockPanel.Dock=\"Left\" Text=\"{i18n:I18n {x:Static lang:Language."+ enu.ClassName + "ManagementDesc}}\" Margin=\"0,10\"/>\r\n");
            AddTranslateKeys(enu.ClassName + "ManagementDesc", enu.ClassName + "ManagementDesc");
            str.Append("				</StackPanel>\r\n");
            str.Append("			</DockPanel>\r\n");

            #endregion

            #region 搜索栏
            str.Append("			<suki:GlassCard x:Name=\"searchGlassCard\" Classes=\"HeaderClassed\" Margin=\"10, 5\" >\r\n");

            var al = enu.CodeFields;
            #region 建立属性
            str.Append("				<Grid ShowGridLines=\"False\">\r\n");
            str.Append("				    <Grid.ColumnDefinitions>\r\n");
            //显示 4组 8列
            for (int i = 0; i < SEARCH_COLUMN_NUMBER; i++)
            {
                if (i % 2 == 0)
                {
                    str.Append("				    <ColumnDefinition Width='Auto'/>\r\n");
                }
                else
                {
                    str.Append("				    <ColumnDefinition Width='*'/>\r\n");
                }
            }
            str.Append("				    </Grid.ColumnDefinitions>\r\n");
            // 计算行数并添加行定义(al.Count * 2 + 2)
            int rowCount = (int)Math.Ceiling((al.Count * 2) / (double)SEARCH_COLUMN_NUMBER);//每个字段占 2 列，搜索和重置按钮也占 2 列，但又不需要考虑 Id，所以结果直接是 2 倍即可
            str.Append("				    <Grid.RowDefinitions>\r\n");
            for (int i = 0; i < rowCount; i++)
                str.Append("				        <RowDefinition Height='Auto'/>\r\n"); // 自动行高
            str.Append("				    </Grid.RowDefinitions>\r\n");
            int row = 0;
            int col = 0;
            int sumCount = 0;
            
            //20250331，用于同步 DatePicker 和 TextBox 的宽带（在改变主界面尺寸时）
            str.Append("                  <TextBox x:Name = \"txtSetWidth\" Grid.Row = \"0\" Grid.Column = \"1\" MinWidth = \"180\" Margin = \"0,0,30,0\" />\r\n");
            // 动态生成元素
            for (int i = 0; i < al.Count; i++)
            {
                //int row = i / 6;
                //int col = i % 6;
                if (al[i].FieldName.Equals("Id") || al[i].FieldName.Equals("aysn_id")) continue;//Id 不需要搜索

                str.Append("                  <TextBlock Grid.Row=\"" + row + "\" Grid.Column=\"" + col + "\" VerticalAlignment=\"Center\" Text=\"{Binding Source={x:Static lang:Language." + al[i].FieldNamePascalCase + "}, Converter={StaticResource i18nAppendConverter}, ConverterParameter=:}\"/>\r\n");
                AddTranslateKeys(al[i].FieldNamePascalCase, al[i].FieldNamePascalCase); 
                str.Append("                  <TextBox Grid.Row=\"" + row + "\" Grid.Column=\"" + (col+1) + "\" MinWidth=\"180\" Text=\"{Binding Search" + al[i].FieldNamePascalCase + "}\" Margin=\"0,0,30,0\"/>\r\n");

                sumCount = sumCount + 2;
                if (sumCount >= SEARCH_COLUMN_NUMBER)
                {
                    row = sumCount / SEARCH_COLUMN_NUMBER;
                }

                col = col + 2;
                if (col == SEARCH_COLUMN_NUMBER)
                {
                    col = 0;
                }
            }
            #region 搜索、重置按钮
            str.Append("                  <StackPanel  Grid.Row=\"" + (rowCount-1) +"\" Grid.Column=\""+(SEARCH_COLUMN_NUMBER - 1) +"\" Spacing=\"10\" Orientation=\"Horizontal\" HorizontalAlignment=\"Right\" Margin=\"0,10,30,0\">\r\n");
            str.Append("                        <Button Content=\"{i18n:I18n {x:Static lang:Language.Search}}\" Classes=\"Flat Round\" Classes.Delete=\"True\"  Height=\"40\" Command=\"{Binding Search" + enu.ClassName + "Command}\"  x:CompileBindings=\"False\"/>\r\n");
            str.Append("                        <Button Content=\"{i18n:I18n {x:Static lang:Language.Reset}}\" Classes=\"Flat Round\" Height=\"40\" HorizontalAlignment=\"Right\" Command=\"{Binding ResetSearchCommand}\" x:CompileBindings=\"False\"/>\r\n");
            str.Append("                  </StackPanel>\r\n");
            #endregion
            str.Append("</Grid>");


           
            str.Append("            </suki:GlassCard>\r\n");
            #endregion
            #endregion

            str.Append("					\r\n");
            str.Append("			<suki:GlassCard Classes=\"HeaderClassed\" Margin=\"10, 5\">\r\n");
            str.Append("				<StackPanel>\r\n");
            str.Append("					<DockPanel Margin=\"20, 0\" >\r\n");
            #region 导出 excel 按钮
            str.Append("						<Button DockPanel.Dock=\"Right\" Classes=\"Flat Round\" Classes.hasItems=\"{Binding " + enu.ClassName + "s.Count, Converter ={StaticResource numToBoolConverter}}\" Margin=\"10,0,0,0\" Height=\"40\" HorizontalAlignment=\"Right\" Command=\"{Binding ExportCommand}\" CommandParameter=\"{ Binding $parent[Window]}\" x:CompileBindings=\"False\" Content=\"{i18n:I18n {x:Static lang:Language.ExportExcel}}\" />\r\n");
            #endregion
            #region 批量删除按钮
            str.Append("						<Button DockPanel.Dock=\"Right\" Content=\"{i18n:I18n {x:Static lang:Language.Delete}}\" Classes=\"Flat Round\" Classes.Delete=\"True\"  Height=\"40\" HorizontalAlignment=\"Right\" Command=\"{Binding Delete"+ enu.ClassName + "Command}\" CommandParameter=\"{Binding $parent[Window]}\" x:CompileBindings=\"False\"/>\r\n");
            #endregion
            #region 打开新增界面的按钮
            str.Append("						<Button DockPanel.Dock=\"Right\"  Margin=\"0, 0, 10, 0\" Content=\"{i18n:I18n {x:Static lang:Language.Add}}\" Classes=\"Flat Round\"  Height=\"40\" HorizontalAlignment=\"Right\" Command=\"{Binding AddCommand}\" CommandParameter=\"{Binding $parent[Window]}\" x:CompileBindings=\"False\"/>\r\n");
            #endregion
            str.Append("					</DockPanel>\r\n");
            str.Append("		\r\n");

            #region DataGrid 表格，显示数据
            str.Append("				<!--表格，显示数据-->\r\n");
            str.Append("				<Panel>\r\n");
            str.Append("					<DataGrid x:Name=\"dg\" Margin=\"20, 0, 0, 0\" MinHeight=\"300\" MaxHeight=\"480\"\r\n");
            str.Append("							  ItemsSource=\"{Binding "+enu.ClassName+"s}\"\r\n");
            str.Append("							  CanUserReorderColumns=\"True\"\r\n");
            str.Append("							  CanUserResizeColumns=\"False\"\r\n");
            str.Append("							  CanUserSortColumns=\"False\"\r\n");
            str.Append("							  GridLinesVisibility=\"All\"\r\n");
            str.Append("							  BorderThickness=\"1\" \r\n");
            str.Append("							  BorderBrush=\"Gray\"\r\n");
            str.Append("							  VerticalScrollBarVisibility=\"Visible\">\r\n");
            str.Append("						<DataGrid.Columns>\r\n");
            #region 复选框
            str.Append("							<DataGridTemplateColumn Width=\"30\">\r\n");
            str.Append("								<DataGridTemplateColumn.HeaderTemplate>\r\n");
            str.Append("									<DataTemplate>\r\n");
            str.Append("									    <!--20250401, 在这里无法水平居中，占据太多空间-->\r\n");
            //str.Append("										<CheckBox IsThreeState=\"True\" Margin=\"40, 0, 0, 0\"  IsChecked=\"{Binding $parent[UserControl].DataContext.IsSelectedAll}\" Command=\"{Binding $parent[UserControl].DataContext.SelectedAllCommand}\" x:CompileBindings=\"False\"/>\r\n");
            str.Append("									</DataTemplate>\r\n");
            str.Append("								</DataGridTemplateColumn.HeaderTemplate>\r\n");
            str.Append("								<DataGridTemplateColumn.CellTemplate>\r\n");
            str.Append("									<DataTemplate DataType=\"model:"+ enu.ClassName + "\">\r\n");
            str.Append("										<CheckBox HorizontalAlignment=\"Center\" IsThreeState=\"False\" IsChecked=\"{Binding IsSelected, Mode = TwoWay}\" Command=\"{Binding SelectedCommand}\"/>\r\n");
            str.Append("									</DataTemplate>\r\n");
            str.Append("								</DataGridTemplateColumn.CellTemplate>\r\n");
            str.Append("							</DataGridTemplateColumn>\r\n");
            str.Append("			\r\n");

            #endregion

            #region 操作按钮
            str.Append("							<DataGridTemplateColumn Header=\"{i18n:I18n {x:Static lang:Language.Operate}}\" Width=\"*\">\r\n");
            str.Append("								<DataGridTemplateColumn.CellTemplate>\r\n");
            str.Append("									<DataTemplate DataType=\"model:"+ enu.ClassName + "\">\r\n");
            str.Append("										<TextBlock HorizontalAlignment = \"Center\">\r\n");
            str.Append("										    <HyperlinkButton HorizontalAlignment = \"Center\" VerticalAlignment = \"Center\" Content = \"{i18n:I18n {x:Static lang:Language.Modify}}\" Command = \"{Binding $parent[UserControl].DataContext.Update" + enu.ClassName + "Command}\" CommandParameter = \"{Binding}\" x:CompileBindings = \"False\" />\r\n");
            str.Append("										</TextBlock>\r\n");
            str.Append("									</DataTemplate>\r\n");
            str.Append("								</DataGridTemplateColumn.CellTemplate>\r\n");
            str.Append("							</DataGridTemplateColumn>\r\n");
            str.Append("\r\n");
            #endregion

            #region Id 列（因为是在父类中定义的）
            //str.Append("							<DataGridTemplateColumn>\r\n");
            //str.Append("							    <DataGridTemplateColumn.HeaderTemplate>\r\n");
            //str.Append("							        <DataTemplate>\r\n");
            //str.Append("							            <ToggleButton Content=\"{i18n:I18n {x:Static lang:Language.Id}}\" Classes=\"DataGridHeader\"  IsChecked=\"False\" IsThreeState=\"False\" Command=\"{Binding $parent[UserControl].DataContext.SortingCommand}\" x:CompileBindings=\"False\">\r\n");
            //str.Append("							                <ToggleButton.CommandParameter>\r\n");
            //str.Append("							                    <MultiBinding Converter=\"{StaticResource listConverter}\">\r\n");
            //str.Append("							                        <Binding Source=\"Id\"/>\r\n");
            //str.Append("							                            <Binding Path=\"$self.IsChecked\"/>\r\n");
            //str.Append("							                    </MultiBinding>\r\n");
            //str.Append("							                </ToggleButton.CommandParameter>\r\n");
            //str.Append("							            </ToggleButton>\r\n");
            //str.Append("							        </DataTemplate>\r\n");
            //str.Append("							    </DataGridTemplateColumn.HeaderTemplate>\r\n");
            //str.Append("							    <DataGridTemplateColumn.CellTemplate>\r\n");
            //str.Append("							        <DataTemplate DataType=\"model:"+ enu.ClassName + "\">\r\n");
            //str.Append("                                        <TextBlock HorizontalAlignment=\"Center\" Text=\"{Binding Id, Mode=TwoWay}\"/>\r\n");
            //str.Append("                                    </DataTemplate>\r\n");
            //str.Append("                                </DataGridTemplateColumn.CellTemplate>\r\n");
            //str.Append("                        </DataGridTemplateColumn>\r\n");
            //str.Append("\r\n");
            #endregion

            //20250402，因为默认是左对齐（Left）的，所以只添加 Center 和 Right 的记录
            Dictionary<int, string> gridHeaderAlignmentDict = new Dictionary<int, string>();
            gridHeaderAlignmentDict.Add(1, "Center");
            int fieldNum = 3;//下标默认从 1 开始的，但是在 foreach 循环前，还有个操作按钮，所以这里的下标从 3 开始
            string realFieldName = string.Empty;
            //20250407，将 Id 或者 aysn_id 排到第一位
            foreach (CodeField field in al.OrderBy(p => (p.FieldName == "Id" || p.FieldName == "aysn_id") ? 0 : 1))
            {

                if (field.FieldName.Equals("Id") || field.FieldName.Equals("aysn_id"))
                {
                    realFieldName = "Id";
                    str.Append("							<DataGridTemplateColumn>\r\n");
                }
                else
                {
                    realFieldName = field.FieldNamePascalCase;
                    str.Append("							<DataGridTemplateColumn Width=\"*\">\r\n");
                }
                str.Append("								<DataGridTemplateColumn.HeaderTemplate>\r\n");
                str.Append("									<DataTemplate>\r\n");
                str.Append("										<ToggleButton Classes=\"DataGridHeader\" Theme=\"{StaticResource DataGridHeaderToggleButton}\" Content=\"{i18n:I18n {x:Static lang:Language."+ realFieldName + "}}\" IsChecked=\"False\" IsThreeState=\"False\" Command=\"{Binding $parent[UserControl].DataContext.SortingCommand}\" x:CompileBindings=\"False\">\r\n");
                AddTranslateKeys(realFieldName, realFieldName);
                str.Append("											<ToggleButton.CommandParameter>\r\n");
                str.Append("												<MultiBinding Converter=\"{StaticResource listConverter}\">\r\n");
                str.Append("													<Binding Source=\""+ realFieldName + "\"/>\r\n");
                str.Append("													<Binding Path=\"$self.IsChecked\"/>\r\n");
                str.Append("												</MultiBinding>\r\n");
                str.Append("											</ToggleButton.CommandParameter>\r\n");
                str.Append("										</ToggleButton>\r\n");
                str.Append("									</DataTemplate>\r\n");
                str.Append("								</DataGridTemplateColumn.HeaderTemplate>\r\n");
                str.Append("								<DataGridTemplateColumn.CellTemplate>\r\n");
                str.Append("									<DataTemplate DataType=\"model:"+ enu.ClassName + "\">\r\n");
                if (SqliteTypeConverter.GetCSharpType(field.FieldType).FullName == "System.String")
                {
                    if (field.DefaultValue.ToLower().Equals("(datetime('now','localtime'))"))
                    {
                        str.Append("										<TextBlock HorizontalAlignment=\"Center\" Text=\"{Binding " + realFieldName + ", Mode = TwoWay}\" TextWrapping=\"Wrap\"/>\r\n");
                        gridHeaderAlignmentDict.Add(fieldNum, "Center");
                    }
                    else
                    {
                        str.Append("										<TextBlock HorizontalAlignment=\"Left\" Text=\"{Binding " + realFieldName + ", Mode = TwoWay}\" TextWrapping=\"Wrap\"/>\r\n");
                    }
                }
                else if (field.FieldName.Equals("Id") || field.FieldName.Equals("aysn_id"))
                {
                    str.Append("										<TextBlock HorizontalAlignment=\"Center\" Text=\"{Binding " + realFieldName + ", Mode = TwoWay}\" TextWrapping=\"Wrap\"/>\r\n");
                    gridHeaderAlignmentDict.Add(fieldNum, "Center");
                }
                else
                {
                    str.Append("										<TextBlock HorizontalAlignment=\"Right\" Text=\"{Binding " + realFieldName + ", Mode = TwoWay}\" TextWrapping=\"Wrap\"/>\r\n");
                    gridHeaderAlignmentDict.Add(fieldNum, "Right");
                }

                fieldNum++;
                    
                str.Append("									</DataTemplate>\r\n");
                str.Append("								</DataGridTemplateColumn.CellTemplate>\r\n");
                str.Append("							</DataGridTemplateColumn>\r\n");
                str.Append("					\r\n");
            }
            str.Append("						</DataGrid.Columns>\r\n");
            str.Append("						<DataGrid.ColumnHeaderTheme>\r\n");
            str.Append("						    <ControlTheme TargetType = \"DataGridColumnHeader\" BasedOn = \"{StaticResource {x:Type DataGridColumnHeader}}\">\r\n");
            str.Append("						        <ControlTheme.Children>\r\n");

            foreach (var item in gridHeaderAlignmentDict)
            {
                str.Append("						            <Style Selector = \"^:nth-child("+item.Key+")\">\r\n");
                str.Append("						                <Setter Property = \"HorizontalContentAlignment\" Value = \""+item.Value+"\"/>\r\n");
                str.Append("						            </Style>\r\n");
            }

            str.Append("						        </ControlTheme.Children>\r\n");
            str.Append("						    </ControlTheme>\r\n");
            str.Append("						</DataGrid.ColumnHeaderTheme>\r\n");
            str.Append("					</DataGrid>\r\n");
            str.Append("				    <CheckBox IsThreeState = \"True\" IsChecked = \"{Binding $parent[UserControl].DataContext.IsSelectedAll}\" Command = \"{Binding $parent[UserControl].DataContext.SelectedAllCommand}\" x:CompileBindings = \"False\" HorizontalAlignment = \"Left\" VerticalAlignment = \"Top\" Margin = \"30,11\"/>\r\n");
            str.Append("				</Panel>\r\n");
                        
            str.Append("\r\n");
            #endregion

            #region 分页组件
            str.Append("\r\n");
            str.Append("\r\n");
            str.Append("					<control:PagerBar \r\n");
            str.Append("							FirstPageCommand=\"{Binding FirstPageCommand}\"\r\n");
            str.Append("							PrevPageCommand=\"{Binding PrevPageCommand}\"\r\n");
            str.Append("							NextPageCommand=\"{Binding NextPageCommand}\"\r\n");
            str.Append("							LastPageCommand=\"{Binding LastPageCommand}\"\r\n");
            str.Append("							GoToPageCommand=\"{Binding GoToPageCommand}\"\r\n");
            str.Append("							SelectionChangedCommand=\"{Binding SelectionChangedCommand}\"\r\n");
            str.Append("							AllCount=\"{Binding AllCount}\"\r\n");
            str.Append("							PageCount=\"{Binding PageCount}\"\r\n");
            str.Append("							CurrentPageIndex=\"{Binding CurrentPageIndex}\"\r\n");
            str.Append("							CommandParameter=\"{Binding $self.IndexToGo}\"\r\n");
            str.Append("							Margin=\"0, 10, 20, 10\"\r\n");
            str.Append("							x:CompileBindings=\"False\"/>\r\n");
            #endregion

            str.Append("\r\n");
            str.Append("				</StackPanel>\r\n");
            str.Append("				\r\n");
            str.Append("				\r\n");
            str.Append("				</suki:GlassCard>\r\n");
            str.Append("			</StackPanel>\r\n");
            str.Append("		</Panel>\r\n");
            str.Append("</UserControl>\r\n");

            return str.ToString();
        }

        private static string CreateViewWithWrapPanel(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            #region 头部标签
            str.Append("<UserControl xmlns=\"https://github.com/avaloniaui\"\r\n");
            str.Append("             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n");
            str.Append("             xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"\r\n");
            str.Append("             xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"\r\n");
            str.Append("			 xmlns:suki=\"https://github.com/kikipoulet/SukiUI\"\r\n");
            str.Append("			 xmlns:i18n=\"https://codewf.com\"\r\n");
            str.Append("			 xmlns:lang=\"clr-namespace:CarParkingSystem.I18n;assembly=CarParkingSystem\"\r\n");
            str.Append("			 xmlns:vm=\"using:CarParkingSystem.ViewModels\"\r\n");
            str.Append("			 xmlns:control=\"using:CarParkingSystem.Controls\"\r\n");
            str.Append("			 xmlns:model=\"using:CarParkingSystem.Models\"\r\n");
            str.Append("             mc:Ignorable=\"d\" d:DesignWidth=\"1000\" d:DesignHeight=\"860\"\r\n");
            str.Append(string.Format("             x:Class=\"{0}.{1}View\"\r\n", enu.ProjectName, enu.ClassName));
            str.Append(string.Format("			 x:DataType=\"vm:{0}ViewModel\">\r\n", enu.ClassName));
            str.Append("\r\n");
            #endregion
            str.Append("	<Panel>\r\n");
            str.Append("		<StackPanel Classes=\"HeaderContent\">\r\n");
            #region 标题
            str.Append("			<DockPanel Margin=\"20, 20, 20, 0\">\r\n");
            str.Append("				<StackPanel>\r\n");
            str.Append("					<TextBlock DockPanel.Dock=\"Left\" Text=\"{i18n:I18n {x:Static lang:Language." + enu.ClassName + "Management}}\" FontSize=\"32\" FontWeight=\"Bold\" />\r\n");
            AddTranslateKeys(enu.ClassName + "Management", enu.ClassName + "Management");
            str.Append("					<TextBlock DockPanel.Dock=\"Left\" Text=\"{i18n:I18n {x:Static lang:Language." + enu.ClassName + "ManagementDesc}}\" Margin=\"0,10\"/>\r\n");
            AddTranslateKeys(enu.ClassName + "ManagementDesc", enu.ClassName + "ManagementDesc");
            str.Append("				</StackPanel>\r\n");
            str.Append("			</DockPanel>\r\n");

            #endregion

            #region 搜索栏
            str.Append("			<suki:GlassCard x:Name=\"searchGlassCard\" Classes=\"HeaderClassed\" Margin=\"10, 5\" >\r\n");

            var al = enu.CodeFields;
            #region 建立属性
            //str.Append("				<Grid>\r\n");
            //str.Append("				    <Grid.ColumnDefinitions>\r\n");
            ////显示 3组 6列
            //for (int i = 0; i < 6; i++)
            //    str.Append("				    <ColumnDefinition Width='*'/>");
            //str.Append("				    </Grid.ColumnDefinitions>");
            //// 计算行数并添加行定义
            //int rowCount = (int)Math.Ceiling(al.Count / 4.0);
            //str.Append("				    <Grid.RowDefinitions>");
            //for (int i = 0; i < rowCount; i++)
            //    str.Append("				        <RowDefinition Height='Auto'/>"); // 自动行高
            //str.Append("				    </Grid.RowDefinitions>");
            //// 动态生成元素
            //for (int i = 0; i < al.Count; i++)
            //{
            //    int row = i / 6;
            //    int col = i % 6;

            //    bool isOdd = i % 2 == 0 ? true : false;

            //    if (isOdd)
            //    {
            //        str.Append("                  <TextBlock Grid.Row=\""+ row +"\" Grid.Column=\""+col+"\" VerticalAlignment=\"Center\" Text=\"{Binding Source={x:Static lang:Language." + al[i].FieldName + "}, Converter={StaticResource i18nAppendConverter}, ConverterParameter=:}\"/>\r\n");
            //        AddTranslateKeys(al[i].FieldName, al[i].FieldName);
            //    }
            //    else
            //    {
            //        str.Append("                  <TextBox Grid.Row=\"" + row + "\" Grid.Column=\""+ col +"\" MinWidth=\"180\" Text=\"{Binding Search" + al[i].FieldName + "}\"/>\r\n");
            //    }
            //}
            //str.Append("</Grid>");


            str.Append("				<WrapPanel Grid.IsSharedSizeScope=\"True\">\r\n");
            foreach (CodeField field in al)
            {
                if (field.FieldName.Equals("CreatedAt"))
                {
                    str.Append("					<Grid ColumnDefinitions=\"*,*\"  ShowGridLines=\"False\">\r\n");
                    str.Append("                        <TextBlock Text=\"{Binding Source={x:Static lang:Language.StartDateTime}, Converter={StaticResource i18nAppendConverter}, ConverterParameter=:}\" VerticalAlignment=\"Center\" />\r\n");
                    str.Append("					    <DatePicker Grid.Column=\"1\" SelectedDate=\"{Binding SearchStartDateTime}\" Margin=\"5,0,0,0\"/>\r\n");
                    str.Append("					</Grid>\r\n");

                    str.Append("                <Grid ColumnDefinitions=\"*,*\"  ShowGridLines=\"False\">\r\n");
                    str.Append("                    <TextBlock Text=\"{Binding Source={x:Static lang:Language.EndDateTime}, Converter={StaticResource i18nAppendConverter}, ConverterParameter=:}\" VerticalAlignment=\"Center\" />\r\n");
                    str.Append("                    <DatePicker Grid.Column=\"1\" SelectedDate=\"{Binding SearchEndDateTime}\" Margin=\"5,0,0,0\"/>\r\n");
                    str.Append("                </Grid>\r\n");
                }
                else if (field.FieldName.Equals("UPdatedAt"))
                {
                    //str.Append("<Grid ColumnDefinitions=\"*,*\"  ShowGridLines=\"False\">\r\n");
                    //str.Append(string.Format("<TextBlock Grid.Row=\"0\" Grid.Column=\"0\" VerticalAlignment=\"Center\" Text=\"{Binding Source={x:Static lang:Language.{0}, Converter={StaticResource i18nAppendConverter}, ConverterParameter=:}\"/>\r\n", field.FieldName));
                    //str.Append(string.Format("<TextBox Grid.Row=\"0\" Grid.Column=\"1\" Text=\"{Binding Search{0}}\"/>\r\n", field.FieldName));
                    //str.Append("</Grid>\r\n");
                }
                else
                {
                    str.Append("                <Grid ColumnDefinitions=\"*,*\"  ShowGridLines=\"False\">\r\n");
                    str.Append("                  <TextBlock VerticalAlignment=\"Center\" Text=\"{Binding Source={x:Static lang:Language." + field.FieldName + "}, Converter={StaticResource i18nAppendConverter}, ConverterParameter=:}\"/>\r\n");
                    AddTranslateKeys(field.FieldName, field.FieldName);
                    str.Append("                  <TextBox Grid.Column=\"1\" MinWidth=\"80\" Text=\"{Binding Search" + field.FieldName + "}\"/>\r\n");
                    str.Append("                </Grid>\r\n");
                }
                str.Append("\r\n");
            }
            #region 搜索、重置按钮
            str.Append("                    <StackPanel  Grid.Row=\"1\" Grid.Column=\"5\" Spacing=\"10\" Orientation=\"Horizontal\" HorizontalAlignment=\"Right\" Margin=\"0,10,0,0\">\r\n");
            str.Append("                      <Button Content=\"{i18n:I18n {x:Static lang:Language.Search}}\" Classes=\"Flat Round\" Classes.Delete=\"True\"  Height=\"40\" Command=\"{Binding Search" + enu.ClassName + "Command}\"  x:CompileBindings=\"False\"/>\r\n");
            str.Append("                        <Button Content=\"{i18n:I18n {x:Static lang:Language.Reset}}\" Classes=\"Flat Round\" Height=\"40\" HorizontalAlignment=\"Right\" Command=\"{Binding ResetSearchCommand}\" x:CompileBindings=\"False\"/>\r\n");
            str.Append("					</StackPanel>\r\n");
            #endregion
            str.Append("                </WrapPanel>\r\n");
            #endregion
            str.Append("            </suki:GlassCard>\r\n");
            #endregion


            str.Append("					\r\n");
            str.Append("			<suki:GlassCard Classes=\"HeaderClassed\" Margin=\"10, 5\">\r\n");
            str.Append("				<StackPanel>\r\n");
            str.Append("					<DockPanel Margin=\"20, 0\" >\r\n");
            #region 导出 excel 按钮
            str.Append("						<Button DockPanel.Dock=\"Right\" Classes=\"Flat Round\" Classes.hasItems=\"{Binding " + enu.ClassName + "s.Count, Converter ={StaticResource numToBoolConverter}}\" Margin=\"0,0,30,0\" Height=\"40\" HorizontalAlignment=\"Right\" Command=\"{Binding ExportCommand}\" CommandParameter=\"{ Binding $parent[Window]}\" x:CompileBindings=\"False\" Content=\"{i18n:I18n {x:Static lang:Language.ExportExcel}}\" />\r\n");
            #endregion
            #region 批量删除按钮
            str.Append("						<Button DockPanel.Dock=\"Right\" Content=\"{i18n:I18n {x:Static lang:Language.Delete}}\" Classes=\"Flat Round\" Classes.Delete=\"True\"  Height=\"40\" HorizontalAlignment=\"Right\" Command=\"{Binding Delete" + enu.ClassName + "Command}\" CommandParameter=\"{Binding $parent[Window]}\" x:CompileBindings=\"False\"/>\r\n");
            #endregion
            #region 打开新增界面的按钮
            str.Append("						<Button DockPanel.Dock=\"Right\"  Margin=\"0, 0, 10, 0\" Content=\"{i18n:I18n {x:Static lang:Language.Add}}\" Classes=\"Flat Round\"  Height=\"40\" HorizontalAlignment=\"Right\" Command=\"{Binding AddCommand}\" CommandParameter=\"{Binding $parent[Window]}\" x:CompileBindings=\"False\"/>\r\n");
            #endregion
            str.Append("					</DockPanel>\r\n");
            str.Append("		\r\n");

            #region DataGrid 表格，显示数据
            str.Append("					<!--表格，显示数据-->\r\n");
            str.Append("					<DataGrid x:Name=\"dg\" Margin=\"20, 0, 0, 0\" MinHeight=\"300\" MaxHeight=\"480\"\r\n");
            str.Append("							  ItemsSource=\"{Binding " + enu.ClassName + "s}\"\r\n");
            str.Append("							  CanUserReorderColumns=\"True\"\r\n");
            str.Append("							  CanUserResizeColumns=\"False\"\r\n");
            str.Append("							  CanUserSortColumns=\"True\"\r\n");
            str.Append("							  GridLinesVisibility=\"All\"\r\n");
            str.Append("							  BorderThickness=\"1\" \r\n");
            str.Append("							  BorderBrush=\"Gray\"\r\n");
            str.Append("							  VerticalScrollBarVisibility=\"Auto\">\r\n");
            str.Append("						<DataGrid.Columns>\r\n");
            #region 复选框
            str.Append("							<DataGridTemplateColumn>\r\n");
            str.Append("								<DataGridTemplateColumn.HeaderTemplate>\r\n");
            str.Append("									<DataTemplate>\r\n");
            str.Append("										<CheckBox IsThreeState=\"True\" Margin=\"40, 0, 0, 0\"  IsChecked=\"{Binding $parent[UserControl].DataContext.IsSelectedAll}\" Command=\"{Binding $parent[UserControl].DataContext.SelectedAllCommand}\" x:CompileBindings=\"False\"/>\r\n");
            str.Append("									</DataTemplate>\r\n");
            str.Append("								</DataGridTemplateColumn.HeaderTemplate>\r\n");
            str.Append("								<DataGridTemplateColumn.CellTemplate>\r\n");
            str.Append("									<DataTemplate DataType=\"model:" + enu.ClassName + "\">\r\n");
            str.Append("										<CheckBox HorizontalAlignment=\"Center\" IsThreeState=\"False\" IsChecked=\"{Binding IsSelected, Mode = TwoWay}\" Command=\"{Binding SelectedCommand}\"/>\r\n");
            str.Append("									</DataTemplate>\r\n");
            str.Append("								</DataGridTemplateColumn.CellTemplate>\r\n");
            str.Append("							</DataGridTemplateColumn>\r\n");
            str.Append("			\r\n");

            #endregion

            #region 操作按钮
            str.Append("							<DataGridTemplateColumn Header=\"{i18n:I18n {x:Static lang:Language.Operate}}\">\r\n");
            str.Append("								<DataGridTemplateColumn.CellTemplate>\r\n");
            str.Append("									<DataTemplate DataType=\"model:" + enu.ClassName + "\">\r\n");
            str.Append("										<TextBlock HorizontalAlignment = \"Center\">\r\n");
            str.Append("										    <HyperlinkButton HorizontalAlignment = \"Center\" VerticalAlignment = \"Center\" Content = \"{i18n:I18n {x:Static lang:Language.Modify}}\" Command = \"{Binding $parent[UserControl].DataContext.Update" + enu.ClassName + "Command}\" CommandParameter = \"{Binding}\" x:CompileBindings = \"False\" />\r\n");
            str.Append("										</TextBlock>\r\n");
            str.Append("									</DataTemplate>\r\n");
            str.Append("								</DataGridTemplateColumn.CellTemplate>\r\n");
            str.Append("							</DataGridTemplateColumn>\r\n");
            str.Append("\r\n");
            #endregion

            #region Id 列（因为是在父类中定义的）
            //str.Append("							<DataGridTemplateColumn>\r\n");
            //str.Append("							    <DataGridTemplateColumn.HeaderTemplate>\r\n");
            //str.Append("							        <DataTemplate>\r\n");
            //str.Append("							            <ToggleButton Content=\"{i18n:I18n {x:Static lang:Language.Id}}\" Classes=\"DataGridHeader\"  IsChecked=\"False\" IsThreeState=\"False\" Command=\"{Binding $parent[UserControl].DataContext.SortingCommand}\" x:CompileBindings=\"False\">\r\n");
            //str.Append("							                <ToggleButton.CommandParameter>\r\n");
            //str.Append("							                    <MultiBinding Converter=\"{StaticResource listConverter}\">\r\n");
            //str.Append("							                        <Binding Source=\"Id\"/>\r\n");
            //str.Append("							                            <Binding Path=\"$self.IsChecked\"/>\r\n");
            //str.Append("							                    </MultiBinding>\r\n");
            //str.Append("							                </ToggleButton.CommandParameter>\r\n");
            //str.Append("							            </ToggleButton>\r\n");
            //str.Append("							        </DataTemplate>\r\n");
            //str.Append("							    </DataGridTemplateColumn.HeaderTemplate>\r\n");
            //str.Append("							    <DataGridTemplateColumn.CellTemplate>\r\n");
            //str.Append("							        <DataTemplate DataType=\"model:"+ enu.ClassName + "\">\r\n");
            //str.Append("                                        <TextBlock HorizontalAlignment=\"Center\" Text=\"{Binding Id, Mode=TwoWay}\"/>\r\n");
            //str.Append("                                    </DataTemplate>\r\n");
            //str.Append("                                </DataGridTemplateColumn.CellTemplate>\r\n");
            //str.Append("                        </DataGridTemplateColumn>\r\n");
            //str.Append("\r\n");
            #endregion
            foreach (CodeField field in al)
            {
                str.Append("							<DataGridTemplateColumn Width=\"*\">\r\n");
                str.Append("								<DataGridTemplateColumn.HeaderTemplate>\r\n");
                str.Append("									<DataTemplate>\r\n");
                str.Append("										<ToggleButton Classes=\"DataGridHeader\" Theme=\"{StaticResource DataGridHeaderToggleButton}\" Content=\"{i18n:I18n {x:Static lang:Language." + field.FieldName + "}}\" IsChecked=\"False\" IsThreeState=\"False\" Command=\"{Binding $parent[UserControl].DataContext.SortingCommand}\" x:CompileBindings=\"False\">\r\n");
                AddTranslateKeys(field.FieldName, field.FieldName);
                str.Append("											<ToggleButton.CommandParameter>\r\n");
                str.Append("												<MultiBinding Converter=\"{StaticResource listConverter}\">\r\n");
                str.Append("													<Binding Source=\"" + field.FieldName + "\"/>\r\n");
                str.Append("													<Binding Path=\"$self.IsChecked\"/>\r\n");
                str.Append("												</MultiBinding>\r\n");
                str.Append("											</ToggleButton.CommandParameter>\r\n");
                str.Append("										</ToggleButton>\r\n");
                str.Append("									</DataTemplate>\r\n");
                str.Append("								</DataGridTemplateColumn.HeaderTemplate>\r\n");
                str.Append("								<DataGridTemplateColumn.CellTemplate>\r\n");
                str.Append("									<DataTemplate DataType=\"model:" + enu.ClassName + "\">\r\n");
                str.Append("										<TextBlock HorizontalAlignment=\"Center\" Text=\"{Binding " + field.FieldName + ", Mode = TwoWay}\" TextWrapping=\"Wrap\"/>\r\n");
                str.Append("									</DataTemplate>\r\n");
                str.Append("								</DataGridTemplateColumn.CellTemplate>\r\n");
                str.Append("							</DataGridTemplateColumn>\r\n");
                str.Append("					\r\n");
            }
            str.Append("						</DataGrid.Columns>\r\n");
            str.Append("					</DataGrid>\r\n");
            str.Append("\r\n");
            #endregion

            #region 分页组件
            str.Append("\r\n");
            str.Append("\r\n");
            str.Append("					<control:PagerBar \r\n");
            str.Append("							FirstPageCommand=\"{Binding FirstPageCommand}\"\r\n");
            str.Append("							PrevPageCommand=\"{Binding PrevPageCommand}\"\r\n");
            str.Append("							NextPageCommand=\"{Binding NextPageCommand}\"\r\n");
            str.Append("							LastPageCommand=\"{Binding LastPageCommand}\"\r\n");
            str.Append("							GoToPageCommand=\"{Binding GoToPageCommand}\"\r\n");
            str.Append("							SelectionChangedCommand=\"{Binding SelectionChangedCommand}\"\r\n");
            str.Append("							AllCount=\"{Binding AllCount}\"\r\n");
            str.Append("							PageCount=\"{Binding PageCount}\"\r\n");
            str.Append("							CurrentPageIndex=\"{Binding CurrentPageIndex}\"\r\n");
            str.Append("							CommandParameter=\"{Binding $self.IndexToGo}\"\r\n");
            str.Append("							Margin=\"0, 10, 20, 10\"\r\n");
            str.Append("							x:CompileBindings=\"False\"/>\r\n");
            #endregion

            str.Append("\r\n");
            str.Append("				</StackPanel>\r\n");
            str.Append("				\r\n");
            str.Append("				\r\n");
            str.Append("				</suki:GlassCard>\r\n");
            str.Append("			</StackPanel>\r\n");
            str.Append("		</Panel>\r\n");
            str.Append("</UserControl>\r\n");

            return str.ToString();
        }
        /// <summary>
        /// 根据类名创建 ModelView 类的后台代码，即创建 model 新增、更改的界面，如 UserActionWindow.axaml.cs
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateViewCs(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("using Avalonia.Controls;\r\n");
            str.Append("\r\n");
            str.Append("namespace " + enu.ProjectName + "\r\n");
            str.Append("{\r\n");
            str.Append(string.Format("    public partial class {0}View : UserControl\r\n", enu.ClassName));
            str.Append("    {\r\n");
            str.Append(string.Format("        public {0}View()\r\n", enu.ClassName));
            str.Append("        {\r\n");
            str.Append("            InitializeComponent();\r\n");
            str.Append("        }\r\n");
            str.Append("    }\r\n");
            str.Append("}\r\n");

            return str.ToString();
        }

        private static string CreateBuildTableMapping(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            //    private void BuildCodeFieldTable(ModelBuilder modelBuilder)
            //{
            //    modelBuilder.Entity<CodeField>((entity) =>
            //    {
            //        entity.ToTable("CodeField");

            //        entity.Property(p => p.Id).HasColumnName("id");
            //        entity.Property(p => p.Cid).HasColumnName("cid");
            //        entity.Property(p => p.FieldName).HasColumnName("field_name");
            //        entity.Property(p => p.FieldType).HasColumnName("field_type");
            //        entity.Property(p => p.FieldLength).HasColumnName("field_length");
            //        entity.Property(p => p.FieldRemark).HasColumnName("field_remark");
            //        entity.Property(p => p.IsMainKey).HasColumnName("is_main_key");
            //        entity.Property(p => p.IsAllowNull).HasColumnName("is_allow_null");
            //        entity.Property(p => p.IsAutoIncrement).HasColumnName("is_auto_increment");
            //        entity.Property(p => p.IsUnique).HasColumnName("is_unique");
            //        entity.Property(p => p.DefaultValue).HasColumnName("default_value");
            //    });
            //}

            //var properties = typeof(T).GetProperties().Where(p =>
            //{
            //    // 不包含标记 NoExportAttribute 的属性，且不是 IRelayCommand 命令
            //    var include = Attribute.IsDefined(p, typeof(NoExportAttribute));
            //    var notCommand = p.PropertyType.Name.Equals("IRelayCommand");
            //    return !include && !notCommand;
            //}).ToList();

            str.Append("        private void Build"+enu.ClassName+"Table(ModelBuilder modelBuilder)\r\n");
            str.Append("        {\r\n");
            str.Append("            modelBuilder.Entity<"+enu.ClassName+">((entity) =>\r\n");
            str.Append("            {\r\n");
            str.Append("                entity.ToTable(\""+enu.TableName+"\");\r\n");
            var al = enu.CodeFields;
            foreach (CodeField item in al)
            {
                if (item.FieldName.Equals("aysn_id"))
                {
                    str.Append("                entity.Property(p => p.Id).HasColumnName(\"" + item.FieldName + "\");\r\n");
                }
                else
                {
                    str.Append("                entity.Property(p => p." + item.FieldNamePascalCase + ").HasColumnName(\"" + item.FieldName + "\");\r\n");
                }
            }
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");

            return str.ToString();
        }

        /// <summary>
        /// 创建表格界面对应的 ViewModel，如：UserViewModel
        /// </summary>
        /// <param name="enu"></param>
        /// <returns></returns>
        private static string CreateViewModel(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("using Avalonia.Controls;\r\n");
            str.Append("using Avalonia.Controls.ApplicationLifetimes;\r\n");
            str.Append("using Avalonia.Threading;\r\n");
            str.Append("using AvaloniaExtensions.Axaml.Markup;\r\n");
            str.Append("using CarParkingSystem.Dao;\r\n");
            str.Append("using CarParkingSystem.Messages;\r\n");
            str.Append("using CarParkingSystem.Models;\r\n");
            str.Append("using CarParkingSystem.Services;\r\n");
            str.Append("using CarParkingSystem.Unities;\r\n");
            str.Append("using CommunityToolkit.Mvvm.ComponentModel;\r\n");
            str.Append("using CommunityToolkit.Mvvm.Input;\r\n");
            str.Append("using CommunityToolkit.Mvvm.Messaging;\r\n");
            str.Append("using LinqKit;\r\n");
            str.Append("using Material.Icons;\r\n");
            str.Append("using Microsoft.Extensions.DependencyInjection;\r\n");
            str.Append("using SukiUI.Dialogs;\r\n");
            str.Append("using SukiUI.Toasts;\r\n");
            str.Append("using System;\r\n");
            str.Append("using System.Collections.Generic;\r\n");
            str.Append("using System.Collections.ObjectModel;\r\n");
            str.Append("using System.Linq;\r\n");
            str.Append("using System.Linq.Expressions;\r\n");
            str.Append("using System.Threading.Tasks;\r\n");
            str.Append("using SukiUI.ColorTheme;\r\n");
            str.Append("using SukiUI.Content;\r\n"); 
            str.Append("namespace " + enu.ProjectName + ".ViewModels\r\n");
            str.Append("{\r\n");
            str.Append(string.Format("  public partial class {0}ViewModel: DemoPageBase\r\n", enu.ClassName));
            str.Append("    {\r\n");
            #region 属性
            str.Append("        [ObservableProperty] private bool _isBusy;\r\n");
            str.Append(string.Format("        [ObservableProperty] private ObservableCollection<{0}> _{1}s;\r\n", enu.ClassName, Functions.FirstCharToLower(enu.ClassName)));
            str.Append(string.Format("        [ObservableProperty] private {0} _selected{0};\r\n", enu.ClassName));
            str.Append("        [ObservableProperty] private string _updateInfo;\r\n");
            str.Append(string.Format("        private {0}Dao _{1}Dao;\r\n", enu.ClassName, Functions.FirstCharToLower(enu.ClassName)));
            str.Append("        private AppDbContext _appDbContext;\r\n");
            str.Append("        public ISukiDialogManager _dialogManager { get; }\r\n");
            str.Append("        public ExpressionStarter<" + enu.ClassName + "> _predicate { get; set; }\r\n");
            str.Append("        public Expression<Func<" + enu.ClassName + ", object>> _expression { get; set; }\r\n");
            str.Append("        public bool _isOrderByDesc { get; set; }\r\n");
            str.Append("        [ObservableProperty] private int _allCount;\r\n");
            str.Append("        [ObservableProperty] private int _pageSize = 15;//默认每页显示15条\r\n");
            str.Append("        [ObservableProperty] private int _pageCount;\r\n");
            str.Append("        [ObservableProperty] private int _currentPageIndex = 1;//默认显示第一页\r\n");
            str.Append("        [ObservableProperty] private int _indexToGo;\r\n");
            str.Append("        [ObservableProperty] private bool?  _isSelectedAll = false;\r\n");
            #region 支持搜索的字段
            var al = enu.CodeFields;
            foreach (CodeField field in al)
            {
                str.Append(string.Format("        [ObservableProperty] private {0} _search{1};\r\n", SqliteTypeConverter.GetCSharpType(field.FieldType), field.FieldNamePascalCase));
            }
            str.Append("        [ObservableProperty] private string _searchStartDateTime;\r\n");
            str.Append("        [ObservableProperty] private string _searchEndDateTime;\r\n");
            #endregion
            #endregion
            str.Append(string.Format("      public {0}ViewModel({0}Dao {1}Dao, AppDbContext appDbContext, ISukiDialogManager dialogManager) : base(\"{2}Management\", MaterialIconKind.{3}, pid: {4}, id: {5}, index: {6})\r\n", enu.ClassName, Functions.FirstCharToLower(enu.ClassName), enu.ClassName, enu.Icon, enu.Pid, enu.Id, enu.Index));
            str.Append("        {\r\n");
            str.Append(string.Format("            _{0}Dao = {0}Dao;\r\n", Functions.FirstCharToLower(enu.ClassName)));
            str.Append("            RefreshData();\r\n");
            str.Append("            _appDbContext = appDbContext;\r\n");
            str.Append("            _dialogManager = dialogManager;\r\n");
            str.Append("            WeakReferenceMessenger.Default.Register<ToastMessage, string>(this, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN, Recive);\r\n");
            str.Append(string.Format(           "WeakReferenceMessenger.Default.Register<Selected{0}Message, string>(this, TokenManage.REFRESH_SUMMARY_SELECTED_{1}_TOKEN, ReciveRefreshSummarySelected{0});\r\n", enu.ClassName, enu.ClassName.ToUpper()));
            str.Append("        }\r\n");
            #region 方法
            str.Append("\r\n");
            str.Append("        /// <summary>\r\n");
            str.Append("        /// 1.在列表内选择时会触发；\r\n");
            str.Append("        /// 2.新增；\r\n");
            str.Append("        /// 3.删除；\r\n");
            str.Append("        /// 4.改变 PageSize；\r\n");
            str.Append("        /// 5.条件搜索时；\r\n");
            str.Append("        /// 6.重置搜索时；\r\n");
            str.Append("        /// </summary>\r\n");
            str.Append("        /// <param name=\"recipient\"></param>\r\n");
            str.Append("        /// <param name=\"message\"></param>\r\n");
            str.Append(string.Format("        private void ReciveRefreshSummarySelected{0}(object recipient, Selected{0}Message message)\r\n", enu.ClassName));
            str.Append("        {\r\n");
            str.Append(string.Format("          var selectedCount = {0}s?.Count(x => x.IsSelected);\r\n", enu.ClassName));
            str.Append("            if (selectedCount == 0)\r\n");
            str.Append("            {\r\n");
            str.Append("                IsSelectedAll = false;\r\n");
            str.Append("            }\r\n");
            str.Append(string.Format("            else if (selectedCount == {0}s.Count())\r\n", enu.ClassName));
            str.Append("            {\r\n");
            str.Append("                IsSelectedAll = true;\r\n");
            str.Append("            }\r\n");
            str.Append("            else {\r\n");
            str.Append("                IsSelectedAll = null;\r\n");
            str.Append("            }\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append("        private void Recive(object recipient, ToastMessage message)\r\n");
            str.Append("        {\r\n");
            str.Append("            //20250402, 不是相同类型发送的消息不处理\r\n");
            str.Append("            if (message.CurrentModelType != typeof("+enu.ClassName+")) return;\r\n");
            str.Append("            \r\n");
            str.Append("            if (message.NeedRefreshData)\r\n");
            str.Append("            {\r\n");
            str.Append("                RefreshData(_predicate, _expression, _isOrderByDesc);\r\n");
            str.Append("            }\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append(string.Format("        private void RefreshData(ExpressionStarter<{0}> predicate = null, Expression<Func<{0}, object>> expression = null, bool isDesc = false)\r\n", enu.ClassName));
            str.Append("        {\r\n");
            str.Append(string.Format("          {0}s = new ObservableCollection<{0}>(_{1}Dao.GetAllPaged(CurrentPageIndex, PageSize, expression, isDesc, predicate));\r\n", enu.ClassName, Functions.FirstCharToLower(enu.ClassName)));
            str.Append(string.Format("          AllCount = _{0}Dao.Count(predicate);\r\n", Functions.FirstCharToLower(enu.ClassName)));
            str.Append("            PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append(string.Format("        private Task RefreshDataAsync(ExpressionStarter<{0}> predicate = null, Expression<Func<{0}, object>> expression = null, bool isDesc = false)\r\n", enu.ClassName));
            str.Append("        {\r\n");
            str.Append("            return Task.Run(async () => {\r\n");
            str.Append(string.Format("              var result = await _{0}Dao.GetAllPagedAsync(CurrentPageIndex, PageSize, expression, isDesc, predicate);\r\n", Functions.FirstCharToLower(enu.ClassName)));
            str.Append(string.Format("              {0}s = new ObservableCollection<{0}>(result);\r\n", enu.ClassName));
            str.Append(string.Format("                AllCount = _{0}Dao.Count(predicate);\r\n", Functions.FirstCharToLower(enu.ClassName)));
            str.Append("                PageCount = (int)Math.Ceiling(AllCount / (double)PageSize);\r\n");
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            #endregion

            #region 命令
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private Task Export(Window window)\r\n");
            str.Append("        {\r\n");
            str.Append("            return Task.Run(async () =>\r\n");
            str.Append("            {\r\n");
            str.Append(string.Format("                var filePath = await ExcelService.ExportToExcelAsync(window, {0}s.ToList(), \"{0}s.xlsx\", I18nManager.GetString(\"{0}Info\"));\r\n", enu.ClassName));
            AddTranslateKeys($"{enu.ClassName}Info", $"{enu.ClassName}Info");
            str.Append("                ExcelService.HighlightFileInExplorer(filePath);\r\n");
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private void SelectedAll()\r\n");
            str.Append("        {\r\n");
            str.Append("            if (IsSelectedAll == null)\r\n"); 
            str.Append("            {\r\n");
            str.Append("                //实现可以显示三种状态，但是点击只有两种情况的功能\r\n");
            str.Append("                IsSelectedAll = false;\r\n");
            str.Append("                //return;\r\n");
            str.Append("            }\r\n");
            str.Append("\r\n");
            str.Append("            if (IsSelectedAll == false) {\r\n");
            str.Append(string.Format("                foreach (var item in {0}s)\r\n", enu.ClassName));
            str.Append("                {\r\n");
            str.Append("                    item.IsSelected = false;\r\n");
            str.Append("                }\r\n");
            str.Append("            }\r\n");
            str.Append("            else\r\n");
            str.Append("            {\r\n");
            str.Append(string.Format("                foreach (var item in {0}s)\r\n", enu.ClassName));
            str.Append("                {\r\n");
            str.Append("                    item.IsSelected = true;\r\n");
            str.Append("                }\r\n");
            str.Append("            }\r\n");
            str.Append("            \r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            #region 排序
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private Task Sorting(object obj)\r\n");
            str.Append("        {\r\n");
            str.Append("            List<object> list = (List<object>)obj;\r\n");
            str.Append("            var fieldName = list[0] as string;\r\n");
            str.Append("            var isDesc = (bool)list[1];\r\n");
            str.Append("\r\n");
            str.Append("            return Task.Run(async () =>\r\n");
            str.Append("            {\r\n");
            str.Append("                _expression = u => u.Id;\r\n");
            str.Append("                _isOrderByDesc = isDesc;\r\n");
            str.Append("\r\n");
            str.Append("                switch (fieldName)\r\n");
            str.Append("                {\r\n");
            foreach (CodeField field in al)
            {
                //20250403, add
                if (field.FieldName.Equals("aysn_id"))
                {
                    str.Append("                    case \"Id\":\r\n");
                    str.Append("                        _expression = u => u.Id; break;\r\n");
                }
                else
                {
                    str.Append("                    case \"" + field.FieldNamePascalCase + "\":\r\n");
                    str.Append("                        _expression = u => u." + field.FieldNamePascalCase + "; break;\r\n");
                }
            }
            str.Append("                        break;\r\n");
            str.Append("                }\r\n");
            str.Append("\r\n");
            str.Append("                // 搜索后的结果进行排序时，也需要考虑过滤的条件\r\n");
            str.Append("                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);\r\n");
            str.Append("\r\n");
            str.Append("                //IsBusy = true;\r\n");
            str.Append("                //await Task.Delay(3000);\r\n");
            str.Append("                //IsBusy = false;\r\n");
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            #endregion
            #region 搜索命令，添加需要搜索的字段
            str.Append("        [RelayCommand]\r\n");
            str.Append(string.Format("        private Task Search{0}()\r\n", enu.ClassName));
            str.Append("        {\r\n");
            str.Append("            return Task.Run(async () =>\r\n");
            str.Append("            {\r\n");
            str.Append("                _predicate = PredicateBuilder.New<"+ enu.ClassName + ">(true);\r\n");
            bool hasCreatedAtField = false;
            foreach (CodeField field in al)
            {
                if (field.FieldName.Contains("CreatedAt") || field.FieldName.Contains("created_at"))
                {
                    hasCreatedAtField = true;
                }
                //默认只添加 string 类型的字段来进行搜索
                if (!field.FieldName.EndsWith("At") && !field.FieldName.EndsWith("at") && SqliteTypeConverter.GetCSharpType(field.FieldType).FullName == "System.String")
                {
                    str.Append(string.Format("                if (!String.IsNullOrEmpty(Search{0})) \r\n", field.FieldNamePascalCase));
                    str.Append("                {\r\n");
                    str.Append(string.Format("                  _predicate = _predicate.And(p => p.{0}.Contains(Search{0}));\r\n", field.FieldNamePascalCase));
                    str.Append("                }\r\n");
                }
            }
            //如果需要按时间排序的
            if (hasCreatedAtField)
            {
                str.Append("                if (!String.IsNullOrEmpty(SearchStartDateTime)) \r\n");
                str.Append("                {\r\n");
                str.Append("                    var endDate = DateTime.Parse(SearchStartDateTime).ToString(\"yyyy-MM-dd HH:mm:ss\");\r\n");
                str.Append("                    _predicate = _predicate.And(p => p.CreatedAt.CompareTo(endDate) > 0);\r\n");
                str.Append("                }\r\n");
                str.Append("\r\n");
                str.Append("                if (!String.IsNullOrEmpty(SearchEndDateTime)) \r\n");
                str.Append("                {\r\n");
                str.Append("                    var endDate = DateTime.Parse(SearchEndDateTime).ToString(\"yyyy-MM-dd HH:mm:ss\");\r\n");
                str.Append("                    _predicate = _predicate.And(p => p.CreatedAt.CompareTo(endDate) < 0);\r\n");
                str.Append("                }\r\n");
            }

            str.Append("                CurrentPageIndex = 1;//点击搜索按钮时，也应该从第一页显示\r\n");
            str.Append("                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);\r\n");
            str.Append(string.Format("                WeakReferenceMessenger.Default.Send<Selected{0}Message, string>(TokenManage.REFRESH_SUMMARY_SELECTED_{1}_TOKEN);\r\n", enu.ClassName, enu.ClassName.ToUpper()));
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            #endregion
            str.Append("\r\n");
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private void ResetSearch()\r\n");
            str.Append("        {\r\n");
            foreach (CodeField field in al)
            {
                if (!field.FieldName.EndsWith("At")
                    && !field.FieldName.EndsWith("at")
                    && SqliteTypeConverter.GetCSharpType(field.FieldType).FullName == "System.String")
                {
                    str.Append(string.Format("            Search{0} = string.Empty;\r\n", field.FieldNamePascalCase));
                }
            }
            str.Append("            SearchStartDateTime = null;\r\n");
            str.Append("            SearchEndDateTime = null;\r\n");
            str.Append("            _predicate = null;//条件查询的 表达式 也进行重置\r\n");
            str.Append("            _expression = null;\r\n");
            str.Append("            _isOrderByDesc = false;\r\n");
            str.Append(string.Format("            WeakReferenceMessenger.Default.Send<Selected{0}Message, string>(TokenManage.REFRESH_SUMMARY_SELECTED_{1}_TOKEN);\r\n", enu.ClassName, enu.ClassName.ToUpper()));
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append("        /// <summary>\r\n");
            str.Append("        /// </summary>\r\n");
            str.Append("        /// <param name=\"obj\"></param>\r\n");
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private Task SelectionChanged(int pageSize)\r\n");
            str.Append("        {\r\n");
            str.Append("            return Task.Run(async ()=>\r\n");
            str.Append("            {\r\n");
            str.Append("                PageSize = pageSize;\r\n");
            str.Append("\r\n");
            str.Append("                CurrentPageIndex = 1;//更改页面数量应该从第一页显示\r\n");
            str.Append("\r\n");
            str.Append("                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);\r\n");
            str.Append("\r\n");
            str.Append(string.Format("                WeakReferenceMessenger.Default.Send<Selected{0}Message, string>(TokenManage.REFRESH_SUMMARY_SELECTED_{1}_TOKEN);\r\n", enu.ClassName, enu.ClassName.ToUpper()));
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            #region 打开新增窗口
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private void Add()\r\n");
            str.Append("        {\r\n");
            str.Append(string.Format("            var actionVM = App.ServiceProvider.GetService<{0}ActionWindowViewModel>();\r\n", enu.ClassName));
            foreach (CodeField field in al)
            {
                if ((field.FieldName.EndsWith("At") || field.FieldName.EndsWith("DateTime")
                    || field.FieldName.EndsWith("at") || field.FieldName.EndsWith("datetime") || field.FieldName.EndsWith("date"))
                    && SqliteTypeConverter.GetCSharpType(field.FieldType).FullName != "System.Int32")
                {
                    str.Append(string.Format("            actionVM.{0} = null;\r\n", field.FieldNamePascalCase));
                }
                else if (SqliteTypeConverter.GetCSharpType(field.FieldType).FullName == "System.String")
                {
                    str.Append(string.Format("            actionVM.{0} = string.Empty;\r\n", field.FieldNamePascalCase));
                }
            }
            str.Append("            actionVM.UpdateInfo = string.Empty;\r\n");
            str.Append("            actionVM.ClearVertifyErrors();\r\n");
            str.Append(string.Format("            actionVM.IsAdd{0} = true;\r\n", enu.ClassName));
            str.Append("            actionVM.Title = I18nManager.GetString(\"CreateNew"+enu.ClassName+"\"); \r\n");
            AddTranslateKeys($"CreateNew{enu.ClassName}", $"CreateNew{enu.ClassName}");
            str.Append(string.Format("            var actionWindow = App.Views.CreateView<{0}ActionWindowViewModel>(App.ServiceProvider) as Window;\r\n", enu.ClassName));
            str.Append("\r\n");
            str.Append("            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;\r\n");
            str.Append("            if (owner != null)\r\n");
            str.Append("            {\r\n");
            str.Append("                actionWindow?.ShowDialog(owner);\r\n");
            str.Append("            }\r\n");
            str.Append("            else\r\n");
            str.Append("            {\r\n");
            str.Append("                actionWindow?.Show();\r\n");
            str.Append("            }\r\n");
            str.Append("\r\n");
            str.Append(string.Format("            WeakReferenceMessenger.Default.Send<Selected{0}Message, string>(TokenManage.REFRESH_SUMMARY_SELECTED_{1}_TOKEN);\r\n", enu.ClassName, enu.ClassName.ToUpper()));
            str.Append("            \r\n");
            str.Append("            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(\r\n");
            str.Append("                new MaskLayerMessage\r\n");
            str.Append("                {\r\n");
            str.Append("                    IsNeedShow = true,\r\n");
            str.Append("                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            #endregion
            #region 打开修改窗口
            str.Append("        [RelayCommand]\r\n");
            str.Append(string.Format("        private void Update{0}({1} {2})\r\n", enu.ClassName, enu.ClassName, Functions.FirstCharToLower(enu.ClassName)));
            str.Append("        {\r\n");
            str.Append("            var actionVM = App.ServiceProvider.GetService<"+enu.ClassName+"ActionWindowViewModel>();\r\n");
            str.Append("            actionVM.Selected"+enu.ClassName+" = "+ Functions.FirstCharToLower(enu.ClassName) + ";\r\n");
            foreach (CodeField field in al)
            {
                if (!(field.FieldName.Equals("Id") 
                    && !(field.FieldName.Equals("aysn_id"))
                    && SqliteTypeConverter.GetCSharpType(field.FieldType).FullName == "System.Int32"))
                {
                    str.Append(string.Format("            actionVM.{0} = " + Functions.FirstCharToLower(enu.ClassName) + ".{0};\r\n", field.FieldNamePascalCase));
                }
                else if ((field.FieldName.EndsWith("At") || field.FieldName.EndsWith("DateTime") 
                    || field.FieldName.EndsWith("at") || field.FieldName.EndsWith("datetime")
                    || field.FieldName.EndsWith("date"))
                    && SqliteTypeConverter.GetCSharpType(field.FieldType).FullName != "System.Int32")
                {
                    str.Append(string.Format("            actionVM.{0} = "+Functions.FirstCharToLower(enu.ClassName)+".{0};\r\n", field.FieldNamePascalCase));
                }
                else if (SqliteTypeConverter.GetCSharpType(field.FieldType).FullName == "System.String")
                {
                    str.Append(string.Format("            actionVM.{0} = "+ Functions.FirstCharToLower(enu.ClassName) + ".{0};\r\n", field.FieldNamePascalCase));
                }
            }
            str.Append("            actionVM.UpdateInfo = string.Empty;\r\n");
            str.Append("            actionVM.ClearVertifyErrors();\r\n");
            str.Append("            actionVM.IsAdd"+enu.ClassName+" = false;\r\n");
            str.Append("            actionVM.Title = I18nManager.GetString(\"Update"+enu.ClassName+"Info\");\r\n");
            AddTranslateKeys($"Update{enu.ClassName}Info", $"Update{enu.ClassName}Info");
            str.Append("            var actionWindow = App.Views.CreateView<"+enu.ClassName+"ActionWindowViewModel>(App.ServiceProvider) as Window;\r\n");
            str.Append("            var owner = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;\r\n");
            str.Append("            if (owner != null)\r\n");
            str.Append("            {\r\n");
            str.Append("                actionWindow?.ShowDialog(owner);\r\n");
            str.Append("            }\r\n");
            str.Append("            else\r\n");
            str.Append("            {\r\n");
            str.Append("                actionWindow?.Show();\r\n");
            str.Append("            }\r\n");
            str.Append("\r\n");
            str.Append("            WeakReferenceMessenger.Default.Send<MaskLayerMessage, string>(\r\n");
            str.Append("                new MaskLayerMessage\r\n");
            str.Append("                {\r\n");
            str.Append("                    IsNeedShow = true,\r\n");
            str.Append("                }, TokenManage.MAIN_WINDOW_MASK_LAYER_TOKEN);\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append("\r\n");
            #endregion
            #region 删除
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private void Delete"+enu.ClassName+"()\r\n");
            str.Append("        {\r\n");
            str.Append("            var selectedCount = "+enu.ClassName+"s.Count(u => u.IsSelected);\r\n");
            str.Append("            if (selectedCount == 0)\r\n");
            str.Append("            {\r\n");
            str.Append("                _dialogManager.CreateDialog()\r\n");
            str.Append("                .WithTitle(I18nManager.GetString(\"Delete"+enu.ClassName+"Prompt\"))\r\n");
            AddTranslateKeys($"Delete{enu.ClassName}Prompt", $"Delete{enu.ClassName}Prompt");
            str.Append("                .WithContent(I18nManager.GetString(\"SelectDelete"+enu.ClassName+"\"))\r\n");
            AddTranslateKeys($"SelectDelete{enu.ClassName}", $"SelectDeletete{enu.ClassName}");
            str.Append("                .Dismiss().ByClickingBackground()\r\n");
            str.Append("                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)\r\n");
            str.Append("                .WithActionButton(I18nManager.GetString(\"Dismiss\"), _ => { }, true)\r\n");
            str.Append("                .TryShow();\r\n");
            str.Append("            }\r\n");
            str.Append("            else\r\n");
            str.Append("            {\r\n");
            str.Append("                _dialogManager.CreateDialog()\r\n");
            str.Append("                .WithTitle(I18nManager.GetString(\"Delete"+enu.ClassName+"\"))\r\n");
            AddTranslateKeys($"Delete{enu.ClassName}", $"Deletete{enu.ClassName}");
            str.Append("                .WithContent(I18nManager.GetString(\"SureWantToDelete"+enu.ClassName+"\"))\r\n");
            AddTranslateKeys($"SureWantToDelete{enu.ClassName}", $"SureWantToDelete{enu.ClassName}");
            str.Append("                .WithActionButton(I18nManager.GetString(\"Sure\"), dialog => SureDeleteAsync(dialog), false)\r\n");
            str.Append("                .WithActionButton(I18nManager.GetString(\"Cancel\"), _ => CancelDelete(), true)\r\n");
            str.Append("                .OfType(Avalonia.Controls.Notifications.NotificationType.Information)\r\n");
            str.Append("                .TryShow();\r\n");
            str.Append("            }\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append("        private void CancelDelete()\r\n");
            str.Append("        {\r\n");
            str.Append("            return;\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append("        private async Task SureDeleteAsync(ISukiDialog dialog)\r\n");
            str.Append("        {\r\n");
            str.Append("            var selectedIds = "+enu.ClassName+"s.Where(u => u.IsSelected).Select(u => u.Id);\r\n");
            str.Append("            int result = await _"+Functions.FirstCharToLower(enu.ClassName)+"Dao.DeleteRangeAsync<"+enu.ClassName+">(new List<int>(selectedIds));\r\n");
            str.Append("\r\n");
            str.Append("            if (result > 0)\r\n");
            str.Append("            {\r\n");
            str.Append("                Dispatcher.UIThread.Invoke(() =>\r\n");
            str.Append("                {\r\n");
            str.Append("                    //20250401, 删除成功也用 dialog 提示\r\n");
            str.Append("                    dialog.Icon = Icons.Check;\r\n");
            str.Append("                    dialog.IconColor = NotificationColor.SuccessIconForeground;\r\n");
            str.Append("                    dialog.Title = I18nManager.GetString(\"Delete"+enu.ClassName+"Prompt\");\r\n");
            str.Append("                    dialog.Content = I18nManager.GetString(\"DeleteSuccessfully\");\r\n");
            str.Append("                    if (dialog.ActionButtons.Count > 1)\r\n");
            str.Append("                    {\r\n");
            str.Append("                        (dialog.ActionButtons[0] as Button).IsVisible = false;//隐藏第一个按钮\r\n");
            str.Append("                        \r\n");
            str.Append("                        var button = dialog.ActionButtons[1] as Button;//将取消按钮赋值为“确定”\r\n");
            str.Append("                        button.Content = I18nManager.GetString(\"Submit\");\r\n");
            str.Append("                    }\r\n");
            str.Append("                    \r\n");
            str.Append("                    WeakReferenceMessenger.Default.Send(\r\n");
            str.Append("                                new ToastMessage\r\n");
            str.Append("                                {\r\n"); 
            str.Append("                                    CurrentModelType = typeof("+enu.ClassName+"),\r\n");
            str.Append("                                    Title = I18nManager.GetString(\"Delete" + enu.ClassName + "Prompt\"),\r\n");
            AddTranslateKeys($"Delete{enu.ClassName}Prompt", $"Delete{enu.ClassName}Prompt");
            str.Append("                                    Content = I18nManager.GetString(\"DeleteSuccessfully\"),\r\n");
            str.Append("                                    NotifyType = Avalonia.Controls.Notifications.NotificationType.Success,\r\n");
            str.Append("                                    NeedRefreshData = true,\r\n");
            str.Append("                                    NeedShowToast = false,\r\n");
            str.Append("                                }, TokenManage.MAIN_WINDOW_SHOW_TOAST_TOKEN);\r\n");
            str.Append("\r\n");

            str.Append("\r\n");
            str.Append("                    WeakReferenceMessenger.Default.Send<Selected"+enu.ClassName+"Message, string>(TokenManage.REFRESH_SUMMARY_SELECTED_"+enu.ClassName.ToUpper()+"_TOKEN);\r\n");
            str.Append("                });\r\n");
            str.Append("            }\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            #endregion
            #region 分页相关
            str.Append("        #region 分页\r\n");
            str.Append("\r\n");
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private Task FirstPage()\r\n");
            str.Append("        {\r\n");
            str.Append("            return Task.Run(async () => {\r\n");
            str.Append("                CurrentPageIndex = 1;\r\n");
            str.Append("                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);\r\n");
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private Task PrevPage()\r\n");
            str.Append("        {\r\n");
            str.Append("            return Task.Run(async () => {\r\n");
            str.Append("                if (CurrentPageIndex > 1)\r\n");
            str.Append("                {\r\n");
            str.Append("                    CurrentPageIndex = CurrentPageIndex - 1;\r\n");
            str.Append("                    await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);\r\n");
            str.Append("                }\r\n");
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private Task NextPage()\r\n");
            str.Append("        {\r\n");
            str.Append("            return Task.Run(async () => {\r\n");
            str.Append("                if (CurrentPageIndex < PageCount)\r\n");
            str.Append("                {\r\n");
            str.Append("                    CurrentPageIndex = CurrentPageIndex + 1;\r\n");
            str.Append("                    await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);\r\n");
            str.Append("                }\r\n");
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private Task LastPage()\r\n");
            str.Append("        {\r\n");
            str.Append("            return Task.Run(async () => {\r\n");
            str.Append("                CurrentPageIndex = PageCount;\r\n");
            str.Append("                await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);\r\n");
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            str.Append("\r\n");
            str.Append("        [RelayCommand]\r\n");
            str.Append("        private Task GoToPage(object obj)\r\n");
            str.Append("        {\r\n");
            str.Append("            return Task.Run(async () => {\r\n");
            str.Append("                int pageIndexToGo = 0;\r\n");
            str.Append("                try\r\n");
            str.Append("                {\r\n");
            str.Append("                    pageIndexToGo = System.Convert.ToInt32(obj);\r\n");
            str.Append("                }\r\n");
            str.Append("                catch (Exception ex)\r\n");
            str.Append("                {\r\n");
            str.Append("                }\r\n");
            str.Append("\r\n");
            str.Append("                // 如果跳转到的页码就是当前页，就不跳转\r\n");
            str.Append("                if (pageIndexToGo > 0\r\n");
            str.Append("                    && pageIndexToGo <= PageCount\r\n");
            str.Append("                    && pageIndexToGo != CurrentPageIndex)\r\n");
            str.Append("                {\r\n");
            str.Append("                    CurrentPageIndex = pageIndexToGo;\r\n");
            str.Append("                    await RefreshDataAsync(_predicate, _expression, _isOrderByDesc);\r\n");
            str.Append("                }\r\n");
            str.Append("            });\r\n");
            str.Append("        }\r\n");
            str.Append("        #endregion\r\n");
            str.Append("\r\n");
            #endregion
            #endregion
            str.Append("    }\r\n");
            str.Append("}\r\n");
            return str.ToString();
        }
        private static string CreateModel(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();
            str.Append("using CarParkingSystem.Unities;\r\n");
            str.Append("using CommunityToolkit.Mvvm.ComponentModel;\r\n");
            str.Append("using CommunityToolkit.Mvvm.Input;\r\n");
            str.Append("using CommunityToolkit.Mvvm.Messaging;\r\n");
            str.Append("using System;\r\n");
            str.Append("using System.Collections.Generic;\r\n");
            str.Append("using System.Linq;\r\n");
            str.Append("using System.Text;\r\n");
            str.Append("using System.Data;\r\n");
            str.Append("using System.Threading.Tasks;\r\n");
            str.Append("namespace " + enu.ProjectName + ".Models\r\n");
            str.Append("{\r\n");
            str.Append(string.Format("public partical class {0}Model: {1} \r\n", enu.ClassName, enu.ClassName));
            str.Append("{\r\n");

            str.Append("          public static  " + enu.ClassName + "Model getInstance(" + enu.ClassName + " obj)\r\n");
            str.Append(" {\r\n");
            str.Append("  return AutoMap.Mapper.Map<" + enu.ClassName + "Model>(obj);\r\n");
            str.Append("   }\r\n");
            str.Append("}\r\n");
            str.Append("}\r\n");
            return str.ToString();
        }
        

        /*
        private static string CreateAction(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();
            str.Append("using System;\r\n");
            str.Append("using System.Collections.Generic;\r\n");
            str.Append("using System.Text;\r\n");
            str.Append("using System.Data;\r\n");
            str.Append("using System.Data.SqlClient;\r\n");
            str.Append("using Core;\r\n");
            str.Append("namespace " + enu.g_ProjectId + "\r\n");
            str.Append("{\r\n");
            str.Append(string.Format("public class {0}Action\r\n", enu.ClassName));
            str.Append("{\r\n");

            str.Append("  public static " + enu.ClassName + " GetDataByID(int id)\r\n");
            str.Append(" {\r\n");
            str.Append("    using (SqlConnection myConnection = new SqlConnection(FaceInfoConfig.DataBaseConnectString))\r\n");
            str.Append("    {\r\n");
            str.Append("       SqlCommand myCommand = new SqlCommand(\"sp_" + enu.ClassName + "_ByID\", myConnection);\r\n");
            str.Append("       myCommand.CommandType = CommandType.StoredProcedure;\r\n");
            string fid = "";
            foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
            {
                if (field.IsMainKey)
                {
                    fid = field.FieldName;
                    break;
                }
            }
            str.Append("       myCommand.Parameters.Add(\"@" + fid + "\", SqlDbType.Int).Value = id;\r\n");
            str.Append("       myConnection.Open();\r\n");
            str.Append("       SqlDataReader dr = myCommand.ExecuteReader();\r\n");
            str.Append("       " + enu.ClassName + " em = new " + enu.ClassName + "();\r\n");
            str.Append("       if (dr.Read())\r\n");
            str.Append("       {\r\n");
            str.Append("          em = " + enu.ClassName + ".PopulateObjectFromIDataReader(dr);\r\n");
            str.Append("       }\r\n");
            str.Append("       dr.Close();\r\n");
            str.Append("       myConnection.Close();\r\n");
            str.Append("       return em;\r\n");
            str.Append("   }\r\n");
            str.Append(" }\r\n");
            str.Append(" public static List<" + enu.ClassName + "> GetDatas()\r\n");
            str.Append(" {\r\n");
            str.Append("     List<" + enu.ClassName + "> al = new List<" + enu.ClassName + ">();\r\n");
            str.Append("     using (SqlConnection myConnection =  new SqlConnection(FaceInfoConfig.DataBaseConnectString))\r\n");
            str.Append("    {\r\n");
            str.Append("        SqlCommand myCommand = new SqlCommand(\"sp_" + enu.ClassName + "_ByData\", myConnection);\r\n");
            str.Append("        myCommand.CommandType = CommandType.StoredProcedure;\r\n");
            str.Append("        myConnection.Open();\r\n");
            str.Append("        SqlDataReader reader = myCommand.ExecuteReader();\r\n");
            str.Append("        while (reader.Read())\r\n");
            str.Append("        {\r\n");
            str.Append("            al.Add(" + enu.ClassName + ".PopulateObjectFromIDataReader(reader));\r\n");
            str.Append("        }\r\n");
            str.Append("          myConnection.Close();\r\n");
            str.Append("     }\r\n");
            str.Append("    return al;\r\n");
            str.Append("  }\r\n");

            str.Append("   public static " + enu.ClassName + " CreateUpdateDelete(" + enu.ClassName + " em, DataProviderAction action)\r\n");
            str.Append("{\r\n");
            str.Append("   using (SqlConnection myConnection =  new SqlConnection(FaceInfoConfig.DataBaseConnectString))\r\n");
            str.Append("   {\r\n");
            str.Append("       SqlCommand myCommand = new SqlCommand(\"sp_" + enu.ClassName + "_CreateUpdateDelete\", myConnection);\r\n");
            str.Append("        myCommand.CommandType = CommandType.StoredProcedure;\r\n");
            str.Append("       myCommand.Parameters.Add(\"@Action\", SqlDbType.Int).Value = action;\r\n");
            str.Append("      if (action == DataProviderAction.Create)\r\n");
            str.Append("           myCommand.Parameters.Add(\"@" + fid + "\", SqlDbType.Int).Direction = ParameterDirection.Output;\r\n");
            str.Append("       else\r\n");
            str.Append("          myCommand.Parameters.Add(\"@" + fid + "\", SqlDbType.Int).Value = em." + fid + ";\r\n");
            str.Append("      if ((action == DataProviderAction.Create) || (action == DataProviderAction.Update))\r\n");
            str.Append("      {\r\n");
            foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
            {
                if (!field.IsMainKey)
                {
                    str.Append("           myCommand.Parameters.Add(\"@" + field.FieldName + "\", SqlDbType." + Formatter.GetSqlType(field.FieldType, field.FieldLength) + ").Value = " + Formatter.GetChangeType(field.FieldType) + "em." + field.FieldName + ";\r\n");
                }
            }
            str.Append("       }\r\n");
            str.Append("     myConnection.Open();\r\n");
            str.Append("     " + enu.ClassName + " rem = em;\r\n");
            str.Append("     try\r\n");
            str.Append("     {\r\n");
            str.Append("         int pid = Formatter.ToInt(myCommand.ExecuteScalar());\r\n");
            str.Append("         if (action == DataProviderAction.Create) rem = " + enu.ClassName + "s.GetDataByID(pid);\r\n");
            str.Append("     }\r\n");
            str.Append("    catch (Exception exc)\r\n");
            str.Append("    {\r\n");
            str.Append("       string str = exc.Message;\r\n");
            str.Append("   }\r\n");
            str.Append("    myConnection.Close();\r\n");
            str.Append("    return rem;\r\n");
            str.Append(" }\r\n");
            str.Append("  }\r\n");

            str.Append("}\r\n");
            str.Append("}\r\n");
            return str.ToString();
        }
        */

        /*
        private static string CreateGrid(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();
            str.Append("using System;\r\n");
            str.Append("using System.Collections.Generic;\r\n");
            str.Append("using System.Linq;\r\n");
            str.Append("using System.Web;\r\n");
            str.Append("using System.Data;\r\n");
            str.Append("using System.Data.SqlClient;\r\n");
            str.Append("using Core;\r\n");
            str.Append("using System.Collections.Specialized;\r\n");

            str.Append("namespace DTO.Face\r\n");
            str.Append("{\r\n");
            str.Append(string.Format("public class {0}Grid\r\n", enu.ClassName));
            str.Append("{\r\n");
            str.Append("/// <summary>\r\n");
            str.Append("/// 转换为实例\r\n");
            str.Append("/// </summary>\r\n");
            str.Append("/// <param name=\"dt\"></param>\r\n");
            str.Append("/// <returns></returns>\r\n");
            str.Append(string.Format("public static List<{0}Model> ToList(DataTable dt)\r\n", enu.ClassName));
            str.Append("{\r\n");
            str.Append(string.Format("var al = new List<{0}Model>();\r\n", enu.ClassName));
            str.Append("foreach (DataRow dr in dt.Rows)\r\n");
            str.Append("{\r\n");
            str.Append("  var item = " + enu.ClassName + ".PopulateObjectFromIDataReader(dr); \r\n");
            str.Append("   al.Add(" + enu.ClassName + "Model.getInstance(item));\r\n");
            str.Append("}\r\n");
            str.Append("return al;\r\n");
            str.Append("}\r\n");

            str.Append(" public static PageGrid FromPages(" + enu.ClassName + "Request request)\r\n");
            str.Append(" {\r\n");
            str.Append("    var where = GetWhere(request);\r\n");
            str.Append("     var orderby = request.sortname;\r\n");
            str.Append("   if (string.IsNullOrWhiteSpace(request.sortname) == false)\r\n");
            str.Append("   {\r\n");
            str.Append("       orderby = orderby.ToLower().Replace(\"g_\", \"\");\r\n");
            str.Append("    }\r\n");

            str.Append("     return new PageGrid(DataBaseEnum.Community, \"tbl" + enu.ClassName + "\",\"*\", where, orderby, request.sortorder, request.page, request.pagesize);\r\n");
            str.Append("  }\r\n");

            str.Append("    public static string GetWhere(" + enu.ClassName + "Request request)\r\n");
            str.Append("   {\r\n");
            str.Append("       var where = \" 1 =1 \";\r\n");
            foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
            {
                if (field.FieldType == "nvarchar" || field.FieldType == "datetime")
                {
                    str.Append("      if (!string.IsNullOrWhiteSpace(request." + field.FieldName + "))\r\n");
                    str.Append("     {\r\n");
                    str.Append("         where = where + \" and " + field.FieldName + " like '%\" + request." + field.FieldName + " + \"%'\";\r\n");
                    str.Append("     }\r\n");
                }
                else if (field.FieldType == "float" || field.FieldType == "int" || field.FieldType == "bit")
                {
                    str.Append("    if (request." + field.FieldName + ">0)\r\n");
                    str.Append("     {\r\n");
                    str.Append("         where = where + \" and " + field.FieldName + " = \" + request." + field.FieldName + " ;\r\n");
                    str.Append("     }\r\n");
                }
            }
            str.Append("     return where;\r\n");
            str.Append("   }\r\n");

            str.Append(" public static List<" + enu.ClassName + "Model> FromReports(" + enu.ClassName + "Request request)\r\n");
            str.Append("{\r\n");
            str.Append("var where = GetWhere(request);\r\n");
            str.Append("if (where != \"\")\r\n");
            str.Append("{\r\n");
            str.Append("where = \" where \" + where;\r\n");
            str.Append("}\r\n");
            str.Append(string.Format("var sql = \"select * from tbl{0} with(nolock)  \" + where + \" order by CreateDate asc\";\r\n", enu.ClassName));
            str.Append(string.Format("var al = new List<{0}Model>();\r\n", enu.ClassName));
            str.Append("using (SqlConnection myConnection = new SqlConnection(WebConfiguration.CommunityConnectString))\r\n");
            str.Append("{\r\n");
            str.Append("var myCommand = new SqlCommand(sql, myConnection);\r\n");
            str.Append("myConnection.Open();\r\n");
            str.Append("var reader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);\r\n");
            str.Append("while (reader.Read())\r\n");
            str.Append("{\r\n");
            str.Append(" var item = " + enu.ClassName + ".PopulateObjectFromIDataReader(reader);\r\n");
            str.Append("al.Add(" + enu.ClassName + "Model.getInstance(item));\r\n");
            str.Append("}\r\n");
            str.Append("myConnection.Close();\r\n");
            str.Append("}\r\n");
            str.Append("return al;\r\n");
            str.Append("}\r\n");
            str.Append("}\r\n");
            str.Append("}\r\n");
            return str.ToString();
        }
        */

        
        //private static string CreateJS(CodeClass enu)
        //{
        //    StringBuilder str = new StringBuilder();

        //    str.Append(" var gird;\r\n");
        //    str.Append("/****************搜索**************/\r\n");
        //    str.Append("//绑定事件\r\n");
        //    str.Append("$(function() {\r\n");
        //    str.Append("    $(\"#btnfind\").on(\"click\", function() { return loadGrid(); });\r\n");
        //    str.Append("    $(\"#btnreset\").on(\"click\", function() { return ResetTxt(); });\r\n");
        //    str.Append("       });\r\n");
        //    str.Append("       //重置\r\n");
        //    str.Append("        function ResetTxt()\r\n");
        //    str.Append("        {\r\n");
        //    str.Append("    $(\"#queryFrom\")[0].reset();\r\n");
        //    str.Append("           loadGrid();\r\n");
        //    str.Append("       }\r\n");
        //    str.Append("/****************网格**************/\r\n");
        //    str.Append("$(document).ready(function() {\r\n");
        //    str.Append("          loadGrid();\r\n");
        //    str.Append("      });\r\n");
        //    str.Append("      //加载表格\r\n");
        //    str.Append("      function loadGrid()\r\n");
        //    str.Append("      {\r\n");
        //    str.Append("        if ($(\"#CommunityId\").val() == \"0\") {\r\n");
        //    str.Append("     $(\".gridResult\").hide();\r\n");
        //    str.Append("     $(\".gridEmpty\").show();\r\n");
        //    str.Append("             return;\r\n");
        //    str.Append("         }\r\n");
        //    str.Append("   else\r\n");
        //    str.Append("           {\r\n");
        //    str.Append("       $(\".gridResult\").show();\r\n");
        //    str.Append("      $(\".gridEmpty\").hide();\r\n");
        //    str.Append("     }\r\n");

        //    str.Append("          var url = \"/Manager/v" + enu.ClassName + "_Grid\";\r\n");
        //    str.Append("         var colums = [\r\n");
        //    str.Append("             { checkbox: true },\r\n");
        //    str.Append("     {\r\n");
        //    string main = "";
        //    foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
        //    {
        //        if (field.IsMainKey)
        //        {
        //            main = field.FieldName;
        //            break;
        //        }
        //    }

        //    str.Append("         field: '" + main + "', title: \"操作\", class: \"text-center\", width: 150, formatter: function (value, row, index) {\r\n");
        //    str.Append("             var actions = [];\r\n");
        //    str.Append("  actions.push('<a href=\"javascript:void(0);\"  data-toggle=\"modal\" data-target=\"#dialog\" onclick=\"return ShowEdit(' + row." + main + " + '); \">修改</a>');\r\n");
        //    str.Append("            return actions.join('');\r\n");
        //    str.Append("         }\r\n");
        //    str.Append("      },\r\n");

        //    foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
        //    {
        //        str.Append(" { field: '" + field.FieldName + "', title: '" + field.Remark + "', sortable: true },\r\n");
        //    }

        //    str.Append(" ];\r\n");
        //    str.Append("  CreateTable(url, colums, \"ProvinceName\", \"asc\");\r\n");
        //    str.Append("}\r\n");
        //    str.Append("/*******************新增********************/\r\n");
        //    str.Append("var openDialog;\r\n");
        //    str.Append("//绑定事件\r\n");
        //    str.Append("$(function () {\r\n");
        //    str.Append("   $(\"#btnadd\").on(\"click\", function () { return ShowAdd(); });\r\n");
        //    str.Append("   $(\"#btndel\").on(\"click\", function () { return ShowDelete(); });\r\n");
        //    str.Append("});\r\n");
        //    str.Append("//显示新增\r\n");
        //    str.Append("function ShowAdd()\r\n");
        //    str.Append("{\r\n");
        //    str.Append("    if ($(\"#CommunityId\").val() == -1) {\r\n");
        //    str.Append("    jqueryAlert({\r\n");
        //    str.Append("       'content': '请选择社区',\r\n");
        //    str.Append("    'closeTime': 2000,\r\n");
        //    str.Append(" });\r\n");
        //    str.Append("       return;\r\n");
        //    str.Append("    }\r\n");
        //    str.Append("  var dt = new Date();\r\n");
        //    str.Append(" var url = \"/Manager/v" + enu.ClassName + "_Add/\" + $(\"#CommunityId\").val() + \"?dt=\" + dt.getTime();\r\n");
        //    str.Append(" showDialog(url);\r\n");
        //    str.Append(" return true;\r\n");

        //    str.Append("}\r\n");
        //    str.Append("//验证新增\r\n");
        //    str.Append("function SaveAdd()\r\n");
        //    str.Append("{\r\n");
        //    str.Append("   var ret = true;\r\n");
        //    str.Append("   $(\"#dialog\").find(\".err\").text(\"\");\r\n");
        //    foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
        //    {
        //        str.Append("   if ($(\"#txt" + field.FieldName + "\").val() == \"\") {\r\n");
        //        str.Append("       $(\"#err_" + field.FieldName + "\").text(\"* " + field.Remark + "不能为空!\");\r\n");
        //        str.Append("       ret = false;\r\n");
        //        str.Append("   }\r\n");
        //    }
        //    str.Append("    if (ret)\r\n");
        //    str.Append("   {\r\n");
        //    str.Append("       SaveAddToServer();\r\n");
        //    str.Append("   }\r\n");
        //    str.Append("   return ret;\r\n");
        //    str.Append("}\r\n");
        //    str.Append("//保存结果到服务器\r\n");
        //    str.Append("function SaveAddToServer(e)\r\n");
        //    str.Append("{\r\n");
        //    str.Append("   //1.拼装数据，Ajax文件上传是用FormData模式\r\n");
        //    str.Append("   var formData = new FormData();\r\n");
        //    foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
        //    {
        //        str.Append("   formData.append(\"txt" + field.FieldName + "\", $(\"#txt" + field.FieldName + "\").val());\r\n");
        //    }
        //    str.Append("   $.ajax({\r\n");
        //    str.Append("  type: \"post\",\r\n");
        //    str.Append("      url: '/Manager/v" + enu.ClassName + "_AddSave',\r\n");
        //    str.Append("     data: formData,\r\n");
        //    str.Append("     processData: false,     // 告诉jquery要传输data对象\r\n");
        //    str.Append("      contentType: false,     // 告诉jquery不需要增加请求头对于contentType的设置\r\n");
        //    str.Append("       success: function(data, stutas, xhr) {\r\n");
        //    str.Append("          var result = data.result;\r\n");
        //    str.Append("          if (result)\r\n");
        //    str.Append("          {\r\n");
        //    str.Append("              loadGrid();\r\n");
        //    str.Append("              $(\"#btnClose\").click();\r\n");
        //    str.Append("         }\r\n");
        //    str.Append("          else\r\n");
        //    str.Append("          {\r\n");
        //    str.Append("             $(\"#err_area\").text(data.desc);\r\n");
        //    str.Append("         }\r\n");
        //    str.Append("      },\r\n");
        //    str.Append("       error: function(xhr, textStatus, errorThrown) {\r\n");
        //    str.Append("           jqueryAlert({\r\n");
        //    str.Append("               'content': \"保存数据错误，错误代码：\" + xhr.status + \"，错误描述：\" + xhr.statusText,\r\n");
        //    str.Append("               'closeTime': 2000\r\n");
        //    str.Append("          });\r\n");
        //    str.Append("      }\r\n");
        //    str.Append("   });\r\n");
        //    str.Append("}\r\n");
        //    str.Append("/***********修改***************/\r\n");
        //    str.Append("//显示修改\r\n");
        //    str.Append("function ShowEdit(id)\r\n");
        //    str.Append("{\r\n");

        //    str.Append("    var dt = new Date();\r\n");
        //    str.Append("   var url = \"/Manager/v" + enu.ClassName + "_Edit/\" + id + \"?dt=\" + dt.getTime();\r\n");
        //    str.Append("   return showDialog(url);\r\n");

        //    str.Append("}\r\n");
        //    str.Append("function SaveEdit(id)\r\n");
        //    str.Append("{\r\n");
        //    str.Append("   var ret = true;\r\n");
        //    str.Append("    $(\"#dialog\").find(\".err\").text(\"\");\r\n");
        //    foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
        //    {
        //        str.Append("   if ($(\"#txt" + field.FieldName + "\").val() == \"\") {\r\n");
        //        str.Append("       $(\"#err_" + field.FieldName + "\").text(\"* " + field.Remark + "不能为空!\");\r\n");
        //        str.Append("       ret = false;\r\n");
        //        str.Append("   }\r\n");
        //    }
        //    str.Append("   if (ret)\r\n");
        //    str.Append("   {\r\n");
        //    str.Append("       SaveEditToServer(id);\r\n");
        //    str.Append("   }\r\n");
        //    str.Append("   return ret;\r\n");
        //    str.Append("}\r\n");
        //    str.Append("//保存结果到服务器\r\n");
        //    str.Append("function SaveEditToServer(id)\r\n");
        //    str.Append("{\r\n");
        //    str.Append("   //1.拼装数据，Ajax文件上传是用FormData模式\r\n");
        //    str.Append("   var formData = new FormData();\r\n");
        //    str.Append("   formData.append(\"txtId\", id);\r\n");
        //    foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
        //    {
        //        if (!field.IsMainKey)
        //        {
        //            str.Append("   formData.append(\"txt" + field.FieldName + "\", $(\"#txt" + field.FieldName + "\").val());\r\n");
        //        }
        //    }
        //    str.Append("   $.ajax({\r\n");
        //    str.Append("   type: \"post\",\r\n");
        //    str.Append("      url: '/Manager/v" + enu.ClassName + "_EditSave',\r\n");
        //    str.Append("       data: formData,\r\n");
        //    str.Append("       processData: false,     // 告诉jquery要传输data对象\r\n");
        //    str.Append("      contentType: false,     // 告诉jquery不需要增加请求头对于contentType的设置\r\n");
        //    str.Append("      success: function(data, stutas, xhr) {\r\n");
        //    str.Append("         var result = data.result;\r\n");
        //    str.Append("         if (result)\r\n");
        //    str.Append("         {\r\n");
        //    str.Append("             loadGrid();\r\n");
        //    str.Append("             $(\"#btnClose\").click();\r\n");
        //    str.Append("         }\r\n");
        //    str.Append("         else\r\n");
        //    str.Append("         {\r\n");
        //    str.Append("             $(\"#err_area\").text(data.desc);\r\n");
        //    str.Append("         }\r\n");
        //    str.Append("     },\r\n");
        //    str.Append("     error: function(xhr, textStatus, errorThrown) {\r\n");
        //    str.Append("         jqueryAlert({\r\n");
        //    str.Append("             'content': \"保存数据错误，错误代码：\" + xhr.status + \"，错误描述：\" + xhr.statusText,\r\n");
        //    str.Append("             'closeTime': 2000\r\n");
        //    str.Append("         });\r\n");
        //    str.Append("     }\r\n");
        //    str.Append("   });\r\n");
        //    str.Append("}\r\n");
        //    str.Append("//删除记录\r\n");
        //    str.Append("function ShowDelete()\r\n");
        //    str.Append("{\r\n");
        //    str.Append("   var rows = $('#table').bootstrapTable('getSelections');\r\n");
        //    str.Append("  var str = \"\";\r\n");
        //    str.Append(" if (rows.length < 1)\r\n");
        //    str.Append("  {\r\n");
        //    str.Append("      jqueryAlert({\r\n");
        //    str.Append("          'content': \"请选择您要删除的记录!\",\r\n");
        //    str.Append("           'closeTime': 2000\r\n");
        //    str.Append("       });\r\n");
        //    str.Append("        return false;\r\n");
        //    str.Append("    }\r\n");
        //    str.Append("    else\r\n");
        //    str.Append("   {\r\n");
        //    str.Append("       swal({\r\n");
        //    str.Append("       title: '操作提示',\r\n");
        //    str.Append("              text: '您确定要删除所选择的记录吗？一旦操作将不可恢复!',\r\n");
        //    str.Append("             icon: 'warning',\r\n");
        //    str.Append("             buttons:\r\n");
        //    str.Append("         {\r\n");
        //    str.Append("         cancel:\r\n");
        //    str.Append("             {\r\n");
        //    str.Append("            text: '取消',\r\n");
        //    str.Append("                   value: false,\r\n");
        //    str.Append("                    visible: true,\r\n");
        //    str.Append("                    closeModal: true\r\n");
        //    str.Append("               },\r\n");
        //    str.Append("                confirm:\r\n");
        //    str.Append("           {\r\n");
        //    str.Append("           text: '确定',\r\n");
        //    str.Append("                   value: true,\r\n");
        //    str.Append("                   visible: true,\r\n");
        //    str.Append("                  className: \"bg-danger\",\r\n");
        //    str.Append("                  closeModal: true\r\n");
        //    str.Append("               }\r\n");
        //    str.Append("       }\r\n");
        //    str.Append("    }).then(function(isConfirm) {\r\n");
        //    str.Append("       if (!isConfirm)\r\n");
        //    str.Append("       {\r\n");
        //    str.Append("          return;\r\n");
        //    str.Append("      }\r\n");
        //    str.Append("          $(rows).each(function() {\r\n");
        //    str.Append("          str = str + this." + main + " + \";\";\r\n");
        //    str.Append("       });\r\n");
        //    str.Append("      var dt = new Date();\r\n");
        //    str.Append("      str = \"/Manager/v" + enu.ClassName + "_Delete?k=\" + str + \"&dt=\" + dt.getTime();\r\n");
        //    str.Append("          $.get(str, function(data) {\r\n");
        //    str.Append("         if (data == \"0\")\r\n");
        //    str.Append("        {\r\n");
        //    str.Append("            jqueryAlert({\r\n");
        //    str.Append("                'content': \"操作失败，请联系系统管理员!\",\r\n");
        //    str.Append("                            'closeTime': 2000\r\n");
        //    str.Append("                });\r\n");
        //    str.Append("        }\r\n");
        //    str.Append("       else\r\n");
        //    str.Append("      {\r\n");
        //    str.Append("           loadGrid();\r\n");
        //    str.Append("       }\r\n");
        //    str.Append("   });\r\n");
        //    str.Append("  });\r\n");
        //    str.Append("   return false;\r\n");
        //    str.Append(" }\r\n");
        //    str.Append("}\r\n");

        //    return str.ToString();
        //}

        /*
        private static string CreateAdd(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("  <div class=\"modal fade\" id=\"dialog\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"myModalLabelLarge\" aria-hidden=\"true\" data-backdrop=\"static\">\r\n");
            str.Append(" <div class=\"modal-dialog modal-lg\">\r\n");
            str.Append("  <div class=\"modal-content\">\r\n");
            str.Append("   <div class=\"modal-header\">\r\n");
            str.Append("     <h4 class=\"modal-title\" id=\"dialogTitle\">新增</h4><button class=\"close\" type=\"button\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>\r\n");
            str.Append(" </div>\r\n");
            str.Append("  <div class=\"modal-body\">\r\n");
            str.Append("    <form class=\"form-horizontal\">\r\n");
            foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
            {
                if (!field.IsMainKey)
                {
                    str.Append("    <div class=\"form-group row\">\r\n");
                    str.Append("      <label class=\"col-xl-2 col-form-label\"><span class=\"text-danger\">*</span>" + field.Remark + "：</label>\r\n");
                    str.Append("      <div class=\"col-xl-7\">\r\n");
                    str.Append("            <input id = \"txt" + field.FieldName + "\" name=\"txt" + field.FieldName + "\" class=\"form-control\" type=\"text\" placeholder=\"请输入" + field.Remark + "\">\r\n");
                    str.Append(" </div>\r\n     <div class=\"col-xl-3\">\r\n");
                    str.Append("           <div id = \"err_" + field.FieldName + "\" class=\"text-danger mt-1 err\"></div>\r\n");
                    str.Append("     </div>\r\n");
                    str.Append("   </div>\r\n");
                }
            }
            str.Append("  </form> \r\n");
            str.Append(" </div>\r\n");
            str.Append("  <div class=\"modal-footer\">\r\n");
            str.Append("     <button class=\"btn btn-primary\" type=\"button\" id=\"btnSaveAdd\" onclick=\"return SaveAdd();\">保存</button>\r\n");
            str.Append("     <button class=\"btn btn-secondary\" type=\"button\" data-dismiss=\"modal\" id=\"btnClose\">关闭</button>\r\n");
            str.Append("    </div>\r\n");
            str.Append("   </div>\r\n");
            str.Append("  </div>\r\n");
            str.Append("</div>\r\n");

            return str.ToString();
        }
        */

        /*
        private static string CreateEdit(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();
            str.Append("   @model " + enu.ClassName + "\r\n");
            str.Append("@using  " + enu.g_ProjectId + ";\r\n");
            str.Append("<div class=\"modal fade\" id=\"dialog\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"myModalLabelLarge\" aria-hidden=\"true\" data-backdrop=\"static\">\r\n");
            str.Append(" <div class=\"modal-dialog modal-lg\">\r\n");
            str.Append(" <div class=\"modal-content\">\r\n");
            str.Append("  <div class=\"modal-header\">\r\n");
            str.Append("    <h4 class=\"modal-title\" id=\"dialogTitle\">编辑</h4><button class=\"close\" type=\"button\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>\r\n");
            str.Append(" </div>\r\n");
            str.Append(" <div class=\"modal-body\">\r\n");
            str.Append("  <form class=\"form-horizontal\">\r\n");

            string main = "";
            foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
            {
                if (!field.IsMainKey)
                {
                    str.Append("   <div class=\"form-group row\">\r\n");
                    str.Append("     <label class=\"col-xl-2 col-form-label\"><span class=\"text-danger\">*</span>" + field.Remark + "：</label>\r\n");
                    str.Append("  <div class=\"col-xl-7\">\r\n");
                    str.Append("       <input id = \"txt" + field.FieldName + "\" name=\"txt" + field.FieldName + "\" class=\"form-control\" type=\"text\" placeholder=\"请输入" + field.Remark + "\" value=\"@Model." + field.FieldName + "\">\r\n");
                    str.Append(" </div> \r\n    <div class=\"col-xl-3\">\r\n");
                    str.Append("       <div id = \"err_" + field.FieldName + "\" class=\"text-danger mt-1 err\"></div>\r\n");
                    str.Append("     </div>\r\n");
                    str.Append("     </div>\r\n");
                }
                else
                {
                    main = field.FieldName;
                }
            }

            str.Append("  </form>\r\n");
            str.Append("  </div>\r\n");
            str.Append(" <div class=\"modal-footer\">\r\n");
            str.Append("    <button class=\"btn btn-primary\" type=\"button\" id=\"btnSaveAdd\" onclick=\"return SaveEdit(@Model." + main + "); \">保存</button>\r\n");
            str.Append("      <button class=\"btn btn-secondary\" type=\"button\" data-dismiss=\"modal\" id=\"btnClose\">关闭</button>\r\n");
            str.Append("     </div>\r\n");
            str.Append("   </div>\r\n");
            str.Append("  </div>\r\n");
            str.Append("</div>\r\n");

            return str.ToString();
        }
        */

        /*
        private static string CreatePage(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            str.Append("   @using " + enu.g_ProjectId + ";\r\n");
            str.Append(" @{\r\n");
            str.Append("            Layout = \"~/Views/Shared/_LayoutMember.cshtml\";\r\n");
            str.Append("           ViewBag.Title = \"购物车\";\r\n");
            str.Append("      }\r\n");

            str.Append(" <input type = \"hidden\" id = \"500_3\" value = \"@Users.isRight(\"500_3\").ToString().ToLower()\" />\r\n");


            str.Append(" <section class=\"section-container\">\r\n");
            str.Append("<!-- Page content-->\r\n");
            str.Append("<div class=\"content-wrapper\">\r\n");
            str.Append(" <div class=\"content-heading\">\r\n");
            str.Append("  <div>购物车<div class=\"mt-2\"><small>查看选择的产品列表</small></div></div>\r\n");
            str.Append("  </div>\r\n");
            str.Append("   <div class=\"row\">\r\n");
            str.Append("  <div class=\"col-xl-12\">\r\n");
            str.Append("    <div role = \"tabpanel\" id=\"nav-top\">\r\n");
            str.Append("       <!-- Nav tabs-->\r\n");
            str.Append("     <ul class=\"nav nav-tabs\" role=\"tablist\">\r\n");
            str.Append("       <li class=\"nav-item\" role=\"presentation\">\r\n");
            str.Append("          <a class=\"nav-link\" href=\"/Member/MallList\">产品商城</a>\r\n");
            str.Append("       </li>\r\n");
            str.Append("         @if(Users.isRight(\"200\"))\r\n");
            str.Append("       {\r\n");
            str.Append("             <li class=\"nav-item\" role=\"presentation\"><a class=\"nav-link active\" href=\"javascript:void(0);\">购物车</a></li>\r\n");

            str.Append("       }\r\n");
            str.Append("        </ul><!-- Tab panes-->\r\n");
            str.Append("    </div><!-- END card-->\r\n");
            str.Append("\r\n");
            str.Append("       <div class=\"container-fluid mt10\">\r\n");
            str.Append("    <!-- DATATABLE DEMO 1-->\r\n");
            str.Append("     <div class=\"card\">\r\n");
            str.Append("   <div class=\"card-body\">\r\n");
            str.Append("     <form id = \"queryFrom\" role=\"form\">\r\n");
            str.Append("        <div class=\"form-row align-items-center\">\r\n");

            foreach (CodeField field in CodeFields.GetDatas(enu.AysnId))
            {
                if (!field.IsMainKey)
                {
                    str.Append("          <div class=\"col-auto\">\r\n");
                    str.Append("            <div class=\"form-check mb-2\">" + field.Remark + "：</div>\r\n");
                    str.Append("          </div>\r\n");
                    str.Append("         <div class=\"col-auto\">\r\n");
                    str.Append("           <div class=\"input-group mb-2\">\r\n");
                    str.Append("              <input id = \"" + field.FieldName + "\" name=\"" + field.FieldName + "\" class=\"form-control\" type=\"text\" placeholder=\"支持模糊查询\">\r\n");
                    str.Append("         </div>\r\n");
                    str.Append("       </div>\r\n");
                }
            }

            str.Append("           <div class=\"col-auto\"><button id = \"btnfind\" class=\"btn btn-primary mb-2\" type=\"button\">查找</button></div>\r\n");
            str.Append("            <div class=\"col-auto\"><button id = \"btnreset\" class=\"btn btn-secondary mb-2\" type=\"button\">重置</button></div>\r\n");
            str.Append("         </div>\r\n");
            str.Append("      </form>\r\n");
            str.Append("     </div>\r\n");
            str.Append("   </div>\r\n");
            str.Append("   <div class=\"card\">\r\n");
            str.Append("   <div class=\"card-header\">\r\n");
            str.Append("     <div class=\"float-left\"><h3><small>查询结果</small></h3></div>\r\n");
            str.Append("    <div class=\"float-right\">\r\n");
            str.Append("        <button id = \"btnadd\" class=\"btn btn-primary mb-2\" type=\"button\" data-toggle=\"modal\" data-target=\"#dialog\">新增</button>\r\n");
            str.Append("         <button id = \"btndel\" class=\"btn btn-danger mb-2\" type=\"button\">删除</button>\r\n");
            str.Append("     </div>\r\n");
            str.Append("   </div>\r\n");
            str.Append("     <div class=\"card-body\" style=\"padding:0px;\">\r\n");
            str.Append("         <table id = \"table\" class=\"table table-striped table-bordered table-hover\"></table>\r\n");
            str.Append("      </div>\r\n");
            str.Append("     </div>\r\n");
            str.Append(" </div>\r\n");
            str.Append("   </div>\r\n");
            str.Append("  </div><!-- END row-->\r\n");
            str.Append("</div>\r\n");
            str.Append("</section>\r\n");

            str.Append("@section scripts\r\n");
            str.Append(" {\r\n");
            str.Append("    @Html.Script(\"/Scripts/Manager/v" + enu.ClassName + ".js\")\r\n");
            str.Append("}\r\n");

            str.Append("\r\n\r\n\r\n");

            str.Append("#region 类目管理\r\n");
            str.Append(" public ActionResult v" + enu.ClassName + "()\r\n");
            str.Append(" {\r\n");
            str.Append("      return View();\r\n");
            str.Append("    }\r\n");

            str.Append("  ///<summary>\r\n");
            str.Append(" /// 表格\r\n");
            str.Append(" ///</summary>\r\n");
            str.Append("  /// <returns></returns>\r\n");
            str.Append("   public JsonResult v" + enu.ClassName + "_Grid(" + enu.ClassName + "Request request)\r\n");
            str.Append("   {\r\n");
            str.Append("       var list = " + enu.ClassName + "Grid.FromPages(request);\r\n");
            str.Append("     var al =" + enu.ClassName + "Grid.ToList(list.PageTable);\r\n");
            str.Append("      var griddata = new { rows = al, total = list.RecordCount };\r\n");
            str.Append("      return Json(griddata);\r\n");
            str.Append("   }\r\n");

            str.Append("    ///<summary>\r\n");
            str.Append("     /// 添加\r\n");
            str.Append("    ///</summary>\r\n");
            str.Append("    /// <returns></returns>\r\n");
            str.Append("    public ActionResult v" + enu.ClassName + "_Add()\r\n");
            str.Append("     {\r\n");
            str.Append("         return PartialView();\r\n");
            str.Append("     }\r\n");

            str.Append("    [HttpPost]\r\n");
            str.Append("    public JsonResult v" + enu.ClassName + "_AddSave()\r\n");
            str.Append("    {\r\n");
            str.Append("        try\r\n");
            str.Append("       {\r\n");
            str.Append("       var entity = new " + enu.ClassName + "();\r\n");

            List<CodeField> al = CodeFields.GetDatas(enu.AysnId);
            foreach (CodeField field in al)
            {
                if (Formatter.GetTypestr(field.FieldType) == "string")
                {
                    str.Append("   entity." + field.FieldName + " = Formatter.ToString(Request.Form[\"txt" + field.FieldName + "\"]);\r\n");
                }
                else if (Formatter.GetTypestr(field.FieldType) == "float")
                {
                    str.Append("     entity." + field.FieldName + " = Formatter.ToSingle(Request.Form[\"txt" + field.FieldName + "\"]);\r\n");
                }
                else if (Formatter.GetTypestr(field.FieldType) == "DateTime")
                {
                    str.Append("     entity." + field.FieldName + " = Formatter.ToDateTime(Request.Form[\"txt" + field.FieldName + "\"]);\r\n");
                }
                else if (Formatter.GetTypestr(field.FieldType) == "bool")
                {
                    str.Append("     entity." + field.FieldName + " =Formatter.ToBoolean(Formatter.ToInt(Request.Form[\"txt" + field.FieldName + "\"]));\r\n");
                }
                else
                {
                    str.Append("     entity." + field.FieldName + " = Formatter.ToInt(Request.Form[\"txt" + field.FieldName + "\"]);\r\n");
                }
            }

            str.Append("       " + enu.ClassName + "s.Create(entity);\r\n");
            str.Append("      var suc = new { result = true, desc = \"数据保存成功\" };\r\n");
            str.Append("         return Json(suc);\r\n");
            str.Append("       }\r\n");
            str.Append("       catch (Exception ex)\r\n");
            str.Append("       {\r\n");
            str.Append("         var result = new { result = false, desc = ex.Message };\r\n");
            str.Append("         return Json(result);\r\n");
            str.Append("      }\r\n");
            str.Append("     }\r\n");

            str.Append("     ///<summary>\r\n");
            str.Append("      /// 修改\r\n");
            str.Append("      ///</summary>\r\n");
            str.Append("       /// <returns></returns>\r\n");
            str.Append("       public ActionResult  v" + enu.ClassName + "_Edit(int id)\r\n");
            str.Append("      {\r\n");
            str.Append("          var obj =  " + enu.ClassName + "s.GetDataByID(id);\r\n");
            str.Append("         return PartialView(obj);\r\n");
            str.Append("    }\r\n");

            str.Append("    [HttpPost]\r\n");
            str.Append("    public JsonResult v" + enu.ClassName + "_EditSave()\r\n");
            str.Append("    {\r\n");
            str.Append("       try\r\n");
            str.Append("     {\r\n");
            str.Append("       var  entity = " + enu.ClassName + "s.GetDataByID(Formatter.ToInt(Request.Form[\"txtId\"]));\r\n");
            foreach (CodeField field in al)
            {
                if (Formatter.GetTypestr(field.FieldType) == "string")
                {
                    str.Append("   entity." + field.FieldName + " = Formatter.ToString(Request.Form[\"txt" + field.FieldName + "\"]);\r\n");
                }
                else if (Formatter.GetTypestr(field.FieldType) == "float")
                {
                    str.Append("     entity." + field.FieldName + " = Formatter.ToSingle(Request.Form[\"txt" + field.FieldName + "\"]);\r\n");
                }
                else if (Formatter.GetTypestr(field.FieldType) == "DateTime")
                {
                    str.Append("     entity." + field.FieldName + " = Formatter.ToDateTime(Request.Form[\"txt" + field.FieldName + "\"]);\r\n");
                }
                else if (Formatter.GetTypestr(field.FieldType) == "bool")
                {
                    str.Append("     entity." + field.FieldName + " =Formatter.ToBoolean(Formatter.ToInt(Request.Form[\"txt" + field.FieldName + "\"]));\r\n");
                }
                else
                {
                    str.Append("     entity." + field.FieldName + " = Formatter.ToInt(Request.Form[\"txt" + field.FieldName + "\"]);\r\n");
                }
            }
            str.Append("       " + enu.ClassName + "s.Update(entity);\r\n");

            str.Append("     var suc = new  {    result = true,         desc = \"数据保存成功\" };\r\n");
            str.Append("      return Json(suc);\r\n");
            str.Append("     }\r\n");
            str.Append("     catch (Exception ex)\r\n");
            str.Append("      {\r\n");
            str.Append("          var result = new { result = false, desc = ex.Message };\r\n");
            str.Append("          return Json(result);\r\n");
            str.Append("      }\r\n");
            str.Append("    }\r\n");

            str.Append("       ///<summary>\r\n");
            str.Append("     /// 删除\r\n");
            str.Append("    ///</summary>\r\n");
            str.Append("   /// <param name=\"id\"></param>\r\n");
            str.Append("   /// <returns></returns>\r\n");
            str.Append("    public string v" + enu.ClassName + "_Delete()\r\n");
            str.Append("    {\r\n");
            str.Append("       var str = Request.QueryString[\"k\"];\r\n");
            str.Append("     var ret = \"0\";\r\n");
            str.Append("      try\r\n");
            str.Append("     {\r\n");
            str.Append("     var arr = str.Split(';');\r\n");
            str.Append("        foreach (string rid in arr)\r\n");
            str.Append("       {\r\n");
            str.Append("         if (!string.IsNullOrWhiteSpace(rid))\r\n");
            str.Append("         {\r\n");
            str.Append("           var obj =" + enu.ClassName + "s.GetDataByID(Formatter.ToInt(rid));\r\n");
            str.Append("             if (obj != null && obj.AysnId != 0)\r\n");
            str.Append("            {\r\n");
            str.Append("               " + enu.ClassName + "s.Delete(obj);\r\n");
            str.Append("            }\r\n");
            str.Append("         }\r\n");
            str.Append("      }\r\n");
            str.Append("       ret = \"1\";\r\n");
            str.Append("     }\r\n");
            str.Append("     catch\r\n");
            str.Append("     {\r\n");
            str.Append("    }\r\n");
            str.Append("       return ret;\r\n");
            str.Append("      }\r\n");
            str.Append("  #endregion\r\n");
            return str.ToString();
        }
        */
        /// <summary>
        /// 建立数据库语句, 在自动生成代码时执行，每次执行会删除之前的数据表
        /// </summary>
        private static string CreateGenerateSql(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();

            #region 初始化,删除表格和存储过程, 
            //1. 禁用外键约束（确保删除表时无依赖冲突）
            str.Append("PRAGMA foreign_keys = OFF;\r\n");
            //2.开启事务（保证原子性）
            str.Append("BEGIN TRANSACTION;\r\n");
            //3.删除指定表
            str.Append("DROP TABLE IF EXISTS \"" + enu.TableName + "\";\r\n");
            //4. 提交事务
            str.Append("COMMIT;\r\n");
            //5.重新启用外键约束
            str.Append("PRAGMA foreign_keys = ON;\r\n");
            str.Append("\r\n");

            #endregion
            #region 建立表格
            str.Append("CREATE TABLE \"" + enu.TableName + "\" (\r\n");
            string mainkey = "";
            int index = 0;
            foreach (CodeField field in enu.CodeFields)
            {
                string type = field.FieldLength > 0 ? $"{field.FieldType}({field.FieldLength})" : $"{field.FieldType}" ;
                string sql = $"    \"{field.FieldName}\" {type}";
                if (field.IsMainKey)
                {
                    sql = sql + " PRIMARY KEY";
                    if (field.IsAutoIncrement) {
                        sql = sql + " AUTOINCREMENT";
                    }
                    //mainkey = field.FieldName;
                }

                if (!field.IsMainKey)
                {
                    if (field.IsAllowNull)
                    {
                        //sql = sql + " NULL";//不需要显示
                    }
                    else
                    {
                        sql = sql + " NOT NULL";
                    }
                }

                if (field.IsUnique)
                {
                    sql = sql + " UNIQUE";
                }

                if (!string.IsNullOrWhiteSpace(field.DefaultValue))
                {
                    sql = sql + " DEFAULT " + field.DefaultValue;
                }

                if (!sql.EndsWith(",") && index != enu.CodeFields.Count - 1)
                {
                    sql = sql + ",";
                }
                index++;
                str.AppendLine(sql);
            }
            #endregion
            str.Append(");\r\n");
            return str.ToString();
        }

        /// <summary>
        /// 建立数据库语句，在 AppDbContext 初始化时调用，如果数据库已存在就不创建，不会删除之前的数据表
        /// </summary>
        private static string CreateInitSql(CodeClass enu)
        {
            StringBuilder str = new StringBuilder();
 
            //1. 禁用外键约束（确保删除表时无依赖冲突）
            str.Append("PRAGMA foreign_keys = OFF;\r\n");
            //2.开启事务（保证原子性）
            str.Append("BEGIN TRANSACTION;\r\n");
            str.Append("\r\n");

            //3.如果不存在就创建指定表
            #region 建立表格
            str.Append("CREATE TABLE IF NOT EXISTS \"" + enu.TableName + "\" (\r\n");
            string mainkey = "";
            int index = 0;
            foreach (CodeField field in enu.CodeFields)
            {
                string type = field.FieldLength > 0 ? $"{field.FieldType}({field.FieldLength})" : $"{field.FieldType}";
                string sql = $"    \"{field.FieldName}\" {type}";
                if (field.IsMainKey)
                {
                    sql = sql + " PRIMARY KEY";
                    if (field.IsAutoIncrement)
                    {
                        sql = sql + " AUTOINCREMENT";
                    }
                    //mainkey = field.FieldName;
                }

                if (!field.IsMainKey)
                {
                    if (field.IsAllowNull)
                    {
                        //sql = sql + " NULL";//不需要显示
                    }
                    else
                    {
                        sql = sql + " NOT NULL";
                    }
                }

                if (field.IsUnique)
                {
                    sql = sql + " UNIQUE";
                }

                if (!string.IsNullOrWhiteSpace(field.DefaultValue))
                {
                    sql = sql + " DEFAULT " + field.DefaultValue;
                }

                if (!sql.EndsWith(",") && index != enu.CodeFields.Count - 1)
                {
                    sql = sql + ",";
                }
                index++;
                str.AppendLine(sql);
            }
            #endregion
            str.Append(");\r\n");
            str.Append("\r\n");

            //4. 提交事务
            str.Append("COMMIT;\r\n");
            //5.重新启用外键约束
            str.Append("PRAGMA foreign_keys = ON;\r\n");
            str.Append("\r\n");

            return str.ToString();
        }
        #endregion
    }


}
