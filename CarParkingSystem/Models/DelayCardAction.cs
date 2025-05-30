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
    public partial class DelayCardAction : BaseEntity
    {
        /// <summary>
        /// 车辆编号
        /// </summary>
        private System.Int32 carId;

        [Column]
        public System.Int32 CarId
        {
            get { return carId; }
            set {
                if (carId != value)
                {
                    carId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 操作日期
        /// </summary>
        private System.String opDate;

        [Column]
        public System.String OpDate
        {
            get { return opDate; }
            set {
                if (opDate != value)
                {
                    opDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        private System.Int32 opKind;

        [Column]
        public System.Int32 OpKind
        {
            get { return opKind; }
            set {
                if (opKind != value)
                {
                    opKind = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 操作金额
        /// </summary>
        private System.Decimal opMoney;

        [Column]
        public System.Decimal OpMoney
        {
            get { return opMoney; }
            set {
                if (opMoney != value)
                {
                    opMoney = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 操作人员
        /// </summary>
        private System.String opNo;

        [Column]
        public System.String OpNo
        {
            get { return opNo; }
            set {
                if (opNo != value)
                {
                    opNo = value;
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

        /// <summary>
        /// 有效期结束
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
        /// 有效期开始
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
            WeakReferenceMessenger.Default.Send<SelectedDelayCardActionMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_DELAYCARDACTION_TOKEN);
        }
        #endregion 
    }
}

