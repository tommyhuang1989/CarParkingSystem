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
    public partial class Calendar : BaseEntity
    {
        /// <summary>
        /// 日历名称
        /// </summary>
        private System.String calendarName;

        [Column]
        public System.String CalendarName
        {
            get { return calendarName; }
            set {
                if (calendarName != value)
                {
                    calendarName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 结束日期
        /// </summary>
        private System.String endDate;

        [Column]
        public System.String EndDate
        {
            get { return endDate; }
            set {
                if (endDate != value)
                {
                    endDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否节假日
        /// </summary>
        private System.Int32 isHoliday;

        [Column]
        public System.Int32 IsHoliday
        {
            get { return isHoliday; }
            set {
                if (isHoliday != value)
                {
                    isHoliday = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 停车时间
        /// </summary>
        private System.String parkingDate;

        [Column]
        public System.String ParkingDate
        {
            get { return parkingDate; }
            set {
                if (parkingDate != value)
                {
                    parkingDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        private System.String startDate;

        [Column]
        public System.String StartDate
        {
            get { return startDate; }
            set {
                if (startDate != value)
                {
                    startDate = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedCalendarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CALENDAR_TOKEN);
        }
        #endregion 
    }
}

