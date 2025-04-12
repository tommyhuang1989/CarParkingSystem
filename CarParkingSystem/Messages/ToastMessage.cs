using Avalonia.Controls.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Messages
{
    public record ToastMessage
    {
        public Type CurrentModelType { get; set; }//20250402, add, 都会发送这个消息，所以使用类型区分，避免不相关的 viewModel 也刷新数据，消耗资源
        public string Title { get; set; }
        public string Content { get; set; }
        public NotificationType NotifyType { get; set; }

        private bool needRefreshData = false;

        public bool NeedRefreshData
        {
            get { return needRefreshData; }
            set { needRefreshData = value; }
        }

        //20250402, add, 默认都会显示 Toast，如果已经显示了 Dialog，就不再显示 Toast
        private bool needShowToast = true;

        public bool NeedShowToast
        {
            get { return needShowToast; }
            set { needShowToast = value; }
        }

    }
}
