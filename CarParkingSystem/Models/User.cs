using CarParkingSystem.Common;
using CarParkingSystem.Messages;
using CarParkingSystem.Unities;
using CarParkingSystem.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Models
{
    public partial class User : BaseEntity
    {
        #region 属性
        //public int Id { get; set; }
        //public string Username { get; set; }
        //public string Password { get; set; }
        //public int Role { get; set; }

        //[ObservableProperty] private int id;
        //[ObservableProperty] private string username;
        //[ObservableProperty] private string password;
        //[ObservableProperty] private int role;

        

        private string username;

        public string Username
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

        private string password;

        public string Password
        {
            get { return password; }
            set { 
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged();
                }
            }
        }

        private string salt;
        [NoExport]
        public string Salt
        {
            get { return salt; }
            set {
                if (salt != value) 
                {
                    salt = value;
                    OnPropertyChanged();
                }
            }
        }

        private string email;

        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged();
                }
            }
        }

        private string phone;

        public string Phone
        {
            get { return phone; }
            set
            {
                if (phone != value)
                {
                    phone = value;
                    OnPropertyChanged();
                }
            }
        }

        private int status;

        public int Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged();
                }
            }
        }

        private string createdAt;

        public string CreatedAt
        {
            get { return createdAt; }
            set
            {
                if (createdAt != value)
                {
                    createdAt = value;
                    OnPropertyChanged();
                }
            }
        }

        private string updatedAt;

        public string UpdatedAt
        {
            get { return updatedAt; }
            set
            {
                if (updatedAt != value)
                {
                    updatedAt = value;
                    OnPropertyChanged();
                }
            }
        }

        private string lastLoginTime;

        public string LastLoginTime
        {
            get { return lastLoginTime; }
            set
            {
                if (lastLoginTime != value)
                {
                    lastLoginTime = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 命令

        [NoExport]
        [RelayCommand]
        private void Selected()
        {
            WeakReferenceMessenger.Default.Send<SelectedUserMessage, string>(TokenManage.REFRESH_SUMMARY_SELECTED_USER_TOKEN);
        }
        #endregion 
    }
}
