using Avalonia.Controls.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Messages
{
    /// <summary>
    /// 当点击 配置按钮 时，发送的消息类型，将会导航到 Table 对应的 Fields 表格界面
    /// </summary>
    public record ConfigureCodeFieldMessage
    {
        public int Cid { get; set; }

        public string Message { get; set; }
    }
}
