﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Views.Accessibility;

using System;
using System.Threading.Tasks;

namespace GAButtonMapper
{
    public static class ETC
    {
        internal delegate Task MonitoringMethod();

        internal static volatile ISharedPreferences sharedPreferences;

        internal static string sdcardPath = "";

        internal static PackageManager packm;
        internal static NotificationManager nm;
        internal static AccessibilityManager acm;
        internal static PowerManager pm;
        internal static AudioManager am;
        internal static Vibrator vibrator;

        internal static MonitoringMethod monitoringMethod;

        internal static volatile bool isMappingEnable = false;
        internal static volatile bool isScreenOffMappingEnable = false;
        internal static volatile bool isScreenOff = false;
        internal static volatile bool isUnbind = false;
        internal static volatile bool isRun = false;
        internal static volatile bool isLongClickVibrate = false;
        internal static volatile bool isTorchOn = false;
        internal static volatile bool isScreenOnOffToastMessageEnable = true;
        internal static volatile short clickCount = 0;
        internal static volatile bool isClickMonitoring = false;
        internal static volatile bool isLongClick = false;
        internal static volatile int loggingCount = 80;
        internal static volatile int longClickInterval = 800;
        internal static volatile int clickInterval = 400;
        internal static volatile int monitoringInterval = 30;

        internal static bool isTest = false;
        internal static bool isClick = false;
        internal static string clickType = "";

        internal static string channelId = "";
        internal const int recorderNotificationId = 0;

        internal static Java.Util.Locale locale;

        internal const string versionURL = "https://raw.githubusercontent.com/URK96/GAButtonMapper/master/Version";
        internal const string updateURL = "https://github.com/URK96/GAButtonMapper/releases";

        internal static bool CheckPermission(Context context, string permission)
        {
            try
            {
                return context.CheckSelfPermission(permission) == Permission.Granted;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static int CalcInterval(int start, int stepSize, int count)
        {
            return start + (stepSize * count);
        }
    }
}

