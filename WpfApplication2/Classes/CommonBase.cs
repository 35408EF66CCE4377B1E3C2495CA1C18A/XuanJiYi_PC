using System;
using System.ComponentModel;

namespace Tai_Shi_Xuan_Ji_Yi.Classes
{
    [Serializable]
    public class CommonBase : INotifyPropertyChanged
    {

        public  event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string PropertyName)
        {
            PropertyChangedEventArgs arg = new PropertyChangedEventArgs(PropertyName);
            if (PropertyChanged != null)
                PropertyChanged(this, arg);
        }
    }
}
