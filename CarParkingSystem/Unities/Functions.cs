using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CarParkingSystem.Unities
{
    public class Functions
    {
        public static string GetExceptionMessage(Exception e)
        {
            StringBuilder error_msg = new StringBuilder();
            error_msg.Append(e.Message);
            if (e.InnerException != null)
            {
                error_msg.AppendLine();
                error_msg.Append(e.InnerException.Message);
            }
            return error_msg.ToString();
        }

        public static void ReadFileWithStringBuilder(string subDir, string fileName)
        {
            string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            //string basePath = "C:\\Users\\tommy\\Desktop\\Working\\20250324\\CarParkingSystem-20250322-com-code-gne\\CarParkingSystem\\CarParkingSystem\\";//Home
            //string basePath = "E:\\Codes\\vs_projects\\CarParkingSystem-20250325-code-generator-almost\\CarParkingSystem\\CarParkingSystem\\";//Company
            //basePath = System.Environment.CurrentDirectory; 
            //string path = Path.Combine(AppContext.BaseDirectory, subDir, fileName);
            string path = Path.Combine(basePath, subDir, fileName);
            string resultPaht = Path.Combine(AppContext.BaseDirectory, fileName+".txt");
            StringBuilder str = new StringBuilder();
            try
            {
                using (var input = new StreamReader(path, Encoding.UTF8))
                using (var output = new StreamWriter(resultPaht, true, Encoding.UTF8))
                {
                    string line;
                    int lineNumber = 0;
                    while ((line = input.ReadLine()) != null)
                    {
                        lineNumber++;
                        try
                        {
                            //output.WriteLine(ValidateLine(line)); // 可能抛出业务异常
                            output.WriteLine("str.Append(\"" + line + "\\r\\n\");"); // 可能抛出业务异常
                        }
                        catch (InvalidDataException ex)
                        {
                            Log.Error($"第{lineNumber}行数据异常: {ex.Message}");
                            output.WriteLine("[ERROR] " + line); // 错误标记
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Fatal($"文件操作失败: {ex.Message}");
            }

        }

        public static string FirstCharToLower(ReadOnlySpan<char> input)
        {
            if (input.IsEmpty)
                return string.Empty;

            Span<char> buffer = stackalloc char[input.Length];
            input.CopyTo(buffer);
            buffer[0] = char.ToLowerInvariant(buffer[0]);
            return new string(buffer);
        }

        /// <summary>
        /// 切割单词，将 HelloWorld 改成 Hello World
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SplitWords(string input)
        {
            // 使用正则表达式在非开头的大写字母前添加空格
            return Regex.Replace(input, @"(?<!^)(?=[A-Z])", " ");
        }

        /// <summary>
        /// 将 HelloWorld 改成 hello_world
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertToSnakeCaseRegex(string input)
        {
            return Regex.Replace(input, "(?<!^)([A-Z])", "_$1").ToLower();
        }

        /// <summary>
        /// 将 HelloWorld 改成 hello_world (下划线)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var builder = new StringBuilder();
            bool previousUpper = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsUpper(c))
                {
                    // 首字母不添加下划线，其他大写字母前添加下划线（仅当前一个字符非大写时）
                    if (i > 0 && !previousUpper)
                    {
                        builder.Append('_');
                    }
                    builder.Append(char.ToLowerInvariant(c));
                    previousUpper = true;
                }
                else
                {
                    builder.Append(c);
                    previousUpper = false;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 将 hello_world (下划线) 改成 HelloWorld
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var builder = new StringBuilder();
            bool nextUpper = true; // 标记下一个字符是否需要大写（首字母强制大写）

            foreach (char c in input)
            {
                if (c == '_')
                {
                    nextUpper = true; // 遇到下划线后，下一个字符大写
                }
                else
                {
                    if (nextUpper)
                    {
                        builder.Append(char.ToUpperInvariant(c));
                        nextUpper = false;
                    }
                    else
                    {
                        builder.Append(char.ToLowerInvariant(c));
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 将 hello_world (下划线) 改成 helloWorld
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var builder = new StringBuilder();
            bool nextUpper = false; // 首字母不大写

            foreach (char c in input)
            {
                if (c == '_')
                {
                    nextUpper = true;
                }
                else
                {
                    if (nextUpper)
                    {
                        builder.Append(char.ToUpperInvariant(c));
                        nextUpper = false;
                    }
                    else
                    {
                        builder.Append(builder.Length == 0 ?
                            char.ToLowerInvariant(c) :
                            char.ToLowerInvariant(c));
                    }
                }
            }
            return builder.ToString();
        }
    }
}
