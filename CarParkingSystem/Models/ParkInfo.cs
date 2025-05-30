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
    public partial class ParkInfo : BaseEntity
    {
        /// <summary>
        /// 支付方式
        /// </summary>
        private System.Int32 payType;

        [Column]
        public System.Int32 PayType
        {
            get { return payType; }
            set {
                if (payType != value)
                {
                    payType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        private System.Int32 updateUser;

        [Column]
        public System.Int32 UpdateUser
        {
            get { return updateUser; }
            set {
                if (updateUser != value)
                {
                    updateUser = value;
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
        /// 支付有效时间（单位分钟）
        /// </summary>
        private System.Int32 payTime;

        [Column]
        public System.Int32 PayTime
        {
            get { return payTime; }
            set {
                if (payTime != value)
                {
                    payTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 商户号
        /// </summary>
        private System.String merchant;

        [Column]
        public System.String Merchant
        {
            get { return merchant; }
            set {
                if (merchant != value)
                {
                    merchant = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 支付平台编号
        /// </summary>
        private System.String payUuid;

        [Column]
        public System.String PayUuid
        {
            get { return payUuid; }
            set {
                if (payUuid != value)
                {
                    payUuid = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车场平台编号
        /// </summary>
        private System.String parkUuid;

        [Column]
        public System.String ParkUuid
        {
            get { return parkUuid; }
            set {
                if (parkUuid != value)
                {
                    parkUuid = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 剩余车位
        /// </summary>
        private System.Int32 remainingCars;

        [Column]
        public System.Int32 RemainingCars
        {
            get { return remainingCars; }
            set {
                if (remainingCars != value)
                {
                    remainingCars = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 总计车位
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

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedParkInfoMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINFO_TOKEN);
        }
        #endregion 
    }
}

