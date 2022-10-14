using RestSharp;
using System.Net;
using System;
using System.Net.Http;
using System.Security.Policy;

namespace HangfireService.common
{
    public class RestClientHelper
    {

        /// <summary>
        /// 基础Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="restClient"></param>
        /// <param name="restRequest"></param>
        /// <returns></returns>
        public static(bool,string)Post(RestClient restClient,RestRequest restRequest)
        {
            try
            {
                var response = restClient.Execute(restRequest);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //获取到返回信息后  将内容转换成自己想要的结构  
                    //var result = JsonConvert.DeserializeObject<T获取保存体检返回结构>(response.Content); 
                    //因为这里是公用方法不做处理，在上一级的业务类处理，这里只返回
                    return (true, response.Content);
                }
                else
                {
                    return (false, $"请求异常：状态码{response.StatusCode}；错误信息：{response.ErrorMessage}；异常信息：{response.ErrorException}");
                }
            }
            catch (Exception e)
            {
                return (false, $"请求异常：错误信息：{e.Message}；");
            }

        }

        /// <summary>
        /// 普通post
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static (bool, string) Post(RestClient client, string data, string contentType)
        {

            try
            {
             
                var request = new RestRequest("", Method.Post);
                request.Timeout = 10000;
                request.AddHeader("Cache-Control", "no-cache");
                request.AddParameter(contentType, data, ParameterType.RequestBody);

                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    //获取到返回信息后  将内容转换成自己想要的结构  
                    //var result = JsonConvert.DeserializeObject<T获取保存体检返回结构>(response.Content); 
                    //因为这里是公用方法不做处理，在上一级的业务类处理，这里只返回
                    return (true, response.Content);
                }
                else
                {
                    return (false, $"请求异常：状态码{response.StatusCode}；错误信息：{response.ErrorMessage}；异常信息：{response.ErrorException}");
                }

            }
            catch (Exception e)
            {
                return (false, $"请求异常：错误信息：{e.Message}；");
            }
        }

        /// <summary>
        /// 个性post
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        /// <param name="cookie"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static (bool, string) Post(RestClient client, string data,ref  string cookie, string contentType = "application/json")
        {

            try
            {
                var request = new RestRequest("", Method.Post);
                request.Timeout = 10000;
                request.AddHeader("Cache-Control", "no-cache");
                request.AddParameter(contentType, data, ParameterType.RequestBody);

                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    foreach (var item in response.Headers)
                    {
                        if (item.Name == "Set-Cookie")
                        {
                            cookie = item.Value.ToString();
                        }
                    }
                    //获取到返回信息后  将内容转换成自己想要的结构  
                    //var result = JsonConvert.DeserializeObject<T获取保存体检返回结构>(response.Content); 
                    //因为这里是公用方法不做处理，在上一级的业务类处理，这里只返回
                    return (true, response.Content);
                }
                else
                {
                    return (false, $"请求异常：状态码{response.StatusCode}；错误信息：{response.ErrorMessage}；异常信息：{response.ErrorException}");
                }

            }
            catch (Exception e)
            {
                return (false, $"请求异常：错误信息：{e.Message}；");
            }
        }

        public static (bool,string) Get(string url,string tokenContent)
        {

            try
            {
                var client = new RestClient(url);
                var request = new RestRequest();//get提交

                if (!string.IsNullOrEmpty(tokenContent))
                {
                    string token = "Bearer " + tokenContent;
                    client.AddDefaultHeader("Authorization", token);//附加token验证信息   登陆时获取
                }

                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //获取到返回信息后  将内容转换成自己想要的结构  
                    //var result = JsonConvert.DeserializeObject<T获取保存体检返回结构>(response.Content); 
                    //因为这里是公用方法不做处理，在上一级的业务类处理，这里只返回
                    return (true, response.Content);
                }
                else
                {
                    return (false, $"请求异常：状态码{response.StatusCode}；错误信息：{response.ErrorMessage}；异常信息：{response.ErrorException}");
                }

            }
            catch (Exception e)
            {
                return (false, $"请求异常：错误信息：{e.Message}；");
            }

        }

    }
}
