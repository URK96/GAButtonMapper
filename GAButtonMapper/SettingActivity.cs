using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Views;

using Xamarin.Essentials;

namespace GAButtonMapper
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingActivity : AppCompatActivity, PreferenceFragmentCompat.IOnPreferenceStartFragmentCallback
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SettingLayout);

            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.MainSettingToolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.MainSettingFragmentContainer, new MainFragment(), null).Commit();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item?.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
            {
                SupportFragmentManager.PopBackStack();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public bool OnPreferenceStartFragment(PreferenceFragmentCompat caller, Preference pref)
        {
            switch (pref.Key)
            {
                case "ButtonSubPreference":
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.MainSettingFragmentContainer, new ButtonSubFragment(), null).AddToBackStack(null).Commit();
                    break;
                case "AdvanceSetting":
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.MainSettingFragmentContainer, new AdvanceSettingFragment(), null).AddToBackStack(null).Commit();
                    break;
            }

            return true;
        }
    }
}

