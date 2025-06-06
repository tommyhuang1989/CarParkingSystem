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
    public partial class ParkingAbnormal : BaseEntity
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
        /// 车牌号码
        /// </summary>
        private System.String carNo;

        [Column]
        public System.String CarNo
        {
            get { return carNo; }
            set {
                if (carNo != value)
                {
                    carNo = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车牌类型
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
        /// 是否修改
        /// </summary>
        private System.Int32 inCpChanged;

        [Column]
        public System.Int32 InCpChanged
        {
            get { return inCpChanged; }
            set {
                if (inCpChanged != value)
                {
                    inCpChanged = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 进入图片
        /// </summary>
        private System.String inImg;

        [Column]
        public System.String InImg
        {
            get { return inImg; }
            set {
                if (inImg != value)
                {
                    inImg = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 进入时间
        /// </summary>
        private System.String inTime;

        [Column]
        public System.String InTime
        {
            get { return inTime; }
            set {
                if (inTime != value)
                {
                    inTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 进入方式
        /// </summary>
        private System.Int32 inType;

        [Column]
        public System.Int32 InType
        {
            get { return inType; }
            set {
                if (inType != value)
                {
                    inType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 进入车道
        /// </summary>
        private System.Int32 inWayId;

        [Column]
        public System.Int32 InWayId
        {
            get { return inWayId; }
            set {
                if (inWayId != value)
                {
                    inWayId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 停车编号
        /// </summary>
        private System.String orderId;

        [Column]
        public System.String OrderId
        {
            get { return orderId; }
            set {
                if (orderId != value)
                {
                    orderId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 记录状态
        /// </summary>
        private System.Int32 recStatus;

        [Column]
        public System.Int32 RecStatus
        {
            get { return recStatus; }
            set {
                if (recStatus != value)
                {
                    recStatus = value;
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
        /// 更新日期
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

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedParkingAbnormalMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGABNORMAL_TOKEN);
        }
        #endregion 
    }
}

