using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Services
{
    public interface ILogService
    {
        void LogFatal(string title, string message);
        void LogError(string title, string message);
        void LogWarring(string title, string message);
        void LogInfo(string title, string message);
        void LogDebug(string title, string message);
    }
}
