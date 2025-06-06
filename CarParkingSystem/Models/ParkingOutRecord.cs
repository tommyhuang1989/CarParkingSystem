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
    public partial class ParkingOutRecord : BaseEntity
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
        /// 修改类型
        /// </summary>
        private System.Int32 chargeType;

        [Column]
        public System.Int32 ChargeType
        {
            get { return chargeType; }
            set {
                if (chargeType != value)
                {
                    chargeType = value;
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
        /// 入场颜色
        /// </summary>
        private System.Int32 inCarColor;

        [Column]
        public System.Int32 InCarColor
        {
            get { return inCarColor; }
            set {
                if (inCarColor != value)
                {
                    inCarColor = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 入场类型
        /// </summary>
        private System.Int32 inCarType;

        [Column]
        public System.Int32 InCarType
        {
            get { return inCarType; }
            set {
                if (inCarType != value)
                {
                    inCarType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 修改车牌
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
        /// 开门方式
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
        /// 出场颜色
        /// </summary>
        private System.Int32 outCarColor;

        [Column]
        public System.Int32 OutCarColor
        {
            get { return outCarColor; }
            set {
                if (outCarColor != value)
                {
                    outCarColor = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 出场类型
        /// </summary>
        private System.Int32 outCarType;

        [Column]
        public System.Int32 OutCarType
        {
            get { return outCarType; }
            set {
                if (outCarType != value)
                {
                    outCarType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 修改车牌
        /// </summary>
        private System.Int32 outCpChanged;

        [Column]
        public System.Int32 OutCpChanged
        {
            get { return outCpChanged; }
            set {
                if (outCpChanged != value)
                {
                    outCpChanged = value;
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
        /// 车辆编号
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
            WeakReferenceMessenger.Default.Send<SelectedParkingOutRecordMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKINGOUTRECORD_TOKEN);
        }
        #endregion 
    }
}

