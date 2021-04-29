using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

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
                ConsoleWriteHandler($"Send get request, url={url}", ConsoleColor.Blue);

                IDictionary<string, string> getRequestHeaderParameters = new Dictionary<string, string>();
                getRequestHeaderParameters.Add("my-sample-header", "Lorem ipsum dolor sit amet");
                HttpResponseMessage responseMessage = NetworkRequestWorker.Get(url);
                ConsoleWriteHandler($@"Response message: {JsonConvert.SerializeObject(new
                {
                    responseMessage.StatusCode,
                    responseMessage.IsSuccessStatusCode,
                    responseMessage.Version
                })}", responseMessage.IsSuccessStatusCode ? ConsoleColor.Green : ConsoleColor.DarkYellow);
                ConsoleWriteHandler($"Result:\n{responseMessage.GetResultAsJObject().ToString(Formatting.Indented)}");
            }
            catch (Exception ex)
            {
                ConsoleWriteHandler($"{ex.Message}\n{ex.StackTrace}", ConsoleColor.Red);
            }

            try
            {
                url = "http://172.16.1.43:8022/restful";
                ConsoleWriteHandler($"Send post request, url={url}", ConsoleColor.Blue);

                IDictionary<string, string> postRequestHeaderParameters = new Dictionary<string, string>();
                postRequestHeaderParameters.Add("digi-protocol", "raw");
                postRequestHeaderParameters.Add("digi-type", "sync");
                postRequestHeaderParameters.Add("digi-host", "{\"ver\":\"5.7\",\"prod\":\"DOP\",\"timezone\":\" + 8\",\"ip\":\"10.20.9.19\",\"id\":\"\",\"lang\":\"zh_CN\",\"acct\":\"dcms\",\"timestamp\":\"2018071990007275\"}");
                postRequestHeaderParameters.Add("digi-service", "{\"prod\":\"E10\",\"ip\":\"127.0.0.1\",\"name\":\"e10.getIssueTreeApis\",\"id\":\"E0_6.0_NJ\"}");
                HttpResponseMessage responseMessage = NetworkRequestWorker.PostAsync(url, new
                {
                    std_data = new
                    {
                        parameter = new { }
                    }
                }, postRequestHeaderParameters).Result;
                ConsoleWriteHandler($@"Response message: {JsonConvert.SerializeObject(new
                {
                    responseMessage.StatusCode,
                    responseMessage.IsSuccessStatusCode,
                    responseMessage.Version
                })}", responseMessage.IsSuccessStatusCode ? ConsoleColor.Green : ConsoleColor.DarkYellow);
                ConsoleWriteHandler($"Result:\n{responseMessage.GetResultAsString()}");
            }
            catch (Exception ex)
            {
                ConsoleWriteHandler($"{ex.Message}\n{ex.StackTrace}", ConsoleColor.Red);
            }

            ConsoleWriteHandler("Send out, press any key to exit...", ConsoleColor.Magenta);
            Console.ReadLine();
        }

        static void ConsoleWriteHandler(string content)
        {
            ConsoleWriteHandler(content, ConsoleColor.White);
        }

        static void ConsoleWriteHandler(string content, ConsoleColor consoleColor, bool isNewLine = true)
        {
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            if (isNewLine)
            {
                Console.WriteLine(content);
            }
            else
            {
                Console.Write(content);
            }
            Console.ForegroundColor = currentForeColor;
        }
    }
}
