using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;
using TaniePrzejazdyKierowca.Helpers;

namespace TaniePrzejazdyKierowca.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();
            var currentUser = AppDataHelper.GetCurrentUser();
            if (currentUser == null)
            {
                StartActivity(typeof(LoginActivity));
            }
            else
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}