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
    public partial class ParkDevice : BaseEntity
    {
        /// <summary>
        /// 允许关闸
        /// </summary>
        private System.Int32 allowCloseGate;

        [Column]
        public System.Int32 AllowCloseGate
        {
            get { return allowCloseGate; }
            set {
                if (allowCloseGate != value)
                {
                    allowCloseGate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 相机IP
        /// </summary>
        private System.String camera0;

        [Column]
        public System.String Camera0
        {
            get { return camera0; }
            set {
                if (camera0 != value)
                {
                    camera0 = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 相机类型
        /// </summary>
        private System.Int32 camera0Type;

        [Column]
        public System.Int32 Camera0Type
        {
            get { return camera0Type; }
            set {
                if (camera0Type != value)
                {
                    camera0Type = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 相机IP
        /// </summary>
        private System.String camera1;

        [Column]
        public System.String Camera1
        {
            get { return camera1; }
            set {
                if (camera1 != value)
                {
                    camera1 = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 相机类型
        /// </summary>
        private System.Int32 camera1Type;

        [Column]
        public System.Int32 Camera1Type
        {
            get { return camera1Type; }
            set {
                if (camera1Type != value)
                {
                    camera1Type = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 485接口
        /// </summary>
        private System.Int32 camera485;

        [Column]
        public System.Int32 Camera485
        {
            get { return camera485; }
            set {
                if (camera485 != value)
                {
                    camera485 = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 双识别过滤
        /// </summary>
        private System.Int32 cameraDoubleFilter;

        [Column]
        public System.Int32 CameraDoubleFilter
        {
            get { return cameraDoubleFilter; }
            set {
                if (cameraDoubleFilter != value)
                {
                    cameraDoubleFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// IO口
        /// </summary>
        private System.Int32 cameraIo;

        [Column]
        public System.Int32 CameraIo
        {
            get { return cameraIo; }
            set {
                if (cameraIo != value)
                {
                    cameraIo = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 密钥
        /// </summary>
        private System.String cameraKey;

        [Column]
        public System.String CameraKey
        {
            get { return cameraKey; }
            set {
                if (cameraKey != value)
                {
                    cameraKey = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 重复进出
        /// </summary>
        private System.Int32 cameraRecomeFilter;

        [Column]
        public System.Int32 CameraRecomeFilter
        {
            get { return cameraRecomeFilter; }
            set {
                if (cameraRecomeFilter != value)
                {
                    cameraRecomeFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 抓拍相机IP
        /// </summary>
        private System.String cardCameraIp;

        [Column]
        public System.String CardCameraIp
        {
            get { return cardCameraIp; }
            set {
                if (cardCameraIp != value)
                {
                    cardCameraIp = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 抓拍相机序号
        /// </summary>
        private System.String cardCameraSn;

        [Column]
        public System.String CardCameraSn
        {
            get { return cardCameraSn; }
            set {
                if (cardCameraSn != value)
                {
                    cardCameraSn = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 抓拍相机类型
        /// </summary>
        private System.Int32 cardCameraType;

        [Column]
        public System.Int32 CardCameraType
        {
            get { return cardCameraType; }
            set {
                if (cardCameraType != value)
                {
                    cardCameraType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 刷卡器端口
        /// </summary>
        private System.Int32 cardPort;

        [Column]
        public System.Int32 CardPort
        {
            get { return cardPort; }
            set {
                if (cardPort != value)
                {
                    cardPort = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 刷卡器IP
        /// </summary>
        private System.String cardIp;

        [Column]
        public System.String CardIp
        {
            get { return cardIp; }
            set {
                if (cardIp != value)
                {
                    cardIp = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 刷卡器序号
        /// </summary>
        private System.String cardSn;

        [Column]
        public System.String CardSn
        {
            get { return cardSn; }
            set {
                if (cardSn != value)
                {
                    cardSn = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 刷卡器类型
        /// </summary>
        private System.Int32 cardType;

        [Column]
        public System.Int32 CardType
        {
            get { return cardType; }
            set {
                if (cardType != value)
                {
                    cardType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 设备状态
        /// </summary>
        private System.Int32 devStatus;

        [Column]
        public System.Int32 DevStatus
        {
            get { return devStatus; }
            set {
                if (devStatus != value)
                {
                    devStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 带刷卡器
        /// </summary>
        private System.Int32 hasCard;

        [Column]
        public System.Int32 HasCard
        {
            get { return hasCard; }
            set {
                if (hasCard != value)
                {
                    hasCard = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 带相机
        /// </summary>
        private System.Int32 hasCarmera;

        [Column]
        public System.Int32 HasCarmera
        {
            get { return hasCarmera; }
            set {
                if (hasCarmera != value)
                {
                    hasCarmera = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 默认显示
        /// </summary>
        private System.String ledDisplay;

        [Column]
        public System.String LedDisplay
        {
            get { return ledDisplay; }
            set {
                if (ledDisplay != value)
                {
                    ledDisplay = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 控制卡IP
        /// </summary>
        private System.String ledIp;

        [Column]
        public System.String LedIp
        {
            get { return ledIp; }
            set {
                if (ledIp != value)
                {
                    ledIp = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 控制类型
        /// </summary>
        private System.Int32 ledType;

        [Column]
        public System.Int32 LedType
        {
            get { return ledType; }
            set {
                if (ledType != value)
                {
                    ledType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车道编号
        /// </summary>
        private System.Int32 wayId;

        [Column]
        public System.Int32 WayId
        {
            get { return wayId; }
            set {
                if (wayId != value)
                {
                    wayId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否显示二维码
        /// </summary>
        private System.Int32 qrCode;

        [Column]
        public System.Int32 QrCode
        {
            get { return qrCode; }
            set {
                if (qrCode != value)
                {
                    qrCode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 默认显示
        /// </summary>
        private System.String setDisplay;

        [Column]
        public System.String SetDisplay
        {
            get { return setDisplay; }
            set {
                if (setDisplay != value)
                {
                    setDisplay = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 播报车牌
        /// </summary>
        private System.String setVoice;

        [Column]
        public System.String SetVoice
        {
            get { return setVoice; }
            set {
                if (setVoice != value)
                {
                    setVoice = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 开双闸
        /// </summary>
        private System.Int32 twoGate;

        [Column]
        public System.Int32 TwoGate
        {
            get { return twoGate; }
            set {
                if (twoGate != value)
                {
                    twoGate = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedParkDeviceMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKDEVICE_TOKEN);
        }
        #endregion 
    }
}

