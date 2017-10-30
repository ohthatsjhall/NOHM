#if !UNITY_WSA || UNITY_EDITOR

namespace Crosstales.Radio.Util
{
    /// <summary>Specialised WebClient.</summary>
    public class CTWebClient : System.Net.WebClient
    {
        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Connection limit for all WebClients
        /// </summary>
        public int ConnectionLimit { get; set; }

        public CTWebClient() : this(5000) { }

        public CTWebClient(int timeout, int connectionLimit = 20)
        {
            Timeout = timeout;
            ConnectionLimit = connectionLimit;
        }

        protected override System.Net.WebRequest GetWebRequest(System.Uri uri)
        {
/*
            System.Net.WebRequest request = base.GetWebRequest(uri);

            if (request != null)
            {
                request.Timeout = Timeout;
            }
            return request;
            */

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)base.GetWebRequest(uri);

            if (request != null)
            {
                request.ServicePoint.ConnectionLimit = ConnectionLimit;
                request.Timeout = Timeout;
            }

            return request;
        }

        public System.Net.WebRequest CTGetWebRequest(System.Uri uri)
        {
            return GetWebRequest(uri);
        }

        public System.Net.WebRequest CTGetWebRequest(string uri)
        {
            return GetWebRequest(new System.Uri(uri));
        }
    }
}

#endif
// © 2017 crosstales LLC (https://www.crosstales.com)