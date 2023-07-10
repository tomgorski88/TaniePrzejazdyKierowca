using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace TaniePrzejazdyKierowca.Fragments
{
    public class NewRequestFragment : AndroidX.Fragment.App.DialogFragment
    {
        private RelativeLayout acceptRideButton;
        private RelativeLayout rejectRideButton;
        TextView pickupAddressText;
        TextView destinationAddressText;

        private string mPickupAddress;
        private string mDestinationAddress;

        public event EventHandler RideAccepted;
        public event EventHandler RideRejected;

        public NewRequestFragment(string pickupAddress, string destinationAddress)
        {
            mPickupAddress = pickupAddress;
            mDestinationAddress = destinationAddress;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.newrequest_dialogue, container, false);
            pickupAddressText = (TextView)view.FindViewById(Resource.Id.newridePickupText);
            destinationAddressText = (TextView)view.FindViewById(Resource.Id.newrideDestinationText);

            pickupAddressText.Text = mPickupAddress;
            destinationAddressText.Text = mDestinationAddress;

            acceptRideButton = (RelativeLayout)view.FindViewById(Resource.Id.acceptRideButton);
            rejectRideButton = (RelativeLayout)view.FindViewById(Resource.Id.rejectRideButton);

            acceptRideButton.Click += AcceptRideButton_Click;
            rejectRideButton.Click += RejectRideButton_Click;

            return view;
        }

        private void RejectRideButton_Click(object sender, System.EventArgs e)
        {
            RideRejected?.Invoke(this, new EventArgs());
        }

        private void AcceptRideButton_Click(object sender, System.EventArgs e)
        {
            RideAccepted?.Invoke(this, new EventArgs());
        }
    }
}