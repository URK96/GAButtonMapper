using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GAButtonMapper
{
    [Activity(Label = "@string/AppSelector_Title", Theme = "@style/AppTheme.NoActionBar")]
    public class AppSelectorActivity : AppCompatActivity
    {
        private AndroidX.AppCompat.Widget.SearchView searchView;
        private RecyclerView recyclerView;

        private PackageManager pm;

        private string clickType = "";

        List<ResolveInfo> pkInfo;
        List<ResolveInfo> tpkInfo;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.AppSelectorLayout);

            clickType = Intent.GetStringExtra("Type");

            SetSupportActionBar(FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.AppSelectorToolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            searchView = FindViewById<AndroidX.AppCompat.Widget.SearchView>(Resource.Id.AppSelectorSearchView);
            searchView.QueryTextChange += async (sender, e) => { await ListApp(e.NewText); };
            recyclerView = FindViewById<RecyclerView>(Resource.Id.AppSelectorRecyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));

            pm = PackageManager;

            var mainIntent = new Intent(Intent.ActionMain);
            mainIntent.AddCategory(Intent.CategoryLauncher);

            pkInfo = pm.QueryIntentActivities(mainIntent, 0).ToList();
            pkInfo.TrimExcess();
            tpkInfo = new List<ResolveInfo>();

            await ListApp("");
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

        private async Task ListApp(string searchName)
        {
            try
            {
                await Task.Delay(10);

                tpkInfo.Clear();

                searchName = searchName.ToLower();

                foreach (var info in pkInfo)
                {
                    if (!string.IsNullOrWhiteSpace(searchName) && 
                        (!info.LoadLabel(pm)?.ToLower().Contains(searchName) ?? true))
                    {
                        continue;
                    }

                    tpkInfo.Add(info);
                }

                tpkInfo.TrimExcess();

                var adapter = new AppListAdapter(ref tpkInfo, ref pm, this);

                if (!adapter.HasOnItemClick())
                {
                    adapter.ItemClick += (sender, e) =>
                    {
                        ETC.sharedPreferences.Edit().PutString($"AppSelector_{clickType}", tpkInfo[e].ActivityInfo.PackageName).Apply();
                        OnBackPressed();
                    };
                }

                await Task.Delay(100);

                RunOnUiThread(() => { recyclerView.SetAdapter(adapter); });
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Fail list app", ToastLength.Short).Show();
            }
        }

        public override void OnBackPressed()
        {
            GC.Collect();

            base.OnBackPressed();
        }
    }

    class AppListViewHolder : RecyclerView.ViewHolder
    {
        public ImageView AppIcon { get; private set; }
        public TextView AppName { get; private set; }
        public TextView PackageName { get; private set; }

        public AppListViewHolder(View view, Action<int> listener) : base(view)
        {
            AppIcon = view.FindViewById<ImageView>(Resource.Id.AppSelectorListIconView);
            AppName = view.FindViewById<TextView>(Resource.Id.AppSelectorListAppName);
            PackageName = view.FindViewById<TextView>(Resource.Id.AppSelectorListAppPackageName);

            view.Click += (sender, e) => listener(LayoutPosition);
        }
    }

    class AppListAdapter : RecyclerView.Adapter
    {
        List<ResolveInfo> items;
        PackageManager pm;
        Activity context;

        public event EventHandler<int> ItemClick;

        public AppListAdapter(ref List<ResolveInfo> items, ref PackageManager pm, Activity context)
        {
            this.items = items;
            this.pm = pm;
            this.context = context;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent?.Context).Inflate(Resource.Layout.AppSelectorListLayout, parent, false);
            var vh = new AppListViewHolder(view, OnClick);

            return vh;
        }

        public override int ItemCount
        {
            get { return items.Count; }
        }

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

        public bool HasOnItemClick()
        {
            return ItemClick != null;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as AppListViewHolder;
            var item = items[position];

            try
            {
                vh.AppIcon.SetImageDrawable(item.LoadIcon(pm));
                vh.AppName.Text = item.LoadLabel(pm);
                vh.PackageName.Text = item.ActivityInfo.PackageName;
            }
            catch (Exception)
            {
                Toast.MakeText(context, "Fail create view", ToastLength.Short).Show();
            }
        }
    }

}