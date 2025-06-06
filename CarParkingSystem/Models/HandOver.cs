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
    public partial class HandOver : BaseEntity
    {
        /// <summary>
        /// 异常车辆数
        /// </summary>
        private System.Int32 abCar;

        [Column]
        public System.Int32 AbCar
        {
            get { return abCar; }
            set {
                if (abCar != value)
                {
                    abCar = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 欠费车辆数
        /// </summary>
        private System.Int32 arrearsCar;

        [Column]
        public System.Int32 ArrearsCar
        {
            get { return arrearsCar; }
            set {
                if (arrearsCar != value)
                {
                    arrearsCar = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 现金收入
        /// </summary>
        private System.Decimal cashFee;

        [Column]
        public System.Decimal CashFee
        {
            get { return cashFee; }
            set {
                if (cashFee != value)
                {
                    cashFee = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 下班时间
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
        /// ETC收入
        /// </summary>
        private System.Decimal etcfee;

        [Column]
        public System.Decimal Etcfee
        {
            get { return etcfee; }
            set {
                if (etcfee != value)
                {
                    etcfee = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 入场车辆数
        /// </summary>
        private System.Int32 inCar;

        [Column]
        public System.Int32 InCar
        {
            get { return inCar; }
            set {
                if (inCar != value)
                {
                    inCar = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否下班
        /// </summary>
        private System.Int32 isFinished;

        [Column]
        public System.Int32 IsFinished
        {
            get { return isFinished; }
            set {
                if (isFinished != value)
                {
                    isFinished = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 出场车辆数
        /// </summary>
        private System.Int32 outCar;

        [Column]
        public System.Int32 OutCar
        {
            get { return outCar; }
            set {
                if (outCar != value)
                {
                    outCar = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 手机支付金额
        /// </summary>
        private System.Decimal phoneFee;

        [Column]
        public System.Decimal PhoneFee
        {
            get { return phoneFee; }
            set {
                if (phoneFee != value)
                {
                    phoneFee = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 上班时间
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
        /// 总计收入
        /// </summary>
        private System.Decimal totalFee;

        [Column]
        public System.Decimal TotalFee
        {
            get { return totalFee; }
            set {
                if (totalFee != value)
                {
                    totalFee = value;
                    OnPropertyChanged();
                }
            }
        }

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
        /// 储值车辆金额
        /// </summary>
        private System.Decimal valueCardFee;

        [Column]
        public System.Decimal ValueCardFee
        {
            get { return valueCardFee; }
            set {
                if (valueCardFee != value)
                {
                    valueCardFee = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedHandOverMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_HANDOVER_TOKEN);
        }
        #endregion 
    }
}

