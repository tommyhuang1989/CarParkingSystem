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
    public partial class Role : BaseEntity
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        private System.Int32 roleId;

        [Column]
        public System.Int32 RoleId
        {
            get { return roleId; }
            set {
                if (roleId != value)
                {
                    roleId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 角色名称
        /// </summary>
        private System.String roleName;

        [Column]
        public System.String RoleName
        {
            get { return roleName; }
            set {
                if (roleName != value)
                {
                    roleName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 角色备注
        /// </summary>
        private System.String roleRemark;

        [Column]
        public System.String RoleRemark
        {
            get { return roleRemark; }
            set {
                if (roleRemark != value)
                {
                    roleRemark = value;
                    OnPropertyChanged();
                }
            }
        }

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedRoleMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_ROLE_TOKEN);
        }
        #endregion 
    }
}

