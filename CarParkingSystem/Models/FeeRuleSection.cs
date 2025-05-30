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
    public partial class FeeRuleSection : BaseEntity
    {
        /// <summary>
        /// 是否规则标记
        /// </summary>
        private System.Int32 feeRuleFlag;

        [Column]
        public System.Int32 FeeRuleFlag
        {
            get { return feeRuleFlag; }
            set {
                if (feeRuleFlag != value)
                {
                    feeRuleFlag = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 收费规则编号
        /// </summary>
        private System.Int32 feeRuleId;

        [Column]
        public System.Int32 FeeRuleId
        {
            get { return feeRuleId; }
            set {
                if (feeRuleId != value)
                {
                    feeRuleId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 入口车道
        /// </summary>
        private System.Int32 inWay;

        [Column]
        public System.Int32 InWay
        {
            get { return inWay; }
            set {
                if (inWay != value)
                {
                    inWay = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 出口车道
        /// </summary>
        private System.Int32 outWay;

        [Column]
        public System.Int32 OutWay
        {
            get { return outWay; }
            set {
                if (outWay != value)
                {
                    outWay = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 超时收费
        /// </summary>
        private System.Int32 overtimeFeeRule;

        [Column]
        public System.Int32 OvertimeFeeRule
        {
            get { return overtimeFeeRule; }
            set {
                if (overtimeFeeRule != value)
                {
                    overtimeFeeRule = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 超时处理
        /// </summary>
        private System.Int32 overtimeType;

        [Column]
        public System.Int32 OvertimeType
        {
            get { return overtimeType; }
            set {
                if (overtimeType != value)
                {
                    overtimeType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 停车收费
        /// </summary>
        private System.Int32 parkingFeeRule;

        [Column]
        public System.Int32 ParkingFeeRule
        {
            get { return parkingFeeRule; }
            set {
                if (parkingFeeRule != value)
                {
                    parkingFeeRule = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 停车时间
        /// </summary>
        private System.Int32 parkingTime;

        [Column]
        public System.Int32 ParkingTime
        {
            get { return parkingTime; }
            set {
                if (parkingTime != value)
                {
                    parkingTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 分段标识
        /// </summary>
        private System.String sectionName;

        [Column]
        public System.String SectionName
        {
            get { return sectionName; }
            set {
                if (sectionName != value)
                {
                    sectionName = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedFeeRuleSectionMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_FEERULESECTION_TOKEN);
        }
        #endregion 
    }
}

