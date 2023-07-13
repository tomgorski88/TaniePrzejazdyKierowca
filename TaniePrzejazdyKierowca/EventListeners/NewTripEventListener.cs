using Android.Locations;
using Firebase.Database;
using TaniePrzejazdyKierowca.Helpers;

namespace TaniePrzejazdyKierowca.EventListeners
{
    public class NewTripEventListener : Java.Lang.Object, IValueEventListener
    {
        private string mRideId;
        private Location mLocation;
        private FirebaseDatabase database;
        private DatabaseReference tripRef;

        private bool isAccepted;

        public NewTripEventListener(string ride_id, Android.Locations.Location lastlocation)
        {
            mRideId = ride_id;
            mLocation = lastlocation;
            database = AppDataHelper.GetDatabase();
        }
        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                if (!isAccepted)
                {
                    isAccepted = true;
                    Accept();
                }
            }
        }

        public void Create()
        {
            tripRef = database.GetReference("rideRequest/" + mRideId);
            tripRef.AddValueEventListener(this);
        }

        private void Accept()
        {
            tripRef.Child("status").SetValue("accepted");
            tripRef.Child("driver_name").SetValue(AppDataHelper.GetFullName());
            tripRef.Child("driver_phone").SetValue(AppDataHelper.GetPhone());
            tripRef.Child("driver_location").Child("latitude").SetValue(mLocation.Latitude);
            tripRef.Child("driver_location").Child("longitude").SetValue(mLocation.Longitude);
            tripRef.Child("driver_id").SetValue(AppDataHelper.GetCurrentUser().Uid);
        }
    }
}