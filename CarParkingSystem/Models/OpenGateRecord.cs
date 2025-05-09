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
    public partial class OpenGateRecord : BaseEntity
    {
        /// <summary>
        /// 开闸时间
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
        /// 抓拍图片
        /// </summary>
        private System.String imageUrl;

        [Column]
        public System.String ImageUrl
        {
            get { return imageUrl; }
            set {
                if (imageUrl != value)
                {
                    imageUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 开闸原因
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
        /// 开闸用户
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
        /// 开闸车道
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

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedOpenGateRecordMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_OPENGATERECORD_TOKEN);
        }
        #endregion 
    }
}

