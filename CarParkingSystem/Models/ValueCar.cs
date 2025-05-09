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
    public partial class ValueCar : BaseEntity
    {
        /// <summary>
        /// 账户余额
        /// </summary>
        private System.Decimal balance;

        [Column]
        public System.Decimal Balance
        {
            get { return balance; }
            set {
                if (balance != value)
                {
                    balance = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 序号
        /// </summary>
        private System.String carCode;

        [Column]
        public System.String CarCode
        {
            get { return carCode; }
            set {
                if (carCode != value)
                {
                    carCode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车辆类型
        /// </summary>
        private System.Int32 card;

        [Column]
        public System.Int32 Card
        {
            get { return card; }
            set {
                if (card != value)
                {
                    card = value;
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
        /// 押金
        /// </summary>
        private System.Decimal deposit;

        [Column]
        public System.Decimal Deposit
        {
            get { return deposit; }
            set {
                if (deposit != value)
                {
                    deposit = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车位号码
        /// </summary>
        private System.String parkSpace;

        [Column]
        public System.String ParkSpace
        {
            get { return parkSpace; }
            set {
                if (parkSpace != value)
                {
                    parkSpace = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车位类型
        /// </summary>
        private System.Int32 parkSpaceType;

        [Column]
        public System.Int32 ParkSpaceType
        {
            get { return parkSpaceType; }
            set {
                if (parkSpaceType != value)
                {
                    parkSpaceType = value;
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
        /// 车位名称
        /// </summary>
        private System.String spaceName;

        [Column]
        public System.String SpaceName
        {
            get { return spaceName; }
            set {
                if (spaceName != value)
                {
                    spaceName = value;
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

        /// <summary>
        /// 车主姓名
        /// </summary>
        private System.String username;

        [Column]
        public System.String Username
        {
            get { return username; }
            set {
                if (username != value)
                {
                    username = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车主地址
        /// </summary>
        private System.String userRemark;

        [Column]
        public System.String UserRemark
        {
            get { return userRemark; }
            set {
                if (userRemark != value)
                {
                    userRemark = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车主电话
        /// </summary>
        private System.String userTel;

        [Column]
        public System.String UserTel
        {
            get { return userTel; }
            set {
                if (userTel != value)
                {
                    userTel = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        private System.String validEnd;

        [Column]
        public System.String ValidEnd
        {
            get { return validEnd; }
            set {
                if (validEnd != value)
                {
                    validEnd = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        private System.String validFrom;

        [Column]
        public System.String ValidFrom
        {
            get { return validFrom; }
            set {
                if (validFrom != value)
                {
                    validFrom = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedValueCarMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_VALUECAR_TOKEN);
        }
        #endregion 
    }
}

