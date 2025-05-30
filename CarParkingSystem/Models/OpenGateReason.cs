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
    public partial class OpenGateReason : BaseEntity
    {
        /// <summary>
        /// 是否出口
        /// </summary>
        private System.Int32 isOut;

        [Column]
        public System.Int32 IsOut
        {
            get { return isOut; }
            set {
                if (isOut != value)
                {
                    isOut = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 原因
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

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedOpenGateReasonMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_OPENGATEREASON_TOKEN);
        }
        #endregion 
    }
}

