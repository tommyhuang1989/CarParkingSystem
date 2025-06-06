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
    public partial class ParkSetting : BaseEntity
    {
        /// <summary>
        /// 异常车辆
        /// </summary>
        private System.Int32 abnormalSetting;

        [Column]
        public System.Int32 AbnormalSetting
        {
            get { return abnormalSetting; }
            set {
                if (abnormalSetting != value)
                {
                    abnormalSetting = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 模糊匹配
        /// </summary>
        private System.Int32 autoMatch;

        [Column]
        public System.Int32 AutoMatch
        {
            get { return autoMatch; }
            set {
                if (autoMatch != value)
                {
                    autoMatch = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车位上限
        /// </summary>
        private System.Int32 carUpperLimit;

        [Column]
        public System.Int32 CarUpperLimit
        {
            get { return carUpperLimit; }
            set {
                if (carUpperLimit != value)
                {
                    carUpperLimit = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车位上限处理
        /// </summary>
        private System.Int32 carUpperLimitProcess;

        [Column]
        public System.Int32 CarUpperLimitProcess
        {
            get { return carUpperLimitProcess; }
            set {
                if (carUpperLimitProcess != value)
                {
                    carUpperLimitProcess = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 有效登记车变成临时车辆管理，0表示自动处理，1表示手动确认
        /// </summary>
        private System.Int32 changeTempCar;

        [Column]
        public System.Int32 ChangeTempCar
        {
            get { return changeTempCar; }
            set {
                if (changeTempCar != value)
                {
                    changeTempCar = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 默认车辆类型
        /// </summary>
        private System.Int32 defaultCardId;

        [Column]
        public System.Int32 DefaultCardId
        {
            get { return defaultCardId; }
            set {
                if (defaultCardId != value)
                {
                    defaultCardId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否一次延期所有车辆
        /// </summary>
        private System.Int32 delayBySpace;

        [Column]
        public System.Int32 DelayBySpace
        {
            get { return delayBySpace; }
            set {
                if (delayBySpace != value)
                {
                    delayBySpace = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 间隔时间
        /// </summary>
        private System.Int32 delayTime;

        [Column]
        public System.Int32 DelayTime
        {
            get { return delayTime; }
            set {
                if (delayTime != value)
                {
                    delayTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车位满后出口有车出去后处理入口正在等待的车，0=不处理（倒车或人工处理），1=系统自动处理
        /// </summary>
        private System.Int32 entryWayWaittingCar;

        [Column]
        public System.Int32 EntryWayWaittingCar
        {
            get { return entryWayWaittingCar; }
            set {
                if (entryWayWaittingCar != value)
                {
                    entryWayWaittingCar = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 免费时长
        /// </summary>
        private System.Int32 freeTime;

        [Column]
        public System.Int32 FreeTime
        {
            get { return freeTime; }
            set {
                if (freeTime != value)
                {
                    freeTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 开闸是否需要填写原因
        /// </summary>
        private System.Int32 isNeedReason;

        [Column]
        public System.Int32 IsNeedReason
        {
            get { return isNeedReason; }
            set {
                if (isNeedReason != value)
                {
                    isNeedReason = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否手动登记
        /// </summary>
        private System.Int32 isSelfEntry;

        [Column]
        public System.Int32 IsSelfEntry
        {
            get { return isSelfEntry; }
            set {
                if (isSelfEntry != value)
                {
                    isSelfEntry = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 支付后离场时间
        /// </summary>
        private System.Int32 leaveDate;

        [Column]
        public System.Int32 LeaveDate
        {
            get { return leaveDate; }
            set {
                if (leaveDate != value)
                {
                    leaveDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 非机动车默认类型
        /// </summary>
        private System.Int32 motorbikeDefaultCard;

        [Column]
        public System.Int32 MotorbikeDefaultCard
        {
            get { return motorbikeDefaultCard; }
            set {
                if (motorbikeDefaultCard != value)
                {
                    motorbikeDefaultCard = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 一位多车卡类
        /// </summary>
        private System.Int32 mulSpace;

        [Column]
        public System.Int32 MulSpace
        {
            get { return mulSpace; }
            set {
                if (mulSpace != value)
                {
                    mulSpace = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 多位多车过期：0＝过期车辆，1＝车位减少1个
        /// </summary>
        private System.Int32 mulSpaceExpired;

        [Column]
        public System.Int32 MulSpaceExpired
        {
            get { return mulSpaceExpired; }
            set {
                if (mulSpaceExpired != value)
                {
                    mulSpaceExpired = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 一位多车入场处理
        /// </summary>
        private System.Int32 oneLotMoreCarEnter;

        [Column]
        public System.Int32 OneLotMoreCarEnter
        {
            get { return oneLotMoreCarEnter; }
            set {
                if (oneLotMoreCarEnter != value)
                {
                    oneLotMoreCarEnter = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 一位多车以临时车进场
        /// </summary>
        private System.Int32 oneLotMoreCarTempCar;

        [Column]
        public System.Int32 OneLotMoreCarTempCar
        {
            get { return oneLotMoreCarTempCar; }
            set {
                if (oneLotMoreCarTempCar != value)
                {
                    oneLotMoreCarTempCar = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车位已满处理
        /// </summary>
        private System.Int32 parkingFull;

        [Column]
        public System.Int32 ParkingFull
        {
            get { return parkingFull; }
            set {
                if (parkingFull != value)
                {
                    parkingFull = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 在同一个机动车道重复识别车牌，时间内直接开闸
        /// </summary>
        private System.Int32 resCanOpenTime;

        [Column]
        public System.Int32 ResCanOpenTime
        {
            get { return resCanOpenTime; }
            set {
                if (resCanOpenTime != value)
                {
                    resCanOpenTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 显示今天收入
        /// </summary>
        private System.Int32 showTodayIncome;

        [Column]
        public System.Int32 ShowTodayIncome
        {
            get { return showTodayIncome; }
            set {
                if (showTodayIncome != value)
                {
                    showTodayIncome = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 临时车辆管理，0表示自动处理，1表示手动确认
        /// </summary>
        private System.Int32 tempCarManager;

        [Column]
        public System.Int32 TempCarManager
        {
            get { return tempCarManager; }
            set {
                if (tempCarManager != value)
                {
                    tempCarManager = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 无牌车触发模式
        /// </summary>
        private System.Int32 unlicensedModel;

        [Column]
        public System.Int32 UnlicensedModel
        {
            get { return unlicensedModel; }
            set {
                if (unlicensedModel != value)
                {
                    unlicensedModel = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 无入场记录
        /// </summary>
        private System.Int32 unsaveInAbnormal;

        [Column]
        public System.Int32 UnsaveInAbnormal
        {
            get { return unsaveInAbnormal; }
            set {
                if (unsaveInAbnormal != value)
                {
                    unsaveInAbnormal = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 手动出场记录异常
        /// </summary>
        private System.Int32 unsaveManualAbnormal;

        [Column]
        public System.Int32 UnsaveManualAbnormal
        {
            get { return unsaveManualAbnormal; }
            set {
                if (unsaveManualAbnormal != value)
                {
                    unsaveManualAbnormal = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 无出场记录
        /// </summary>
        private System.Int32 unsaveOutAbnormal;

        [Column]
        public System.Int32 UnsaveOutAbnormal
        {
            get { return unsaveOutAbnormal; }
            set {
                if (unsaveOutAbnormal != value)
                {
                    unsaveOutAbnormal = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 储值卡扣费规则：0－仅供本车使用；1＝车主多车共享余额
        /// </summary>
        private System.Int32 valueCardDeduction;

        [Column]
        public System.Int32 ValueCardDeduction
        {
            get { return valueCardDeduction; }
            set {
                if (valueCardDeduction != value)
                {
                    valueCardDeduction = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedParkSettingMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKSETTING_TOKEN);
        }
        #endregion 
    }
}

