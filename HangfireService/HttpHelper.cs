using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace HangfireService
{
    public  class HttpHelper
    {
        private static string _url;

        public  HttpHelper()
        {
            string token = GetToken();
            _url = $@"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={token}";
        }

        public static void PostMessage()
        {
            var wb = new WebClient();
            var data = new NameValueCollection();
            string url = "www.example.com";
            data["username"] = "myUser";
            data["password"] = "myPassword";
            var response = wb.UploadValues(url, "POST", data);

        }

        public void PostMessage01(MsgBody msgBody)
        {

            string content = JsonConvert.SerializeObject(msgBody);
            byte[] data = Encoding.UTF8.GetBytes(content);
            var request = (HttpWebRequest)WebRequest.Create(_url);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            try
            {
                var response = (HttpWebResponse)request.GetResponse();

            }
            catch (Exception ex)
            {

                throw ex;
            }

           
        }

        public static async Task PostMessage02Async()
        {
            HttpClient client = new HttpClient();
            var values = new Dictionary<string, string>()
{
    { "username", "myUser" },
    { "password", "myPassword" }
};

            var data = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(_url, data);

        }


        public static string GetToken()
        {
            string corpid = "wxec56ca668e7f9155";
            string secret = "xlrFFNs9x8wVCPabFyWLZLRm0doD1mNch-_MTKbDYQI";
            string url = $@"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpid}&corpsecret={secret}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = null;


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();


            tokenBody tokenBody = Newtonsoft.Json.JsonConvert.DeserializeObject<tokenBody>(retString);
            return tokenBody.access_token;
        }

    }
}
