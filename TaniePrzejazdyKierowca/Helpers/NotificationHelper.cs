using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;

namespace TaniePrzejazdyKierowca.Helpers
{
    public class NotificationHelper : Java.Lang.Object
    {
        public const string PRIMARY_CHANNEL = "Urgent";
        public const int NOTIFY_ID = 100;

        public void NotifyVersion26(Context context, Android.Content.Res.Resources res, Android.App.NotificationManager manager)
        {
            var channelName = "Secondary Channel";
            var importance = NotificationImportance.High;
            var channel = new NotificationChannel(PRIMARY_CHANNEL, channelName, importance);

            var path = Android.Net.Uri.Parse("android.resource://com.companyname.tanieprzejazdykierowca" + Resource.Raw.alert);
            var audioAttribute = new AudioAttributes.Builder().SetContentType(AudioContentType.Sonification).SetUsage(AudioUsageKind.Notification).Build();

            channel.EnableLights(true);
            channel.EnableVibration(true);
            channel.SetSound(path, audioAttribute);
            channel.LockscreenVisibility = NotificationVisibility.Public;

            manager.CreateNotificationChannel(channel);
            Intent intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);

            Notification.Builder builder = new Notification.Builder(context).SetContentTitle("Tanie przejazdy").SetSmallIcon(Resource.Drawable.greenmarker)
                .SetLargeIcon(BitmapFactory.DecodeResource(res, Resource.Drawable.iconimage))
                .SetContentText("You have a trip request")
                .SetChannelId(PRIMARY_CHANNEL)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);
            manager.Notify(NOTIFY_ID,builder.Build());
        }
        
    }
}