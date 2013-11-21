using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Maps;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
    public abstract class TileServerBase : DispatcherObject, ITileServer
    {
        protected TileServerBase()
        {
#if DEBUG
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
#endif
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Server \"{0}\" - Statistics:", GetCustomName());
            builder.Append(Environment.NewLine);

            string[] toString = statistics.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in toString)
            {
                builder.Append('\t');
                builder.AppendLine(item);
            }

            MapsTraceSource.Instance.ServerInformationTraceSource.TraceInformation(builder.ToString());
        }

        private readonly TileServerStatistics statistics = new TileServerStatistics();
        public TileServerStatistics Statistics
        {
            get { return statistics; }
        }

        private string serverName = String.Empty;
        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>The name.</value>
        public virtual string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        protected virtual string GetCustomName()
        {
            return serverName;
        }

        protected void BeginLoadBitmapImpl(Stream stream, TileIndex id)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                BitmapImage bmp = new BitmapImage();
                SubscribeBitmapEvents(bmp);
                pendingBitmaps.Add(bmp, id);

                MemoryStream memStream = new MemoryStream();
                stream.CopyTo(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                stream.Dispose();

                bmp.BeginInit();
                bmp.StreamSource = memStream;
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                if (!bmp.IsDownloading)
                {
                    UnsubscribeBitmapEvents(bmp);
                    ReportSuccess(memStream, bmp, id);
                    pendingBitmaps.Remove(bmp);
                    bmp.Freeze();
                }
            }));
        }

        protected void UpdateStatistics(Action action)
        {
            Dispatcher.BeginInvoke(action);
        }

        private readonly PendingBitmapSet pendingBitmaps = new PendingBitmapSet();

        private void SubscribeBitmapEvents(BitmapImage bmp)
        {
            bmp.DownloadCompleted += OnBmpDownloadCompleted;
            bmp.DownloadFailed += OnBmpDownloadFailed;
        }

        private void UnsubscribeBitmapEvents(BitmapImage bmp)
        {
            bmp.DownloadFailed -= OnBmpDownloadFailed;
            bmp.DownloadCompleted -= OnBmpDownloadCompleted;
        }

        protected virtual void OnBmpDownloadFailed(object sender, ExceptionEventArgs e)
        {
            BitmapImage bmp = (BitmapImage)sender;
            bmp.StreamSource.Dispose();

            UnsubscribeBitmapEvents(bmp);

            TileIndex id = pendingBitmaps[bmp];
            pendingBitmaps.Remove(bmp);

            ReportFailure(id);
        }

        protected virtual void OnBmpDownloadCompleted(object sender, EventArgs e)
        {
            BitmapImage bmp = (BitmapImage)sender;
            bmp.Freeze();

            UnsubscribeBitmapEvents(bmp);

            TileIndex id = pendingBitmaps[bmp];
            pendingBitmaps.Remove(bmp);

            ReportSuccess(bmp.StreamSource, bmp, id);
        }

        protected void ReportSuccessAsync(Stream stream, BitmapSource bmp, TileIndex id)
        {
            Dispatcher.BeginInvoke(() => { ReportSuccess(stream, bmp, id); }, DispatcherPriority.Background);
        }

        protected void ReportSuccess(BitmapSource bmp, TileIndex id)
        {
            ReportSuccess(null, bmp, id);
        }

        protected virtual void ReportSuccess(Stream stream, BitmapSource bmp, TileIndex id)
        {
            MapsTraceSource.Instance.ServerInformationTraceSource.TraceInformation("{0}: loaded id = {1}", GetCustomName(), id);

            RaiseImageLoaded(new TileLoadResultEventArgs
            {
                Image = bmp,
                Stream = stream,
                Result = TileLoadResult.Success,
                ID = id
            });
        }

        protected virtual void ReportFailure(TileIndex id)
        {
            MapsTraceSource.Instance.ServerInformationTraceSource.TraceInformation("{0}: failed id = {1}", GetCustomName(), id);

            RaiseImageLoaded(new TileLoadResultEventArgs
            {
                Image = null,
                Stream = null,
                ID = id,
                Result = TileLoadResult.Failure
            });
        }

        #region ITileServer Members

        public abstract bool Contains(TileIndex id);

        public abstract void BeginLoadImage(TileIndex id);

        private void RaiseImageLoaded(TileLoadResultEventArgs args)
        {
            if (ImageLoaded == null) { }

            ImageLoaded.Raise(this, args);
        }
        public event EventHandler<TileLoadResultEventArgs> ImageLoaded;
        protected EventHandler<TileLoadResultEventArgs> ImageLoadedHandler
        {
            get { return ImageLoaded; }
        }

        protected void RaiseChangedEvent()
        {
            Changed.Raise(this);
        }
        public event EventHandler Changed;

        public void ForceUpdate()
        {
            Changed.Raise(this);
        }

        #endregion
    }
}
