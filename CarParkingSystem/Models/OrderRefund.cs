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
    public partial class OrderRefund : BaseEntity
    {
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
        /// 订单金额
        /// </summary>
        private System.Int32 orderMoney;

        [Column]
        public System.Int32 OrderMoney
        {
            get { return orderMoney; }
            set {
                if (orderMoney != value)
                {
                    orderMoney = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 退款方式
        /// </summary>
        private System.Int32 refundType;

        [Column]
        public System.Int32 RefundType
        {
            get { return refundType; }
            set {
                if (refundType != value)
                {
                    refundType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 退款金额
        /// </summary>
        private System.Int32 refundMoney;

        [Column]
        public System.Int32 RefundMoney
        {
            get { return refundMoney; }
            set {
                if (refundMoney != value)
                {
                    refundMoney = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 退款原因
        /// </summary>
        private System.String reason;

        [Column]
        public System.String Reason
        {
            get { return reason; }
            set {
                if (reason != value)
                {
                    reason = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 退款状态
        /// </summary>
        private System.Int32 refundStatus;

        [Column]
        public System.Int32 RefundStatus
        {
            get { return refundStatus; }
            set {
                if (refundStatus != value)
                {
                    refundStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 退款备注
        /// </summary>
        private System.String refundRemark;

        [Column]
        public System.String RefundRemark
        {
            get { return refundRemark; }
            set {
                if (refundRemark != value)
                {
                    refundRemark = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 操作用户
        /// </summary>
        private System.Int32 createUser;

        [Column]
        public System.Int32 CreateUser
        {
            get { return createUser; }
            set {
                if (createUser != value)
                {
                    createUser = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 操作时间
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
        /// 退款订单号
        /// </summary>
        private System.Int32 refundOrderId;

        [Column]
        public System.Int32 RefundOrderId
        {
            get { return refundOrderId; }
            set {
                if (refundOrderId != value)
                {
                    refundOrderId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 退款流水单
        /// </summary>
        private System.String refundTransactionId;

        [Column]
        public System.String RefundTransactionId
        {
            get { return refundTransactionId; }
            set {
                if (refundTransactionId != value)
                {
                    refundTransactionId = value;
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

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedOrderRefundMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ORDERREFUND_TOKEN);
        }
        #endregion 
    }
}

