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
    public partial class CarVisitor : BaseEntity
    {
        /// <summary>
        /// 车辆类型
        /// </summary>
        private System.String cardNo;

        [Column]
        public System.String CardNo
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
        /// 客户端类型
        /// </summary>
        private System.Int32 client;

        [Column]
        public System.Int32 Client
        {
            get { return client; }
            set {
                if (client != value)
                {
                    client = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 客户端编号
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
        /// 登记时间
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
        /// 离场时间
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
        /// 手机号码
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
        /// 入场时间
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
        /// 真实姓名
        /// </summary>
        private System.String trueName;

        [Column]
        public System.String TrueName
        {
            get { return trueName; }
            set {
                if (trueName != value)
                {
                    trueName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 访问客户
        /// </summary>
        private System.String visitorHouse;

        [Column]
        public System.String VisitorHouse
        {
            get { return visitorHouse; }
            set {
                if (visitorHouse != value)
                {
                    visitorHouse = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedCarVisitorMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARVISITOR_TOKEN);
        }
        #endregion 
    }
}

