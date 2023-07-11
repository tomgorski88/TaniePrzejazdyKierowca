using Android;
using Android.App;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.ViewPager.Widget;
using Com.Ittianyu.Bottomnavigationviewex;
using System;
using TaniePrzejazdy.Helpers;
using TaniePrzejazdyKierowca.Adapters;
using TaniePrzejazdyKierowca.DataModels;
using TaniePrzejazdyKierowca.EventListeners;
using TaniePrzejazdyKierowca.Fragments;
using TaniePrzejazdyKierowca.Helpers;

namespace TaniePrzejazdyKierowca
{
    [Activity(Label = "@string/app_name", Theme = "@style/tpTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        ViewPager viewPager;
        BottomNavigationViewEx bnve;

        Button goOnlineButton;

        //Fragments
        HomeFragment homeFragment = new HomeFragment();
        AccountFragment accountFragment = new AccountFragment();
        EarningsFragment earningsFragment = new EarningsFragment();
        RatingsFragment ratingsFragment = new RatingsFragment();

        NewRequestFragment requestFoundDialogue;

        private readonly string[] permissionGroupLocation = { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };
        private const int requestId = 0;

        ProfileEventListener profileEventListener = new ProfileEventListener();
        AvailabilityListener availabilityListener;
        RideDetailsListener rideDetailsListener;

        MediaPlayer player;

        Android.Locations.Location mLastLocation;
        LatLng mLastLatLng;

        bool availabilityStatus;
        bool isBackground;
        bool newRideAssigned;

        private RideDetails newRideDetails;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            ConnectViews();
            CheckSpecialPermission();
            profileEventListener.Create();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void ConnectViews()
        {
            goOnlineButton = (Button)FindViewById(Resource.Id.goOnlineButton);
            goOnlineButton.Click += GoOnlineButton_Click;

            bnve = (BottomNavigationViewEx)FindViewById(Resource.Id.bnve);
            bnve.EnableItemShiftingMode(false);
            bnve.EnableShiftingMode(false);
            bnve.NavigationItemSelected += Bnve_NavigationItemSelected;

            BnveToActionColor(0);

            viewPager = (ViewPager)FindViewById(Resource.Id.viewpager);

            viewPager.OffscreenPageLimit = 3;
            viewPager.BeginFakeDrag();

            SetupViewPager();

            homeFragment.CurrentLocation += HomeFragment_CurrentLocation;
        }

        private void HomeFragment_CurrentLocation(object sender, LocationCallbackHelper.OnLocationCapturedEventArgs e)
        {
            mLastLocation = e.Location;
            mLastLatLng = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);

            if (availabilityListener != null)
            {
                availabilityListener.UpdateLocation(mLastLocation);
            }

            if (availabilityStatus && availabilityListener == null)
            {
                TakeDriverOnline();
            }
        }

        private void TakeDriverOnline()
        {
            availabilityListener = new AvailabilityListener();
            availabilityListener.Create(mLastLocation);
            availabilityListener.RideAssigned += AvailabilityListener_RideAssigned;
            availabilityListener.RideTimeout += AvailabilityListener_RideTimeout;
            availabilityListener.RideCancelled += AvailabilityListener_RideCancelled;
        }

        private void AvailabilityListener_RideCancelled(object sender, EventArgs e)
        {
            if (requestFoundDialogue != null)
            {
                requestFoundDialogue.Dismiss();
                requestFoundDialogue = null;
                player.Stop();
                player = null;
            }
            Toast.MakeText(this, "New trip cancelled", ToastLength.Long).Show();
            availabilityListener.Reactivate();
        }

        private void AvailabilityListener_RideTimeout(object sender, EventArgs e)
        {
            if (requestFoundDialogue != null)
            {
                requestFoundDialogue.Dismiss();
                requestFoundDialogue = null;
                player.Stop();
                player = null;
            }

            Toast.MakeText(this, "New trip timed out", ToastLength.Long).Show();
            availabilityListener.Reactivate();
        }

        private void AvailabilityListener_RideAssigned(object sender, AvailabilityListener.RideAssignedIDEventArgs e)
        {
            Toast.MakeText(this, "New trip assigned = " + e.RideId, ToastLength.Long).Show();

            rideDetailsListener = new RideDetailsListener();
            rideDetailsListener.Create(e.RideId);
            rideDetailsListener.RideDetailsFound += RideDetailsListener_RideDetailsFound;
            rideDetailsListener.RideDetailsNotFound += RideDetailsListener_RideDetailsNotFound;
        }

        private void RideDetailsListener_RideDetailsNotFound(object sender, EventArgs e)
        {

        }

        void CreateNewRequestDialogue()
        {
            requestFoundDialogue = new NewRequestFragment(newRideDetails.PickupAddress, newRideDetails.DestinationAddress);
            requestFoundDialogue.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            requestFoundDialogue.Show(trans, "Request");

            player = MediaPlayer.Create(this, Resource.Raw.alert);
            player.Start();
        }

        private void RideDetailsListener_RideDetailsFound(object sender, RideDetailsListener.RideDetailsEventArgs e)
        {
            newRideDetails = e.RideDetails;
            if (!isBackground)
            {
                CreateNewRequestDialogue();
            }
            else
            {
                newRideAssigned = true;
                NotificationHelper notificationHelper = new NotificationHelper();
                if ((int)Build.VERSION.SdkInt >= 26)
                {
                   // notificationHelper.NotifyVersion26(this, Resources, (NotificationManager)GetSystemService(NotificationService));
                }
            }
        }

        private void TakeDriverOffline()
        {
            availabilityListener.RemoveListener();
            availabilityListener = null;
        }

        private void GoOnlineButton_Click(object sender, EventArgs e)
        {
            if (!CheckSpecialPermission())
            {
                return;
            }
            if (availabilityStatus)
            {
                var alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Go offline");
                alert.SetMessage("You will not be able to receive Ride requests");
                alert.SetPositiveButton("Continue", (senderAlert, args) =>
                {
                    homeFragment.GoOffline();
                    goOnlineButton.Text = "Go online";
                    availabilityStatus = false;
                    goOnlineButton.Background = ContextCompat.GetDrawable(this, Resource.Drawable.tproundbutton_offline);

                    TakeDriverOffline();
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    alert.Dispose();
                });

                alert.Show();
            }
            else
            {
                availabilityStatus = true;
                homeFragment.GoOnline();
                goOnlineButton.Text = "Go offline";
                goOnlineButton.Background = ContextCompat.GetDrawable(this, Resource.Drawable.tproundbutton_offline);
            }
        }

        private void Bnve_NavigationItemSelected(object sender, Android.Support.Design.Widget.BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.action_earning)
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
        protected override void OnPause()
        {
            isBackground = true;
            base.OnPause();
        }
        protected override void OnResume()
        {
            isBackground = false;
            if (newRideAssigned)
            {
                CreateNewRequestDialogue();
                newRideAssigned = false;
            }
            base.OnResume();
        }
    }
}