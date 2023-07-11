using Firebase.Database;
using System;
using System.Globalization;
using TaniePrzejazdyKierowca.DataModels;
using TaniePrzejazdyKierowca.Helpers;

namespace TaniePrzejazdyKierowca.EventListeners
{
    public class RideDetailsListener : Java.Lang.Object, IValueEventListener
    {
        public class RideDetailsEventArgs : EventArgs
        {
            public RideDetails RideDetails { get; set; }
        }

        public event EventHandler<RideDetailsEventArgs> RideDetailsFound;
        public event EventHandler RideDetailsNotFound;

        public void OnCancelled(DatabaseError error)
        {
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {

                var rideDetails = new RideDetails
                {
                    DestinationAddress = snapshot.Child("destination_address").Value.ToString(),
                    DestinationLat = Double.Parse(snapshot.Child("destination").Child("latitude").Value.ToString(), CultureInfo.InvariantCulture),
                    DestinationLng = double.Parse(snapshot.Child("destination").Child("longitude").Value.ToString(), CultureInfo.InvariantCulture),

                    PickupAddress = snapshot.Child("pickup_address").Value.ToString(),
                    PickupLat = double.Parse(snapshot.Child("location").Child("latitude").Value.ToString(), CultureInfo.InvariantCulture),
                    PickupLng = double.Parse(snapshot.Child("location").Child("longitude").Value.ToString(), CultureInfo.InvariantCulture),

                    RideId = snapshot.Key,
                    RiderName = snapshot.Child("rider_name").Value.ToString(),
                    RiderPhone = snapshot.Child("rider_phone").Value.ToString()
                };

                RideDetailsFound?.Invoke(this, new RideDetailsEventArgs { RideDetails = rideDetails });
            }
            else
            {
                RideDetailsNotFound?.Invoke(this, new EventArgs());
            }
        }

        public void Create(string ride_id)
        {
            var database = AppDataHelper.GetDatabase();
            var rideDetailsRef = database.GetReference("rideRequest/" + ride_id);
            rideDetailsRef.AddListenerForSingleValueEvent(this);
        }
    }
}