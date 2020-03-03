using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V14.Preferences;
using Android.Support.V7.Preferences;
using Android.Views;
using Android.Widget;
using Android.Provider;
using Android.Content.PM;

namespace GAButtonMapper
{
    internal class ButtonSubFragment : PreferenceFragmentCompat
    {
        private ISharedPreferencesEditor editor;

        private Preference logCounting;
        private Preference clickInterval;
        private Preference longClickInterval;

        private Preference[] appSelectorPs;
        private ListPreference[] actionSelectorPs;
        readonly string[] clickType =
        {
            "SingleClick",
            "DoubleClick",
            "TripleClick",
            "SingleLongClick",
            "DoubleLongClick",
            "TripleLongClick"
        };

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.ButtonMenus);

            logCounting = FindPreference("LogCounting");
            clickInterval = FindPreference("ClickInterval");
            longClickInterval = FindPreference("LongClickInterval");

            appSelectorPs = new Preference[]
            {
                FindPreference("AppSelector_SingleClick"),
                FindPreference("AppSelector_DoubleClick"),
                FindPreference("AppSelector_TripleClick"),
                FindPreference("AppSelector_SingleLongClick"),
                FindPreference("AppSelector_DoubleLongClick"),
                FindPreference("AppSelector_TripleLongClick")
            };
            actionSelectorPs = new ListPreference[]
            {
                FindPreference("ActionSelector_SingleClick") as ListPreference,
                FindPreference("ActionSelector_DoubleClick") as ListPreference,
                FindPreference("ActionSelector_TripleClick") as ListPreference,
                FindPreference("ActionSelector_SingleLongClick") as ListPreference,
                FindPreference("ActionSelector_DoubleLongClick") as ListPreference,
                FindPreference("ActionSelector_TripleLongClick") as ListPreference
            };

            InitMainMenus();
        }

        public override void OnResume()
        {
            base.OnResume();

            logCounting.Summary =
                 $"{Resources.GetString(Resource.String.MainMenu_LogCounting_Summary)} {ETC.loggingCount}";
            clickInterval.Summary =
                $"{Resources.GetString(Resource.String.MainMenu_ButtonInterval_Summary)} {ETC.clickInterval}ms";
            longClickInterval.Summary =
                $"{Resources.GetString(Resource.String.MainMenu_LongClickInterval_Summary)} {ETC.longClickInterval}ms";

            for (int i = 0; i < clickType.Length; ++i)
            {
                string pkName = ETC.sharedPreferences.GetString($"AppSelector_{clickType[i]}", "");

                if (!string.IsNullOrWhiteSpace(pkName))
                {
                    try
                    {
                        appSelectorPs[i].Summary =
                            $"{Resources.GetString(Resource.String.MainMenu_Detail_AppSelector_Summary_NowApp)} : {ETC.packm.GetApplicationInfo(pkName, 0).LoadLabel(ETC.packm)} ({pkName})";
                    }
                    catch (Exception)
                    {
                        ETC.sharedPreferences.Edit().PutString($"AppSelector_{clickType[i]}", "").Apply();
                    }
                }
            }
        }

        private void InitMainMenus()
        {
            editor = ETC.sharedPreferences.Edit();

            // Click Timing

            logCounting.PreferenceClick += delegate
            {
                var view = Activity.LayoutInflater.Inflate(Resource.Layout.NumberPickerDialogLayout, null);

                var np = view.FindViewById<NumberPicker>(Resource.Id.NumberPickerControl);
                np.MaxValue = 400;
                np.MinValue = 10;
                np.Value = ETC.sharedPreferences.GetInt("LogCounting", 80);

                var ad = new Android.Support.V7.App.AlertDialog.Builder(Activity);
                ad.SetTitle(Resource.String.MainMenu_ButtonInterval_Title);
                ad.SetCancelable(true);
                ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { });
                ad.SetNeutralButton(Resource.String.AlertDialog_Reset, delegate
                {
                    editor.PutInt("LogCounting", 80);
                    editor.Apply();

                    ETC.loggingCount = ETC.sharedPreferences.GetInt("LogCounting", 80);
                    logCounting.Summary =
                        $"{Resources.GetString(Resource.String.MainMenu_LogCounting_Summary)} {ETC.loggingCount}";
                });
                ad.SetPositiveButton(Resource.String.AlertDialog_Set, delegate
                {
                    editor.PutInt("LogCounting", np.Value);
                    editor.Apply();

                    ETC.loggingCount = ETC.sharedPreferences.GetInt("LogCounting", 80);
                    logCounting.Summary =
                        $"{Resources.GetString(Resource.String.MainMenu_LogCounting_Summary)} {ETC.loggingCount}";

                });
                ad.SetView(view);

                ad.Show();
            };

            clickInterval.PreferenceClick += delegate
            {
                var view = Activity.LayoutInflater.Inflate(Resource.Layout.NumberPickerDialogLayout, null);

                var np = view.FindViewById<NumberPicker>(Resource.Id.NumberPickerControl);
                np.MaxValue = 8;
                np.MinValue = 0;
                np.Value = ETC.sharedPreferences.GetInt("ClickInterval", 0);

                string[] values = new string[np.MaxValue - np.MinValue + 1];

                for (int i = 0; i < values.Length; ++i)
                {
                    values[i] = ETC.CalcInterval(400, 50, i).ToString();
                }

                np.SetDisplayedValues(values);

                var ad = new Android.Support.V7.App.AlertDialog.Builder(Activity);
                ad.SetTitle(Resource.String.MainMenu_ButtonInterval_Title);
                ad.SetCancelable(true);
                ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { });
                ad.SetNeutralButton(Resource.String.AlertDialog_Reset, delegate
                {
                    editor.PutInt("ClickInterval", 0);
                    editor.Apply();

                    ETC.clickInterval = ETC.CalcInterval(400, 50, ETC.sharedPreferences.GetInt("ClickInterval", 0));
                    clickInterval.Summary =
                        $"{Resources.GetString(Resource.String.MainMenu_ButtonInterval_Summary)} {ETC.clickInterval}ms";
                });
                ad.SetPositiveButton(Resource.String.AlertDialog_Set, delegate
                {
                    editor.PutInt("ClickInterval", np.Value);
                    editor.Apply();

                    ETC.clickInterval = ETC.CalcInterval(400, 50, ETC.sharedPreferences.GetInt("ClickInterval", 0));
                    clickInterval.Summary =
                        $"{Resources.GetString(Resource.String.MainMenu_ButtonInterval_Summary)} {ETC.clickInterval}ms";

                });
                ad.SetView(view);

                ad.Show();
            };

            longClickInterval.PreferenceClick += delegate
            {
                var view = Activity.LayoutInflater.Inflate(Resource.Layout.NumberPickerDialogLayout, null);

                var np = view.FindViewById<NumberPicker>(Resource.Id.NumberPickerControl);
                np.MaxValue = 8;
                np.MinValue = 0;
                np.Value = ETC.sharedPreferences.GetInt("longClickInterval", 0);

                string[] values = new string[np.MaxValue - np.MinValue + 1];

                for (int i = 0; i < values.Length; ++i)
                {
                    values[i] = ETC.CalcInterval(800, 50, i).ToString();
                }

                np.SetDisplayedValues(values);

                var ad = new Android.Support.V7.App.AlertDialog.Builder(Activity);
                ad.SetTitle(Resource.String.MainMenu_ButtonInterval_Title);
                ad.SetCancelable(true);
                ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { });
                ad.SetNeutralButton(Resource.String.AlertDialog_Reset, delegate
                {
                    editor.PutInt("longClickInterval", 0);
                    editor.Apply();

                    ETC.longClickInterval = ETC.CalcInterval(800, 50, ETC.sharedPreferences.GetInt("longClickInterval", 0));
                    longClickInterval.Summary =
                        $"{Resources.GetString(Resource.String.MainMenu_LongClickInterval_Summary)} {ETC.longClickInterval}ms";
                });
                ad.SetPositiveButton(Resource.String.AlertDialog_Set, delegate
                {
                    editor.PutInt("longClickInterval", np.Value);
                    editor.Apply();

                    ETC.longClickInterval = ETC.CalcInterval(800, 50, ETC.sharedPreferences.GetInt("longClickInterval", 0));
                    longClickInterval.Summary =
                        $"{Resources.GetString(Resource.String.MainMenu_LongClickInterval_Summary)} {ETC.longClickInterval}ms";
                });
                ad.SetView(view);

                ad.Show();
            };

            // Button Mapping

            for (int i = 0; i < clickType.Length; ++i)
            {
                string type = clickType[i];

                var enableP = FindPreference($"Enable{type}") as SwitchPreference;
                enableP.Checked = ETC.sharedPreferences.GetBoolean($"Enable{type}", false);
                enableP.PreferenceChange += delegate { editor.PutBoolean($"Enable{type}", enableP.Checked).Apply(); };

                var mappingTypeP = FindPreference($"MappingType_{type}") as ListPreference;
                mappingTypeP.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString($"MappingType_{type}", "0")));
                mappingTypeP.PreferenceChange += (sender, e) =>
                {
                    int index = 0;
                    var lp = sender as ListPreference;

                    for (index = 0; index < clickType.Length; ++index)
                    {
                        if (lp.Key == $"MappingType_{clickType[index]}")
                        {
                            break;
                        }
                    }

                    switch (int.Parse((string)e.NewValue))
                    {
                        case 0:
                            appSelectorPs[index].Visible = false;
                            actionSelectorPs[index].Visible = true;
                            break;
                        case 1:
                            appSelectorPs[index].Visible = true;
                            actionSelectorPs[index].Visible = false;
                            break;
                        default:
                            appSelectorPs[index].Visible = false;
                            actionSelectorPs[index].Visible = false;
                            break;
                    }
                };

                if (int.Parse(ETC.sharedPreferences.GetString($"MappingType_{type}", "0")) == 1)
                {
                    appSelectorPs[i].Visible = true;
                    actionSelectorPs[i].Visible = false;
                }
                else if (int.Parse(ETC.sharedPreferences.GetString($"MappingType_{type}", "0")) == 0)
                {
                    appSelectorPs[i].Visible = false;
                    actionSelectorPs[i].Visible = true;
                }

                appSelectorPs[i].PreferenceClick += (sender, e) =>
                {
                    int index = 0;
                    var p = sender as Preference;

                    for (index = 0; index < clickType.Length; ++index)
                    {
                        if (p.Key == $"AppSelector_{clickType[index]}")
                        {
                            break;
                        }
                    }

                    var intent = new Intent(Activity, typeof(AppSelectorActivity));
                    intent.PutExtra("Type", clickType[index]);
                    Activity.StartActivity(intent);
                };

                actionSelectorPs[i].SetValueIndex(int.Parse(ETC.sharedPreferences.GetString($"ActionSelector_{type}", "0")));
                actionSelectorPs[i].PreferenceChange += CheckActionSelector;
            }
        }

        private void CheckActionSelector(object sender, Preference.PreferenceChangeEventArgs e)
        {
            int index = 0;
            var lp = sender as ListPreference;

            for (index = 0; index < clickType.Length; ++index)
            {
                if (lp.Key == $"ActionSelector_{clickType[index]}")
                {
                    break;
                }
            }

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
                        ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { editor.PutString($"ActionSelector_{clickType[index]}", "0").Apply(); });
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
                        ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { editor.PutString($"ActionSelector_{clickType[index]}", "0").Apply(); });
                        ad.SetCancelable(false);

                        ad.Show();
                    }
                    break;
                case "18":
                    if (!Settings.System.CanWrite(Activity))
                    {
                        var ad = new Android.Support.V7.App.AlertDialog.Builder(Activity);
                        ad.SetTitle(Resource.String.AlertDialog_WriteSettingPermission_Title);
                        ad.SetMessage(Resource.String.AlertDialog_WriteSettingPermission_Message);
                        ad.SetPositiveButton(Resource.String.AlertDialog_WriteSettingPermission_OK, delegate { StartActivity(new Intent(Settings.ActionManageWriteSettings)); });
                        ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { editor.PutString($"ActionSelector_{clickType[index]}", "0").Apply(); });
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