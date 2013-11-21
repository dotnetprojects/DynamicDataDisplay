using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using System.Threading.Tasks;
using System.Threading.Collections;

namespace Microsoft.Research.DynamicDataDisplay.Fractals
{
    public class MandelbrotServer : SourceTileServer
    {
        private const int size = 128;

        ConcurrentStack<TaskInfo> tasks = new ConcurrentStack<TaskInfo>();
        TaskManager manager;
        public MandelbrotServer()
        {
            TileWidth = size;
            TileHeight = size;

            MinLevel = 0;
            MaxLevel = 31;

            ServerName = "Mandelbrot";

            // todo number of processors being used is a problem - too small is slow, too big blocks all other threads, including rendering thread, so
            // application stops to respond.
            TaskManagerPolicy policy = new TaskManagerPolicy(1, Environment.ProcessorCount, 1, 0, System.Threading.ThreadPriority.Lowest);
            manager = new TaskManager(policy);
        }

        public override bool Contains(TileIndex id)
        {
            return true;
        }

        public override void BeginLoadImage(TileIndex id)
        {
            DataRect firstLevel = DataRect.Create(-1.7, -1.3, 0.8, 1.2);

            double width = firstLevel.Width / MapTileProvider.GetSideTilesCount(id.Level);
            double xmin = firstLevel.XMin + (id.X + MapTileProvider.GetSideTilesCount(id.Level) / 2) * width;

            double height = firstLevel.Height / MapTileProvider.GetSideTilesCount(id.Level);
            double ymin = firstLevel.YMin + (MapTileProvider.GetSideTilesCount(id.Level) / 2 - id.Y - 1) * height;

            DataRect tileBounds = new DataRect(xmin, ymin, width, height);

            if (tasks.Count == 0)
            {
                CreateTask(id, tileBounds);
            }
            else
            {
                tasks.Push(new TaskInfo { ID = id, TileBounds = tileBounds });
            }
        }

        private void CreateTask(TileIndex id, DataRect bounds)
        {
            Task task = Task.Create(o =>
            {
                var set = new MandelbrotSet(size, bounds);
                set.Palette = new HSBPalette();
                var bmp = set.Draw();
                bmp.Freeze();
                ReportSuccessAsync(null, bmp, id);

                LookForNexttask();
            }, manager);
        }

        private void LookForNexttask()
        {
            TaskInfo taskInfo = new TaskInfo();
            if (tasks.TryPop(out taskInfo))
            {
                CreateTask(taskInfo.ID, taskInfo.TileBounds);
            }
        }

        private sealed class TaskInfo
        {
            public DataRect TileBounds { get; set; }
            public TileIndex ID { get; set; }
        }
    }
}
