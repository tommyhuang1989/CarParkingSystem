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
    public partial class ValueCardType : BaseEntity
    {
        /// <summary>
        /// 共享余额
        /// </summary>
        private System.Decimal amount;

        [Column]
        public System.Decimal Amount
        {
            get { return amount; }
            set {
                if (amount != value)
                {
                    amount = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 类型名称
        /// </summary>
        private System.String cardName;

        [Column]
        public System.String CardName
        {
            get { return cardName; }
            set {
                if (cardName != value)
                {
                    cardName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 类型列表
        /// </summary>
        private System.Int32 cardType;

        [Column]
        public System.Int32 CardType
        {
            get { return cardType; }
            set {
                if (cardType != value)
                {
                    cardType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车位总数
        /// </summary>
        private System.Int32 carSpace;

        [Column]
        public System.Int32 CarSpace
        {
            get { return carSpace; }
            set {
                if (carSpace != value)
                {
                    carSpace = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 预约结束时间
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
        /// 到期转为
        /// </summary>
        private System.Int32 expireCard;

        [Column]
        public System.Int32 ExpireCard
        {
            get { return expireCard; }
            set {
                if (expireCard != value)
                {
                    expireCard = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 收费规则类型
        /// </summary>
        private System.Int32 feeRuleType;

        [Column]
        public System.Int32 FeeRuleType
        {
            get { return feeRuleType; }
            set {
                if (feeRuleType != value)
                {
                    feeRuleType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 入场检测
        /// </summary>
        private System.Int32 inCheck;

        [Column]
        public System.Int32 InCheck
        {
            get { return inCheck; }
            set {
                if (inCheck != value)
                {
                    inCheck = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 月卡转临时车折扣
        /// </summary>
        private System.String monthToTempDiscount;

        [Column]
        public System.String MonthToTempDiscount
        {
            get { return monthToTempDiscount; }
            set {
                if (monthToTempDiscount != value)
                {
                    monthToTempDiscount = value;
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
        /// 预约开始日期
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

        /// <summary>
        /// 共享车位
        /// </summary>
        private System.Int32 totalCar;

        [Column]
        public System.Int32 TotalCar
        {
            get { return totalCar; }
            set {
                if (totalCar != value)
                {
                    totalCar = value;
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
            WeakReferenceMessenger.Default.Send<SelectedValueCardTypeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_VALUECARDTYPE_TOKEN);
        }
        #endregion 
    }
}

