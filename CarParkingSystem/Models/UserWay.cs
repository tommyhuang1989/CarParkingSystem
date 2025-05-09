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
    public partial class UserWay : BaseEntity
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        private System.Int32 userId;

        [Column]
        public System.Int32 UserId
        {
            get { return userId; }
            set {
                if (userId != value)
                {
                    userId = value;
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
        /// 排列序号
        /// </summary>
        private System.Int32 orderNo;

        [Column]
        public System.Int32 OrderNo
        {
            get { return orderNo; }
            set {
                if (orderNo != value)
                {
                    orderNo = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedUserWayMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_USERWAY_TOKEN);
        }
        #endregion 
    }
}

