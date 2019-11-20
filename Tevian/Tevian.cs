using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Tevian
{
    public class Match
    {
        public Bbox face1_bbox { get; set; }
        public Bbox face2_bbox { get; set; }
        public float score { get; set; }
    }

    public class Detect
    {
        public FaceWithInfo[] Faces { get; set; }
    }

    class Login
    {
        public string AccessToken { get; set; }
    }

    class TevianException : Exception
    {
        public TevianException()
        {
        }

        public TevianException(string message) : base(message)
        {
        }

        public TevianException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class Tevian
    {
        static HttpClient client = new HttpClient();
        private static string baseUrl = "https://backend.facecloud.tevian.ru/api/v1";

        public string jwt { get; protected set; }

        protected JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        protected class Response<TData>
        {
            public TData Data { get; set; }
            public int? StatusCode { get; set; }
            public string Message { get; set; }
        }


        private async Task<string> Login(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/" + "login");


            request.Content = new StringContent(
                JsonConvert.SerializeObject(new {email = email, password = password}, jsonSettings),
                Encoding.UTF8, "application/json");


            var r = await client.SendAsync(request);

            var resp = JsonConvert.DeserializeObject<Response<Login>>(await r.Content.ReadAsStringAsync());


            if (resp.StatusCode != 200)
                throw new TevianException(resp.Message ?? "Unknown error.");


            return resp.Data.AccessToken;
        }

        public string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                where p.GetValue(obj, null) != null
                select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return string.Join("&", properties.ToArray());
        }

        private async Task<TResult> Post<TResult>(string method, HttpContent content, object query = null)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl + "/" + method + (query != null ? "?" + GetQueryString(query) : "")),
                Content = content
            };


            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await client.SendAsync(request);
            var data = JsonConvert.DeserializeObject<Response<TResult>>(await response.Content.ReadAsStringAsync(),
                jsonSettings);
            if (data.StatusCode == null)
                throw new TevianException("Deserializing response failed.");
            if (data.StatusCode != 200)
                throw new TevianException(data.Message ?? "Unknown error.");
            return data.Data;
        }

        public Tevian(string email, string passw)
        {
            jwt = Login(email, passw).Result;
            if (string.IsNullOrEmpty(jwt))
                throw new TevianException("Login error.");
        }


        public async Task<Detect> Detect(byte[] image,
            int? fd_min_size = null, int? fd_max_size = null, float? fd_threshold = null,
            int[] face = null, bool? demographics = null, bool? attributes = null,
            bool? landmarks = null, bool? liveness = null)
        {
            var content = new ByteArrayContent(image);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return await Post<Detect>("detect", content, new
            {
                fd_min_size,
                fd_max_size,
                fd_threshold,
                face,
                demographics,
                attributes,
                landmarks,
                liveness
            });
        }

        public async Task<Match> Match(byte[] image1, byte[] image2,
            int? fd_min_size = null, int? fd_max_size = null, float? fd_threshold = null,
            int[] face1 = null, int[] face2 = null
        )
        {
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(image1), "image1");
            content.Add(new ByteArrayContent(image2), "image2");

            return await Post<Match>("detect", content, new
            {
                fd_min_size,
                fd_max_size,
                fd_threshold,
                face1,
                face2
            });
        }
    }
}