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
    public partial class ParkingArrears : BaseEntity
    {
        /// <summary>
        /// 应付金额
        /// </summary>
        private System.String amountMoney;

        [Column]
        public System.String AmountMoney
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
        /// 出场记录
        /// </summary>
        private System.Int32 carOutId;

        [Column]
        public System.Int32 CarOutId
        {
            get { return carOutId; }
            set {
                if (carOutId != value)
                {
                    carOutId = value;
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
        private System.String discountMoney;

        [Column]
        public System.String DiscountMoney
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
        /// 欠费金额
        /// </summary>
        private System.Decimal fee;

        [Column]
        public System.Decimal Fee
        {
            get { return fee; }
            set {
                if (fee != value)
                {
                    fee = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 入场时间
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
        /// 入场备注
        /// </summary>
        private System.String inRemark;

        [Column]
        public System.String InRemark
        {
            get { return inRemark; }
            set {
                if (inRemark != value)
                {
                    inRemark = value;
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
        /// 停车记录
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
        /// 出场图片
        /// </summary>
        private System.String outImg;

        [Column]
        public System.String OutImg
        {
            get { return outImg; }
            set {
                if (outImg != value)
                {
                    outImg = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 收费人员
        /// </summary>
        private System.Int32 outOperatorId;

        [Column]
        public System.Int32 OutOperatorId
        {
            get { return outOperatorId; }
            set {
                if (outOperatorId != value)
                {
                    outOperatorId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 出场备注
        /// </summary>
        private System.String outRemark;

        [Column]
        public System.String OutRemark
        {
            get { return outRemark; }
            set {
                if (outRemark != value)
                {
                    outRemark = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 出场时间
        /// </summary>
        private System.String outTime;

        [Column]
        public System.String OutTime
        {
            get { return outTime; }
            set {
                if (outTime != value)
                {
                    outTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 出场类型
        /// </summary>
        private System.Int32 outType;

        [Column]
        public System.Int32 OutType
        {
            get { return outType; }
            set {
                if (outType != value)
                {
                    outType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 出场车道
        /// </summary>
        private System.Int32 outWayId;

        [Column]
        public System.Int32 OutWayId
        {
            get { return outWayId; }
            set {
                if (outWayId != value)
                {
                    outWayId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 已付金额
        /// </summary>
        private System.String paidMoney;

        [Column]
        public System.String PaidMoney
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

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedParkingArrearsMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGARREARS_TOKEN);
        }
        #endregion 
    }
}

