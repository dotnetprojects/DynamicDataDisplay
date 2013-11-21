using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.Visualization3D.MainLoops
{
    public class TimeEntity
    {
        private TimeSpan elapsedTime;
        private TimeSpan totalTime;

        public TimeSpan ElapsedTime
        {
            get { return elapsedTime; }
        }

        public TimeSpan TotalTime
        {
            get { return totalTime; }
        }

        public TimeEntity(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            this.elapsedTime = elapsedTime;
            this.totalTime = totalTime;
        }

        public TimeEntity()
        {
            this.elapsedTime = TimeSpan.FromTicks(0);
            this.totalTime = TimeSpan.FromTicks(0);
        }

    }
}
