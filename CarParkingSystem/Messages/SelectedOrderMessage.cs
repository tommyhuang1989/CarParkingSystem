using Avalonia.Controls.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Messages
{
    /// <summary>
    /// 当在表格中勾选时，发送的消息类型
    /// </summary>
    public record SelectedOrderMessage
    {
        public string Title { get; set; }
        public string Content { get; set; }

    }
}

