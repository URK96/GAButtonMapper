﻿using Android;
using Android.AccessibilityServices;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Camera2;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Support.V4.App;
using Android.Support.V7.Preferences;
using Android.Views;
using Android.Views.Accessibility;
using Android.Widget;
using Plugin.AudioRecorder;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using Xamarin.Essentials;

using static GAButtonMapper.ETC;

namespace GAButtonMapper
{
    [Service(Label = "GAButtonMapper", Permission = Manifest.Permission.BindAccessibilityService)]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    public class AccesibilityServiceMapper : AccessibilityService
    {
        private CameraManager cm;
        private AudioManager am;

        private Stopwatch longClickSW;
        private Stopwatch clickSW;

        private KeyEvent keyDownEvent;
        private KeyEvent keyUpEvent;

        private AudioRecorderService recorder;

        private NotificationCompat.Builder recorderNBuilder;

        private string recorderFileName = "";

        public override void OnAccessibilityEvent(AccessibilityEvent e)
        {
            
        }

        protected override async void OnServiceConnected()
        {
            base.OnServiceConnected();

            Toast.MakeText(this, Resource.String.AccessibilitySevice_Connected, ToastLength.Short).Show();

            isUnbind = false;

            if (sharedPreferences == null)
            {
                sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            }

            if (vibrator == null)
            {
                vibrator = (GetSystemService(VibratorService) as Vibrator);
            }

            if (pm == null)
            {
                pm = GetSystemService(PowerService) as PowerManager;
            }

            if (am == null)
            {
                am = GetSystemService(AudioService) as AudioManager;
            }

            if (cm == null)
            {
                cm = (GetSystemService(CameraService) as CameraManager);
            }

            if (longClickSW == null)
            {
                longClickSW = new Stopwatch();
            }

            if (clickSW == null)
            {
                clickSW = new Stopwatch();
            }

            if (recorder == null)
            {
                recorder = new AudioRecorderService();
            }

            loggingCount = sharedPreferences.GetInt("LogCounting", 80);
            clickInterval = CalcInterval(400, 50, sharedPreferences.GetInt("ClickInterval", 0));
            longClickInterval = CalcInterval(800, 50, sharedPreferences.GetInt("LongClickInterval", 0));
            monitoringInterval = sharedPreferences.GetInt("MonitoringInterval", 30);

            monitoringMethod = new MonitoringMethod(MonitoringKeyState);

            //await MonitoringKeyState();

            await monitoringMethod();
        }

        internal async Task MonitoringKeyState()
        { 

            try
            {
                if (isUnbind ||
                !sharedPreferences.GetBoolean("EnableMapping", false) ||
                (sharedPreferences.GetBoolean("ScreenOffDisableMapping", false) && !pm.IsInteractive))
                {
                    return;
                }

                string s = "";

                Toast.MakeText(this, Resource.String.MonitoringProcess_Start, ToastLength.Short).Show();

                isRun = true;

                while (true)
                {
                    await Task.Delay(monitoringInterval);

                    /*if ()
                    {
                        await Task.Delay(2000);

                        continue;
                    }*/

                    if (isUnbind ||
                        !sharedPreferences.GetBoolean("EnableMapping", false) ||
                        (sharedPreferences.GetBoolean("ScreenOffDisableMapping", false) && !pm.IsInteractive))
                    {
                        isRun = false;

                        Toast.MakeText(this, Resource.String.MonitoringProcess_End, ToastLength.Short).Show();

                        return;
                    }

                    var p = Java.Lang.Runtime.GetRuntime().Exec(new string[] { "/bin/sh", "-c", $"logcat -t {loggingCount} | grep 'keycode=165' | tail -1" });
                    await p.WaitForAsync();

                    using (var sr = new StreamReader(p.InputStream))
                    {
                        s = await sr.ReadToEndAsync();
                    }

                    if (s.Contains("down=true"))
                    {
                        isClickMonitoring = true;

                        while (true)
                        {
                            await Task.Delay(1);

                            if (clickSW.IsRunning)
                            {
                                StopClickSW();
                            }

                            if (!isLongClick && !longClickSW.IsRunning)
                            {
                                StartLongClickSW();
                            }

                            var p1 = Java.Lang.Runtime.GetRuntime().Exec(new string[] { "/bin/sh", "-c", $"logcat -t {loggingCount} | grep 'keycode=165' | tail -1" });
                            await p1.WaitForAsync();

                            using (var sr = new StreamReader(p1.InputStream))
                            {
                                s = await sr.ReadToEndAsync();
                            }

                            if (s.Contains("down=false"))
                            {
                                if (!clickSW.IsRunning)
                                {
                                    StartClickSW();
                                }

                                if (longClickSW.IsRunning)
                                {
                                    StopLongClickSW();
                                }

                                break;
                            }

                            if (isUnbind ||
                                !sharedPreferences.GetBoolean("EnableMapping", false) ||
                                (sharedPreferences.GetBoolean("ScreenOffDisableMapping", false) && !pm.IsInteractive))
                            {
                                isRun = false;

                                Toast.MakeText(this, Resource.String.MonitoringProcess_End, ToastLength.Short).Show();

                                return;
                            }
                        }

                        clickCount += 1;
                    }

                    if (clickCount > 0 && !isClickMonitoring)
                    {
                        if (longClickSW.IsRunning)
                        {
                            StopLongClickSW();
                        }

                        await RunButtonCommand(clickCount, isLongClick);

                        clickCount = 0;
                        isLongClick = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, Resource.String.MonitoringProcess_End, ToastLength.Short).Show();
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();

                isRun = false;
            }
        }

        private async Task RunButtonCommand(short count, bool longClick = false)
        {
            await Task.Delay(10);

            try
            {
                string countS = "";

                switch (count)
                {
                    case 1 when longClick:
                        countS = "SingleLongClick";
                        break;
                    case 1 when !longClick:
                        countS = "SingleClick";
                        break;
                    case 2 when longClick:
                        countS = "DoubleLongClick";
                        break;
                    case 2 when !longClick:
                        countS = "DoubleClick";
                        break;
                    case 3 when longClick:
                        countS = "TripleLongClick";
                        break;
                    case 3 when !longClick:
                        countS = "TripleClick";
                        break;
                    default:
                        return;
                }

                if (isTest)
                {
                    isClick = true;
                    clickType = countS;

                    return;
                }

                //MainThread.BeginInvokeOnMainThread(() => { Toast.MakeText(this, countS, ToastLength.Short).Show(); });

                if (!sharedPreferences.GetBoolean($"Enable{countS}", false))
                {
                    return;
                }

                if (int.Parse(sharedPreferences.GetString($"MappingType_{countS}", "0")) == 1)
                {
                    string pkName = sharedPreferences.GetString($"AppSelector_{countS}", "");

                    if (!string.IsNullOrWhiteSpace(pkName))
                    {
                        try
                        {
                            Intent intent = PackageManager.GetLaunchIntentForPackage(pkName);
                            intent.AddFlags(ActivityFlags.NewTask);
                            StartActivity(intent);
                        }
                        catch (Exception)
                        {
                            Toast.MakeText(this, $"Cannot run {pkName}", ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        throw new Exception($"{pkName} is not available");
                    }
                }
                else if (int.Parse(sharedPreferences.GetString($"MappingType_{countS}", "0")) == 0)
                {
                    switch (int.Parse(sharedPreferences.GetString($"ActionSelector_{countS}", "0")))
                    {
                        case 0:
                            PerformGlobalAction(GlobalAction.Back);
                            break;
                        case 1:
                            PerformGlobalAction(GlobalAction.Home);
                            break;
                        case 2:
                            PerformGlobalAction(GlobalAction.Recents);
                            break;
                        case 3:
                            PerformGlobalAction(GlobalAction.Notifications);
                            break;
                        case 4:
                            PerformGlobalAction(GlobalAction.TakeScreenshot);
                            break;
                        case 5:
                            PerformGlobalAction(GlobalAction.LockScreen);
                            break;
                        case 6:
                            PerformGlobalAction(GlobalAction.QuickSettings);
                            break;
                        case 7:
                            PerformGlobalAction(GlobalAction.ToggleSplitScreen);
                            break;
                        case 8:
                            isTorchOn = !isTorchOn;
                            cm.SetTorchMode("0", isTorchOn);
                            break;
                        case 9:
                            var quickMemoIntent = new Intent();
                            quickMemoIntent.SetPackage("com.lge.qmemoplus");
                            quickMemoIntent.SetAction("com.lge.qmemoplus.action.START_QUICKMODE");
                            quickMemoIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.IncludeStoppedPackages);
                            SendBroadcast(quickMemoIntent, "com.lge.qmemoplus.receiver.permission.BROADCAST_CAPTURE_PLUS");
                            break;
                        case 10:
                            var prestoMemoIntent = new Intent();
                            prestoMemoIntent.SetPackage("com.lge.qmemoplus");
                            prestoMemoIntent.SetAction("com.lge.qmemoplus.action.START_PRESTOMEMO");
                            prestoMemoIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.IncludeStoppedPackages);
                            SendBroadcast(prestoMemoIntent, "com.lge.qmemoplus.receiver.permission.BROADCAST_CAPTURE_PLUS");
                            break;
                        case 11:
                            switch (am.RingerMode)
                            {
                                default:
                                case RingerMode.Normal:
                                    am.RingerMode = RingerMode.Vibrate;
                                    break;
                                case RingerMode.Vibrate:
                                    am.RingerMode = RingerMode.Silent;
                                    vibrator.Vibrate(VibrationEffect.CreateOneShot(200, VibrationEffect.DefaultAmplitude));
                                    break;
                                case RingerMode.Silent:
                                    am.RingerMode = RingerMode.Normal;
                                    break;
                            }
                            break;
                        case 12:
                            await HeadSetButtonClick(1);
                            break;
                        case 13:
                            await HeadSetButtonClick(2);
                            break;
                        case 14:
                            await HeadSetButtonClick(3);
                            break;
                        case 15:
                            if (pm.IsInteractive)
                            {
                                Toast.MakeText(this, Resource.String.ToastMessage_QPayNotification, ToastLength.Short).Show();
                            }

                            if (sharedPreferences.GetBoolean("ActionFeatureVibrator", true))
                            {
                                vibrator.Vibrate(VibrationEffect.CreateWaveform(new long[] { 200, 0, 200, 0, 200 }, new int[] { 30, 0, 100, 0, 30 }, -1));
                            }

                            /*Intent qPayIntent = new Intent();
                            qPayIntent.SetClassName("com.lge.lgpay", "com.lge.lgpay.view.voiceassistant.VoiceAssistantLaunchActivity");
                            qPayIntent.SetAction("com.lge.lgpay.action.SHOW_CARDHISTORY");
                            qPayIntent.AddCategory("android.intent.category.DEFAULT");
                            qPayIntent.SetData(Android.Net.Uri.Parse("com.lge.lgpay"));
                            qPayIntent.SetFlags(ActivityFlags.NewTask);
                            StartActivity(qPayIntent);*/

                            Intent quickPayIntent = new Intent();
                            quickPayIntent.SetPackage("com.lge.lgpay");
                            quickPayIntent.SetAction("com.lge.lgpay.action.SHOW_QUICKPAY");
                            quickPayIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.IncludeStoppedPackages);
                            SendBroadcast(quickPayIntent);
                            break;
                        case 16:
                            if (nm == null)
                            {
                                nm = GetSystemService("notification") as NotificationManager;
                            }

                            if (recorder.IsRecording)
                            {
                                try
                                {
                                    await recorder.StopRecording();

                                    if (sharedPreferences.GetBoolean("ActionFeatureVibrator", true))
                                    {
                                        vibrator.Vibrate(VibrationEffect.CreateWaveform(new long[] { 500, 0, 500, 0 }, new int[] { 30, 0, 60, 0 }, -1));
                                    }

                                    /*var uri = Android.Support.V4.Content.FileProvider.GetUriForFile(this, $"{PackageName}.provider", new Java.IO.File(GetExternalFilesDir(null).AbsolutePath, recorderFileName));

                                    var recorderIntent = new Intent();
                                    recorderIntent.SetAction(Intent.ActionView);
                                    recorderIntent.SetDataAndType(uri, "audio/*");
                                    recorderIntent.SetFlags(ActivityFlags.GrantReadUriPermission);

                                    recorderNBuilder.SetContentTitle(Resources.GetString(Resource.String.Notification_Recorder_Stop_Title));
                                    recorderNBuilder.SetContentText(Resources.GetString(Resource.String.Notification_Recorder_Stop_Message));
                                    recorderNBuilder.SetSmallIcon(Resource.Drawable.splash_icon);
                                    recorderNBuilder.SetContentIntent(PendingIntent.GetActivity(this, 0, recorderIntent, PendingIntentFlags.OneShot));

                                    nm.Notify(recorderNotificationId, recorderNBuilder.Build());*/

                                    MainThread.BeginInvokeOnMainThread(() => { Toast.MakeText(this, "Stop Voice Recording", ToastLength.Short).Show(); });
                                }
                                catch (Exception ex)
                                {
                                    Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
                                }
                            }
                            else
                            {
                                if (sharedPreferences.GetBoolean("ActionFeatureVibrator", true))
                                {
                                    vibrator.Vibrate(VibrationEffect.CreateWaveform(new long[] { 500, 0 }, new int[] { 30, 0 }, -1));
                                }

                                var dtNow = DateTime.Now;

                                recorderFileName = $"gamap_{dtNow.Year}{dtNow.Month}{dtNow.Day}_{dtNow.Hour}{dtNow.Minute}{dtNow.Second}.mp3";

                                string filePath = Path.Combine(GetExternalFilesDir(null).AbsolutePath, recorderFileName);

                                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                                }

                                recorder.FilePath = filePath;
                                recorder.StopRecordingOnSilence = false;
                                recorder.StopRecordingAfterTimeout = true;
                                recorder.TotalAudioTimeout = new TimeSpan(5, 0, 0);

                                recorder.AudioInputReceived += delegate
                                {
                                    var uri = Android.Support.V4.Content.FileProvider.GetUriForFile(this, $"{PackageName}.provider", new Java.IO.File(GetExternalFilesDir(null).AbsolutePath, recorderFileName));

                                    var recorderIntent = new Intent();
                                    recorderIntent.SetAction(Intent.ActionView);
                                    recorderIntent.SetDataAndType(uri, "audio/*");
                                    recorderIntent.SetFlags(ActivityFlags.GrantReadUriPermission);

                                    recorderNBuilder.SetContentTitle(Resources.GetString(Resource.String.Notification_Recorder_Stop_Title));
                                    recorderNBuilder.SetContentText(Resources.GetString(Resource.String.Notification_Recorder_Stop_Message));
                                    recorderNBuilder.SetSmallIcon(Resource.Drawable.splash_icon);
                                    recorderNBuilder.SetContentIntent(PendingIntent.GetActivity(this, 0, recorderIntent, PendingIntentFlags.OneShot));
                                    recorderNBuilder.SetAutoCancel(true);

                                    nm.Notify(recorderNotificationId, recorderNBuilder.Build());
                                };

                                await recorder.StartRecording();

                                recorderNBuilder = new NotificationCompat.Builder(this, channelId);
                                recorderNBuilder.SetContentTitle(Resources.GetString(Resource.String.Notification_Recorder_Start_Title));
                                recorderNBuilder.SetContentText(Resources.GetString(Resource.String.Notification_Recorder_Start_Message));
                                recorderNBuilder.SetSmallIcon(Resource.Drawable.splash_icon);

                                var notification = recorderNBuilder.Build();
                                notification.Flags = NotificationFlags.NoClear;

                                nm.Notify(recorderNotificationId, notification);

                                MainThread.BeginInvokeOnMainThread(() => { Toast.MakeText(this, "Start Voice Recording", ToastLength.Short).Show(); });
                            }
                            break;
                        case 17:
                            PerformGlobalAction(GlobalAction.Recents);
                            await Task.Delay(500);
                            PerformGlobalAction(GlobalAction.Recents);
                            break;
                        case 18:
                            if (sharedPreferences.GetBoolean("ActionFeatureVibrator", true))
                            {
                                vibrator.Vibrate(VibrationEffect.CreateWaveform(new long[] { 200, 0 }, new int[] { 30, 0 }, -1));
                            }

                            Settings.System.PutInt(ContentResolver, Settings.System.AccelerometerRotation, Settings.System.GetInt(ContentResolver, Settings.System.AccelerometerRotation) == 0 ? 1 : 0);
                            break;
                        case 19:
                            Settings.Global.PutInt(ContentResolver, Settings.Global.AirplaneModeOn, Settings.Global.GetInt(ContentResolver, Settings.Global.AirplaneModeOn) == 0 ? 1 : 0);
                            break;
                        case 20:
                            var mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

                            if (mBluetoothAdapter.IsEnabled)
                            {
                                mBluetoothAdapter.Disable();
                            }
                            else
                            {
                                mBluetoothAdapter.Enable();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
            }
            finally
            {
                await Task.Delay(10);
            }
        }

        public override bool OnUnbind(Intent intent)
        {
            Toast.MakeText(this, Resource.String.AccessibilitySevice_Unbind, ToastLength.Short).Show();
            isUnbind = true;

            return base.OnUnbind(intent);
        }

        public override void OnInterrupt()
        {

        }

        private async Task HeadSetButtonClick(int repeatCount)
        {
            for (int i = 1; i <= repeatCount; ++i)
            {
                var downTime = SystemClock.UptimeMillis();
                keyDownEvent = new KeyEvent(downTime, downTime, KeyEventActions.Down, Keycode.Headsethook, 0, MetaKeyStates.None, 1, 226, KeyEventFlags.FromSystem, InputSourceType.Keyboard);
                keyUpEvent = new KeyEvent(downTime, downTime, KeyEventActions.Up, Keycode.Headsethook, 0, MetaKeyStates.None, 1, 226, KeyEventFlags.FromSystem, InputSourceType.Keyboard);

                am.DispatchMediaKeyEvent(keyDownEvent);
                am.DispatchMediaKeyEvent(keyUpEvent);
                await Task.Delay(50);
            }
        }

        internal void StartLongClickSW()
        {
            try
            {
                Task.Run(() =>
                {
                    longClickSW.Start();

                    while (longClickSW.ElapsedMilliseconds < longClickInterval)
                    {
                        if (!longClickSW.IsRunning)
                        {
                            longClickSW.Reset();
                            return;
                        }
                    }

                    StopLongClickSW();

                    MainThread.BeginInvokeOnMainThread(() => 
                    {
                        isLongClick = true;

                        if (sharedPreferences.GetBoolean("LongClickVibrator", true))
                        {
                            vibrator.Vibrate(VibrationEffect.CreateWaveform(new long[] { 100, 0 }, new int[] { 20, 0 }, -1));
                        }
                    });
                });
            }
            catch (Exception)
            {

            }
        }

        internal void StopLongClickSW()
        {
            try
            {
                longClickSW.Stop();
                longClickSW.Reset();
            }
            catch (Exception)
            {

            }
        }

        internal void StartClickSW()
        {
            try
            {
                Task.Run(() =>
                {
                    clickSW.Start();

                    while (clickSW.ElapsedMilliseconds < clickInterval)
                    {
                        if (!clickSW.IsRunning)
                        {
                            clickSW.Reset();
                            return;
                        }
                    }

                    isClickMonitoring = false;

                    StopClickSW();
                });
            }
            catch (Exception)
            {

            }
        }

        internal void StopClickSW()
        {
            try
            {
                clickSW.Stop();
                clickSW.Reset();
            }
            catch (Exception)
            {

            }
        }
    }
}