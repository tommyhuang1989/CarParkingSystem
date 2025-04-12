using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    /// <summary>
    /// 继承自 ObservableValidator 的 ViewModel 基类，提供了数据验证的方法
    /// </summary>
    public class ViewModelValidatorBase : ObservableValidator
    {
        /// <summary>
        /// 要求接收字段验证信息的字段名为：property + ValidationMessage
        /// </summary>
        /// <param name="propertyName"></param>
        public void UpdateValidationMessage([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName)) return;

            // 根据属性名生成验证消息属性名（例如 Username → UsernameValidationMessage）
            var validationPropertyName = $"{propertyName}ValidationMessage";
            var errors = GetErrors(propertyName);

            // 通过反射设置验证消息属性值
            var property = this.GetType().GetProperty(validationPropertyName);
            if (property != null)
            {
                property.SetValue(this, HasErrors ? string.Join(" ", errors) : string.Empty);
            }
        }
    }
}
