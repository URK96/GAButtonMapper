using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Xamarin.Essentials;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;

namespace GAButtonMapper
{
    internal class CustomTimer
    {
        public delegate void Tick();

        private Stopwatch sw;
        public int Interval { get; set; }
        public Tick TickMethod { get; set; }
        public bool IsRunning { get { return sw.IsRunning; } }

        public CustomTimer(int interval = int.MaxValue)
        {
            sw = new Stopwatch();
            Interval = interval;
        }

        public void Start()
        {
            try
            {
                Task.Run(() =>
                {
                    sw.Start();

                    while (sw.ElapsedMilliseconds < Interval)
                    {
                        if (!IsRunning)
                        {
                            sw.Reset();
                            return;
                        }
                    }

                    Stop();

                    MainThread.BeginInvokeOnMainThread(() => { TickMethod(); });
                });
            }
            catch (Exception)
            {
                throw new Exception("Timer Exception : Start");
            }
        }

        public void Stop()
        {
            try
            {
                Task.Run(() =>
                {
                    sw.Stop();
                    sw.Reset();
                });
            }
            catch (Exception)
            {
                throw new Exception("Timer Exception : Stop");
            }
        }

        public void Dispose()
        {
            sw = null;
            TickMethod = null;
        }
    }
}