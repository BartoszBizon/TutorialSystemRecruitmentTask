using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ArtifexMundi.Client.Mvvm
{
    /// <summary>
    /// "Event (OnPropertyChanged)" arrow: ViewModel -> Binding.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Sets the field and raises PropertyChanged only when the value actually changed.</summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
