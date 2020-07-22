using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using System.IO;
using Android.Content.PM;

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

            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.QnAMainToolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetTitle(Resource.String.QnAActivity_Title);

            cautionTextView = FindViewById<TextView>(Resource.Id.QnAText);

            string assetName = "QnA_ko.txt";

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