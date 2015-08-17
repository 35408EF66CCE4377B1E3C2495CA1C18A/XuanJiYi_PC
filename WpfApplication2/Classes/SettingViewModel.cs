using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Tai_Shi_Xuan_Ji_Yi.Classes
{
    public class SettingViewModel : ViewModelBase
    {
        string new_password;
        string conf_new_password;

        public SettingViewModel()
        {
            
        }

        public string NewPassword
        {
            set
            {
                new_password = value;
                RaisePropertyChanged("NewPassword");
            }
            get
            {
                return new_password;
            }
        }

        public string ConfirmNewPassword
        {
            set
            {
                conf_new_password = value;
                RaisePropertyChanged("ConfirmNewPassword");
            }
            get
            {
                return conf_new_password;
            }
        }

        /// <summary>
        /// 更改Password的Excute
        /// </summary>
        public RelayCommand ChangePassword
        {
            get
            {
                return new RelayCommand(() => 
                {
                    if(NewPassword == "" || ConfirmNewPassword == "")
                    {
                        Messenger.Default.Send<GenericMessage<string>>(new GenericMessage<string>(this, "请输入新密码"), "ChangePasswordFailed");
                    }
                    else if(NewPassword != ConfirmNewPassword)   // 如果两次输入的密码不同
                    {
                        Messenger.Default.Send<GenericMessage<string>>(new GenericMessage<string>(this, "两次输入的密码不同，请重新输入"), "ChangePasswordFailed");
                    }
                    else if(NewPassword == CPublicVariables.Configuration.LoginPassword)
                    {
                        Messenger.Default.Send<GenericMessage<string>>(new GenericMessage<string>(this, "新密码不能和旧密码相同"), "ChangePasswordFailed");
                    }
                    else // 更改成功
                    {
                        CPublicVariables.Configuration.LoginPassword = NewPassword;
                        CPublicVariables.Configuration.Save();
                        Messenger.Default.Send<GenericMessage<string>>(new GenericMessage<string>(this, "锁屏密码更改成功！"), "ChangePasswordSucceed");
                    }
                });
            }
        }
    }
}
