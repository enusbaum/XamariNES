namespace XamariNES.UI.App.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new XamariNES.UI.App.App());
        }
    }
}
