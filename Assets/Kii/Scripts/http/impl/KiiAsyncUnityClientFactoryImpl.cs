using System;
using KiiCorp.Cloud.Storage;

namespace KiiCorp.Cloud.Unity
{
    internal class KiiAsyncUnityClientFactoryImpl : KiiUnityClientFactoryImpl
    {
        public KiiAsyncUnityClientFactoryImpl () : base()
        {
        }
        /// <summary>
        /// Create the new Client with specified url.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="appID">AppID.</param>
        /// <param name="appKey">AppKey.</param>
        /// <param name="method">HTTP method.</param>
        public override KiiHttpClient Create(string url, string appID, string appKey, KiiHttpMethod method)
        {
            return new KiiAsyncHttpUnityClientImpl(this.SetDisableCacheQueryParameter(url), appID, appKey, method);
        }
    }
}

