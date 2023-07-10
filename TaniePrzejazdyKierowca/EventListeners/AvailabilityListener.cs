using Firebase.Database;
using Java.Util;
using System;
using TaniePrzejazdyKierowca.Helpers;

namespace TaniePrzejazdyKierowca.EventListeners
{
    public class AvailabilityListener : Java.Lang.Object, IValueEventListener
    {
        FirebaseDatabase database;
        DatabaseReference availabilityRef;

        public class RideAssignedIDEventArgs : EventArgs
        {
            public string RideId { get; set; }
        }

        public event EventHandler<RideAssignedIDEventArgs> RideAssigned;
        public event EventHandler RideCancelled;
        public event EventHandler RideTimeout;

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                var ride_id = snapshot.Child("ride_id").Value.ToString();
                if (ride_id != "waiting" && ride_id != "timeout" && ride_id != "cancelled")
                {
                    RideAssigned?.Invoke(this, new RideAssignedIDEventArgs { RideId = ride_id });
                }
                else if (ride_id == "timeout")
                {
                    RideTimeout?.Invoke(this, new EventArgs());
                }
                else if (ride_id == "cancelled")
                {
                    RideCancelled?.Invoke(this, new EventArgs());
                }
            }
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
            driverInfo.Put("ride_id", "waiting");

            availabilityRef.AddValueEventListener(this);
            availabilityRef.SetValue(driverInfo);

        }

        public void RemoveListener()
        {
            availabilityRef.RemoveValue();
            availabilityRef.RemoveEventListener(this);
            availabilityRef = null;
        }


        public void UpdateLocation(Android.Locations.Location myLocation)
        {
            var driverId = AppDataHelper.GetCurrentUser().Uid;
            if (availabilityRef != null)
            {
                var locationRef = database.GetReference("driversAvailable/" + driverId + "/location");

                var locationMap = new HashMap();
                locationMap.Put("latitude", myLocation.Latitude);
                locationMap.Put("longitude", myLocation.Longitude);
                locationRef.SetValue(locationMap);
            }
        }

        public void Reactivate()
        {
            availabilityRef.Child("ride_id").SetValue("waiting");
        }
    }
}