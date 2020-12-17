using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordBot.Services
{
    public class HttpClientService : HttpClient
    {
        private readonly HttpClient _httpClient;

        public HttpClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            this.Timeout = new TimeSpan(0, 0, 15);
            this.MaxResponseContentBufferSize = 8000000;
        }

        public static List<T>Deserialize<T>(string serializedJsonString)
        {
            var stuff = JsonConvert.DeserializeObject<List<T>>(serializedJsonString);
            return stuff;
        }

        public Task<T> GetWithHostAsync<T>(string uri, string host)
        {
            return GetWithHostAsync<T>(uri, host, "*/*");
        }

        public async Task<T> GetWithHostAsync<T>(string uri, string host, string accept)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.All;
            request.Accept = accept;
            request.Host = host;
            using HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();
            await using Stream stream = response.GetResponseStream();
            using StreamReader streamReader = new StreamReader(stream);
            try
            {
                return JsonConvert.DeserializeObject<T>(await streamReader.ReadToEndAsync());
            }
            catch
            {
                return default;
            }
        }
    }
}
