
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Provider;
using System;

namespace GAButtonMapper
{
    [Activity(Label = "MainActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private readonly string shortcutAIOpenAssistant = "shortcut_ai_key_open_assistant";
        private readonly string shortcutAIOpenLens = "shortcut_ai_key_open_lens";
        private readonly string shortcutAITalkToAssistant = "shortcut_ai_key_talk_to_assistant";

        TextView welcomeTextView;
        CardView aiShortcutDisableCardView;
        TextView aiShortcutDisableSummaryTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.MainLayout);

            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.MainToolbar));
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDisplayUseLogoEnabled(true);
            SupportActionBar.SetLogo(Resource.Mipmap.ic_launcher);

            welcomeTextView = FindViewById<TextView>(Resource.Id.MainWelcomeText);
            aiShortcutDisableCardView = FindViewById<CardView>(Resource.Id.MainDisableAIShortcutCardView);
            aiShortcutDisableSummaryTextView = FindViewById<TextView>(Resource.Id.MainDisableAIShortcutSummaryText);

            FindViewById<CardView>(Resource.Id.MainAppUsageCautionCardView).Click += delegate { StartActivity(typeof(AppUsageCautionActivity)); };
            aiShortcutDisableCardView.Click += AiShortcutDisableCardView_Click;
            FindViewById<CardView>(Resource.Id.MainSettingEnterCardView).Click += delegate { StartActivity(typeof(SettingActivity)); };
        }

        protected override void OnResume()
        {
            base.OnResume();

            welcomeTextView.SetText(ETC.sharedPreferences.GetBoolean("EnableMapping", false) ? Resource.String.Main_Welcome_On : Resource.String.Main_Welcome_Off);

            try
            {
                if ((Settings.Global.GetInt(ContentResolver, shortcutAIOpenAssistant) == 0) &&
                    (Settings.Global.GetInt(ContentResolver, shortcutAIOpenLens) == 0) &&
                    (Settings.Global.GetInt(ContentResolver, shortcutAITalkToAssistant) == 0))
                {
                    aiShortcutDisableSummaryTextView.SetText(Resource.String.Main_DisableAIShortcut_Summary_Complete);
                    aiShortcutDisableCardView.Enabled = false;
                }
                else
                {
                    aiShortcutDisableSummaryTextView.SetText(Resource.String.Main_DisableAIShortcut_Summary);
                    aiShortcutDisableCardView.Enabled = true;
                }
            }
            catch (Exception)
            {
                aiShortcutDisableSummaryTextView.SetText(Resource.String.Main_DisableAIShortcut_Summary_Unable);
                aiShortcutDisableCardView.Enabled = false;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainToolbarMenu, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item?.ItemId)
            {
                case Resource.Id.MainAppInfo:
                    StartActivity(typeof(AppInfoActivity));
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void AiShortcutDisableCardView_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.Global.PutInt(ContentResolver, shortcutAIOpenAssistant, 0);
                Settings.Global.PutInt(ContentResolver, shortcutAIOpenLens, 0);
                Settings.Global.PutInt(ContentResolver, shortcutAITalkToAssistant, 0);

                OnResume();
            }
            catch (Exception)
            {
                aiShortcutDisableSummaryTextView.SetText(Resource.String.Main_DisableAIShortcut_Summary_Unable);
                aiShortcutDisableCardView.Enabled = false;
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            FinishAffinity();
        }
    }
}