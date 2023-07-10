using Firebase.Database;

namespace TaniePrzejazdyKierowca.EventListeners
{
    public class RideDetailsListener : Java.Lang.Object, IValueEventListener
    {
        public void OnCancelled(DatabaseError error)
        {
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
        }
    }
}