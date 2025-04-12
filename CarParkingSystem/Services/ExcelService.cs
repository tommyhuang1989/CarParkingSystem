using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaExtensions.Axaml.Markup;
using CarParkingSystem.Common;
using ClosedXML.Excel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Services
{
    public static class ExcelService
    {
        public static async Task<string> ExportToExcelAsync<T>(Window window, List<T> data, string defaultFileName, string sheetName)
        {
            string saveFilePath = null;
            var topLevel = TopLevel.GetTopLevel(window); // 在 Avalonia 中获取窗口上下文
            var storageProvider = topLevel?.StorageProvider;

            if (storageProvider != null)
            {
                var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "保存 Excel 文件",
                    SuggestedFileName = defaultFileName,
                    FileTypeChoices = new[] { new FilePickerFileType("Excel 文件") { Patterns = new[] { "*.xlsx" } } }
                });

                if (file != null)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add(sheetName);
                        var properties = typeof(T).GetProperties().Where(p =>
                        {
                            // 不包含标记 NoExportAttribute 的属性，且不是 IRelayCommand 命令
                            var include = Attribute.IsDefined(p, typeof(NoExportAttribute));
                            var notCommand = p.PropertyType.Name.Equals("IRelayCommand");
                            return !include && !notCommand;
                        }).ToList();

                        // 写入表头
                        for (int i = 0; i < properties.Count; i++)
                        {
                            //20250402, add
                            var propertyName = string.IsNullOrWhiteSpace(I18nManager.GetString(properties[i].Name)) ? properties[i].Name : I18nManager.GetString(properties[i].Name);
                            worksheet.Cell(1, i + 1).Value = propertyName;
                        }

                        // 写入数据
                        for (int row = 0; row < data.Count; row++)
                        {
                            for (int col = 0; col < properties.Count; col++)
                            {
                                var value = properties[col].GetValue(data[row]);
                                worksheet.Cell(row + 2, col + 1).Value = value?.ToString() ?? "";
                            }
                        }

                        await using var stream = await file.OpenWriteAsync();
                        workbook.SaveAs(stream);
                        saveFilePath = file.Path.LocalPath;//返回保存的文件路径
                    }
                }
            }

            return saveFilePath;
        }

        public static void HighlightFileInExplorer(string filePath)
        {
            if (filePath != null && File.Exists(filePath))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start("explorer.exe", $"/select,\"{filePath}\""); // Windows 精准定位
                }
                else
                {
                    var directoryPath = Path.GetDirectoryName(filePath);
                    Process.Start("explorer", directoryPath); // 其他系统仅打开文件夹
                }
            }
        }
    }
}
