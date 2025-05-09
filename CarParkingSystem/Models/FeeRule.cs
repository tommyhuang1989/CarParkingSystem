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
    public partial class FeeRule : BaseEntity
    {
        /// <summary>
        /// 收费规则JSON格式
        /// </summary>
        private System.String feeRuleData;

        [Column]
        public System.String FeeRuleData
        {
            get { return feeRuleData; }
            set {
                if (feeRuleData != value)
                {
                    feeRuleData = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 收费规则名称
        /// </summary>
        private System.String feeRuleName;

        [Column]
        public System.String FeeRuleName
        {
            get { return feeRuleName; }
            set {
                if (feeRuleName != value)
                {
                    feeRuleName = value;
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

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedFeeRuleMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_FEERULE_TOKEN);
        }
        #endregion 
    }
}

