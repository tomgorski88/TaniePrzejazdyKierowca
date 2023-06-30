using Firebase.Database;
using Java.Util;
using TaniePrzejazdyKierowca.Helpers;

namespace TaniePrzejazdyKierowca.EventListeners
{
    public class AvailabilityListener : Java.Lang.Object, IValueEventListener
    {
        FirebaseDatabase database;
        DatabaseReference availabilityRef;
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            
        }

        public void Create(Android.Locations.Location myLocation)
        {
            database = AppDataHelper.GetDatabase();
            var driverId = AppDataHelper.GetCurrentUser().Uid;

            availabilityRef = database.GetReference("driversAvailable/" + driverId);

            var location = new HashMap();
            location.Put("latitude", myLocation.Latitude);
            location.Put("longitude", myLocation.Longitude);

            var driverInfo = new HashMap();
            driverInfo.Put("location", location);
            driverInfo.Put("ride_id", "wating");

            availabilityRef.AddValueEventListener(this);
            availabilityRef.SetValue(driverInfo);

        }

        public void RemoveListener()
        {
            availabilityRef.RemoveValue();
            availabilityRef.RemoveEventListener(this);
            availabilityRef = null;
        }
    }
}