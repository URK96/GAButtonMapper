using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Transitions;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Hoang8f.Widgets;

using System;
using System.Threading.Tasks;

namespace GAButtonMapper
{
    [Activity(Label = "InitSettingActivity", Theme = "@style/AppTheme.InitSetting")]
    public class InitSettingActivity : AppCompatActivity
    {
        private CardView cvGrantPermission;
        private TextView tvGrantSummary;
        private FButton btGrantCheck;
        private CardView cvAccessibility;
        private TextView tvAccessibilitySummary;
        private FButton btAccessibilityCheck;

        private bool hasMoveSetting = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.InitSettingLayout);

            cvGrantPermission = FindViewById<CardView>(Resource.Id.InitSettingGrantPermissionCardView);
            cvGrantPermission.Click += CvGrantPermission_Click;
            tvGrantSummary = FindViewById<TextView>(Resource.Id.InitSettingGrantPermissionCardView_SummaryText);
            btGrantCheck = FindViewById<FButton>(Resource.Id.InitSetting_GrantPermission_ExplainCheckButton);
            btGrantCheck.Click += BtGrantCheck_Click;

            cvAccessibility = FindViewById<CardView>(Resource.Id.InitSettingAccessibilityCardView);
            cvAccessibility.Click += CvAccessibility_Click;
            tvAccessibilitySummary = FindViewById<TextView>(Resource.Id.InitSettingAccessibilityCardView_SummaryText);
            btAccessibilityCheck = FindViewById<FButton>(Resource.Id.InitSetting_Accessibility_ExplainCheckButton);
            btAccessibilityCheck.Click += BtAccessibilityCheck_Click;

            var c = Java.Lang.Runtime.GetRuntime().Exec("logcat -c");
            c.WaitFor();

            _ = ShowFirstCard();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (hasMoveSetting)
            {
                btAccessibilityCheck.Text = "Finish";
            }
        }

        private void BtAccessibilityCheck_Click(object sender, EventArgs e)
        {
            if (hasMoveSetting)
            {
                _ = FinishSetting();
            }
            else
            {
                var intent = new Intent(Android.Provider.Settings.ActionAccessibilitySettings);
                StartActivity(intent);
                hasMoveSetting = true;
            }
        }

        private async void BtGrantCheck_Click(object sender, EventArgs e)
        {
            if (!ETC.sharedPreferences.GetBoolean("HasRestart", false))
            {
                ETC.sharedPreferences.Edit().PutBoolean("HasRestart", true).Apply();
                await Task.Delay(500);
                FinishAffinity();
                Process.KillProcess(Process.MyPid());
            }

            await Task.Delay(1000);

            if (!ETC.CheckPermission(this, Manifest.Permission.ReadLogs))
            {
                Toast.MakeText(this, "Read Logs check Fail", ToastLength.Short).Show();

                return;
            }

            if (!ETC.CheckPermission(this, Manifest.Permission.WriteSecureSettings))
            {
                Toast.MakeText(this, "Write Secure Settings check Fail", ToastLength.Short).Show();

                return;
            }

            Toast.MakeText(this, "OK", ToastLength.Short).Show();
            _ = ShowSecondCard();

        }

        private void CvGrantPermission_Click(object sender, EventArgs e)
        {
            TransitionManager.BeginDelayedTransition(cvGrantPermission);

            tvGrantSummary.Visibility = ViewStates.Gone;
            FindViewById<LinearLayout>(Resource.Id.InitSetting_GrantPermission_ExplainLayout).Visibility = ViewStates.Visible;

            btGrantCheck.Enabled = true;

            if (ETC.sharedPreferences.GetBoolean("HasRestart", false))
            {
                FindViewById<TextView>(Resource.Id.InitSetting_GrantPermission_ExplainText).SetText(Resource.String.InitSetting_GrantPermission_ExplainAfterRestart);
                btGrantCheck.Text = "Check";
            }

            cvGrantPermission.Click -= CvGrantPermission_Click;
        }

        private async void CvAccessibility_Click(object sender, EventArgs e)
        {
            TransitionManager.BeginDelayedTransition(cvGrantPermission);

            cvGrantPermission.Visibility = ViewStates.Gone;

            await Task.Delay(500);

            TransitionManager.BeginDelayedTransition(cvAccessibility);

            tvAccessibilitySummary.Visibility = ViewStates.Gone;
            FindViewById<LinearLayout>(Resource.Id.InitSetting_Accessibility_ExplainLayout).Visibility = ViewStates.Visible;

            btAccessibilityCheck.Enabled = true;

            cvAccessibility.Click -= CvAccessibility_Click;
        }

        private async Task ShowFirstCard()
        {
            await Task.Delay(500);

            TransitionManager.BeginDelayedTransition(cvGrantPermission);

            cvGrantPermission.Visibility = ViewStates.Visible;
        }

        private async Task ShowSecondCard()
        {
            await Task.Delay(500);

            TransitionManager.BeginDelayedTransition(cvGrantPermission);

            FindViewById<LinearLayout>(Resource.Id.InitSetting_GrantPermission_ExplainLayout).Visibility = ViewStates.Gone;
            btGrantCheck.Enabled = false;

            await Task.Delay(500);

            tvGrantSummary.Visibility = ViewStates.Visible;
            tvGrantSummary.SetText(Resource.String.Common_CheckSuccess);
            cvGrantPermission.Enabled = false;

            await Task.Delay(500);

            TransitionManager.BeginDelayedTransition(cvAccessibility);

            cvAccessibility.Visibility = ViewStates.Visible;
        }

        private async Task FinishSetting()
        {
            TransitionManager.BeginDelayedTransition(cvAccessibility);

            await Task.Delay(500);

            FindViewById<LinearLayout>(Resource.Id.InitSetting_Accessibility_ExplainLayout).Visibility = ViewStates.Gone;
            btAccessibilityCheck.Enabled = false;

            await Task.Delay(500);

            tvAccessibilitySummary.Visibility = ViewStates.Visible;
            tvAccessibilitySummary.SetText(Resource.String.Common_CheckSuccess);
            cvAccessibility.Enabled = false;

            await Task.Delay(500);

            TransitionManager.BeginDelayedTransition(cvGrantPermission);

            cvGrantPermission.Visibility = ViewStates.Visible;

            await Task.Delay(1000);

            TransitionManager.BeginDelayedTransition(FindViewById<RelativeLayout>(Resource.Id.InitSettingMainLayout));

            cvGrantPermission.Visibility = ViewStates.Gone;
            cvAccessibility.Visibility = ViewStates.Gone;

            FindViewById<TextView>(Resource.Id.InitSettingTitleText).LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            FindViewById<TextView>(Resource.Id.InitSettingTitleText).Gravity = GravityFlags.Center;
            FindViewById<TextView>(Resource.Id.InitSettingTitleText).SetText(Resource.String.Common_CheckSuccess);

            await Task.Delay(1000);

            ETC.sharedPreferences.Edit().PutBoolean("HasRestart", false).Apply();
            StartActivity(typeof(MainActivity));
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            FinishAffinity();

            /*if (!exitTimer.Enabled)
            {
                exitTimer.Start();
                ETC.ShowSnackbar(snackbarLayout, Resource.String.Main_CheckExit, Snackbar.LengthLong, Android.Graphics.Color.DarkOrange);
            }
            else
            {
                FinishAffinity();
                OverridePendingTransition(Resource.Animation.Activity_SlideInLeft, Resource.Animation.Activity_SlideOutRight);
                Process.KillProcess(Process.MyPid());
            }*/
        }
    }
}