using System;
using System.ComponentModel;
using System.Linq;

namespace WatermarkApp
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected void OnPropertyChange(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
