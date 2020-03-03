using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Android.Support.V7.Preferences;
using Android.Support.V14.Preferences;

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

            // Button Part

            var buttonTest = FindPreference("TestButtonClick");
            buttonTest.PreferenceClick += delegate { Activity.StartActivity(typeof(ButtonTestActivity)); };

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

            /*var viewRecordingFiles = FindPreference("ViewRecordingFiles");
            viewRecordingFiles.PreferenceClick += async delegate
            {
                var task = await Launcher.TryOpenAsync($"file://{Activity.GetExternalFilesDir(null).AbsolutePath}");

                Toast.MakeText(Activity, task.ToString(), ToastLength.Short).Show();
            };*/
        }
    }
}