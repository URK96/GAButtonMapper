
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

using System.IO;

namespace GAButtonMapper
{
    [Activity(Label = "AppUsageCautionActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AppUsageCautionActivity : AppCompatActivity
    {
        TextView cautionTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.AppUsageCautionLayout);

            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.AppUsageCautionMainToolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetTitle(Resource.String.AppUsageCautionActivity_Title);

            cautionTextView = FindViewById<TextView>(Resource.Id.AppUsageCautionText);

            string assetName = "Caution_ko.txt";

            using (StreamReader sr = new StreamReader(Assets.Open(assetName)))
            {
                cautionTextView.Text = sr.ReadToEnd();
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