using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Tevian
{
    public partial class Tevian
    {
        static HttpClient client = new HttpClient();
        private static string baseUrl = "https://backend.facecloud.tevian.ru/api/v1";

        public string jwt { get; set; }

        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };

        protected static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, jsonSettings);
        }

        protected static T Deserialize<T>(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value, jsonSettings);
            }
            catch (JsonReaderException ex)
            {
                throw new AggregateException($"Failed to deserialize data \"{value}\".", ex);
            }
        }

        protected class Response<TData>
        {
            public TData Data { get; set; }
            public Pagination Pagination { get; set; }
            public int? StatusCode { get; set; }
            public string Message { get; set; }
        }

        protected class Response : Response<object>
        {
        }

        protected static string GetQueryString(object obj)
        {
            bool isBool(PropertyInfo p)
            {
                return (p.PropertyType == typeof(bool?) || p.PropertyType == typeof(bool)) &&
                       (bool) p.GetValue(obj, null) == false;
            }
            /*var properties = from p in obj.GetType().GetProperties()
                where p.GetValue(obj, null) != null &&
                      !((p.PropertyType == typeof(bool?) || p.PropertyType == typeof(bool)) &&
                        ((bool) (p.GetValue(obj, null)) == false))
                select p.Name + "=" +
                       HttpUtility.UrlEncode(Convert.ToString(p.GetValue(obj, null), CultureInfo.InvariantCulture)
                           ?.ToString());*/

            var properties = new List<string>();
            foreach (var p in obj.GetType().GetProperties())
            {
                if (p.GetValue(obj, null) == null || isBool(p))
                    continue;
                string prop;
                if (p.PropertyType.IsArray)
                {
                    prop = string.Join(",",
                        from object v in (IEnumerable) p.GetValue(obj)
                        select HttpUtility.UrlEncode(Convert.ToString(v, CultureInfo.InvariantCulture)));
                }
                else
                {
                    prop = HttpUtility.UrlEncode(Convert.ToString(p.GetValue(obj, null),
                        CultureInfo.InvariantCulture));
                }

                properties.Add(p.Name + "=" + prop);
            }

            return string.Join("&", properties.ToArray());
        }


        protected static void CheckCode<TData>(Response<TData> data, IList<int> goodStatus)
        {
            if (data.StatusCode == null)
                throw new TevianException("Deserializing response failed.");
            if (!goodStatus.Contains(data.StatusCode.Value))
                throw new TevianException(data.Message ?? "Unknown error. StatusCode = " + data.StatusCode);
        }

        protected async Task<TResult> PostBase<TResult>(string method, HttpContent content, object query = null)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl + "/" + method + (query != null ? "?" + GetQueryString(query) : "")),
                Content = content
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await client.SendAsync(request);
            var rstr = await response.Content.ReadAsStringAsync();
            Console.WriteLine(rstr);
            var data = Deserialize<TResult>(rstr);

            content.Dispose();

            return data;
        }

        protected async Task<TResult> Post<TResult>(string method, HttpContent content, object query = null,
            int goodStatus = 200)
        {
            var data = await PostBase<Response<TResult>>(method, content, query);
            CheckCode(data, new List<int> {goodStatus});
            return data.Data;
        }

        protected async Task<Response<TResult>> Get<TResult>(string method, object query = null)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(baseUrl + "/" + method + (query != null ? "?" + GetQueryString(query) : "")),
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await client.SendAsync(request);
            var data = Deserialize<Response<TResult>>(await response.Content.ReadAsStringAsync());

            CheckCode(data, new List<int> {200, 404});

            return data;
        }

        protected async Task Delete(string method)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(baseUrl + "/" + method),
            };
            var response = await client.SendAsync(request);
            var data = Deserialize<Response>(await response.Content.ReadAsStringAsync());

            CheckCode(data, new List<int> { 200, 404 });
        }

        protected HttpContent JpegContent(byte[] image)
        {
            var content = new ByteArrayContent(image);
            content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return content;
        }
    }
}
