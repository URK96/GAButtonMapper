using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Views.Accessibility;
using Android.Widget;

using System.IO;
using System.Threading.Tasks;

namespace GAButtonMapper
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Splash", MainLauncher = true)]
    public class Splash : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ETC.sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            ETC.packm = PackageManager;
            ETC.acm = GetSystemService("accessibility") as AccessibilityManager;
            ETC.pm = GetSystemService(PowerService) as PowerManager;

            var intentToAccessibility = new Intent(this, typeof(AccesibilityServiceMapper));
            StartService(intentToAccessibility);

            var di = new DirectoryInfo(GetExternalFilesDir(null).AbsolutePath);

            while (true)
            {
                di = di.Parent;

                if (di.Name == "Android")
                {
                    break;
                }
            }

            ETC.sdcardPath = di.Parent.FullName;

            RequestPermissions(new string[]
            {
                Manifest.Permission.WriteExternalStorage,
                Manifest.Permission.ReadExternalStorage,
                Manifest.Permission.Bluetooth,
                Manifest.Permission.BluetoothAdmin,
                Manifest.Permission.AccessWifiState,
                Manifest.Permission.ChangeWifiState,
                Manifest.Permission.Nfc
            }, 0);
        }

        private async Task StartUp()
        {
            await Task.Delay(1000);

            CreateNotificationChannel();

            if (!ETC.CheckPermission(this, Manifest.Permission.ReadLogs) || !ETC.CheckPermission(this, Manifest.Permission.WriteSecureSettings) || ETC.sharedPreferences.GetBoolean("HasRestart", false))
            {
                StartActivity(typeof(InitSettingActivity));
            }
            else
            {
                StartActivity(typeof(MainActivity));
            }
        }

        void CreateNotificationChannel()
        {
            ETC.channelId = Resources.GetString(Resource.String.Notification_Channel_Id);

            var channelName = Resources.GetString(Resource.String.Notification_Channel_Name);
            var channelDescription = GetString(Resource.String.Notification_Channel_Description);
            var channel = new NotificationChannel(ETC.channelId, channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            if (ETC.nm == null)
            {
                ETC.nm = GetSystemService(NotificationService) as NotificationManager;
            }

            ETC.nm.CreateNotificationChannel(channel);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            foreach (var p in grantResults)
            {
                if (p == Permission.Denied)
                {
                    Toast.MakeText(this, Resource.String.Common_EssentialPermissionDenied, ToastLength.Short).Show();
                    FinishAffinity();
                }
            }

            _ = StartUp();
        }
    }
}