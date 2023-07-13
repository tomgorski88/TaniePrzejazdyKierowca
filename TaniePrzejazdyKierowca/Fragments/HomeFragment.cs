using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using TaniePrzejazdy.Helpers;
using static TaniePrzejazdy.Helpers.LocationCallbackHelper;

namespace TaniePrzejazdyKierowca.Fragments
{
    public class HomeFragment : AndroidX.Fragment.App.Fragment, IOnMapReadyCallback
    {
        private ImageView centerMarker;

        public GoogleMap mainMap;
        public EventHandler<OnLocationCapturedEventArgs> CurrentLocation;
        
        private Android.Gms.Location.LocationRequest mLocationRequest;
        FusedLocationProviderClient locationProviderClient;
        private Android.Locations.Location mLastLocation;
        private LocationCallbackHelper mLocationCallback;

        static int UPDATE_INTERVAL = 5000;
        static int FASTEST_INTERVAL = 5000;
        static int DISPLACEMENT = 3;

        private LinearLayout rideInfoLayout;
        private TextView riderNameText;

        ImageButton cancelTripButton;
        ImageButton callRiderButton;
        ImageButton navigateButton;
        Button tripButton;

        bool tripCreated;
       

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CreateLocationRequest();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var view = inflater.Inflate(Resource.Layout.home, container, false);
            var mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            centerMarker = (ImageView)view.FindViewById(Resource.Id.centerMarker);
            mapFragment.GetMapAsync(this);

            cancelTripButton = (ImageButton)view.FindViewById(Resource.Id.cancelTripButton);
            callRiderButton  = (ImageButton) view.FindViewById(Resource.Id.callRiderButton);
            navigateButton = (ImageButton)view.FindViewById(Resource.Id.navigateButton);
            tripButton = (Button)view.FindViewById(Resource.Id.tripButton);

            riderNameText = (TextView)view.FindViewById(Resource.Id.riderNameText);
            rideInfoLayout = (LinearLayout)view.FindViewById(Resource.Id.rideInfoLayout);

            return view;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mainMap = googleMap;
            //var mapkey = Resources.GetString(Resource.String.mapkey);
            //mapFunctionHelper = new MapFunctionHelper(mapkey, googleMap);

            //mainMap.CameraIdle += MainMap_CameraIdle;
        }

        public void CreateLocationRequest()
        {
            mLocationRequest = new Android.Gms.Location.LocationRequest();
            mLocationRequest.SetInterval(UPDATE_INTERVAL);
            mLocationRequest.SetFastestInterval(FASTEST_INTERVAL);
            mLocationRequest.SetPriority(Android.Gms.Location.LocationRequest.PriorityHighAccuracy);
            mLocationRequest.SetSmallestDisplacement(DISPLACEMENT);
            locationProviderClient = LocationServices.GetFusedLocationProviderClient(Activity);
            mLocationCallback = new LocationCallbackHelper();
            mLocationCallback.MyLocation += MLocationCallback_MyLocation;

           // StartLocationUpdates();
        }

        private void MLocationCallback_MyLocation(object sender, LocationCallbackHelper.OnLocationCapturedEventArgs e)
        {
            mLastLocation = e.Location;
            var myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 15));

            CurrentLocation?.Invoke(this, new OnLocationCapturedEventArgs { Location = e.Location });
        }

        void StartLocationUpdates()
        {
            locationProviderClient.RequestLocationUpdates(mLocationRequest, mLocationCallback, null);
        }
        private void StopLocationUpdates()
        {
            locationProviderClient.RemoveLocationUpdates(mLocationCallback);
        }

        public void GoOnline()
        {
            centerMarker.Visibility = ViewStates.Visible;
            StartLocationUpdates();
        }

        public void GoOffline()
        {
            centerMarker.Visibility = ViewStates.Invisible;
            StopLocationUpdates();
        }

       
    }
}