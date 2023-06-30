using Android;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
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

        private readonly string[] permissionGroupLocation = { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };
        private const int requestId = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            ConnectViews();
            CheckSpecialPermission();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void ConnectViews()
        {
            bnve = (BottomNavigationViewEx)FindViewById(Resource.Id.bnve);
            bnve.EnableItemShiftingMode(false);
            bnve.EnableShiftingMode(false);
            bnve.NavigationItemSelected += Bnve_NavigationItemSelected;

            BnveToActionColor(0);

            viewPager = (ViewPager)FindViewById(Resource.Id.viewpager);

            viewPager.OffscreenPageLimit = 3;
            viewPager.BeginFakeDrag();

            SetupViewPager();
        }

        private void Bnve_NavigationItemSelected(object sender, Android.Support.Design.Widget.BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if(e.Item.ItemId == Resource.Id.action_earning)
            {
                viewPager.SetCurrentItem(1, true);
                BnveToActionColor(1);
            }
            else if (e.Item.ItemId == Resource.Id.action_home)
            {
                viewPager.SetCurrentItem(0, true);
                BnveToActionColor(0);
            }
            else if (e.Item.ItemId == Resource.Id.action_rating)
            {
                viewPager.SetCurrentItem(2, true);
                BnveToActionColor(2);
            }
            else if (e.Item.ItemId == Resource.Id.action_account)
            {
                viewPager.SetCurrentItem(3, true);
                BnveToActionColor(3);
            }
        }

        void BnveToActionColor(int index)
        {
            var img = bnve.GetIconAt(1);
            var txt = bnve.GetLargeLabelAt(1);
            img.SetColorFilter(Color.Rgb(255, 255, 255));
            txt.SetTextColor(Color.Rgb(255, 255, 255));

            var img0 = bnve.GetIconAt(0);
            var txt0 = bnve.GetLargeLabelAt(0);
            img.SetColorFilter(Color.Rgb(255, 255, 255));
            txt.SetTextColor(Color.Rgb(255, 255, 255));

            var img2 = bnve.GetIconAt(2);
            var txt2 = bnve.GetLargeLabelAt(2);
            img.SetColorFilter(Color.Rgb(255, 255, 255));
            txt.SetTextColor(Color.Rgb(255, 255, 255));

            var img3 = bnve.GetIconAt(3);
            var txt3 = bnve.GetLargeLabelAt(3);
            img.SetColorFilter(Color.Rgb(255, 255, 255));
            txt.SetTextColor(Color.Rgb(255, 255, 255));

            var imgIndex = bnve.GetIconAt(index);
            var textIndex = bnve.GetLargeLabelAt(index);

            imgIndex.SetColorFilter(Color.Rgb(24, 191, 242));
            textIndex.SetTextColor(Color.Rgb(24, 191, 242));
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

        bool CheckSpecialPermission()
        {
            bool permissionGranted;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                permissionGranted = false;
                RequestPermissions(permissionGroupLocation, requestId);
            }
            else
            {
                permissionGranted = true;
            }
            return permissionGranted;
        }
    }
}