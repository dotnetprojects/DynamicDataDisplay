using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Diagnostics;

namespace Microsoft.Research.Visualization3D.MainLoops
{
    class TimeManager
    {
        private delegate void EmptyEventHandler();
        public delegate void TimedEventHandler(object sender, TimedEventArgs e);
        private long oldTime;
        private long delta;
        private float timeProportion = 100000.0f;
        Stopwatch stopwatch;

        private TimeEntity currentTimeEntity;

        public TimeEntity Current
        {
            get 
            {
                long newTime = stopwatch.ElapsedTicks;
                delta = newTime - currentTimeEntity.TotalTime.Ticks;
                currentTimeEntity = new TimeEntity(TimeSpan.FromTicks(delta), TimeSpan.FromTicks(newTime));
                return currentTimeEntity; 
            }
        }
        
       

        public event TimedEventHandler Update;

        public TimeManager()
        {
            currentTimeEntity = new TimeEntity(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(0));
        }

        private void RaiseUpdate(TimedEventArgs e)
        {
            if (Update != null)
                Update(this, e);
        }

        public void Start()
        {
            stopwatch = Stopwatch.StartNew();
            Dispatcher.CurrentDispatcher.BeginInvoke(new EmptyEventHandler(MainLoop), DispatcherPriority.ApplicationIdle, null);
        }

        private void MainLoop()
        {
            long newTime = stopwatch.ElapsedTicks;
            delta = newTime - currentTimeEntity.TotalTime.Ticks;
            currentTimeEntity = new TimeEntity(TimeSpan.FromTicks(delta), TimeSpan.FromTicks(newTime));
            RaiseUpdate(new TimedEventArgs { TimeEntity = currentTimeEntity });
            Dispatcher.CurrentDispatcher.BeginInvoke(new EmptyEventHandler(MainLoop), DispatcherPriority.ApplicationIdle, null);
        }
    }

    public class TimedEventArgs
    {
        public TimeEntity TimeEntity
        {
            get;
            set;
        }
    }
}
