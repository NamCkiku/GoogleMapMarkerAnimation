using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RouteDemo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PinInfowindowActiveView : Label
    {
        public PinInfowindowActiveView(string text = "")
        {
            InitializeComponent();
            WidthRequest = -1;
            Text = text;
        }
    }
}