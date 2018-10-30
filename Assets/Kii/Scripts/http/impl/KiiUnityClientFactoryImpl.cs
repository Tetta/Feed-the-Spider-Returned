using System;
using KiiCorp.Cloud.Storage;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace KiiCorp.Cloud.Unity
{
    /// <summary>
    /// Kii unity client factory impl.
    /// </summary>
    internal class KiiUnityClientFactoryImpl : KiiHttpClientFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KiiCorp.Cloud.Storage.KiiUnityClientFactoryImpl"/> class.
        /// </summary>
        public KiiUnityClientFactoryImpl () : base()
        {
            ServicePointManager.ServerCertificateValidationCallback += this.OnRemoteCertificateValidationCallback;
        }

        private bool OnRemoteCertificateValidationCallback(object sender, X509Certificate certificate,
                                                           X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            HttpWebRequest request = sender as HttpWebRequest;
            
            string hostName = request.RequestUri.Host;
            if( hostName.EndsWith("kii.com"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual KiiHttpClient Create(string url, string appID, string appKey, KiiHttpMethod method)
        {
            return new KiiHttpUnityClientImpl(this.SetDisableCacheQueryParameter(url), appID, appKey, method);
        }
        protected string SetDisableCacheQueryParameter(string url)
        {
            StringBuilder sb = new StringBuilder(url);
            if (url.Contains("?"))
            {
                sb.Append("&");
            }
            else
            {
                sb.Append("?");
            }
            sb.Append("disable_cache=");
            sb.Append(Utils.CurrentTimeMills.ToString());
            return sb.ToString();
        }
    }
}

