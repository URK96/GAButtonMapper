using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Widget;

using AndroidX.Preference;

using System;
using System.Text;

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
        private Preference[] urlSelectorPs;

        readonly string[] clickType =
        {
            "SingleClick",
            "DoubleClick",
            //"TripleClick",
            "SingleLongClick",
            "DoubleLongClick",
            //"TripleLongClick"
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
                //FindPreference("AppSelector_TripleClick"),
                FindPreference("AppSelector_SingleLongClick"),
                FindPreference("AppSelector_DoubleLongClick"),
                //FindPreference("AppSelector_TripleLongClick")
            };
            actionSelectorPs = new ListPreference[]
            {
                FindPreference("ActionSelector_SingleClick") as ListPreference,
                FindPreference("ActionSelector_DoubleClick") as ListPreference,
                //FindPreference("ActionSelector_TripleClick") as ListPreference,
                FindPreference("ActionSelector_SingleLongClick") as ListPreference,
                FindPreference("ActionSelector_DoubleLongClick") as ListPreference,
                //FindPreference("ActionSelector_TripleLongClick") as ListPreference
            };
            urlSelectorPs = new Preference[]
            {
                FindPreference("URLSelector_SingleClick"),
                FindPreference("URLSelector_DoubleClick"),
                //FindPreference("URLSelector_TripleClick"),
                FindPreference("URLSelector_SingleLongClick"),
                FindPreference("URLSelector_DoubleLongClick"),
                //FindPreference("URLSelector_TripleLongClick")
            };

            InitMainMenus();
        }

        public override void OnResume()
        {
            base.OnResume();

            UpdateSummary();
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

                var ad = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
                ad.SetTitle(Resource.String.MainMenu_ButtonInterval_Title);
                ad.SetCancelable(true);
                ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { });
                ad.SetNeutralButton(Resource.String.AlertDialog_Reset, delegate
                {
                    editor.PutInt("LogCounting", 80).Apply();

                    ETC.loggingCount = ETC.sharedPreferences.GetInt("LogCounting", 80);

                    UpdateSummary();
                });
                ad.SetPositiveButton(Resource.String.AlertDialog_Set, delegate
                {
                    editor.PutInt("LogCounting", np.Value).Apply();

                    ETC.loggingCount = ETC.sharedPreferences.GetInt("LogCounting", 80);

                    UpdateSummary();
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

                var ad = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
                ad.SetTitle(Resource.String.MainMenu_ButtonInterval_Title);
                ad.SetCancelable(true);
                ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { });
                ad.SetNeutralButton(Resource.String.AlertDialog_Reset, delegate
                {
                    editor.PutInt("ClickInterval", 0).Apply();

                    ETC.clickInterval = ETC.CalcInterval(400, 50, ETC.sharedPreferences.GetInt("ClickInterval", 0));

                    UpdateSummary();
                });
                ad.SetPositiveButton(Resource.String.AlertDialog_Set, delegate
                {
                    editor.PutInt("ClickInterval", np.Value).Apply();

                    ETC.clickInterval = ETC.CalcInterval(400, 50, ETC.sharedPreferences.GetInt("ClickInterval", 0));

                    UpdateSummary();
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

                var ad = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
                ad.SetTitle(Resource.String.MainMenu_ButtonInterval_Title);
                ad.SetCancelable(true);
                ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { });
                ad.SetNeutralButton(Resource.String.AlertDialog_Reset, delegate
                {
                    editor.PutInt("longClickInterval", 0).Apply();

                    ETC.longClickInterval = ETC.CalcInterval(800, 50, ETC.sharedPreferences.GetInt("longClickInterval", 0));

                    UpdateSummary();
                });
                ad.SetPositiveButton(Resource.String.AlertDialog_Set, delegate
                {
                    editor.PutInt("longClickInterval", np.Value).Apply();

                    ETC.longClickInterval = ETC.CalcInterval(800, 50, ETC.sharedPreferences.GetInt("longClickInterval", 0));

                    UpdateSummary();
                });
                ad.SetView(view);

                ad.Show();
            };

            // Button Mapping

            for (int i = 0; i < clickType.Length; ++i)
            {
                string type = clickType[i];

                UpdateSelectorVisibility(i, ETC.sharedPreferences.GetString($"MappingType_{type}", "0"));

                var enableP = FindPreference($"Enable{type}") as SwitchPreference;
                enableP.Checked = ETC.sharedPreferences.GetBoolean($"Enable{type}", false);
                enableP.PreferenceChange += (sender, e) => { editor.PutBoolean($"Enable{type}", (bool)e.NewValue).Apply(); };

                var mappingTypeP = FindPreference($"MappingType_{type}") as ListPreference;
                mappingTypeP.SetValueIndex(int.Parse(ETC.sharedPreferences.GetString($"MappingType_{type}", "0")));
                mappingTypeP.PreferenceChange += (sender, e) =>
                {
                    var lp = sender as ListPreference;
                    var index = 0;

                    for (index = 0; index < clickType.Length; ++index)
                    {
                        if (lp.Key == $"MappingType_{clickType[index]}")
                        {
                            break;
                        }
                    }

                    UpdateSelectorVisibility(index, (string)e.NewValue);
                };

                appSelectorPs[i].PreferenceClick += (sender, e) =>
                {
                    var p = sender as Preference;
                    int index = 0;

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

                urlSelectorPs[i].PreferenceClick += delegate
                {
                    var view = Activity.LayoutInflater.Inflate(Resource.Layout.TextInputDialogLayout, null);

                    var inputControl = view.FindViewById<AndroidX.AppCompat.Widget.AppCompatEditText>(Resource.Id.TextInputControl);
                    inputControl.Text = ETC.sharedPreferences.GetString($"URLSelector_{type}", "");

                    var ad = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
                    ad.SetTitle(Resource.String.MainMenu_Detail_URLSelector_Dialog_Title);
                    ad.SetCancelable(true);
                    ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { });
                    ad.SetNeutralButton(Resource.String.AlertDialog_Reset, delegate
                    {
                        editor.PutString($"URLSelector_{type}", "").Apply();
                        UpdateSummary();
                    });
                    ad.SetPositiveButton(Resource.String.AlertDialog_Set, delegate
                    {
                        editor.PutString($"URLSelector_{type}", inputControl.Text).Apply();
                        UpdateSummary();
                    });
                    ad.SetView(view);

                    ad.Show();
                };
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
                        var ad = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
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
                        var ad = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
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
                        var ad = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
                        ad.SetTitle(Resource.String.AlertDialog_WriteSettingPermission_Title);
                        ad.SetMessage(Resource.String.AlertDialog_WriteSettingPermission_Message);
                        ad.SetPositiveButton(Resource.String.AlertDialog_WriteSettingPermission_OK, delegate { StartActivity(new Intent(Settings.ActionManageWriteSettings)); });
                        ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { editor.PutString($"ActionSelector_{clickType[index]}", "0").Apply(); });
                        ad.SetCancelable(false);

                        ad.Show();
                    }
                    break;
            }

            editor.PutString($"ActionSelector_{clickType[index]}", (string)e.NewValue).Apply();
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            try
            {
                var sb = new StringBuilder();

                logCounting.Summary = $"{Resources.GetString(Resource.String.MainMenu_LogCounting_Summary)} {ETC.loggingCount}";
                clickInterval.Summary = $"{Resources.GetString(Resource.String.MainMenu_ButtonInterval_Summary)} {ETC.clickInterval}ms";
                longClickInterval.Summary = $"{Resources.GetString(Resource.String.MainMenu_LongClickInterval_Summary)} {ETC.longClickInterval}ms";

                for (int i = 0; i < clickType.Length; ++i)
                {
                    sb.Clear();

                    switch (ETC.sharedPreferences.GetString($"MappingType_{clickType[i]}", "0"))
                    {
                        case "0":
                            sb.Append(Resources.GetString(Resource.String.MainMenu_Detail_ActionSelector_Summary));
                            sb.Append(" ");
                            sb.Append(Resources.GetStringArray(Resource.Array.CustomActionList)[int.Parse(ETC.sharedPreferences.GetString($"ActionSelector_{clickType[i]}", "0"))]);

                            actionSelectorPs[i].Summary = sb.ToString();
                            break;
                        case "1":
                            string pkName = ETC.sharedPreferences.GetString($"AppSelector_{clickType[i]}", "");

                            if (!string.IsNullOrWhiteSpace(pkName))
                            {
                                try
                                {
                                    sb.Append(Resources.GetString(Resource.String.MainMenu_Detail_AppSelector_Summary));
                                    sb.Append(" ");
                                    sb.Append(ETC.packm.GetApplicationInfo(pkName, 0).LoadLabel(ETC.packm));
                                    sb.Append($"({pkName})");

                                    appSelectorPs[i].Summary = sb.ToString();
                                }
                                catch (Exception)
                                {
                                    ETC.sharedPreferences.Edit().PutString($"AppSelector_{clickType[i]}", "").Apply();
                                }
                            }
                            break;
                        case "2":
                            sb.Append(Resources.GetString(Resource.String.MainMenu_Detail_URLSelector_Summary));
                            sb.Append(" ");
                            sb.Append(ETC.sharedPreferences.GetString($"URLSelector_{clickType[i]}", ""));

                            urlSelectorPs[i].Summary = sb.ToString();
                            break;
                    }
                }
            }
            catch
            {

            }
        }

        private void UpdateSelectorVisibility(int clickTypeIndex, string typeCode)
        {
            switch (typeCode)
            {
                case "0":
                    appSelectorPs[clickTypeIndex].Visible = false;
                    actionSelectorPs[clickTypeIndex].Visible = true;
                    urlSelectorPs[clickTypeIndex].Visible = false;
                    break;
                case "1":
                    appSelectorPs[clickTypeIndex].Visible = true;
                    actionSelectorPs[clickTypeIndex].Visible = false;
                    urlSelectorPs[clickTypeIndex].Visible = false;
                    break;
                case "2":
                    appSelectorPs[clickTypeIndex].Visible = false;
                    actionSelectorPs[clickTypeIndex].Visible = false;
                    urlSelectorPs[clickTypeIndex].Visible = true;
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