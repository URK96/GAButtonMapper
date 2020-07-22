using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace GAButtonMapper
{
    [Activity(Label = "@string/AppInfo_Title", Theme = "@style/AppTheme.NoActionBar")]
    public class AppInfoActivity : AppCompatActivity
    {
        //TextView tv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.AppInfoLayout);

            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.AppInfoToolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            FindViewById<TextView>(Resource.Id.AppInfoAppVersion).Text = $"v{ETC.packm.GetPackageInfo(PackageName, 0).VersionName} Beta";
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