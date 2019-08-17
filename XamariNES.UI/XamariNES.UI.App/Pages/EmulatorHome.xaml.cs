using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace XamariNES.UI.App.Pages
{
    public partial class EmulatorHome : ContentPage
    {
        public EmulatorHome()
        {
            InitializeComponent();

            //Safely handle the iPhone "notch" on iOS devices
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            BindingContext = new ViewModels.EmulatorHomeViewModel();

            //Subscribe to Events from ViewModel
            MessagingCenter.Subscribe<ViewModels.EmulatorHomeViewModel>(this, "InvalidateEmulatorSurface", (obj) => Device.BeginInvokeOnMainThread(
                () => canvasEmulator.InvalidateSurface()));
            MessagingCenter.Subscribe<ViewModels.EmulatorHomeViewModel>(this, "InvalidateConsoleSurface", (obj) => Device.BeginInvokeOnMainThread(
                () => canvasConsole.InvalidateSurface()));
        }
    }
}
