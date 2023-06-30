using Android.App;
using Android.Content;
using Firebase.Database;
using TaniePrzejazdyKierowca.Helpers;

namespace TaniePrzejazdyKierowca.EventListeners
{
    public class ProfileEventListener : Java.Lang.Object, IValueEventListener
    {
        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo",FileCreationMode.Private);
        private ISharedPreferencesEditor editor;

        public void OnCancelled(DatabaseError error)
        {
           
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
           if(snapshot.Value != null)
            {
                var fullName = (snapshot.Child("fullname") != null) ? snapshot.Child("fullname").Value.ToString() : string.Empty;
                var email = snapshot.Child("email") != null ? snapshot.Child("email").Value.ToString() : string.Empty;
                var phone = snapshot.Child("phone") != null ? snapshot.Child("phone").Value.ToString() : string.Empty;

                editor.PutString("fullname", fullName);
                editor.PutString("email", email);
                editor.PutString("phone", phone);
                editor.Apply();
            }
        }

        public void Create()
        {
            editor = preferences.Edit();
            var database = AppDataHelper.GetDatabase();
            var driverId = AppDataHelper.GetCurrentUser().Uid;
            var driverRef = database.GetReference("drivers/" + driverId);
            driverRef.AddValueEventListener(this);
        }
    }
}