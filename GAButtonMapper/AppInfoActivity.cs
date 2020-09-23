using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.AppCompat.App;

namespace GAButtonMapper
{
    [Activity(Label = "@string/AppInfo_Title", Theme = "@style/AppTheme.NoActionBar")]
    public class AppInfoActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.AppInfoLayout);

            SetSupportActionBar(FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.AppInfoToolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            FindViewById<TextView>(Resource.Id.AppInfoAppVersion).Text = $"v{ETC.packm.GetPackageInfo(PackageName, 0).VersionName}";
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