using System;
using UnityEngine;
using System.Collections.Generic;
using KiiCorp.Cloud.Storage;

namespace KiiCorp.Cloud.Unity
{
    public class WWWUtils
    {
        // IL2CPP does not implement StringComparer! So we convert keys to lowercase one by one.
        public static Dictionary<string, string> LowerCaseHeaders(WWW www)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            if (www.responseHeaders != null)
            {
                foreach(KeyValuePair<string,string> entry in www.responseHeaders)
                {
                    headers.Add(entry.Key.ToLower(), entry.Value);
                }
            }
            // Still CurrentCultureIgnoreCase is useful other than IL2CPP.
            return new Dictionary<string, string>(headers, StringComparer.CurrentCultureIgnoreCase);
        }

        public static string GetHeader(Dictionary<string, string> lowerCaseHeaders, string key)
        {
            string lowerCaseKey = key.ToLower();
            if (lowerCaseHeaders.ContainsKey(lowerCaseKey))
            {
                return lowerCaseHeaders[lowerCaseKey];
            }
            return null;
        }

        public static int GetStatusCode(Dictionary<string, string> lowerCaseHeaders, int fallback)
        {
            string key = "x-http-status-code";
            try
            {
                return int.Parse(lowerCaseHeaders[key]);
            }
            catch (Exception e)
            {
                if (Kii.Logger != null)
                {
                    Kii.Logger.Debug("[WARN] Response dose not contain X-HTTP-Status-Code or failed to parse.");
                }
                return fallback;
            }
        }
    }
}

