using CarParkingSystem.Models;
using CarParkingSystem.Unities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.Vml.Office;
using Material.Icons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarParkingSystem.ViewModels
{
    public partial class CreateFirstFloorWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _projectName = "CarParkingSystem";

        /// <summary>
        /// DisplayName 会对应翻译文件中的字段，所以一般格式为：xxxManagement, 如：UserManagement
        /// </summary>
        [ObservableProperty]
        private string _displayName;

        [ObservableProperty]
        private string _className;

        [ObservableProperty]
        private MaterialIconKind _icon = MaterialIconKind.AccountAlertOutline;//暂时先默认

        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private int _index;

        [ObservableProperty]
        private string _message;

        public ObservableCollection<DemoPageBase> DemoPages { get; }
        public CreateFirstFloorWindowViewModel(IEnumerable<DemoPageBase> demoPages)
        {
            DemoPages = new ObservableCollection<DemoPageBase>(demoPages);
            //DemoPages = DemoPageBase.GetSubPages(0, new ObservableCollection<DemoPageBase>(demoPages));
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                CodeClassFirstFloor codeClass = new CodeClassFirstFloor();
                codeClass.ProjectName = ProjectName;
                codeClass.ClassName = ClassName;
                codeClass.DisplayName = DisplayName;
                codeClass.Icon = Icon;
                codeClass.Pid = 0;
                var maxId = DemoPages.Max(x => x.Id);
                var maxIndex = DemoPages.Max(x => x.Index);
                codeClass.Id = maxId + 1;
                codeClass.Index = maxId + 1;

                var result = GenerateCodeHelper.Run(codeClass);
                if (result)
                {
                    //WeakReferenceMessenger.Default.Send("Add new first menu", TokenManage.MAIN_WINDOW_REFRESH_MENU_TOKEN);
                    WeakReferenceMessenger.Default.Send(ClassName, TokenManage.MAIN_WINDOW_REFRESH_MENU_TOKEN);
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }
    }
}
