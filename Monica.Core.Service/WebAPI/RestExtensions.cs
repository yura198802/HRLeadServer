using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Monica.Core.Service.WebAPI
{
    public static class RestExtensions
    {

        public static TResult SendPostAnonymous<TResult>(string route, string host, object content, Func<string, string> getError = null) where TResult : class
        {
            RestClient restClient = new RestClient(host);
            return restClient.SendPost<TResult>(route, content, getError);
        }
        private static TResult SendPost<TResult>(this RestClient client, string route, object content, Func<string, string> getError) where TResult : class
        {
            var request = new RestRequest(new Uri(client.BaseUrl + route), Method.POST, DataFormat.Json);
            request.AddJsonBody(content);
            var response = client.Post(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorText = string.Empty;
                var responseContent = response.Content?.StartsWith("\"\"") ?? false ? response.Content.Remove(0, 2) : response.Content;
                errorText = getError?.Invoke(responseContent);
                throw new Exception($"{errorText}");
            }
            return JsonConvert.DeserializeObject<TResult>(response.Content);
        }
    }
}
