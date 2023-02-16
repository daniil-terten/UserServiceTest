using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceTest.Client
{
    public class BaseClient
    {
        public HttpClient client;

        public BaseClient()
        {
            client = new HttpClient() { BaseAddress = new Uri(Config.BaseUrl) };
        }

        public async Task<ResponseModel<R>> SendGetAsync<R>(string url)
        {
            var response = await client.GetAsync(url);

            return await CheckResponseIsSuccesful<R>(response);
        }

        public async Task<ResponseModel<R>> SendPostAsync<T,R>(string url, T content)
        {
            var stringContent = JsonConvert.SerializeObject(content);
            var response =  await client.PostAsync(url, new StringContent(stringContent));

            return await CheckResponseIsSuccesful<R>(response);
        }

        public async Task<ResponseModel<R>> SendPutAsync<T, R>(string url, T content)
        {
            var stringContent = JsonConvert.SerializeObject(content);
            var response = await client.PutAsync(url, new StringContent(stringContent));

            return await CheckResponseIsSuccesful<R>(response);
        }

        public async Task<ResponseModel<string>> SendPutAsync(string url)
        {
            var response = await client.PutAsync(url, new StringContent(""));

            return await CheckResponseIsSuccesful<string>(response);
        }

        private static async Task<ResponseModel<T>> CheckResponseIsSuccesful<T>(HttpResponseMessage response)
        {
            var responseModel = new ResponseModel<T>();

            if (!response.IsSuccessStatusCode)
            {
                responseModel.IsSuccessStatusCode = false;
                responseModel.StatusCode = (int) response.StatusCode;
            }
            else
            {
                responseModel.IsSuccessStatusCode = true;
                responseModel.StatusCode = (int) response.StatusCode;
                var responseString = await response.Content.ReadAsStringAsync();
                responseModel.Content = JsonConvert.DeserializeObject<T>(responseString);
            }
            return responseModel;
        }
    }

    public class ResponseModel<R>
    {
        public R? Content { get; set; }

        public int StatusCode { get; set; }

        public bool IsSuccessStatusCode { get; set; }
    }
}
