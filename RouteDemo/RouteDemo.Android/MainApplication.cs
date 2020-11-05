using System;
using Android.App;
using Android.Runtime;

namespace RouteDemo.Droid
{
    [Application(
        Theme = "@style/MainTheme"
        )]
    [MetaData("com.google.android.maps.v2.API_KEY", Value = "AIzaSyDwhz_8SoIcFYMLVh3rcto1cWGbAPdQfGI")]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Xamarin.Essentials.Platform.Init(this);
        }
    }
}
