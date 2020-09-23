using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.AppCompat.App;

using System;
using System.Threading.Tasks;

using Xamarin.Essentials;

namespace GAButtonMapper
{
    [Activity(Label = "ButtonTestActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ButtonTestActivity : AppCompatActivity
    {
        private RelativeLayout mainLayout;
        private TextView clickText;

        private readonly Color[] colors =
        {
            Color.Coral,
            Color.DarkGreen,
            Color.DarkMagenta,
            Color.DarkKhaki,
            Color.DarkSlateBlue
        };

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.ButtonTestLayout);

                ETC.isTest = true;

                SetSupportActionBar(FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.ButtonTestMainToolbar));
                SupportActionBar.SetTitle(Resource.String.MainMenu_ButtonSub_TestButtonClick_Title);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);

                mainLayout = FindViewById<RelativeLayout>(Resource.Id.ButtonTestMainLayout);
                clickText = FindViewById<TextView>(Resource.Id.ButtonTestResultText);

                clickText.SetText(Resource.String.ButtonTestActivity_ClickInitText);

                await CheckClicking();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
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

        public async Task CheckClicking()
        {
            int index = 0;

            while (true)
            {
                await Task.Delay(10);

                try
                {
                    if (ETC.isClick)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            clickText.Text = ETC.clickType;

                            mainLayout.SetBackgroundColor(colors[index++]);

                        });

                        ETC.isClick = false;

                        if (index == colors.Length)
                        {
                            index = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ETC.isClick = false;
                    Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
                }

                if (!ETC.isTest)
                {
                    break;
                }
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            ETC.isTest = false;
        }
    }
}