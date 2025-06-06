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
    public partial class Order : BaseEntity
    {
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
        /// 产品类型（月卡或临停）
        /// </summary>
        private System.Int32 productType;

        [Column]
        public System.Int32 ProductType
        {
            get { return productType; }
            set {
                if (productType != value)
                {
                    productType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 产品编号
        /// </summary>
        private System.String productId;

        [Column]
        public System.String ProductId
        {
            get { return productId; }
            set {
                if (productId != value)
                {
                    productId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车牌号码
        /// </summary>
        private System.String buyer;

        [Column]
        public System.String Buyer
        {
            get { return buyer; }
            set {
                if (buyer != value)
                {
                    buyer = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 支付订单号
        /// </summary>
        private System.Int32 payOrder;

        [Column]
        public System.Int32 PayOrder
        {
            get { return payOrder; }
            set {
                if (payOrder != value)
                {
                    payOrder = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        private System.Int32 payName;

        [Column]
        public System.Int32 PayName
        {
            get { return payName; }
            set {
                if (payName != value)
                {
                    payName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 支付金额
        /// </summary>
        private System.Int32 payMoney;

        [Column]
        public System.Int32 PayMoney
        {
            get { return payMoney; }
            set {
                if (payMoney != value)
                {
                    payMoney = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 建立日期
        /// </summary>
        private System.String createDate;

        [Column]
        public System.String CreateDate
        {
            get { return createDate; }
            set {
                if (createDate != value)
                {
                    createDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 支付状态
        /// </summary>
        private System.Int32 payStatus;

        [Column]
        public System.Int32 PayStatus
        {
            get { return payStatus; }
            set {
                if (payStatus != value)
                {
                    payStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 支付类型（无感支付，被扫等）
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
        /// 客户端类型
        /// </summary>
        private System.Int32 clientType;

        [Column]
        public System.Int32 ClientType
        {
            get { return clientType; }
            set {
                if (clientType != value)
                {
                    clientType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 客户编码
        /// </summary>
        private System.String clientId;

        [Column]
        public System.String ClientId
        {
            get { return clientId; }
            set {
                if (clientId != value)
                {
                    clientId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 支付地址
        /// </summary>
        private System.String payUrl;

        [Column]
        public System.String PayUrl
        {
            get { return payUrl; }
            set {
                if (payUrl != value)
                {
                    payUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 过期时间
        /// </summary>
        private System.String expireDate;

        [Column]
        public System.String ExpireDate
        {
            get { return expireDate; }
            set {
                if (expireDate != value)
                {
                    expireDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否分账
        /// </summary>
        private System.Int32 isProfitSharing;

        [Column]
        public System.Int32 IsProfitSharing
        {
            get { return isProfitSharing; }
            set {
                if (isProfitSharing != value)
                {
                    isProfitSharing = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 交易单号
        /// </summary>
        private System.String transactionId;

        [Column]
        public System.String TransactionId
        {
            get { return transactionId; }
            set {
                if (transactionId != value)
                {
                    transactionId = value;
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
        /// 联系电话
        /// </summary>
        private System.String phone;

        [Column]
        public System.String Phone
        {
            get { return phone; }
            set {
                if (phone != value)
                {
                    phone = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 购买数量
        /// </summary>
        private System.Int32 buyNumber;

        [Column]
        public System.Int32 BuyNumber
        {
            get { return buyNumber; }
            set {
                if (buyNumber != value)
                {
                    buyNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 产品开始时间
        /// </summary>
        private System.String startTime;

        [Column]
        public System.String StartTime
        {
            get { return startTime; }
            set {
                if (startTime != value)
                {
                    startTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 产品结束时间
        /// </summary>
        private System.String endTime;

        [Column]
        public System.String EndTime
        {
            get { return endTime; }
            set {
                if (endTime != value)
                {
                    endTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否开票
        /// </summary>
        private System.String invoice;

        [Column]
        public System.String Invoice
        {
            get { return invoice; }
            set {
                if (invoice != value)
                {
                    invoice = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedOrderMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDER_TOKEN);
        }
        #endregion 
    }
}

