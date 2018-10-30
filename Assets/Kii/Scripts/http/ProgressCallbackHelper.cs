using System;
using KiiCorp.Cloud.Storage;
using UnityEngine;

namespace KiiCorp.Cloud.Unity
{
    internal class ProgressCallbackHelper
    {
        private KiiHttpClientProgressCallback progressCallback;
        private KiiHttpClientProgressPercentageCallback progressPercentageCallback;

        public ProgressCallbackHelper (KiiHttpClientProgressCallback callback)
        {
            this.progressCallback = callback;
            this.progressPercentageCallback = null;
        }
        public ProgressCallbackHelper (KiiHttpClientProgressPercentageCallback callback)
        {
            this.progressCallback = null;
            this.progressPercentageCallback = callback;
        }
        public void NotifyUploadProgress(WWW www, long contentLength)
        {
            if (this.progressCallback != null)
            {
                this.progressCallback((long)(contentLength * www.uploadProgress), contentLength);
            }
            if (this.progressPercentageCallback != null)
            {
                this.progressPercentageCallback(www.uploadProgress);
            }
        }
        public void NotifyDownloadProgress(WWW www)
        {
            if (this.progressCallback != null)
            {
                // TODO:
                // www.bytesDownloaded has bug. so we cannot notify client of progress correctly.
                // see:http://issuetracker.unity3d.com/issues/the-new-www-dot-bytesdownloaded-feature-completely-freezes-unity
                // We will enable the following code when this bug will be fixed.
                //------------------------------------------------------------------
                //long total = (long)(www.bytesDownloaded / www.progress);
                //progress(www.bytesDownloaded, total);
                //------------------------------------------------------------------
                this.progressCallback(0, 0);
            }
            if (this.progressPercentageCallback != null)
            {
                this.progressPercentageCallback(www.progress);
            }
        }
    }
}

