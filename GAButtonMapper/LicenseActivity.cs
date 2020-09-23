
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.AppCompat.App;

using System.IO;

namespace GAButtonMapper
{
    [Activity(Label = "AppUsageCautionActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class LicenseActivity : AppCompatActivity
    {
        TextView licenseTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.TextViewerLayout);

            SetSupportActionBar(FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.TextViewerMainToolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetTitle(Resource.String.LicenseActivity_Title);

            licenseTextView = FindViewById<TextView>(Resource.Id.TextViewerText);

            string assetName = "License.txt";

            using (var sr = new StreamReader(Assets.Open(assetName)))
            {
                licenseTextView.Text = sr.ReadToEnd();
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