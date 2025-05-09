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
    public partial class CarTypePara : BaseEntity
    {
        /// <summary>
        /// 车辆类型
        /// </summary>
        private System.Int32 cardNo;

        [Column]
        public System.Int32 CardNo
        {
            get { return cardNo; }
            set {
                if (cardNo != value)
                {
                    cardNo = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 车型名称
        /// </summary>
        private System.String carTypeName;

        [Column]
        public System.String CarTypeName
        {
            get { return carTypeName; }
            set {
                if (carTypeName != value)
                {
                    carTypeName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 高度
        /// </summary>
        private System.String height;

        [Column]
        public System.String Height
        {
            get { return height; }
            set {
                if (height != value)
                {
                    height = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 宽度
        /// </summary>
        private System.String width;

        [Column]
        public System.String Width
        {
            get { return width; }
            set {
                if (width != value)
                {
                    width = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedCarTypeParaMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_CARTYPEPARA_TOKEN);
        }
        #endregion 
    }
}

