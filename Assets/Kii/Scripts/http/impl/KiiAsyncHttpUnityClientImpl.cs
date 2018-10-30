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
    internal class KiiAsyncHttpUnityClientImpl : KiiHttpUnityClientImpl
    {
        public KiiAsyncHttpUnityClientImpl (string url, string appID, string appKey, KiiHttpMethod method) : base(url, appID, appKey, method)
        {
        }
        public override void SendRequest (KiiHttpClientCallback callback)
        {
            this._SendRequest(callback);
        }
        public override void SendRequest (string body, KiiHttpClientCallback callback)
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
            WWWRequestLooper.RunOnMainThread (() => {
                this.ExecSendRequest(null, callback);
            });
        }
        public override void SendRequest(Stream body, KiiHttpClientProgressCallback progressCallback, KiiHttpClientCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback is null");
            }
            this.body = ReadStream(body);
            this.SetHttpMethodOverride ();
            WWWRequestLooper.RunOnMainThread (() => {
                // TODO:If huge file will be uploaded, this way is bad performance.
                // Try to Upload an Object Body in Multiple Pieces
                // See:http://documentation.kii.com/en/guides/rest/managing-data/object-storages/uploading/
                this.ExecSendRequest(new ProgressCallbackHelper(progressCallback), callback);
            });
        }
        public override void SendRequest(Stream body, KiiHttpClientProgressPercentageCallback progressCallback, KiiHttpClientCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback is null");
            }
            this.body = ReadStream(body);
            this.SetHttpMethodOverride ();
            WWWRequestLooper.RunOnMainThread (() => {
                // TODO:If huge file will be uploaded, this way is bad performance.
                // Try to Upload an Object Body in Multiple Pieces
                // See:http://documentation.kii.com/en/guides/rest/managing-data/object-storages/uploading/
                this.ExecSendRequest(new ProgressCallbackHelper(progressCallback), callback);
            });
        }
        public override void SendRequestForDownload(Stream outStream, KiiHttpClientProgressCallback progressCallback, KiiHttpClientCallback callback)
        {
            this.ExecSendRequestForDownload (outStream, new ProgressCallbackHelper (progressCallback), callback);
        }
        public override void SendRequestForDownload(Stream outStream, KiiHttpClientProgressPercentageCallback progressCallback, KiiHttpClientCallback callback)
        {
            this.ExecSendRequestForDownload (outStream, new ProgressCallbackHelper (progressCallback), callback);
        }
        private void ExecSendRequestForDownload(Stream outStream, ProgressCallbackHelper progressCallback, KiiHttpClientCallback callback)
        {
            if (callback == null)
            {
                callback(null, new ArgumentNullException("callback is null"));
                return;
            }
            if (outStream == null)
            {
                callback(null, new ArgumentNullException("outStream is null"));
                return;
            }
            float timeout = Time.realtimeSinceStartup + Timeout;
            this.SetHttpMethodOverride ();
            this.SetSDKClientInfo();
            WWWRequestLooper.RunOnMainThread (() => {
                WWWRequestLooper.RegisterNetworkRequest(new WWW (this.url, this.body, this.headers.GetHeadersAsDictionary()), progressCallback, (WWW www, ProgressCallbackHelper progress)=> {
                    if (!www.isDone)
                    {
                        if (timeout < Time.realtimeSinceStartup)
                        {
                            DisposeWWW(www);
                            callback(null, new NetworkException (new TimeoutException("Connection timeout. (did not finish within " + Timeout + " seconds)")));
                            return true;
                        }
                        if (progress != null)
                        {
                            progress.NotifyDownloadProgress(www);
                        }
                        return false;
                    }
                    else
                    {
                        try
                        {
                            Exception e = this.CheckHttpError(www);
                            if (e != null)
                            {
                                callback (null, e);
                                return true;
                            }
                            ApiResponse response = new ApiResponse ();
                            Dictionary<string,string> responseHeaders = WWWUtils.LowerCaseHeaders(www);
                            this.CopyHttpHeaders(responseHeaders, response);
                            response.Status = WWWUtils.GetStatusCode(responseHeaders, www.bytes == null ? 204 : 200);
                            response.ContentType = WWWUtils.GetHeader(responseHeaders, "Content-Type");
                            response.ETag = WWWUtils.GetHeader(responseHeaders, "ETag");

                            response.Body = "";
                            if (www.bytes != null)
                            {
                                BinaryWriter writer = new BinaryWriter(outStream);
                                writer.Write(www.bytes);
                            }
                            callback (response, null);
                            return true;
                        }
                        catch (Exception e)
                        {
                            callback (null, e);
                            return true;
                        }
                        finally
                        {
                            DisposeWWW(www);
                        }
                    }
                });
            });
        }
        private void ExecSendRequest(ProgressCallbackHelper progressCallback, KiiHttpClientCallback callback)
        {
            float timeout = Time.realtimeSinceStartup + Timeout;
            this.SetSDKClientInfo();
            WWWRequestLooper.RegisterNetworkRequest(new WWW (this.url, this.body, this.headers.GetHeadersAsDictionary()), progressCallback, (WWW www, ProgressCallbackHelper progress)=> {
                if (!www.isDone)
                {
                    if (timeout < Time.realtimeSinceStartup)
                    {
                        DisposeWWW(www);
                        callback(null, new NetworkException (new TimeoutException("Connection timeout. (did not finish within " + Timeout + " seconds)")));
                        return true;
                    }
                    if (progress != null)
                    {
                        progress.NotifyUploadProgress(www, this.body.Length);
                    }
                    return false;
                }
                else
                {
                    try
                    {
                        Exception e = this.CheckHttpError(www);
                        if (e != null)
                        {
                            callback (null, e);
                            return true;
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
                        return true;
                    }
                    catch (Exception e)
                    {
                        if (Kii.Logger != null)
                        {
                            Kii.Logger.Debug("[ERROR] Unexpected exception occurred when handling http response. msg=" + e.Message);
                        }
                        callback (null, e);
                        return true;
                    }
                    finally
                    {
                        DisposeWWW(www);
                    }
                }
            });
        }
    }
}

