using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.AppCompat.App;

using Hoang8f.Widgets;

using System.Net;
using System.Threading.Tasks;

using Xamarin.Essentials;

namespace GAButtonMapper
{
    [Activity(Label = "@string/AppInfo_Title", Theme = "@style/AppTheme.NoActionBar")]
    public class AppInfoActivity : AppCompatActivity
    {
        TextView checkStatus;
        FButton updateButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.AppInfoLayout);

            SetSupportActionBar(FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.AppInfoToolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            FindViewById<TextView>(Resource.Id.AppInfoAppVersion).Text = $"v{ETC.packm.GetPackageInfo(PackageName, 0).VersionName}";

            checkStatus = FindViewById<TextView>(Resource.Id.AppInfoCheckVersion);
            updateButton = FindViewById<FButton>(Resource.Id.AppInfoUpdateButton);

            updateButton.Click += delegate { Launcher.TryOpenAsync(ETC.updateURL); };
            FindViewById<FButton>(Resource.Id.AppInfoLicenseButton).Click += delegate { StartActivity(typeof(LicenseActivity)); };

            Task.Run(CheckVersion);
        }

        private void CheckVersion()
        {
            try
            {
                long nowVersionCode = ETC.packm.GetPackageInfo(PackageName, 0).LongVersionCode;
                long serverVersionCode = 0;

                MainThread.BeginInvokeOnMainThread(() => { checkStatus.SetText(Resource.String.AppInfo_CheckVersion_Checking); });

                using (var wc = new WebClient())
                {
                    serverVersionCode = long.Parse(wc.DownloadString(ETC.versionURL));
                }

                if (serverVersionCode > nowVersionCode)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        checkStatus.SetText(Resource.String.AppInfo_CheckVersion_NewUpdate);
                        updateButton.Visibility = ViewStates.Visible;
                    });
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() => { checkStatus.SetText(Resource.String.AppInfo_CheckVersion_Updated); });
                }
            }
            catch
            {
                MainThread.BeginInvokeOnMainThread(() => { checkStatus.SetText(Resource.String.AppInfo_CheckVersion_CheckFail); });
            }
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
    }
}