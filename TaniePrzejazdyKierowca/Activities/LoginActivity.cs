using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Firebase.Auth;
using Firebase.Database;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaniePrzejazdyKierowca.EventListeners;
using TaniePrzejazdyKierowca.Helpers;

namespace TaniePrzejazdyKierowca.Activities
{
    [Activity(Label = "LoginActivity", Theme = "@style/tpTheme", MainLauncher = true)]
    public class LoginActivity : Activity
    {
        private TextInputLayout emailText;
        private TextInputLayout passwordText;
        private Button loginButton;
        private CoordinatorLayout rootView;
        private TextView clickToRegisterText;

        private FirebaseAuth mAuth;
        private FirebaseDatabase database;
        private FirebaseUser currentUser;

        private Android.App.AlertDialog.Builder alert;
        private Android.App.AlertDialog alertDialog;



        private string fullname, phone, email, password;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);

            ConnectViews();
            InitializeFirebase();
        }

        void InitializeFirebase()
        {
            mAuth = AppDataHelper.GetFirebaseAuth();
            currentUser = AppDataHelper.GetCurrentUser();
            database = AppDataHelper.GetDatabase();
        }

        void ConnectViews()
        {
            emailText = (TextInputLayout)FindViewById(Resource.Id.emailText);
            passwordText = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            loginButton = (Button)FindViewById(Resource.Id.loginButton);
            clickToRegisterText = (TextView)FindViewById(Resource.Id.clickToRegisterText);

            loginButton.Click += LoginButton_Click;
            clickToRegisterText.Click += ClickToRegisterText_Click;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            password = passwordText.EditText.Text;
            email = emailText.EditText.Text;

            ShowProgressDialogue();

            var taskCompletionListener = new TaskCompletionListener();
            taskCompletionListener.Success += TaskCompletionListener_Success;
            taskCompletionListener.Failure += TaskCompletionListener_Failure;

            mAuth.SignInWithEmailAndPassword(email, password)
                .AddOnSuccessListener(taskCompletionListener)
                .AddOnFailureListener(taskCompletionListener);
        }

        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            CloseProgressDialogue();
            Snackbar.Make(rootView, "Login failed", Snackbar.LengthShort).Show();
        }

        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            CloseProgressDialogue();
            StartActivity(typeof(MainActivity));
        }

        private void ClickToRegisterText_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrationActivity));
            Finish();
        }
        private void ShowProgressDialogue()
        {
            alert = new AlertDialog.Builder(this);
            alert.SetView(Resource.Layout.progress);
            alert.SetCancelable(false);

            alertDialog = alert.Show();
        }
        private void CloseProgressDialogue()
        {
            if(alertDialog != null)
            {
                alertDialog.Dismiss();
                alertDialog = null;
                alert = null;
            }
        }
    }
}