using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

using AndroidX.CardView.Widget;
using Plugin.CurrentActivity;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;

using System;
using System.Threading.Tasks;

namespace GAButtonMapper
{
    [Activity(Label = "DonationActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DonationActivity : AppCompatActivity
    {
        CardView[] donationCardList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            CrossCurrentActivity.Current.Activity = this;

            // Create your application here
            SetContentView(Resource.Layout.DonationLayout);

            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.DonationActivityToolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetTitle(Resource.String.DonationActivity_Title);

            donationCardList = new CardView[6]
            {
                FindViewById<CardView>(Resource.Id.DonationActivityCardView1),
                FindViewById<CardView>(Resource.Id.DonationActivityCardView2),
                FindViewById<CardView>(Resource.Id.DonationActivityCardView3),
                FindViewById<CardView>(Resource.Id.DonationActivityCardView4),
                FindViewById<CardView>(Resource.Id.DonationActivityCardView5),
                FindViewById<CardView>(Resource.Id.DonationActivityCardView6)
            };
            
            foreach (var cv in donationCardList)
            {
                cv.Click += DonationCardView_Click;
            }
        }

        private async void DonationCardView_Click(object sender, EventArgs e)
        {
            await PurchaseProcess("donation", "donation1");
        }

        private async Task PurchaseProcess(string productId, string payload)
        {
            var billing = CrossInAppBilling.Current;

            try
            {
                var connected = await billing.ConnectAsync(ItemType.InAppPurchase);

                if (!connected)
                {
                    //we are offline or can't connect, don't try to purchase
                    Toast.MakeText(this, Resource.String.DonationActivity_PurchaseFail_ConnectError, ToastLength.Long).Show();

                    return;
                }

                //check purchases
                var purchase = await billing.PurchaseAsync(productId, ItemType.InAppPurchase, payload);

                //possibility that a null came through.
                if (purchase == null)
                {
                    //did not purchase
                    Toast.MakeText(this, Resource.String.DonationActivity_PurchaseFail, ToastLength.Long).Show();
                }
                else if (purchase.State == PurchaseState.Purchased)
                {
                    //purchased!
                    Toast.MakeText(this, Resource.String.DonationActivity_PurchaseSuccess, ToastLength.Long).Show();
                }
            }
            catch (InAppBillingPurchaseException ex)
            {
                //Billing Exception handle this based on the type
                Toast.MakeText(this, Resource.String.DonationActivity_PurchaseFail, ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                //Something else has gone wrong, log it
                Toast.MakeText(this, Resource.String.DonationActivity_PurchaseFail, ToastLength.Long).Show();
            }
            finally
            {
                await billing.DisconnectAsync();
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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }
    }
}