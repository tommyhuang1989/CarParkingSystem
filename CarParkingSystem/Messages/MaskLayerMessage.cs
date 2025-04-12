using Avalonia.Controls.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Messages
{
    /// <summary>
    /// 控制主窗口遮罩层隐藏/显示时，发送的消息类型
    /// </summary>
    public record MaskLayerMessage
	{
		private bool isNeedShow = false;

		public bool IsNeedShow
		{
			get { return isNeedShow; }
			set { isNeedShow = value; }
		}

        public string Message { get; set; }
    }
}
