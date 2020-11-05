using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RouteDemo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PinInfowindowView : Label
    {
        public PinInfowindowView(string text = "")
        {
            InitializeComponent();

            Text = text;
            WidthRequest = -1;
        }
    }
}