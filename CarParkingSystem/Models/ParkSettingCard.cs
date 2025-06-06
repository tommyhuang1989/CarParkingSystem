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
    public partial class ParkSettingCard : BaseEntity
    {
        /// <summary>
        /// 车牌颜色
        /// </summary>
        private System.Int32 carColor;

        [Column]
        public System.Int32 CarColor
        {
            get { return carColor; }
            set {
                if (carColor != value)
                {
                    carColor = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车辆类型
        /// </summary>
        private System.Int32 cardNo;

        [Column]
        public System.Int32 CardNo
        {
            get { return cardNo; }
            set {
                if (cardNo != value)
                {
                    cardNo = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车牌种类
        /// </summary>
        private System.Int32 carType;

        [Column]
        public System.Int32 CarType
        {
            get { return carType; }
            set {
                if (carType != value)
                {
                    carType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        private System.String updateDt;

        [Column]
        public System.String UpdateDt
        {
            get { return updateDt; }
            set {
                if (updateDt != value)
                {
                    updateDt = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 更新日期
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

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedParkSettingCardMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKSETTINGCARD_TOKEN);
        }
        #endregion 
    }
}

