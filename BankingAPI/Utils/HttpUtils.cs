using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BankingAPI.Utils
{
    internal static class HttpUtils
    {
        internal static IHttpClientFactory httpClientFactory;

        private static HttpClientHandler handler;
        internal static HttpClientHandler HttpClientHandler
        {
            get
            {
                return handler = new HttpClientHandler { SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls };
            }
        }
        internal static string Host = "https://api.awhere.com/v2/";

        private static bool CheckConnectivity()
        {
            try
            {
                var testConnectivity = new Ping();
                string testHost = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                var pingOptions = new PingOptions();
                var reply = testConnectivity.Send(testHost, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static async Task<string> GetResponse(StringBuilder uri, bool isAwhereAPICall = false)
        {
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
            await semaphoreSlim.WaitAsync();
            try
            {
                if (uri.Length == 0)
                    return string.Empty;

                string jsonResponse = string.Empty;

                var isConnected = CheckConnectivity();

                if(isConnected)
                {
                    string requestString = uri.ToString().Trim().Replace(" ", "");
                    var httpClient = httpClientFactory.CreateClient((isAwhereAPICall ? "awhereClient" : "agroObservClient"));
                    var request = new HttpRequestMessage(HttpMethod.Get, requestString);

                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        jsonResponse = await response.Content.ReadAsStringAsync();
                    }

                }

                return jsonResponse;
            }
            finally
            {
                semaphoreSlim.Release();
            }


        }

        internal static string GetRequestBody(Stream Request)
        {
            if(Request != null)
            {
                using (var reader = new StreamReader(Request, Encoding.UTF8))
                {
                    return reader.ReadToEndAsync().GetAwaiter().GetResult();
                }
            }
            return string.Empty;
        }

    }
}
