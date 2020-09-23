using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;

using AndroidX.Preference;

namespace GAButtonMapper
{
    internal class AdvanceSettingFragment : PreferenceFragmentCompat
    {
        private ISharedPreferencesEditor editor;

        private Preference monitoringInterval;

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.AdvanceMenus);

            monitoringInterval = FindPreference("MonitoringInterval");

            InitMainMenus();
        }

        public override void OnResume()
        {
            base.OnResume();

            monitoringInterval.Summary =
                 $"{Resources.GetString(Resource.String.MainMenu_Advance_MonitoringInterval_Summary)} {ETC.monitoringInterval}ms";
        }

        private void InitMainMenus()
        {
            editor = ETC.sharedPreferences.Edit();

            // Click Timing

            monitoringInterval.PreferenceClick += delegate
            {
                var view = Activity.LayoutInflater.Inflate(Resource.Layout.NumberPickerDialogLayout, null);

                var np = view.FindViewById<NumberPicker>(Resource.Id.NumberPickerControl);
                np.MaxValue = 100;
                np.MinValue = 1;
                np.Value = ETC.sharedPreferences.GetInt("MonitoringInterval", 10);

                var ad = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
                ad.SetTitle(Resource.String.MainMenu_Advance_MonitoringInterval_Title);
                ad.SetMessage(Resource.String.MainMenu_Advance_MonitoringInterval_Caution);
                ad.SetCancelable(true);
                ad.SetNegativeButton(Resource.String.AlertDialog_Close, delegate { });
                ad.SetNeutralButton(Resource.String.AlertDialog_Reset, delegate
                {
                    editor.PutInt("MonitoringInterval", 30).Apply();

                    ETC.monitoringInterval = ETC.sharedPreferences.GetInt("MonitoringInterval", 30);
                    monitoringInterval.Summary =
                        $"{Resources.GetString(Resource.String.MainMenu_Advance_MonitoringInterval_Summary)} {ETC.monitoringInterval}ms";
                });
                ad.SetPositiveButton(Resource.String.AlertDialog_Set, delegate
                {
                    editor.PutInt("MonitoringInterval", np.Value).Apply();

                    ETC.monitoringInterval = ETC.sharedPreferences.GetInt("MonitoringInterval", 30);
                    monitoringInterval.Summary =
                        $"{Resources.GetString(Resource.String.MainMenu_Advance_MonitoringInterval_Summary)} {ETC.monitoringInterval}ms";

                });
                ad.SetView(view);

                ad.Show();
            };
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