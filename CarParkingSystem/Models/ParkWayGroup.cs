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
    public partial class ParkWayGroup : BaseEntity
    {
        /// <summary>
        /// 间隔时间
        /// </summary>
        private System.Int32 blankingTime;

        [Column]
        public System.Int32 BlankingTime
        {
            get { return blankingTime; }
            set {
                if (blankingTime != value)
                {
                    blankingTime = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 下一个车道
        /// </summary>
        private System.Int32 nextWayId;

        [Column]
        public System.Int32 NextWayId
        {
            get { return nextWayId; }
            set {
                if (nextWayId != value)
                {
                    nextWayId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 上一个车道
        /// </summary>
        private System.Int32 preWayId;

        [Column]
        public System.Int32 PreWayId
        {
            get { return preWayId; }
            set {
                if (preWayId != value)
                {
                    preWayId = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedParkWayGroupMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_PARKWAYGROUP_TOKEN);
        }
        #endregion 
    }
}

