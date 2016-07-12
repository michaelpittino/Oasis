using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace Launcher.Network
{

    public class WebRequest : IDisposable
    {

        private HttpWebRequest httpWebRequest;
        private HttpWebResponse httpWebResponse;

        private Encoding encoding;

        public WebRequest(string uri) : this(uri, Encoding.UTF8) { }

        public WebRequest(string uri, Encoding encoding)
        {
            this.httpWebRequest = HttpWebRequest.CreateHttp(uri);
            this.httpWebResponse = null;

            this.encoding = encoding;
        }

        public dynamic JsonPost(Dictionary<string, string> parameters, bool xmlHttpRequest = false)
        {
            byte[] postData = this.encoding.GetBytes(this.GenerateParameterFromDictionary(parameters));

            this.httpWebRequest.Method = "POST";
            this.httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            this.httpWebRequest.ContentLength = postData.Length;
            this.httpWebRequest.KeepAlive = true;

            if (xmlHttpRequest)
                this.httpWebRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");

            using (Stream requestStream = this.httpWebRequest.GetRequestStream())
                requestStream.Write(postData, 0, postData.Length);

            this.httpWebResponse = (HttpWebResponse) this.httpWebRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(this.httpWebResponse.GetResponseStream()))
                return JsonConvert.DeserializeObject<dynamic>(streamReader.ReadToEnd());
        }

        public void Dispose() => this.httpWebResponse.Dispose();

        private string GenerateParameterFromDictionary(Dictionary<string, string> data)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (KeyValuePair<string, string> item in data)
            {
                string key = HttpUtility.UrlEncode(item.Key);
                string value = HttpUtility.UrlEncode(item.Value);

                stringBuilder.Append($"{key}={value}");

                if (!item.Equals(data.Last()))
                    stringBuilder.Append("&");
            }

            return stringBuilder.ToString();
        }

    }

}
