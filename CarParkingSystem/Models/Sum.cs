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
    public partial class Sum : BaseEntity
    {
        /// <summary>
        /// 出场车辆
        /// </summary>
        private System.Int32 carOutCount;

        [Column]
        public System.Int32 CarOutCount
        {
            get { return carOutCount; }
            set {
                if (carOutCount != value)
                {
                    carOutCount = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 现金支付
        /// </summary>
        private System.Decimal cash;

        [Column]
        public System.Decimal Cash
        {
            get { return cash; }
            set {
                if (cash != value)
                {
                    cash = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 储值金额
        /// </summary>
        private System.Decimal chuzhi;

        [Column]
        public System.Decimal Chuzhi
        {
            get { return chuzhi; }
            set {
                if (chuzhi != value)
                {
                    chuzhi = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 储值卡扣费
        /// </summary>
        private System.Decimal chuzhika;

        [Column]
        public System.Decimal Chuzhika
        {
            get { return chuzhika; }
            set {
                if (chuzhika != value)
                {
                    chuzhika = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 优惠金额
        /// </summary>
        private System.Decimal discount;

        [Column]
        public System.Decimal Discount
        {
            get { return discount; }
            set {
                if (discount != value)
                {
                    discount = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 统计日期
        /// </summary>
        private System.String dt;

        [Column]
        public System.String Dt
        {
            get { return dt; }
            set {
                if (dt != value)
                {
                    dt = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 应付金额
        /// </summary>
        private System.Decimal needPay;

        [Column]
        public System.Decimal NeedPay
        {
            get { return needPay; }
            set {
                if (needPay != value)
                {
                    needPay = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 实际支付
        /// </summary>
        private System.Decimal paid;

        [Column]
        public System.Decimal Paid
        {
            get { return paid; }
            set {
                if (paid != value)
                {
                    paid = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 线上支付
        /// </summary>
        private System.Decimal pp;

        [Column]
        public System.Decimal Pp
        {
            get { return pp; }
            set {
                if (pp != value)
                {
                    pp = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 月卡延期费用
        /// </summary>
        private System.Decimal yueka;

        [Column]
        public System.Decimal Yueka
        {
            get { return yueka; }
            set {
                if (yueka != value)
                {
                    yueka = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedSumMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_SUM_TOKEN);
        }
        #endregion 
    }
}

