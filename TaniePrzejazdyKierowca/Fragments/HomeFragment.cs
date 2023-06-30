using Android.Gms.Maps;
using Android.OS;
using Android.Views;

namespace TaniePrzejazdyKierowca.Fragments
{
    public class HomeFragment : AndroidX.Fragment.App.Fragment, IOnMapReadyCallback
    {
        private GoogleMap mainMap;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
             var view = inflater.Inflate(Resource.Layout.home, container, false);
            var mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);
            return view;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mainMap = googleMap;
            //var mapkey = Resources.GetString(Resource.String.mapkey);
            //mapFunctionHelper = new MapFunctionHelper(mapkey, googleMap);

            //mainMap.CameraIdle += MainMap_CameraIdle;
        }
    }
}