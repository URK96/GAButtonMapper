using Android.Content;
using Android.OS;
using Android.Provider;

using AndroidX.Preference;

namespace GAButtonMapper
{
    internal class MainFragment : PreferenceFragmentCompat
    {
        private ISharedPreferencesEditor editor;

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

            if (!ETC.acm.IsEnabled)
            {
                ETC.sharedPreferences.Edit().PutBoolean("EnableMapping", false).Apply();
                goAccessibilitySettingP.SetSummary(Resource.String.MainMenu_ETC_GoAccessibilitySetting_Summary_Off);
            }
            else
            {
                goAccessibilitySettingP.SetSummary(Resource.String.MainMenu_ETC_GoAccessibilitySetting_Summary_On);
            }

            goIgnoreBatteryOptimizationSettingP.Summary = ETC.pm.IsIgnoringBatteryOptimizations(Activity.PackageName) ?
                $"{Resources.GetString(Resource.String.MainMenu_ETC_GoIgnoreBatteryOptimizationSetting_Summary)}\n{Resources.GetString(Resource.String.MainMenu_ETC_GoIgnoreBatteryOptimizationSetting_Summary_On)}" :
                $"{Resources.GetString(Resource.String.MainMenu_ETC_GoIgnoreBatteryOptimizationSetting_Summary)}\n{Resources.GetString(Resource.String.MainMenu_ETC_GoIgnoreBatteryOptimizationSetting_Summary_Off)}";
        }

        private void InitMainMenus()
        {
            editor = ETC.sharedPreferences.Edit();

            goAccessibilitySettingP = FindPreference("GoAccessibilitySetting");
            goIgnoreBatteryOptimizationSettingP = FindPreference("GoIgnoreBatteryOptimizationSetting");


            // Basic Part

            var enableMapping = FindPreference("EnableMapping") as SwitchPreference;

            if (!ETC.acm.IsEnabled)
            {
                ETC.sharedPreferences.Edit().PutBoolean("EnableMapping", false).Apply();
            }

            enableMapping.Checked = ETC.sharedPreferences.GetBoolean("EnableMapping", false);
            enableMapping.PreferenceChange += (sender, e) =>
            {
                if (ETC.acm.IsEnabled)
                {
                    var value = (bool)e.NewValue;

                    editor.PutBoolean("EnableMapping", value).Apply();

                    ETC.isMappingEnable = value;

                    if (value && !ETC.isRun)
                    {
                        ETC.monitoringMethod?.Invoke();
                    }
                }
                else
                {
                    var ad = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
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
            screenOffDiableMapping.PreferenceChange += (sender, e) =>
            {
                var value = (bool)e.NewValue;

                editor.PutBoolean("ScreenOffDisableMapping", value).Apply();

                ETC.isScreenOffMappingEnable = value;
            };

            var longClickVibrator = FindPreference("LongClickVibrator") as SwitchPreference;
            longClickVibrator.Checked = ETC.sharedPreferences.GetBoolean("LongClickVibrator", true);
            longClickVibrator.PreferenceChange += (sender, e) =>
            {
                var value = (bool)e.NewValue;

                editor.PutBoolean("LongClickVibrator", value).Apply();

                ETC.isLongClickVibrate = value;
            };

            var actionFeatureVibrator = FindPreference("ActionFeatureVibrator") as SwitchPreference;
            actionFeatureVibrator.Checked = ETC.sharedPreferences.GetBoolean("ActionFeatureVibrator", true);
            actionFeatureVibrator.PreferenceChange += (sender, e) =>
            {
                editor.PutBoolean("ActionFeatureVibrator", (bool)e.NewValue).Apply();
            };

            var screenOnOffToastMessageEnable = FindPreference("ScreenOnOffToastMessageEnable") as SwitchPreference;
            screenOnOffToastMessageEnable.Checked = ETC.sharedPreferences.GetBoolean("ScreenOnOffToastMessageEnable", true);
            screenOnOffToastMessageEnable.PreferenceChange += (sender, e) =>
            {
                var value = (bool)e.NewValue;

                ETC.isScreenOnOffToastMessageEnable = value;

                editor.PutBoolean("ScreenOnOffToastMessageEnable", value).Apply();
            };

            // Button Part

            var buttonTest = FindPreference("TestButtonClick");
            buttonTest.PreferenceClick += delegate { Activity.StartActivity(typeof(ButtonTestActivity)); };

            // ETC Part

            goAccessibilitySettingP.PreferenceClick += delegate { StartActivity(new Intent(Settings.ActionAccessibilitySettings)); };

            goIgnoreBatteryOptimizationSettingP.PreferenceClick += delegate
            {
                var ad = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
                ad.SetTitle(Resource.String.AlertDialog_IgnoreBatteryOptimization_Title);
                ad.SetMessage(Resource.String.AlertDialog_IgnoreBatteryOptimization_Message);
                ad.SetPositiveButton(Resource.String.AlertDialog_IgnoreBatteryOptimization_OK, delegate { StartActivity(new Intent(Settings.ActionIgnoreBatteryOptimizationSettings)); });
                ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { });
                ad.SetCancelable(false);

                ad.Show();
            };

            /*var viewRecordingFiles = FindPreference("ViewRecordingFiles");
            viewRecordingFiles.PreferenceClick += delegate
            {
                try
                {
                    var intent = new Intent();
                    intent.SetAction(Intent.ActionView);
                    intent.SetDataAndType(Android.Net.Uri.Parse("file://" + Activity.GetExternalFilesDir(null).AbsolutePath), "resource/folder");
                    StartActivity(Intent.CreateChooser(intent, "Open Folder"));
                }
                catch (Exception ex)
                {
                    Toast.MakeText(Activity, ex.ToString(), ToastLength.Long).Show();
                }
            };*/
        }
    }
}