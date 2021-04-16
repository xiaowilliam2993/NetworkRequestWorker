using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRequestExample.Netframework4_5
{
    /// <summary>
    /// 网络请求工具类，基于.Net Framework 4.5框架，为网络请求提供便捷方法
    ///     目前只提供GET、POST两种最常用的请求方式，返回值为HttpResponseMessage，根据该对象可以处理各种业务方面的事情
    ///     GetHttpResponseMessageAs...，提供了一些快速将HttpResponseMessage结果转换为目标对象的入口
    /// </summary>
    public static class NetworkRequestWorker
    {
        public readonly static ILogger _Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 处理GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static HttpResponseMessage Get(string url, IDictionary<string, string> headerParameters = null)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HeaderParametersHandler(httpClient, headerParameters);
                    return httpClient.GetAsync(url).Result;
                }
            }
            catch(Exception ex)
            {
                _Logger.Error(ex, $"params: {JsonConvert.SerializeObject(new { url, headerParameters })}");
                throw ex;
            }
        }

        /// <summary>
        /// 处理GET请求（异步）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetAsync(string url, IDictionary<string, string> headerParameters = null)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HeaderParametersHandler(httpClient, headerParameters);
                    return await httpClient.GetAsync(url);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, $"params: {JsonConvert.SerializeObject(new { url, headerParameters })}");
                throw ex;
            }
        }

        /// <summary>
        /// 处理POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static HttpResponseMessage Post(string url, dynamic body, IDictionary<string, string> headerParameters = null)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(url);
#if Debug
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
#endif
                    HeaderParametersHandler(httpClient, headerParameters);
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                    if (body != null)
                    {
                        httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                    }
                    return httpClient.SendAsync(httpRequestMessage).Result;
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, $"params: {JsonConvert.SerializeObject(new { url, headerParameters })}");
                throw ex;
            }
        }

        /// <summary>
        /// 处理POST请求（异步）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAsync(string url, dynamic body, IDictionary<string, string> headerParameters = null)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(url);
                    HeaderParametersHandler(httpClient, headerParameters);
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                    if (body != null)
                    {
                        httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                    }
                    return await httpClient.SendAsync(httpRequestMessage);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, $"params: {JsonConvert.SerializeObject(new { url, headerParameters })}");
                throw ex;
            }
        }

        /// <summary>
        /// 处理POST请求，压缩上传
        ///     调用此方法上传数据，适用于在后台向其它系统传输比较大的数据
        ///     接收方需解码本方法传过来的body，具体方法请参考DecompressedHandler
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostByCompressAsync(string url, dynamic body, IDictionary<string, string> headerParameters = null)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(url);
                    HeaderParametersHandler(httpClient, headerParameters);
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, string.Empty);
                    if (body != null)
                    {
                        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                        httpRequestMessage.Content = new CompressedContent(httpContent, CompressionMethod.GZip);
                    }
                    return await httpClient.SendAsync(httpRequestMessage);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, $"params: {JsonConvert.SerializeObject(new { url, headerParameters })}");
                throw ex;
            }
        }

        /// <summary>
        /// 取得请求相应结果，返回string
        /// </summary>
        /// <param name="httpResponseMessage"></param>
        /// <returns></returns>
        public static string GetHttpResponseMessageAsString(this HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage == null) throw new ArgumentNullException(nameof(httpResponseMessage));

            return httpResponseMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 取得请求相应结果，返回JObject
        /// </summary>
        /// <param name="httpResponseMessage"></param>
        /// <returns></returns>
        public static JObject GetHttpResponseMessageAsJObject(this HttpResponseMessage httpResponseMessage)
        {
            try
            {
                return JObject.Parse(GetHttpResponseMessageAsString(httpResponseMessage));
            }
            catch (JsonReaderException ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 取得请求相应结果（泛型）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpResponseMessage"></param>
        /// <returns></returns>
        public static T GetHttpResponseMessageAsGeneric<T>(this HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<T>(GetHttpResponseMessageAsString(httpResponseMessage));
        }

        /// <summary>
        /// 写入请求头部参数
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="headerParameters"></param>
        private static void HeaderParametersHandler(HttpClient httpClient, IDictionary<string, string> headerParameters)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(headerParameters));

            if (headerParameters != null && headerParameters.Count > 0)
            {
                var headers = httpClient.DefaultRequestHeaders;
                foreach (KeyValuePair<string, string> item in headerParameters)
                {
                    if (!headers.Contains(item.Key))
                    {
                        headers.Add(item.Key, item.Value);
                    }
                }
            }
        }
    }
}
