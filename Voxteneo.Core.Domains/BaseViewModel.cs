using AutoMapper;
using System.ComponentModel;
using Voxteneo.Core.Helper;

namespace Voxteneo.Core.Domains
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public BaseViewModel()
        {
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion


        public TDestination ToMap<TDestination>()
        {
            return AopHelper.CreateObject<TDestination>(Mapper.Map<TDestination>(this));
        }
    }
}
