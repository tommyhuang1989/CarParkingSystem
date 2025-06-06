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
    public partial class ParkArea : BaseEntity
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        private System.String areaName;

        [Column]
        public System.String AreaName
        {
            get { return areaName; }
            set {
                if (areaName != value)
                {
                    areaName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 显示区域剩余车位
        /// </summary>
        private System.Int32 showAreaLot;

        [Column]
        public System.Int32 ShowAreaLot
        {
            get { return showAreaLot; }
            set {
                if (showAreaLot != value)
                {
                    showAreaLot = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 临时车满位允许进入
        /// </summary>
        private System.Int32 tempCarFullCanIn;

        [Column]
        public System.Int32 TempCarFullCanIn
        {
            get { return tempCarFullCanIn; }
            set {
                if (tempCarFullCanIn != value)
                {
                    tempCarFullCanIn = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 统计车位
        /// </summary>
        private System.Int32 totalCars;

        [Column]
        public System.Int32 TotalCars
        {
            get { return totalCars; }
            set {
                if (totalCars != value)
                {
                    totalCars = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        private System.Int32 upateUser;

        [Column]
        public System.Int32 UpateUser
        {
            get { return upateUser; }
            set {
                if (upateUser != value)
                {
                    upateUser = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 更新日期
        /// </summary>
        private System.String updateDate;

        [Column]
        public System.String UpdateDate
        {
            get { return updateDate; }
            set {
                if (updateDate != value)
                {
                    updateDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 使用车位
        /// </summary>
        private System.Int32 usedCars;

        [Column]
        public System.Int32 UsedCars
        {
            get { return usedCars; }
            set {
                if (usedCars != value)
                {
                    usedCars = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedParkAreaMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKAREA_TOKEN);
        }
        #endregion 
    }
}

