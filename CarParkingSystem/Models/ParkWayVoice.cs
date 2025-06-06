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
    public partial class ParkWayVoice : BaseEntity
    {
        /// <summary>
        /// 结束小时
        /// </summary>
        private System.Int32 endHour;

        [Column]
        public System.Int32 EndHour
        {
            get { return endHour; }
            set {
                if (endHour != value)
                {
                    endHour = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 结束分钟
        /// </summary>
        private System.Int32 endMinute;

        [Column]
        public System.Int32 EndMinute
        {
            get { return endMinute; }
            set {
                if (endMinute != value)
                {
                    endMinute = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 最后同步时间
        /// </summary>
        private System.String lastUpdateDate;

        [Column]
        public System.String LastUpdateDate
        {
            get { return lastUpdateDate; }
            set {
                if (lastUpdateDate != value)
                {
                    lastUpdateDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 开始小时
        /// </summary>
        private System.Int32 startHour;

        [Column]
        public System.Int32 StartHour
        {
            get { return startHour; }
            set {
                if (startHour != value)
                {
                    startHour = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 开始分钟
        /// </summary>
        private System.Int32 startMinute;

        [Column]
        public System.Int32 StartMinute
        {
            get { return startMinute; }
            set {
                if (startMinute != value)
                {
                    startMinute = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 音量
        /// </summary>
        private System.Int32 volume;

        [Column]
        public System.Int32 Volume
        {
            get { return volume; }
            set {
                if (volume != value)
                {
                    volume = value;
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

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedParkWayVoiceMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKWAYVOICE_TOKEN);
        }
        #endregion 
    }
}

