using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager.Widget;
using Com.Ittianyu.Bottomnavigationviewex;
using System;
using TaniePrzejazdyKierowca.Adapters;
using TaniePrzejazdyKierowca.Fragments;

namespace TaniePrzejazdyKierowca
{
    [Activity(Label = "@string/app_name", Theme = "@style/tpTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        ViewPager viewPager;
        BottomNavigationViewEx bnve;

        //Fragments
        HomeFragment homeFragment = new HomeFragment();
        AccountFragment accountFragment = new AccountFragment();
        EarningsFragment earningsFragment = new EarningsFragment();
        RatingsFragment ratingsFragment = new RatingsFragment();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void ConnectViews()
        {
            bnve = (BottomNavigationViewEx)FindViewById(Resource.Id.bnve);
            viewPager = (ViewPager)FindViewById(Resource.Id.viewpager);

            viewPager.OffscreenPageLimit = 3;
            viewPager.BeginFakeDrag();

            SetupViewPager();
        }

        private void SetupViewPager()
        {
            var adapter = new ViewPagerAdapter(SupportFragmentManager);
            adapter.AddFragment(homeFragment, "Home");
            adapter.AddFragment(earningsFragment, "Earnings");
            adapter.AddFragment(ratingsFragment, "Rating");
            adapter.AddFragment(accountFragment, "Account");

            viewPager.Adapter = adapter; 
        }
    }
}