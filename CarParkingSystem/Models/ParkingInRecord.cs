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
    public partial class ParkingInRecord : BaseEntity
    {
        /// <summary>
        /// 应付金额
        /// </summary>
        private System.Decimal amountMoney;

        [Column]
        public System.Decimal AmountMoney
        {
            get { return amountMoney; }
            set {
                if (amountMoney != value)
                {
                    amountMoney = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 无感支付
        /// </summary>
        private System.Int32 autoPay;

        [Column]
        public System.Int32 AutoPay
        {
            get { return autoPay; }
            set {
                if (autoPay != value)
                {
                    autoPay = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 无感支付编号
        /// </summary>
        private System.String autoPayId;

        [Column]
        public System.String AutoPayId
        {
            get { return autoPayId; }
            set {
                if (autoPayId != value)
                {
                    autoPayId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 算费时间
        /// </summary>
        private System.String calculateOutTime;

        [Column]
        public System.String CalculateOutTime
        {
            get { return calculateOutTime; }
            set {
                if (calculateOutTime != value)
                {
                    calculateOutTime = value;
                    OnPropertyChanged();
                }
            }
        }

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
        /// 类型转换时间
        /// </summary>
        private System.String cardChangeTime;

        [Column]
        public System.String CardChangeTime
        {
            get { return cardChangeTime; }
            set {
                if (cardChangeTime != value)
                {
                    cardChangeTime = value;
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
        /// 车辆状态
        /// </summary>
        private System.Int32 carStatus;

        [Column]
        public System.Int32 CarStatus
        {
            get { return carStatus; }
            set {
                if (carStatus != value)
                {
                    carStatus = value;
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
        /// 优惠金额
        /// </summary>
        private System.Decimal discountMoney;

        [Column]
        public System.Decimal DiscountMoney
        {
            get { return discountMoney; }
            set {
                if (discountMoney != value)
                {
                    discountMoney = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否修改
        /// </summary>
        private System.Int32 incpChanged;

        [Column]
        public System.Int32 IncpChanged
        {
            get { return incpChanged; }
            set {
                if (incpChanged != value)
                {
                    incpChanged = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 入场图片
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
        /// 收费人员
        /// </summary>
        private System.Int32 inOperatorId;

        [Column]
        public System.Int32 InOperatorId
        {
            get { return inOperatorId; }
            set {
                if (inOperatorId != value)
                {
                    inOperatorId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 入场时间
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
        /// 入场类型
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
        /// 入场车道
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
        /// 月卡转临停时间
        /// </summary>
        private System.Int32 monthToTempNumber;

        [Column]
        public System.Int32 MonthToTempNumber
        {
            get { return monthToTempNumber; }
            set {
                if (monthToTempNumber != value)
                {
                    monthToTempNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 开闸方式
        /// </summary>
        private System.Int32 openType;

        [Column]
        public System.Int32 OpenType
        {
            get { return openType; }
            set {
                if (openType != value)
                {
                    openType = value;
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
        /// 原始类型
        /// </summary>
        private System.Int32 originCardNo;

        [Column]
        public System.Int32 OriginCardNo
        {
            get { return originCardNo; }
            set {
                if (originCardNo != value)
                {
                    originCardNo = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 支付金额
        /// </summary>
        private System.Decimal paidMoney;

        [Column]
        public System.Decimal PaidMoney
        {
            get { return paidMoney; }
            set {
                if (paidMoney != value)
                {
                    paidMoney = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 停车编号
        /// </summary>
        private System.String plateId;

        [Column]
        public System.String PlateId
        {
            get { return plateId; }
            set {
                if (plateId != value)
                {
                    plateId = value;
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
        /// 备注信息
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
            WeakReferenceMessenger.Default.Send<SelectedParkingInRecordMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGINRECORD_TOKEN);
        }
        #endregion 
    }
}

