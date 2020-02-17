using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V14.Preferences;
using Android.Provider;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Views;

using System;
using Android;
using Android.Content.PM;
using Android.Widget;

namespace GAButtonMapper
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MainLayout);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.MainToolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDisplayUseLogoEnabled(true);
            SupportActionBar.SetLogo(Resource.Mipmap.ic_launcher);

            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.MainFragmentContainer, new MainFragment(), null).Commit();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainToolbarMenu, menu);
                
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item?.ItemId)
            {
                case Resource.Id.MainAppInfo:
                    StartActivity(typeof(AppInfoActivity));
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            FinishAffinity();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}

    internal class MainFragment : PreferenceFragmentCompat
    {
        private ISharedPreferencesEditor editor;

        private Preference appSelectorSingleP;
        private ListPreference actionSelectorSingleP;
        private Preference appSelectorDoubleP;
        private ListPreference actionSelectorDoubleP;
        private Preference appSelectorTripleP;
        private ListPreference actionSelectorTripleP;
        private Preference appSelectorSingleLongP;
        private ListPreference actionSelectorSingleLongP;
        private Preference appSelectorDoubleLongP;
        private ListPreference actionSelectorDoubleLongP;
        private Preference appSelectorTripleLongP;
        private ListPreference actionSelectorTripleLongP;

        private Preference goAccessibilitySettingP;
        private Preference goIgnoreBatteryOptimizationSettingP;

        private bool isAccessbilityEnable = false;

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.MainMenus);

            isAccessbilityEnable = ETC.acm.IsEnabled;

            InitMainMenus();
        }

        public override void OnResume()
        {
            base.OnResume();

            string pkNameSingle = ETC.sharedPreferences.GetString("AppSelector_SingleClick", "");
            if (!string.IsNullOrWhiteSpace(pkNameSingle))
            {
                try
                {
                    appSelectorSingleP.Summary = 
                        $"{Resources.GetString(Resource.String.MainMenu_Detail_AppSelector_Summary_NowApp)} : {ETC.packm.GetApplicationInfo(pkNameSingle, 0).LoadLabel(ETC.packm)} ({pkNameSingle})";
                }
                catch (Exception)
                {
                    ETC.sharedPreferences.Edit().PutString("AppSelector", "").Apply();
                }
            }

            string pkNameDouble = ETC.sharedPreferences.GetString("AppSelector_DoubleClick", "");
            if (!string.IsNullOrWhiteSpace(pkNameDouble))
            {
                try
                {
                    appSelectorDoubleP.Summary =
                        $"{Resources.GetString(Resource.String.MainMenu_Detail_AppSelector_Summary_NowApp)} : {ETC.packm.GetApplicationInfo(pkNameDouble, 0).LoadLabel(ETC.packm)} ({pkNameDouble})";
                }
                catch (Exception)
                {
                    ETC.sharedPreferences.Edit().PutString("AppSelector", "").Apply();
                }
            }

            if (!ETC.acm.IsEnabled)
            {
                ETC.sharedPreferences.Edit().PutBoolean("EnableMapping", false).Apply();
                goAccessibilitySettingP.SetSummary(Resource.String.MainMenu_ETC_GoAccessibilitySetting_Summary_Off);
            }
            else
            {
                goAccessibilitySettingP.SetSummary(Resource.String.MainMenu_ETC_GoAccessibilitySetting_Summary_On);
            }

            if (ETC.pm.IsIgnoringBatteryOptimizations(Activity.PackageName))
            {
                goIgnoreBatteryOptimizationSettingP.Summary = 
                    $"{Resources.GetString(Resource.String.MainMenu_ETC_GoIgnoreBatteryOptimizationSetting_Summary)}\n{Resources.GetString(Resource.String.MainMenu_ETC_GoIgnoreBatteryOptimizationSetting_Summary_On)}";
            }
            else
            {
                goIgnoreBatteryOptimizationSettingP.Summary =
                    $"{Resources.GetString(Resource.String.MainMenu_ETC_GoIgnoreBatteryOptimizationSetting_Summary)}\n{Resources.GetString(Resource.String.MainMenu_ETC_GoIgnoreBatteryOptimizationSetting_Summary_Off)}";
            }
        }

        private void InitMainMenus()
        {
            editor = ETC.sharedPreferences.Edit();

            appSelectorSingleP = FindPreference("AppSelector_SingleClick");
            actionSelectorSingleP = FindPreference("ActionSelector_SingleClick") as ListPreference;
            appSelectorDoubleP = FindPreference("AppSelector_DoubleClick");
            actionSelectorDoubleP = FindPreference("ActionSelector_DoubleClick") as ListPreference;
            appSelectorTripleP = FindPreference("AppSelector_TripleClick");
            actionSelectorTripleP = FindPreference("ActionSelector_TripleClick") as ListPreference;
            appSelectorSingleLongP = FindPreference("AppSelector_SingleLongClick");
            actionSelectorSingleLongP = FindPreference("ActionSelector_SingleLongClick") as ListPreference;
            appSelectorDoubleLongP = FindPreference("AppSelector_DoubleLongClick");
            actionSelectorDoubleLongP = FindPreference("ActionSelector_DoubleLongClick") as ListPreference;
            appSelectorTripleLongP = FindPreference("AppSelector_TripleLongClick");
            actionSelectorTripleLongP = FindPreference("ActionSelector_TripleLongClick") as ListPreference;

            goAccessibilitySettingP = FindPreference("GoAccessibilitySetting");
            goIgnoreBatteryOptimizationSettingP = FindPreference("GoIgnoreBatteryOptimizationSetting");


            // Basic Part

            var enableMapping = FindPreference("EnableMapping") as SwitchPreference;
            if (!ETC.acm.IsEnabled)
            {
                ETC.sharedPreferences.Edit().PutBoolean("EnableMapping", false).Apply();
            }
            enableMapping.Checked = ETC.sharedPreferences.GetBoolean("EnableMapping", false);
            enableMapping.PreferenceChange += delegate 
            { 
                if (ETC.acm.IsEnabled)
                {
                    editor.PutBoolean("EnableMapping", enableMapping.Checked).Apply();
                }
                else
                {
                    var ad = new Android.Support.V7.App.AlertDialog.Builder(Activity);
                    ad.SetTitle(Resource.String.AlertDialog_Accessibility_Title);
                    ad.SetMessage(Resource.String.AlertDialog_Accessibility_Message);
                    ad.SetPositiveButton(Resource.String.AlertDialog_Accessibility_OK, delegate 
                    { 
                        StartActivity(new Intent(Settings.ActionAccessibilitySettings));
                        enableMapping.Checked = false;
                    });
                    ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { enableMapping.Checked = false; });
                    ad.SetCancelable(false);

                    ad.Show();
                }
            };

            var screenOffDiableMapping = FindPreference("ScreenOffDisableMapping") as SwitchPreference;
            screenOffDiableMapping.Checked = ETC.sharedPreferences.GetBoolean("ScreenOffDisableMapping", false);
            screenOffDiableMapping.PreferenceChange += delegate 
            {
                editor.PutBoolean("ScreenOffDisableMapping", screenOffDiableMapping.Checked);
            };

            var longClickVibrator = FindPreference("LongClickVibrator") as SwitchPreference;
            longClickVibrator.Checked = ETC.sharedPreferences.GetBoolean("LongClickVibrator", true);
            longClickVibrator.PreferenceChange += delegate
            {
                editor.PutBoolean("LongClickVibrator", longClickVibrator.Checked);
            };

            var actionFeatureVibrator = FindPreference("ActionFeatureVibrator") as SwitchPreference;
            actionFeatureVibrator.Checked = ETC.sharedPreferences.GetBoolean("ActionFeatureVibrator", true);
            actionFeatureVibrator.PreferenceChange += delegate
            {
                editor.PutBoolean("ActionFeatureVibrator", actionFeatureVibrator.Checked);
            };

            // Single Click Part

            var enableSingleClick = FindPreference("EnableSingleClick") as SwitchPreference;
            enableSingleClick.Checked = ETC.sharedPreferences.GetBoolean("EnableSingleClick", false);
            enableSingleClick.PreferenceChange += delegate { editor.PutBoolean("EnableSingleClick", enableSingleClick.Checked).Apply(); };

            var mappingTypeSingle = FindPreference("MappingType_SingleClick") as ListPreference;
            mappingTypeSingle.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("MappingType_SingleClick", "0")));
            mappingTypeSingle.PreferenceChange += (sender, e) =>
            {
                switch (int.Parse((string)e.NewValue))
                {
                    case 0:
                        appSelectorSingleP.Visible = false;
                        actionSelectorSingleP.Visible = true;
                        break;
                    case 1:
                        appSelectorSingleP.Visible = true;
                        actionSelectorSingleP.Visible = false;
                        break;
                    default:
                        appSelectorSingleP.Visible = false;
                        actionSelectorSingleP.Visible = false;
                        break;
                }
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_SingleClick", "0")) == 1)
            {
                appSelectorSingleP.Visible = true;
                actionSelectorSingleP.Visible = false;
            }
            appSelectorSingleP.PreferenceClick += delegate 
            {
                var intentSingle = new Intent(Activity, typeof(AppSelectorActivity));
                intentSingle.PutExtra("Type", "SingleClick");
                Activity.StartActivity(intentSingle); 
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_SingleClick", "0")) == 0)
            {
                appSelectorSingleP.Visible = false;
                actionSelectorSingleP.Visible = true;
            }
            actionSelectorSingleP.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("ActionSelector_SingleClick", "0")));
            actionSelectorSingleP.PreferenceChange += CheckActionSelector;


            // Double Click Part

            var enableDoubleClick = FindPreference("EnableDoubleClick") as SwitchPreference;
            enableDoubleClick.Checked = ETC.sharedPreferences.GetBoolean("EnableDoubleClick", false);
            enableDoubleClick.PreferenceChange += delegate { editor.PutBoolean("EnableDoubleClick", enableDoubleClick.Checked).Apply(); };

            var mappingTypeDouble = FindPreference("MappingType_DoubleClick") as ListPreference;
            mappingTypeDouble.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("MappingType_DoubleClick", "0")));
            mappingTypeDouble.PreferenceChange += (sender, e) =>
            {
                switch (int.Parse((string)e.NewValue))
                {
                    case 0:
                        appSelectorDoubleP.Visible = false;
                        actionSelectorDoubleP.Visible = true;
                        break;
                    case 1:
                        appSelectorDoubleP.Visible = true;
                        actionSelectorDoubleP.Visible = false;
                        break;
                    default:
                        appSelectorDoubleP.Visible = false;
                        actionSelectorDoubleP.Visible = false;
                        break;
                }
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_DoubleClick", "0")) == 1)
            {
                appSelectorDoubleP.Visible = true;
                actionSelectorDoubleP.Visible = false;
            }
            appSelectorDoubleP.PreferenceClick += delegate
            {
                var intentDouble = new Intent(Activity, typeof(AppSelectorActivity));
                intentDouble.PutExtra("Type", "DoubleClick");
                Activity.StartActivity(intentDouble);
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_DoubleClick", "0")) == 0)
            {
                appSelectorDoubleP.Visible = false;
                actionSelectorDoubleP.Visible = true;
            }
            actionSelectorDoubleP.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("ActionSelector_DoubleClick", "0")));
            actionSelectorDoubleP.PreferenceChange += CheckActionSelector;


            // Triple Click Part

            var enableTripleClick = FindPreference("EnableTripleClick") as SwitchPreference;
            enableTripleClick.Checked = ETC.sharedPreferences.GetBoolean("EnableTripleClick", false);
            enableTripleClick.PreferenceChange += delegate { editor.PutBoolean("EnableTripleClick", enableTripleClick.Checked).Apply(); };

            var mappingTypeTriple = FindPreference("MappingType_TripleClick") as ListPreference;
            mappingTypeTriple.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("MappingType_TripleClick", "0")));
            mappingTypeTriple.PreferenceChange += (sender, e) =>
            {
                switch (int.Parse((string)e.NewValue))
                {
                    case 0:
                        appSelectorTripleP.Visible = false;
                        actionSelectorTripleP.Visible = true;
                        break;
                    case 1:
                        appSelectorTripleP.Visible = true;
                        actionSelectorTripleP.Visible = false;
                        break;
                    default:
                        appSelectorTripleP.Visible = false;
                        actionSelectorTripleP.Visible = false;
                        break;
                }
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_TripleClick", "0")) == 1)
            {
                appSelectorTripleP.Visible = true;
                actionSelectorTripleP.Visible = false;
            }
            appSelectorTripleP.PreferenceClick += delegate
            {
                var intentTriple = new Intent(Activity, typeof(AppSelectorActivity));
                intentTriple.PutExtra("Type", "TripleClick");
                Activity.StartActivity(intentTriple);
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_TripleClick", "0")) == 0)
            {
                appSelectorTripleP.Visible = false;
                actionSelectorTripleP.Visible = true;
            }
            actionSelectorTripleP.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("ActionSelector_TripleClick", "0")));
            actionSelectorTripleP.PreferenceChange += CheckActionSelector;


            // SingleLong Click Part

            var enableSingleLongClick = FindPreference("EnableSingleLongClick") as SwitchPreference;
            enableSingleLongClick.Checked = ETC.sharedPreferences.GetBoolean("EnableSingleLongClick", false);
            enableSingleLongClick.PreferenceChange += delegate { editor.PutBoolean("EnableSingleLongClick", enableSingleLongClick.Checked).Apply(); };

            var mappingTypeSingleLong = FindPreference("MappingType_SingleLongClick") as ListPreference;
            mappingTypeSingleLong.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("MappingType_SingleLongClick", "0")));
            mappingTypeSingleLong.PreferenceChange += (sender, e) =>
            {
                switch (int.Parse((string)e.NewValue))
                {
                    case 0:
                        appSelectorSingleLongP.Visible = false;
                        actionSelectorSingleLongP.Visible = true;
                        break;
                    case 1:
                        appSelectorSingleLongP.Visible = true;
                        actionSelectorSingleLongP.Visible = false;
                        break;
                    default:
                        appSelectorSingleLongP.Visible = false;
                        actionSelectorSingleLongP.Visible = false;
                        break;
                }
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_SingleLongClick", "0")) == 1)
            {
                appSelectorSingleLongP.Visible = true;
                actionSelectorSingleLongP.Visible = false;
            }
            appSelectorSingleLongP.PreferenceClick += delegate
            {
                var intentSingleLong = new Intent(Activity, typeof(AppSelectorActivity));
                intentSingleLong.PutExtra("Type", "SingleLongClick");
                Activity.StartActivity(intentSingleLong);
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_SingleLongClick", "0")) == 0)
            {
                appSelectorSingleLongP.Visible = false;
                actionSelectorSingleLongP.Visible = true;
            }
            actionSelectorSingleLongP.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("ActionSelector_SingleLongClick", "0")));
            actionSelectorSingleLongP.PreferenceChange += CheckActionSelector;


            // DoubleLong Click Part

            var enableDoubleLongClick = FindPreference("EnableDoubleLongClick") as SwitchPreference;
            enableDoubleLongClick.Checked = ETC.sharedPreferences.GetBoolean("EnableDoubleLongClick", false);
            enableDoubleLongClick.PreferenceChange += delegate { editor.PutBoolean("EnableDoubleLongClick", enableDoubleLongClick.Checked).Apply(); };

            var mappingTypeDoubleLong = FindPreference("MappingType_DoubleLongClick") as ListPreference;
            mappingTypeDoubleLong.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("MappingType_DoubleLongClick", "0")));
            mappingTypeDoubleLong.PreferenceChange += (sender, e) =>
            {
                switch (int.Parse((string)e.NewValue))
                {
                    case 0:
                        appSelectorDoubleLongP.Visible = false;
                        actionSelectorDoubleLongP.Visible = true;
                        break;
                    case 1:
                        appSelectorDoubleLongP.Visible = true;
                        actionSelectorDoubleLongP.Visible = false;
                        break;
                    default:
                        appSelectorDoubleLongP.Visible = false;
                        actionSelectorDoubleLongP.Visible = false;
                        break;
                }
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_DoubleLongClick", "0")) == 1)
            {
                appSelectorDoubleLongP.Visible = true;
                actionSelectorDoubleLongP.Visible = false;
            }
            appSelectorDoubleLongP.PreferenceClick += delegate
            {
                var intentDoubleLong = new Intent(Activity, typeof(AppSelectorActivity));
                intentDoubleLong.PutExtra("Type", "DoubleLongClick");
                Activity.StartActivity(intentDoubleLong);
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_DoubleLongClick", "0")) == 0)
            {
                appSelectorDoubleLongP.Visible = false;
                actionSelectorDoubleLongP.Visible = true;
            }
            actionSelectorDoubleLongP.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("ActionSelector_DoubleLongClick", "0")));
            actionSelectorDoubleLongP.PreferenceChange += CheckActionSelector;


            // TripleLong Click Part

            var enableTripleLongClick = FindPreference("EnableTripleLongClick") as SwitchPreference;
            enableTripleLongClick.Checked = ETC.sharedPreferences.GetBoolean("EnableTripleLongClick", false);
            enableTripleLongClick.PreferenceChange += delegate { editor.PutBoolean("EnableTripleLongClick", enableTripleLongClick.Checked).Apply(); };

            var mappingTypeTripleLong = FindPreference("MappingType_TripleLongClick") as ListPreference;
            mappingTypeTripleLong.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("MappingType_TripleLongClick", "0")));
            mappingTypeTripleLong.PreferenceChange += (sender, e) =>
            {
                switch (int.Parse((string)e.NewValue))
                {
                    case 0:
                        appSelectorTripleLongP.Visible = false;
                        actionSelectorTripleLongP.Visible = true;
                        break;
                    case 1:
                        appSelectorTripleLongP.Visible = true;
                        actionSelectorTripleLongP.Visible = false;
                        break;
                    default:
                        appSelectorTripleLongP.Visible = false;
                        actionSelectorTripleLongP.Visible = false;
                        break;
                }
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_TripleLongClick", "0")) == 1)
            {
                appSelectorTripleLongP.Visible = true;
                actionSelectorTripleLongP.Visible = false;
            }
            appSelectorTripleLongP.PreferenceClick += delegate
            {
                var intentTripleLong = new Intent(Activity, typeof(AppSelectorActivity));
                intentTripleLong.PutExtra("Type", "TripleLongClick");
                Activity.StartActivity(intentTripleLong);
            };

            if (int.Parse(ETC.sharedPreferences.GetString("MappingType_TripleLongClick", "0")) == 0)
            {
                appSelectorTripleLongP.Visible = false;
                actionSelectorTripleLongP.Visible = true;
            }
            actionSelectorTripleLongP.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString("ActionSelector_TripleLongClick", "0")));
            actionSelectorTripleLongP.PreferenceChange += CheckActionSelector;


            // ETC Part

            goAccessibilitySettingP.PreferenceClick += delegate { StartActivity(new Intent(Settings.ActionAccessibilitySettings)); };

            goIgnoreBatteryOptimizationSettingP.PreferenceClick += delegate 
            {
                var ad = new Android.Support.V7.App.AlertDialog.Builder(Activity);
                ad.SetTitle(Resource.String.AlertDialog_IgnoreBatteryOptimization_Title);
                ad.SetMessage(Resource.String.AlertDialog_IgnoreBatteryOptimization_Message);
                ad.SetPositiveButton(Resource.String.AlertDialog_IgnoreBatteryOptimization_OK, delegate { StartActivity(new Intent(Settings.ActionIgnoreBatteryOptimizationSettings)); });
                ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { });
                ad.SetCancelable(false);

                ad.Show();
            };
        }

        private void CheckActionSelector(object sender, Preference.PreferenceChangeEventArgs e)
        {
            switch ((string)e.NewValue)
            {
                case "11":
                    if (ETC.nm == null)
                    {
                        ETC.nm = Activity.GetSystemService("notification") as NotificationManager;
                    }

                    if (!ETC.nm.IsNotificationPolicyAccessGranted)
                    {
                        var ad = new Android.Support.V7.App.AlertDialog.Builder(Activity);
                        ad.SetTitle(Resource.String.AlertDialog_DoNotDisturb_Title);
                        ad.SetMessage(Resource.String.AlertDialog_DoNotDisturb_Message);
                        ad.SetPositiveButton(Resource.String.AlertDialog_DoNotDisturb_OK, delegate { StartActivity(new Intent(Settings.ActionNotificationPolicyAccessSettings)); });
                        ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { editor.PutString("ActionSelector_SingleClick", "0").Apply(); });
                        ad.SetCancelable(false);

                        ad.Show();
                    }
                    break;
                case "16":
                    if (!ETC.CheckPermission(Activity, Manifest.Permission.RecordAudio))
                    {
                        var ad = new Android.Support.V7.App.AlertDialog.Builder(Activity);
                        ad.SetTitle(Resource.String.AlertDialog_AudioRecorderPermission_Title);
                        ad.SetMessage(Resource.String.AlertDialog_AudioRecorderPermission_Message);
                        ad.SetPositiveButton(Resource.String.AlertDialog_AudioRecorderPermission_OK, delegate { RequestPermissions(new string[] { Manifest.Permission.RecordAudio }, 0); });
                        ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { editor.PutString("ActionSelector_SingleClick", "0").Apply(); });
                        ad.SetCancelable(false);

                        ad.Show();
                    }
                    break;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (permissions[0])
            {
                case Manifest.Permission.RecordAudio:
                    if (grantResults[0] == Permission.Granted)
                    {
                        Toast.MakeText(Activity, Resource.String.Common_PermissionGranted, ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(Activity, Resource.String.Common_PermissionDenied, ToastLength.Short).Show();
                    }
                    break;
            }
        }
    }
}

