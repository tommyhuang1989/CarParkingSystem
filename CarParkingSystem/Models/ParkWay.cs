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
    public partial class ParkWay : BaseEntity
    {
        /// <summary>
        /// 应付金额
        /// </summary>
        private System.Decimal amount;

        [Column]
        public System.Decimal Amount
        {
            get { return amount; }
            set {
                if (amount != value)
                {
                    amount = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 所属区域
        /// </summary>
        private System.Int32 areaId;

        [Column]
        public System.Int32 AreaId
        {
            get { return areaId; }
            set {
                if (areaId != value)
                {
                    areaId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 入场记录编号
        /// </summary>
        private System.Int32 carInId;

        [Column]
        public System.Int32 CarInId
        {
            get { return carInId; }
            set {
                if (carInId != value)
                {
                    carInId = value;
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
        /// 车牌颜色
        /// </summary>
        private System.Int32 carNoColor;

        [Column]
        public System.Int32 CarNoColor
        {
            get { return carNoColor; }
            set {
                if (carNoColor != value)
                {
                    carNoColor = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车牌类型
        /// </summary>
        private System.Int32 carNoType;

        [Column]
        public System.Int32 CarNoType
        {
            get { return carNoType; }
            set {
                if (carNoType != value)
                {
                    carNoType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车辆状态
        /// </summary>
        private System.Int32 carStatus;

        [Column]
        public System.Int32 CarStatus
        {
            get { return carStatus; }
            set {
                if (carStatus != value)
                {
                    carStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车辆类型
        /// </summary>
        private System.Int32 carTypeId;

        [Column]
        public System.Int32 CarTypeId
        {
            get { return carTypeId; }
            set {
                if (carTypeId != value)
                {
                    carTypeId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否修改车牌
        /// </summary>
        private System.Int32 changedCarNo;

        [Column]
        public System.Int32 ChangedCarNo
        {
            get { return changedCarNo; }
            set {
                if (changedCarNo != value)
                {
                    changedCarNo = value;
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
        /// 当前显示
        /// </summary>
        private System.String display;

        [Column]
        public System.String Display
        {
            get { return display; }
            set {
                if (display != value)
                {
                    display = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 入场图片
        /// </summary>
        private System.String inImage;

        [Column]
        public System.String InImage
        {
            get { return inImage; }
            set {
                if (inImage != value)
                {
                    inImage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 入场时间
        /// </summary>
        private System.String inTime;

        [Column]
        public System.String InTime
        {
            get { return inTime; }
            set {
                if (inTime != value)
                {
                    inTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否允许进入
        /// </summary>
        private System.Int32 isAllowEnter;

        [Column]
        public System.Int32 IsAllowEnter
        {
            get { return isAllowEnter; }
            set {
                if (isAllowEnter != value)
                {
                    isAllowEnter = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否需要确认
        /// </summary>
        private System.Int32 isCsConfirm;

        [Column]
        public System.Int32 IsCsConfirm
        {
            get { return isCsConfirm; }
            set {
                if (isCsConfirm != value)
                {
                    isCsConfirm = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否需要同步
        /// </summary>
        private System.Int32 isNeedAysn;

        [Column]
        public System.Int32 IsNeedAysn
        {
            get { return isNeedAysn; }
            set {
                if (isNeedAysn != value)
                {
                    isNeedAysn = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否支付
        /// </summary>
        private System.Int32 isPaid;

        [Column]
        public System.Int32 IsPaid
        {
            get { return isPaid; }
            set {
                if (isPaid != value)
                {
                    isPaid = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 上一个车牌
        /// </summary>
        private System.String lastCarNo;

        [Column]
        public System.String LastCarNo
        {
            get { return lastCarNo; }
            set {
                if (lastCarNo != value)
                {
                    lastCarNo = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 上一个时间
        /// </summary>
        private System.String lastCarTime;

        [Column]
        public System.String LastCarTime
        {
            get { return lastCarTime; }
            set {
                if (lastCarTime != value)
                {
                    lastCarTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 订单编号
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
        /// 支付金额
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
        /// 车牌编号
        /// </summary>
        private System.String plateId;

        [Column]
        public System.String PlateId
        {
            get { return plateId; }
            set {
                if (plateId != value)
                {
                    plateId = value;
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
        /// 特殊车辆
        /// </summary>
        private System.Int32 specialCar;

        [Column]
        public System.Int32 SpecialCar
        {
            get { return specialCar; }
            set {
                if (specialCar != value)
                {
                    specialCar = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 手动触发
        /// </summary>
        private System.Int32 triggerFlag;

        [Column]
        public System.Int32 TriggerFlag
        {
            get { return triggerFlag; }
            set {
                if (triggerFlag != value)
                {
                    triggerFlag = value;
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
        /// 视频呼叫
        /// </summary>
        private System.Int32 videoCall;

        [Column]
        public System.Int32 VideoCall
        {
            get { return videoCall; }
            set {
                if (videoCall != value)
                {
                    videoCall = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 呼叫二维码
        /// </summary>
        private System.Int32 videoCallQrcode;

        [Column]
        public System.Int32 VideoCallQrcode
        {
            get { return videoCallQrcode; }
            set {
                if (videoCallQrcode != value)
                {
                    videoCallQrcode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 呼叫时间
        /// </summary>
        private System.String videoCallTime;

        [Column]
        public System.String VideoCallTime
        {
            get { return videoCallTime; }
            set {
                if (videoCallTime != value)
                {
                    videoCallTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 播报
        /// </summary>
        private System.String voice;

        [Column]
        public System.String Voice
        {
            get { return voice; }
            set {
                if (voice != value)
                {
                    voice = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 等待支付
        /// </summary>
        private System.Decimal waitPay;

        [Column]
        public System.Decimal WaitPay
        {
            get { return waitPay; }
            set {
                if (waitPay != value)
                {
                    waitPay = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 排队车牌
        /// </summary>
        private System.String waittingCarNo;

        [Column]
        public System.String WaittingCarNo
        {
            get { return waittingCarNo; }
            set {
                if (waittingCarNo != value)
                {
                    waittingCarNo = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 排队车牌颜色
        /// </summary>
        private System.Int32 waittingCarNoColor;

        [Column]
        public System.Int32 WaittingCarNoColor
        {
            get { return waittingCarNoColor; }
            set {
                if (waittingCarNoColor != value)
                {
                    waittingCarNoColor = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 排队车牌类型
        /// </summary>
        private System.Int32 waittingCarNoType;

        [Column]
        public System.Int32 WaittingCarNoType
        {
            get { return waittingCarNoType; }
            set {
                if (waittingCarNoType != value)
                {
                    waittingCarNoType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 排队车牌图片
        /// </summary>
        private System.String waittingImg;

        [Column]
        public System.String WaittingImg
        {
            get { return waittingImg; }
            set {
                if (waittingImg != value)
                {
                    waittingImg = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 排队车牌编号
        /// </summary>
        private System.String waittingPlateId;

        [Column]
        public System.String WaittingPlateId
        {
            get { return waittingPlateId; }
            set {
                if (waittingPlateId != value)
                {
                    waittingPlateId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 排队时间
        /// </summary>
        private System.String waittingTime;

        [Column]
        public System.String WaittingTime
        {
            get { return waittingTime; }
            set {
                if (waittingTime != value)
                {
                    waittingTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车道车牌类型
        /// </summary>
        private System.Int32 wayCarType;

        [Column]
        public System.Int32 WayCarType
        {
            get { return wayCarType; }
            set {
                if (wayCarType != value)
                {
                    wayCarType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车道连接方式
        /// </summary>
        private System.Int32 wayConnect;

        [Column]
        public System.Int32 WayConnect
        {
            get { return wayConnect; }
            set {
                if (wayConnect != value)
                {
                    wayConnect = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车道名称
        /// </summary>
        private System.String wayName;

        [Column]
        public System.String WayName
        {
            get { return wayName; }
            set {
                if (wayName != value)
                {
                    wayName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车道编号
        /// </summary>
        private System.String wayNo;

        [Column]
        public System.String WayNo
        {
            get { return wayNo; }
            set {
                if (wayNo != value)
                {
                    wayNo = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车道状态
        /// </summary>
        private System.Int32 wayStatus;

        [Column]
        public System.Int32 WayStatus
        {
            get { return wayStatus; }
            set {
                if (wayStatus != value)
                {
                    wayStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车道类型（入口或出口）
        /// </summary>
        private System.Int32 wayType;

        [Column]
        public System.Int32 WayType
        {
            get { return wayType; }
            set {
                if (wayType != value)
                {
                    wayType = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedParkWayMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKWAY_TOKEN);
        }
        #endregion 
    }
}

