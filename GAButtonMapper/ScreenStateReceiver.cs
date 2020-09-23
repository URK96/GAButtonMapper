
using Android.App;
using Android.Content;

using static GAButtonMapper.ETC;

namespace GAButtonMapper
{
    [BroadcastReceiver]
    [IntentFilter(new string[] { Intent.ActionScreenOn, Intent.ActionScreenOff })]
    public class ScreenStateReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            switch (intent.Action)
            {
                case Intent.ActionScreenOn:
                    isScreenOff = false;

                    if (isMappingEnable && isScreenOffMappingEnable && !isScreenOff && !isRun)
                    {
                        monitoringMethod();
                    }
                    break;
                case Intent.ActionScreenOff:
                    isScreenOff = true;
                    break;
            }
        }
    }
}