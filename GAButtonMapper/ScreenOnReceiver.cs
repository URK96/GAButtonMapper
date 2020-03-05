
using Android.App;
using Android.Content;

using static GAButtonMapper.ETC;

namespace GAButtonMapper
{
    [BroadcastReceiver]
    [IntentFilter(new string[] { Intent.ActionScreenOn })]
    public class ScreenOnReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (sharedPreferences.GetBoolean("EnableMapping", false) &&
                sharedPreferences.GetBoolean("ScreenOffDisableMapping", false) &&
                pm.IsInteractive &&
                !isRun)
            {
                monitoringMethod();
            }
        }
    }
}