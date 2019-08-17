using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XamariNES.UI.App.Pages.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
