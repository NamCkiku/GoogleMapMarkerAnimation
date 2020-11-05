using Xamarin.Forms.GoogleMaps;

namespace RouteDemo.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            map.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(new Position(20.9735, 105.847), 14);
            map.UiSettings.ZoomGesturesEnabled = true;
            map.UiSettings.ZoomControlsEnabled = false;
            map.UiSettings.RotateGesturesEnabled = false;
        }
    }
}
