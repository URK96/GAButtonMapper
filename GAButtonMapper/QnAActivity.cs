
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
    public class QnAActivity : AppCompatActivity
    {
        TextView cautionTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.QnALayout);

            SetSupportActionBar(FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.QnAMainToolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetTitle(Resource.String.QnAActivity_Title);

            cautionTextView = FindViewById<TextView>(Resource.Id.QnAText);

            string assetName = "QnA_ko.txt";

            using (var sr = new StreamReader(Assets.Open(assetName)))
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