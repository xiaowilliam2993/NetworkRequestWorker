using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace NetworkRequestExample.Netframework4_5
{
    class Program
    {
        /// <summary>
        /// 测试方法
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string url = "";
            try
            {
                url = "https://postman-echo.com/headers";
                Console.WriteLine($"Send get request, url={url}");

                IDictionary<string, string> getRequestHeaderParameters = new Dictionary<string, string>();
                getRequestHeaderParameters.Add("my-sample-header", "Lorem ipsum dolor sit amet");
                JObject getResult = NetworkRequestWorker.Get(url).GetResultAsJObject();
                Console.WriteLine($"response:\n{getResult.ToString(Formatting.Indented)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }

            try
            {
                url = "http://172.16.1.43:8022/restful";
                Console.WriteLine($"Send post request, url={url}");

                IDictionary<string, string> postRequestHeaderParameters = new Dictionary<string, string>();
                postRequestHeaderParameters.Add("digi-protocol", "raw");
                postRequestHeaderParameters.Add("digi-type", "sync");
                postRequestHeaderParameters.Add("digi-host", "{\"ver\":\"5.7\",\"prod\":\"DOP\",\"timezone\":\" + 8\",\"ip\":\"10.20.9.19\",\"id\":\"\",\"lang\":\"zh_CN\",\"acct\":\"dcms\",\"timestamp\":\"2018071990007275\"}");
                postRequestHeaderParameters.Add("digi-service", "{\"prod\":\"E10\",\"ip\":\"127.0.0.1\",\"name\":\"e10.getIssueTreeApis\",\"id\":\"E0_6.0_NJ\"}");
                string postResult = NetworkRequestWorker.PostAsync(url, new
                {
                    std_data = new
                    {
                        parameter = new { }
                    }
                }, postRequestHeaderParameters).Result.GetResultAsString();
                Console.WriteLine($"response:\n{postResult}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }

            Console.WriteLine("Send out, press any key to exit...");
            Console.ReadLine();
        }
    }
}
