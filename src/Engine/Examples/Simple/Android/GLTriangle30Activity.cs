using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Fusee.Engine.Imp.Graphics.Android;

namespace Fusee.Engine.Examples.Simple.Android
{
	[Activity (Label = "@string/app_name", MainLauncher = false, Icon = "@drawable/app_gltriangle",
#if __ANDROID_11__
		HardwareAccelerated=false,
#endif
		ConfigurationChanges = ConfigChanges.KeyboardHidden, LaunchMode = LaunchMode.SingleTask)]
	public class GLTriangle30Activity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);

            // Inflate our UI from its XML layout description
            // - should match filename res/layout/main.xml ?
            // SetContentView(Resource.Layout.main);

            SetContentView(new LibPaintingView(ApplicationContext, null));

			// Load the view
			// FindViewById (Resource.Id.paintingview);
        }
    }
}
