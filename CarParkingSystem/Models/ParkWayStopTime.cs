using CarParkingSystem.Common;
using CarParkingSystem.Messages;
using CarParkingSystem.Unities;
using CarParkingSystem.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Models
{
    public partial class ParkWayStopTime : BaseEntity
    {
        /// <summary>
        /// 车辆类型
        /// </summary>
        private System.Int32 cardId;

        [Column]
        public System.Int32 CardId
        {
            get { return cardId; }
            set {
                if (cardId != value)
                {
                    cardId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private System.String remark;

        [Column]
        public System.String Remark
        {
            get { return remark; }
            set {
                if (remark != value)
                {
                    remark = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 结束小时
        /// </summary>
        private System.Int32 stopEndHour;

        [Column]
        public System.Int32 StopEndHour
        {
            get { return stopEndHour; }
            set {
                if (stopEndHour != value)
                {
                    stopEndHour = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 结束分钟
        /// </summary>
        private System.Int32 stopEndMinute;

        [Column]
        public System.Int32 StopEndMinute
        {
            get { return stopEndMinute; }
            set {
                if (stopEndMinute != value)
                {
                    stopEndMinute = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 开始小时
        /// </summary>
        private System.Int32 stopStartHour;

        [Column]
        public System.Int32 StopStartHour
        {
            get { return stopStartHour; }
            set {
                if (stopStartHour != value)
                {
                    stopStartHour = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 开始分钟
        /// </summary>
        private System.Int32 stopStartMinute;

        [Column]
        public System.Int32 StopStartMinute
        {
            get { return stopStartMinute; }
            set {
                if (stopStartMinute != value)
                {
                    stopStartMinute = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车道编号
        /// </summary>
        private System.Int32 wayId;

        [Column]
        public System.Int32 WayId
        {
            get { return wayId; }
            set {
                if (wayId != value)
                {
                    wayId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 星期几
        /// </summary>
        private System.String weeks;

        [Column]
        public System.String Weeks
        {
            get { return weeks; }
            set {
                if (weeks != value)
                {
                    weeks = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedParkWayStopTimeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKWAYSTOPTIME_TOKEN);
        }
        #endregion 
    }
}

