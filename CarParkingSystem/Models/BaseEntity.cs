using CarParkingSystem.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.Models
{
    public interface IBaseEntity
    {
        int Id { get; set; }
    }

    /// <summary>
    /// 添加 Id 属性，方便进行批量删除
    /// Id 和 IsSelected 属性在基类中定义
    /// </summary>
    public partial class BaseEntity : ObservableObject, IBaseEntity
    {
        private bool isSelected = false;
        [NotMapped]
        [NoExport]
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        private int id;
        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
