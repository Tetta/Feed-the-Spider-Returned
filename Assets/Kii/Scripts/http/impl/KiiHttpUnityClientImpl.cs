using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using KiiCorp.Cloud.Storage;
using UnityEngine;

namespace KiiCorp.Cloud.Unity
{
    internal class KiiHttpUnityClientImpl : KiiHttpClient
    {
        
        private static Encoding enc = Encoding.GetEncoding("UTF-8");
        
        protected string url;
        protected string appID;
        protected string appKey;
        protected KiiHttpMethod method;
        protected SimpleHttpHeaderList headers;
        protected byte[] body;

        /// <summary>
        /// Initializes a new instance of the <see cref="KiiCorp.Cloud.Storage.KiiHttpUnityClientImpl"/> class.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="appID">AppID.</param>
        /// <param name="appKey">AppKey.</param>
        /// <param name="method">Method.</param>
        public KiiHttpUnityClientImpl (string url, string appID, string appKey, KiiHttpMethod method)
        {
            this.url = url;
            this.appID = appID;
            this.appKey = appKey;
            this.method = method;
            this.headers = new SimpleHttpHeaderList ();
            this.headers["X-Kii-AppID"] = this.appID;
            this.headers["X-Kii-AppKey"] = this.appKey;
        }

        #region properties
        public string ContentType {
            set {
                this.headers["Content-Type"] = value;
            }
        }
        public string Accept {
            set {
                this.headers["Accept"] = value;
            }
        }
        public KiiHttpHeaderList Headers {
            get {
                return this.headers;
            }
        }
        public string Body {
            set {
                this.body = this.GetBytes(value);
            }
        }
        #endregion
        
        internal void SetSDKClientInfo() {
            this.headers["X-Kii-SDK"] = SDKClientInfo.GetSDKClientInfo();
        }

        [System.Obsolete("Use async method. This method blocks main thread.")]
        public ApiResponse SendRequest ()
        {
            ApiResponse result = null;
            Exception exception = null;
            this.SetHttpMethodOverride ();
            this.ExecSendRequest(null, null, (ApiResponse response, Exception e)=>{
                result = response;
                exception = e;
            });
            if (exception != null)
            {
                throw exception;
            }
            return result;
        }
        public virtual void SendRequest (KiiHttpClientCallback callback)
        {
            this._SendRequest(callback);
        }
        public virtual void SendRequest (string body, KiiHttpClientCallback callback)
        {
            this.Body = body;
            this._SendRequest(callback);
        }
        private void _SendRequest (KiiHttpClientCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback is null");
            }
            this.SetHttpMethodOverride ();
            this.ExecSendRequest (null, null, callback);
        }
        public virtual void SendRequest(Stream body, KiiHttpClientProgressCallback progressCallback, KiiHttpClientCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback is null");
            }
            this.body = ReadStream(body);
            this.SetHttpMethodOverride ();
            this.ExecSendRequest (progressCallback, null, callback);
        }
        public virtual void SendRequest(Stream body, KiiHttpClientProgressPercentageCallback progressCallback, KiiHttpClientCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback is null");
            }
            this.body = ReadStream(body);
            this.SetHttpMethodOverride ();
            this.ExecSendRequest (null, progressCallback, callback);
        }
        public virtual void SendRequestForDownload(Stream outStream, KiiHttpClientProgressCallback progressCallback, KiiHttpClientCallback callback)
        {
            this.ExecSendRequestForDownload (outStream, progressCallback, null, callback);
        }
        public virtual void SendRequestForDownload(Stream outStream, KiiHttpClientProgressPercentageCallback progressCallback, KiiHttpClientCallback callback)
        {
            this.ExecSendRequestForDownload (outStream, null, progressCallback, callback);
        }
        private void ExecSendRequestForDownload(Stream outStream, KiiHttpClientProgressCallback progressCallback, KiiHttpClientProgressPercentageCallback progressPercentageCallback, KiiHttpClientCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback is null");
            }
            this.SetSDKClientInfo();
            this.SetHttpMethodOverride ();
            WWW www = new WWW (this.url, this.body, this.headers.GetHeadersAsDictionary());
            try
            {
                float timeout = Time.realtimeSinceStartup + Timeout;
                while (!www.isDone) {
                    if (timeout < Time.realtimeSinceStartup) {
                        callback(null, new NetworkException (new TimeoutException("Connection timeout. (did not finish within " + Timeout + " seconds)")));
                        return;
                    }
                    Thread.Sleep (500);
                    if (progressCallback != null)
                    {
                        progressCallback(www.bytesDownloaded, www.size);
                    }
                    if (progressPercentageCallback != null)
                    {
                        progressPercentageCallback(www.progress);
                    }
                }
                Exception e = this.CheckHttpError(www);
                if (e != null)
                {
                    callback(null, e);
                    return;
                }
                ApiResponse response = new ApiResponse ();
                Dictionary<string,string> responseHeaders = WWWUtils.LowerCaseHeaders(www);
                this.CopyHttpHeaders(responseHeaders, response);

                response.Status = WWWUtils.GetStatusCode(responseHeaders, www.bytes == null ? 204 : 200);
                response.ContentType = WWWUtils.GetHeader(responseHeaders, "Content-Type");
                response.ETag = WWWUtils.GetHeader(responseHeaders, "ETag");

                response.Body = "";
                BinaryWriter writer = new BinaryWriter(outStream);
                writer.Write(www.bytes);
                callback (response, null);
            }
            finally
            {
                DisposeWWW(www);
            }
        }
        private void ExecSendRequest(KiiHttpClientProgressCallback progressCallback, KiiHttpClientProgressPercentageCallback progressPercentageCallback, KiiHttpClientCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback is null");
            }
            this.SetSDKClientInfo();
            WWW www = new WWW (this.url, this.body, this.headers.GetHeadersAsDictionary());
            try
            {
                float timeout = Time.realtimeSinceStartup + Timeout;
                while (!www.isDone) {
                    if (timeout < Time.realtimeSinceStartup) {
                        callback(null, new NetworkException (new TimeoutException("Connection timeout. (did not finish within " + Timeout + " seconds)")));
                        return;
                    }
                    Thread.Sleep (500);
                    if (progressCallback != null)
                    {
                        progressCallback((long)(this.body.Length * www.uploadProgress), this.body.Length);
                    }
                    if (progressPercentageCallback != null)
                    {
                        progressPercentageCallback(www.progress);
                    }
                }
                Exception e = this.CheckHttpError(www);
                if (e != null)
                {
                    callback(null, e);
                    return;
                }
                ApiResponse response = new ApiResponse ();
                Dictionary<string,string> responseHeaders = WWWUtils.LowerCaseHeaders(www);
                this.CopyHttpHeaders(responseHeaders, response);

                response.Status = WWWUtils.GetStatusCode(responseHeaders, www.bytes == null ? 204 : 200);
                response.ContentType = WWWUtils.GetHeader(responseHeaders, "Content-Type");
                response.ETag = WWWUtils.GetHeader(responseHeaders, "ETag");

                if (www.bytes != null)
                {
                    response.Body = this.GetString(www.bytes);
                }
                callback (response, null);
            }
            finally
            {
                DisposeWWW(www);
            }
        }
        protected void SetHttpMethodOverride()
        {
            switch (this.method)
            {
            case KiiHttpMethod.HEAD:
                // HEAD request with custom headers is not supported on WebPlayer, so we need to send http header using query string or POST method.
                this.headers ["X-HTTP-Method-Override"] = "HEAD";
                if (this.body == null)
                {
                    this.Body = "{\"_method\":\"HEAD\"}";
                }
                break;
            case KiiHttpMethod.GET:
                // GET request with custom headers is not supported on WebPlayer, so we need to send http header using query string or POST method.
                this.headers ["X-HTTP-Method-Override"] = "GET";
                if (this.body == null)
                {
                    this.Body = "{\"_method\":\"GET\"}";
                }
                break;
            case KiiHttpMethod.POST:
                if (this.body == null)
                {
                    // WWW class cannot send POST request with empty body.
                    this.ContentType = "application/X-Dummy-Content";
                    this.Body = "{\"_method\":\"POST\"}";
                }
                break;
            case KiiHttpMethod.PUT:
                // The WWW class will use GET by default and POST if you supply a postData parameter.
                // So we need to set dummy data to the body.
                this.headers ["X-HTTP-Method-Override"] = "PUT";
                if (this.body == null)
                {
                    this.Body = "{\"_method\":\"PUT\"}";
                }
                break;
            case KiiHttpMethod.DELETE:
                // The WWW class will use GET by default and POST if you supply a postData parameter.
                // So we need to set dummy data to the body.
                this.headers ["X-HTTP-Method-Override"] = "DELETE";
                if (this.body == null)
                {
                    this.Body = "{\"_method\":\"DELETE\"}";
                }
                break;
            }
        }
        protected void DisposeWWW(WWW www)
        {
            // Sometimes WWW.Dispose causes a total freeze up. So we need to call Dispose in separate thread.
            // see: http://issuetracker.unity3d.com/issues/unity-freezes-when-www-dot-dispose-is-used-while-downloading-assetbundle
            new Thread(() =>{
                try
                {
                    if (www != null)
                    {
                        www.Dispose();
                    }
                } 
                catch (Exception e)
                {
                    if (Kii.Logger != null)
                    {
                        Kii.Logger.Debug("[ERROR] Unexpected exception occurred when disposing WWW. msg=" + e.Message);
                    }
                }
            }).Start();
        }
        protected byte[] ReadStream(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                byte[] buff = new byte[65536];
                while (true)
                {
                    int read = stream.Read(buff, 0, buff.Length);
                    if (read > 0)
                    {
                        ms.Write(buff, 0, read);
                    }
                    else
                    {
                        break;
                    }
                }
                return ms.ToArray ();
            }
            finally
            {
                ms.Dispose();
            }
        }
        protected byte[] GetBytes(string s)
        {
            if (s == null){
                return null;
            }
            return enc.GetBytes (s);
        }
        protected string GetString(byte[] b)
        {
            if (b == null)
            {
                return null;
            }
            return enc.GetString (b);
        }
        protected void CopyHttpHeaders(Dictionary<string,string> source, ApiResponse dest)
        {
            if (source != null && dest != null)
            {
                foreach (String key in source.Keys)
                {
                    dest.Headers.Add(key, source[key]);
                }
            }
        }
        protected Exception CheckHttpError(WWW www)
        {
            if (!Utils.IsEmpty(www.error))
            {
                Dictionary<string,string> responseHeaders = WWWUtils.LowerCaseHeaders(www);
                int httpStatus = WWWUtils.GetStatusCode(responseHeaders, 0);
                if (httpStatus == 0)
                {
                    StringBuilder log = new StringBuilder();
                    log.Append(www.error + " ");
                    log.Append(Enum.GetName(typeof(KiiHttpMethod), this.method) + ":");
                    log.AppendLine(this.url);
                    Hashtable headers = this.headers.GetHeadersAsHashtable();
                    foreach (String key in headers.Keys)
                    {
                        log.AppendLine(key + ":" + headers[key]);
                    }
                    return new NetworkException (new SystemException(log.ToString()));
                }
                else
                {
                    // WWW cannot get response body if server returns a error status.
                    return KiiHttpUtils.TypedException(httpStatus, null);
                }
            }
            return null;
        }
        protected int Timeout
        {
            get
            {
                return WWWRequestLooper.ApiTimeout;
            }
        }

        #region inner class
        protected class SimpleHttpHeaderList : KiiHttpHeaderList
        {
            private Dictionary<String, String> headers = new Dictionary<string, string>();
            public SimpleHttpHeaderList()
            {
            }
            public string this[string key] {
                get
                {
                    foreach (String k in headers.Keys)
                    {
                        // Compare header name by ignoring case 
                        if (k.ToLower() == key.ToLower())
                        {
                            return this.headers[k];
                        }
                    }
                    return null;
                }
                set
                {
                    this.headers[key] = value;
                }
            }
            public Hashtable GetHeadersAsHashtable()
            {
                return new Hashtable(this.headers);
            }

            public Dictionary<string, string> GetHeadersAsDictionary()
            {
                return this.headers;
            }
        }
        #endregion
    }
}

