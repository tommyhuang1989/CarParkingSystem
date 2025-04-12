using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Services
{
    public class LogService : ILogService
    {
        public LogService()
        {
            //Log.Logger = new LoggerConfiguration().WriteTo.File("Logs\\.txt", (Serilog.Events.LogEventLevel)RollingInterval.Day).CreateLogger();
            Log.Logger = new LoggerConfiguration().WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // 配置文件接收器，每天创建一个新文件 (log-yyyyMMdd.txt)
            .CreateLogger();

        }

        void ILogService.LogFatal(string message, string title)
        {
            Serilog.Log.Fatal(message, title);
        }

        void ILogService.LogError(string message, string title)
        {
            Serilog.Log.Error(message, title);
        }
        void ILogService.LogWarring(string message, string title)
        {
            Serilog.Log.Warning(message, title);
        }
        void ILogService.LogInfo(string message, string title)
        {
            Serilog.Log.Information(message, title);
        }
        void ILogService.LogDebug(string message, string title)
        {
            Serilog.Log.Debug(message, title);
        }
    }
}
