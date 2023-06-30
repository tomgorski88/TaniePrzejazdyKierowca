using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CoordinatorLayout.Widget;
using Firebase.Database;
using Firebase;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firebase.Auth;
using TaniePrzejazdyKierowca.Helpers;
using TaniePrzejazdyKierowca.EventListeners;
using Java.Util;

namespace TaniePrzejazdyKierowca.Activities
{
    [Activity(Label = "RegistrationActivity", MainLauncher =false, Theme = "@style/tpTheme")]
    public class RegistrationActivity : AppCompatActivity
    {
        private TextInputLayout fullNameText;
        private TextInputLayout phoneText;
        private TextInputLayout emailText;
        private TextInputLayout passwordText;
        private Button registerButton;
        private CoordinatorLayout rootView;
        private TextView loginText;

        private FirebaseAuth mAuth;
        private FirebaseDatabase database;
        private FirebaseUser currentUser;

        private readonly TaskCompletionListener taskCompletionListener = new TaskCompletionListener();

        private string fullname, phone, email, password;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.register);
            ConnectViews();
            SetupDatabase();
        }

        void ConnectViews()
        {
            fullNameText = (TextInputLayout)FindViewById(Resource.Id.fullNameText);
            phoneText = (TextInputLayout)FindViewById(Resource.Id.phoneText);
            emailText = (TextInputLayout)FindViewById(Resource.Id.emailText);
            passwordText = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            registerButton = (Button)FindViewById(Resource.Id.registerButton);

            registerButton.Click += RegisterButton_Click;
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            fullname = fullNameText.EditText.Text;
            phone = phoneText.EditText.Text;
            email = emailText.EditText.Text;
            password = passwordText.EditText.Text;

            if (fullname.Length < 3)
            {
                Snackbar.Make(rootView, "Please enter a valid name.", Snackbar.LengthShort).Show();
                return;
            }
            else if (phone.Length < 9)
            {
                Snackbar.Make(rootView, "Please enter a valid phone number.", Snackbar.LengthShort).Show();
                return;
            }
            else if (!email.Contains('@'))
            {
                Snackbar.Make(rootView, "Please enter a valid email.", Snackbar.LengthShort).Show();
                return;
            }
            else if (password.Length < 8)
            {
                Snackbar.Make(rootView, "Please enter a password longer than 7 characters.", Snackbar.LengthShort).Show();
                return;
            }
            mAuth.CreateUserWithEmailAndPassword(email, password)
                .AddOnSuccessListener(this, taskCompletionListener)
                .AddOnFailureListener(this, taskCompletionListener);
            taskCompletionListener.Success += (o, g) =>
            {
                var newDriverRef = database.GetReference("drivers/" + mAuth.CurrentUser.Uid);
                var userMap = new HashMap();
                userMap.Put("email", email);
                userMap.Put("phone", phone);
                userMap.Put("fullname", fullname);
                userMap.Put("created_at", DateTime.Now.ToString());

                newDriverRef.SetValue(userMap);
                Snackbar.Make(rootView, "Driver registration was successful", Snackbar.LengthShort).Show();
                StartActivity(typeof(MainActivity));
            };
            taskCompletionListener.Failure += (w, r) =>
            {
                Snackbar.Make(rootView, "Driver registration failed", Snackbar.LengthShort).Show();
            };
        }

        void SetupDatabase()
        {
            database = AppDataHelper.GetDatabase();
            mAuth = AppDataHelper.GetFirebaseAuth();
            currentUser = AppDataHelper.GetCurrentUser();
        }
    }
}