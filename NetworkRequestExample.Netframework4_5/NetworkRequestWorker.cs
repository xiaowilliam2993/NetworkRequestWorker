using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRequestExample.Netframework4_5
{
    /// <summary>
    /// 网络请求工具类，基于.Net Framework 4.5框架，为网络请求提供便捷方法
    ///     提供GET、POST、PUT、DELET等几种常用的请求方式，或者调Send方法自行组织参数，返回值为HttpResponseMessage，根据该对象可以处理各种业务方面的事情
    ///     GetResultAs...，提供了一些快速将HttpResponseMessage结果转换为目标对象的入口
    /// </summary>
    public static class NetworkRequestWorker
    {
        private readonly static ILogger _Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 发送请求，请求方式为动态参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="httpMethod"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static HttpResponseMessage Send(string url, dynamic body, HttpMethod httpMethod, IDictionary<string, string> headerParameters = null)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            if (httpMethod == null) throw new ArgumentNullException(nameof(httpMethod));

            string serial = $"{Guid.NewGuid():N}";
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                _Logger.Info($"{serial} send request {url}");

                using (HttpClient httpClient = new HttpClient())
                {
#if Debug
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
#endif
                    HeaderParametersHandler(httpClient, headerParameters);
                    httpClient.BaseAddress = new Uri(url);
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, string.Empty);
                    if (httpMethod != HttpMethod.Get && body != null)
                    {
                        httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                    }
                    return httpClient.SendAsync(httpRequestMessage).Result;
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, $"params: {JsonConvert.SerializeObject(new { url, body, httpMethod, headerParameters })}");
                throw ex;
            }
            finally
            {
                stopwatch.Stop();
                _Logger.Info($"{serial} get response, time consuming {stopwatch.Elapsed.TotalSeconds}(s)");
            }
        }

        /// <summary>
        /// 发送（异步）请求，请求方式为动态参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="httpMethod"></param>
        /// <param name="headerParameters"></param>
        /// <param name="isNeedCompression">标记是否要求压缩上传请求Body</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> SendAsync(string url, dynamic body, HttpMethod httpMethod, IDictionary<string, string> headerParameters = null, bool isNeedCompression = false)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            if (httpMethod == null) throw new ArgumentNullException(nameof(httpMethod));

            string serial = $"{Guid.NewGuid():N}";
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                _Logger.Info($"{serial} send request {url}");

                using (HttpClient httpClient = new HttpClient())
                {
#if Debug
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
#endif
                    HeaderParametersHandler(httpClient, headerParameters);
                    httpClient.BaseAddress = new Uri(url);
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, string.Empty);
                    if (httpMethod != HttpMethod.Get && body != null)
                    {
                        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                        httpRequestMessage.Content = isNeedCompression ? new CompressedContent(httpContent, CompressionMethod.GZip) : httpContent;
                    }
                    return await httpClient.SendAsync(httpRequestMessage);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, $"params: {JsonConvert.SerializeObject(new { url, body, httpMethod, headerParameters })}");
                throw ex;
            }
            finally
            {
                stopwatch.Stop();
                _Logger.Info($"{serial} get response, time consuming {stopwatch.Elapsed.TotalSeconds}(s)");
            }
        }

        /// <summary>
        /// 处理GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static HttpResponseMessage Get(string url, IDictionary<string, string> headerParameters = null)
        {
            return Send(url, null, HttpMethod.Get, headerParameters);
        }

        /// <summary>
        /// 处理GET请求（异步）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetAsync(string url, IDictionary<string, string> headerParameters = null)
        {
            return await SendAsync(url, null, HttpMethod.Get, headerParameters);
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
            return Send(url, body, HttpMethod.Post, headerParameters);
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
            return await SendAsync(url, body, HttpMethod.Post, headerParameters);
        }

        /// <summary>
        /// 处理PUT请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static HttpResponseMessage Put(string url, dynamic body, IDictionary<string, string> headerParameters = null)
        {
            return Send(url, body, HttpMethod.Put, headerParameters);
        }

        /// <summary>
        /// 处理PUT请求（异步）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PutAsync(string url, dynamic body, IDictionary<string, string> headerParameters = null)
        {
            return await SendAsync(url, body, HttpMethod.Put, headerParameters);
        }

        /// <summary>
        /// 处理DELETE请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static HttpResponseMessage Delete(string url, dynamic body, IDictionary<string, string> headerParameters = null)
        {
            return Send(url, body, HttpMethod.Delete, headerParameters);
        }

        /// <summary>
        /// 处理DELETE请求（异步）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="headerParameters"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> DeleteAsync(string url, dynamic body, IDictionary<string, string> headerParameters = null)
        {
            return await SendAsync(url, body, HttpMethod.Delete, headerParameters);
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
            return await SendAsync(url, body, HttpMethod.Post, headerParameters, true);
        }

        /// <summary>
        /// 取得请求响应结果，返回string
        /// </summary>
        /// <param name="httpResponseMessage"></param>
        /// <returns></returns>
        public static string GetResultAsString(this HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage == null) throw new ArgumentNullException(nameof(httpResponseMessage));

            return httpResponseMessage.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 取得请求响应结果，返回JObject
        /// </summary>
        /// <param name="httpResponseMessage"></param>
        /// <returns></returns>
        public static JObject GetResultAsJObject(this HttpResponseMessage httpResponseMessage)
        {
            try
            {
                return JObject.Parse(GetResultAsString(httpResponseMessage));
            }
            catch (JsonReaderException ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 取得请求响应结果（泛型）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpResponseMessage"></param>
        /// <returns></returns>
        public static T GetResultAsGeneric<T>(this HttpResponseMessage httpResponseMessage) where T : class
        {
            return JsonConvert.DeserializeObject<T>(GetResultAsString(httpResponseMessage));
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
