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
    public partial class CarFree : BaseEntity
    {
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
        /// 结束时间
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
        /// 免费原因
        /// </summary>
        private System.String freeDesc;

        [Column]
        public System.String FreeDesc
        {
            get { return freeDesc; }
            set {
                if (freeDesc != value)
                {
                    freeDesc = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        private System.String fromTime;

        [Column]
        public System.String FromTime
        {
            get { return fromTime; }
            set {
                if (fromTime != value)
                {
                    fromTime = value;
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
        /// 车主地址
        /// </summary>
        private System.String userAddr;

        [Column]
        public System.String UserAddr
        {
            get { return userAddr; }
            set {
                if (userAddr != value)
                {
                    userAddr = value;
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
        /// 车主电话
        /// </summary>
        private System.String userPhone;

        [Column]
        public System.String UserPhone
        {
            get { return userPhone; }
            set {
                if (userPhone != value)
                {
                    userPhone = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车主类型
        /// </summary>
        private System.Int32 userType;

        [Column]
        public System.Int32 UserType
        {
            get { return userType; }
            set {
                if (userType != value)
                {
                    userType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 微信编号
        /// </summary>
        private System.String wxOpenId;

        [Column]
        public System.String WxOpenId
        {
            get { return wxOpenId; }
            set {
                if (wxOpenId != value)
                {
                    wxOpenId = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedCarFreeMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARFREE_TOKEN);
        }
        #endregion 
    }
}

